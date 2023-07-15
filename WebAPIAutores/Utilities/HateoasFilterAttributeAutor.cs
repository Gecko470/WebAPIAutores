using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using WebAPIAutores.DTOs;
using WebAPIAutores.Services;

namespace WebAPIAutores.Utilities
{
    public class HateoasFilterAttributeAutor : HateoasFilterAttribute
    {
        private readonly GeneradorEnlaces generador;

        public HateoasFilterAttributeAutor(GeneradorEnlaces generador)
        {
            this.generador = generador;
        }

        public override async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            var debeIncluir = DebeIncluirHateoas(context);

            if (!debeIncluir)
            {
                await next();
                return;
            }

            var resultado = context.Result as ObjectResult;
            var autorDTO = resultado.Value as AutorDTO;

            if (autorDTO == null)
            {
                var autoresDTO = resultado.Value as List<AutorDTO> ?? throw new ArgumentException("Se esperaba una instancia de AutorDTO o List<AutorDTO> ..");
                autoresDTO.ForEach(async autorDTO => await generador.GenerarEnlaces(autorDTO));
                resultado.Value = autoresDTO;
            }
            else
            {
                await generador.GenerarEnlaces(autorDTO);
            }

            await next();
        }
    }
}
