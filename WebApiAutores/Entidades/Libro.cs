
using System.ComponentModel.DataAnnotations;
using WebApiAutores.Validaciones;

namespace WebApiAutores.Entidades;
public class Libro
{
    public int Id { get; set; }
    
    [Required(ErrorMessage = "El campo {0} es requerido")]
    [StringLength(maximumLength:250, ErrorMessage = "El campo {0} no debe tener más de {1} carácteres")]
    [PrimeraLetraMayuscula] // Validacion personalizada 
    public String Titulo { get; set; }
    public DateTime? FechaPublicacion { get; set; }
    public List<Comentario> Comentarios { get; set; }
    public List<AutorLibro> AutoresLibros {  get; set; } 
}
