using System.ComponentModel.DataAnnotations;
using WebAPIAutores.Vadlidations;

namespace WebAPIAutores.DTOs
{
    public class AutorCreacionDTO
    {
        [Required(ErrorMessage = "El campo {0} es obligatorio..")]
        [StringLength(maximumLength: 50, ErrorMessage = "El campo {0} no debe tener más de {1} caracteres..")]
        [Capitalize]
        public string Nombre { get; set; }
    }
}
