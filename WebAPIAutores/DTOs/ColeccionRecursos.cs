namespace WebAPIAutores.DTOs
{
    public class ColeccionRecursos<T> : Recurso where T : Recurso
    {
        public List<T> Valores { get; set; }
    }
}
