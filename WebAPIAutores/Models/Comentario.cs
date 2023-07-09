using System.ComponentModel.DataAnnotations;

namespace WebAPIAutores.Models
{
    public class Comentario
    {
        public int Id { get; set; }
        [Required]
        public string Contenido { get; set; }
        public int LibroId { get; set; }
        public Libro Libro { get; set; }
    }
}
