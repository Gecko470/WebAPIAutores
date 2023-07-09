using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using WebAPIAutores.Vadlidations;

namespace WebAPIAutores.Models
{
    public class Autor /*: IValidatableObject*/
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "El campo {0} es obligatorio..")]
        [StringLength(maximumLength: 50, ErrorMessage = "El campo {0} no debe tener más de {1} caracteres..")]
        [Capitalize]
        public string Nombre { get; set; }
        public List<AutorLibro> AutoresLibros { get; set; }
        //public List<Libro> Libros { get; set; } Para relación 1 a muchos con Autores(1) - Libros(muchos)

        //IEnumerable<ValidationResult> IValidatableObject.Validate(ValidationContext validationContext)
        //{
        //    if (!string.IsNullOrEmpty(Nombre))
        //    {
        //        var primeraLetra = Nombre[0].ToString();

        //        if (primeraLetra != primeraLetra.ToUpper())
        //        {
        //            yield return new ValidationResult("La primera letra debe ser mayúscula..", new string[] { nameof(Nombre) });
        //        }
        //    }
        //}
    }
}
