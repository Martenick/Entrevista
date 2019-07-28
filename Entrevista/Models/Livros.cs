using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Entrevista.Models
{
    public class Livros
    {
        [Key]
        public int LivrosID { get; set; }

        [MaxLength(17, ErrorMessage = "Tamanho máximo 17 caracteres")]
        [Index("Livros_ISBN_Index", IsUnique = true)]
        [Required(ErrorMessage = "O campo ISBN é obrigatório!")]
        [Display(Name = "ISBN")]
        public string ISBNLivros { get; set; }

        [Required(ErrorMessage = "O campo Autor é obrigatório!")]
        [Display(Name = "Autor")]
        public string AutorLivros { get; set; }

        [Required(ErrorMessage = "O campo Nome é obrigatório!")]
        [Display(Name = "Nome do Livro")]
        public string NomeLivros { get; set; }

        [Display(Name = "Preço")]
        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = false)]
        public decimal ValorLivros { get; set; }

        [RegularExpression(@"^[\d,]+$", ErrorMessage = "Preço do Livro deve ser um valor monetário.")]
        [Required(ErrorMessage = "O campo Preço do Livro é obrigatório!")]
        [Display(Name = "Preço")]
        public string TextoValorLivros { get; set; }

        [Required(ErrorMessage = "O campo Data de Publicação é obrigatória!")]
        [Display(Name = "Data de Publicação")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = false)]
        public DateTime DataPublicacaoLivros { get; set; }

        [Display(Name = "Imagem da Capa")]
        [DataType(DataType.ImageUrl)]
        public string ImagemCapaLivros { get; set; }

        [NotMapped]
        public HttpPostedFileBase ImagemCapaLivrosFile { get; set; }

    }
}