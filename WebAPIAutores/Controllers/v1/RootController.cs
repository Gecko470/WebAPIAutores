using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAPIAutores.DTOs;

namespace WebAPIAutores.Controllers.v1
{
    [ApiController]
    [Route("api/v1")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class RootController : ControllerBase
    {
        private readonly IAuthorizationService authorizationService;

        public RootController(IAuthorizationService authorizationService)
        {
            this.authorizationService = authorizationService;
        }


        [HttpGet(Name = "getRoot")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<HateoasDTO>>> Get()
        {
            var datosHateoas = new List<HateoasDTO>();

            var esAdmin = await authorizationService.AuthorizeAsync(User, policyName: "esAdmin");

            datosHateoas.Add(new HateoasDTO(enlace: Url.Link("getRoot", new { }), descripcion: "self", metodo: "GET"));
            datosHateoas.Add(new HateoasDTO(enlace: Url.Link("getAutores", new { }), descripcion: "autores", metodo: "GET"));

            if (esAdmin.Succeeded)
            {
                datosHateoas.Add(new HateoasDTO(enlace: Url.Link("postAutor", new { }), descripcion: "autores-create", metodo: "POST"));
                datosHateoas.Add(new HateoasDTO(enlace: Url.Link("postLibro", new { }), descripcion: "libros-creae", metodo: "POST"));
            }

            return datosHateoas;
        }
    }
}
