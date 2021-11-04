
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using WebApiAutores.Entidades;

namespace WebApiAutores;
public class ApplicationDbContext: IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions options): base(options)
    {

    }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        // Se especifican las llaves primarias compuestas de la entidad AutorLibro
        modelBuilder.Entity<AutorLibro>()
            .HasKey(al => new { al.AutorId, al.LibroId });
    }

    public DbSet<Autor> Autores { get; set; }
    public DbSet<Libro> Libros { get; set; }
    public DbSet<Comentario> Comentarios { get; set; }
    public DbSet<AutorLibro> AutoresLibros { get; set; }
}
