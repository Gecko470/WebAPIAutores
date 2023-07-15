namespace WebAPIAutores.DTOs
{
    public class HateoasDTO
    {
        public string Enlace { get; private set; }
        public string Descripcion { get; private set; }
        public string Metodo { get; private set; }

        public HateoasDTO(string enlace, string descripcion, string metodo)
        {
            Enlace = enlace;
            Descripcion = descripcion;
            Metodo = metodo;
        }

    }
}
