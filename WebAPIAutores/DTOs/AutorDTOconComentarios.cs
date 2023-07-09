namespace WebAPIAutores.DTOs
{
    public class AutorDTOconComentarios : AutorDTOconLibros
    {
        public List<ComentarioDTO> Comentarios { get; set; }
    }
}
