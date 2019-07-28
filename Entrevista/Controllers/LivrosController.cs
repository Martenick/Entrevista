using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Entrevista.Classes;
using Entrevista.Models;

namespace Entrevista.Controllers
{
    [Authorize(Roles = "Admin")]
    public class LivrosController : Controller
    {
        private EntrevistaContext db = new EntrevistaContext();

        // GET: Livros
        public ActionResult Index(string ordem, string filtroAtual, string BuscaAutor, string BuscaNome, string BuscaISBN, decimal? BuscaValor, string BuscaData)
        {
            DateTime Publicacao;
            ViewBag.Ordenacao = ordem;
            ViewBag.OrdemAutor = String.IsNullOrEmpty(ordem) ? "autor-desc" : "";
            ViewBag.OrdemNome = ordem == "nome" ? "nome-desc" : "nome";
            ViewBag.OrdemValor = ordem == "valor" ? "valor-desc" : "valor";
            ViewBag.OrdemData = ordem == "data" ? "data-desc" : "data";
            ViewBag.OrdemISBN = ordem == "ISBN" ? "ISBN-desc" : "ISBN";

            if (BuscaAutor == null)
            {
                BuscaAutor = filtroAtual;
            }
            ViewBag.FiltroAtual = BuscaAutor;

            var livros = from s in db.Livros
                         select s;

            if (!String.IsNullOrEmpty(BuscaAutor))
            {
                livros = livros.Where(s => s.AutorLivros.ToUpper().Contains(BuscaAutor.ToUpper()));
            }

            if (!String.IsNullOrEmpty(BuscaNome))
            {
                livros = livros.Where(s => s.NomeLivros.ToUpper().Contains(BuscaNome.ToUpper()));
            }

            if (!String.IsNullOrEmpty(BuscaISBN))
            {
                livros = livros.Where(s => s.ISBNLivros.ToUpper().Contains(BuscaISBN.ToUpper()));
            }

            if (!String.IsNullOrEmpty(BuscaValor.ToString()))
            {
                livros = livros.Where(s => s.ValorLivros <= BuscaValor);
            }

            if (DateTime.TryParse(BuscaData, out Publicacao))
            {
                livros = livros.Where(s => s.DataPublicacaoLivros == Publicacao);
            }

            switch (ordem)
            {
                case "autor-desc":
                    livros = livros.OrderByDescending(s => s.AutorLivros).ThenBy(s => s.NomeLivros);
                    break;
                case "nome":
                    livros = livros.OrderBy(s => s.NomeLivros);
                    break;
                case "nome-desc":
                    livros = livros.OrderByDescending(s => s.NomeLivros);
                    break;
                case "data":
                    livros = livros.OrderBy(s => s.DataPublicacaoLivros);
                    break;
                case "data-desc":
                    livros = livros.OrderByDescending(s => s.DataPublicacaoLivros);
                    break;
                case "valor":
                    livros = livros.OrderBy(s => s.ValorLivros);
                    break;
                case "valor-desc":
                    livros = livros.OrderByDescending(s => s.ValorLivros);
                    break;
                case "ISBN":
                    livros = livros.OrderBy(s => s.ISBNLivros);
                    break;
                case "ISBN-desc":
                    livros = livros.OrderByDescending(s => s.ISBNLivros);
                    break;
                default:
                    livros = livros.OrderBy(s => s.AutorLivros).ThenBy(s => s.NomeLivros);
                    break;
            }

            return View(livros.ToList());
        }

        // GET: Livros/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Livros livros = db.Livros.Find(id);
            if (livros == null)
            {
                return HttpNotFound();
            }
            return View(livros);
        }

        // GET: Livros/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Livros/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Livros livros)
        {
            livros.ValorLivros = Decimal.Parse(livros.TextoValorLivros);
            if (ModelState.IsValid)
            {
                db.Livros.Add(livros);
                try
                {
                    db.SaveChanges();
                    if (livros.ImagemCapaLivrosFile != null)
                    {
                        var pic = string.Empty;
                        var folder = "~/Content/Capas";

                        var respbool = FilesHelper.UploadPhoto(livros.ImagemCapaLivrosFile, folder, string.Format("{0}.jpg", livros.LivrosID));
                        if (respbool)
                        {
                            pic = string.Format("{0}/{1}.jpg", folder, livros.LivrosID);
                            livros.ImagemCapaLivros = pic;
                            db.Entry(livros).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                    }
                    return RedirectToAction("Index");
                }
                catch (System.Exception ex)
                {
                    if (ex.InnerException != null && ex.InnerException.InnerException != null &&
                        ex.InnerException.InnerException.Message.Contains("_Index"))
                    {
                        ModelState.AddModelError(string.Empty, "ISBN já cadastrado !");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, ex.InnerException.InnerException.Message);
                    }
                }
            }

            return View(livros);
        }

        // GET: Livros/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Livros livros = db.Livros.Find(id);
            if (livros == null)
            {
                return HttpNotFound();
            }
            ViewBag.DataPublicacao = livros.DataPublicacaoLivros.ToShortDateString();
            return View(livros);
        }

        // POST: Livros/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Livros livros, string dataPublicacao)
        {
            livros.ValorLivros = Decimal.Parse(livros.TextoValorLivros);
            DateTime Publicacao;

            if (!DateTime.TryParse(dataPublicacao, out Publicacao))
            {
                ModelState.AddModelError(string.Empty, "A Data de Publicação não é válida !");
            }
            else
            {
                livros.DataPublicacaoLivros = Publicacao;
                if (ModelState.IsValid)
                {
                    if (livros.ImagemCapaLivrosFile != null)
                    {
                        var pic = string.Empty;
                        var folder = "~/Content/Capas";

                        var respbool = FilesHelper.UploadPhoto(livros.ImagemCapaLivrosFile, folder, string.Format("{0}.jpg", livros.LivrosID));
                        if (respbool)
                        {
                            pic = string.Format("{0}/{1}.jpg", folder, livros.LivrosID);
                            livros.ImagemCapaLivros = pic;
                        }
                    }
                    db.Entry(livros).State = EntityState.Modified;
                    try
                    {
                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                    catch (System.Exception ex)
                    {
                        if (ex.InnerException != null && ex.InnerException.InnerException != null &&
                            ex.InnerException.InnerException.Message.Contains("_Index"))
                        {
                            ModelState.AddModelError(string.Empty, "ISBN já cadastrado !");
                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty, ex.InnerException.InnerException.Message);
                        }
                    }

                }
            }
            return View(livros);
        }

        // GET: Livros/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Livros livros = db.Livros.Find(id);
            if (livros == null)
            {
                return HttpNotFound();
            }
            return View(livros);
        }

        // POST: Livros/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Livros livros = db.Livros.Find(id);
            db.Livros.Remove(livros);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
