
namespace WebApiAutores.DTOs;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApiAutores.Validaciones;
public class AutorCreacionDTO
{
    [Required(ErrorMessage = "El campo {0} es requerido")] // Campo nombre como requerido
    [StringLength(maximumLength: 120, ErrorMessage = "El campo {0} no debe tener mas de un {1} carácteres")]
    [PrimeraLetraMayuscula]
    public string Nombre { get; set; }
}
