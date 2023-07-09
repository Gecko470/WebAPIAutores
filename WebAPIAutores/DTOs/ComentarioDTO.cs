using System.ComponentModel.DataAnnotations;

namespace WebAPIAutores.DTOs
{
    public class ComentarioDTO
    {
        public int Id { get; set; }
        public string Contenido { get; set; }
    }
}
