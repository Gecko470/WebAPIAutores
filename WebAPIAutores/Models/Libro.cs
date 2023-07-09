using System.ComponentModel.DataAnnotations;
using WebAPIAutores.Vadlidations;

namespace WebAPIAutores.Models
{
    public class Libro
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "El campo {0} es obligatorio..")]
        [StringLength(maximumLength: 50, ErrorMessage = "El campo {0} no debe tener más de {1} caracteres..")]
        [Capitalize]
        public string Titulo { get; set; }
        public DateTime? FechaPublicacion { get; set; }
        public List<Comentario> Comentarios { get; set; }
        public List<AutorLibro> AutoresLibros { get; set; }
        //public int AutorId { get; set; } Para relación 1 a muchos con Autores(1) - Libros(muchos)
        //public Autor Autor { get; set; } Para relación 1 a muchos con Autores(1) - Libros(muchos)
    }
}
