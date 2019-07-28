using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace Entrevista.Models
{
    public class EntrevistaContext : DbContext
    {
        public EntrevistaContext() : base("DefaultConnection")
        {

        }

        //DESABILITAR CASCATAS

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
        }

        public DbSet<Entrevista.Models.Livros> Livros { get; set; }
    }
}

