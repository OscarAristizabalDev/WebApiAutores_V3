
using System.ComponentModel.DataAnnotations;

namespace WebApiAutores.Validaciones;
public class PrimeraLetraMayusculaAttribute: ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if(value == null || string.IsNullOrEmpty(value.ToString()))
        {
            // Validacion exitosa
            return ValidationResult.Success;
        }
        // Se obtiene la primera letra
        var primeraLetra = value.ToString()[0].ToString();
        // Si la primera letra no es mayuscula
        if(primeraLetra != primeraLetra.ToUpper())
        {
            // Se retorna una validación
            return new ValidationResult("La primera letra debe ser mayúscula");
        }
        return ValidationResult.Success;
    }
}
