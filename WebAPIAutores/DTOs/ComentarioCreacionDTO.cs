using System.ComponentModel.DataAnnotations;

namespace WebAPIAutores.DTOs
{
    public class ComentarioCreacionDTO
    {
        [Required]
        public string Contenido { get; set; }
    }
}
