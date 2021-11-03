
using System.ComponentModel.DataAnnotations;
using WebApiAutores.Validaciones;

namespace WebApiAutores.DTOs;
public class LibroCreacionDTO
{
    [Required(ErrorMessage = "El campo {0} es requerido")]
    [StringLength(maximumLength: 250, ErrorMessage = "El campo {0} no debe tener más de {1} carácteres")]
    [PrimeraLetraMayuscula] // Validacion personalizada 
    public string Titulo { get; set; }
    public DateTime FechaPublicacion { get; set; }
    public List<int> AutoresIds { get; set; }
}
