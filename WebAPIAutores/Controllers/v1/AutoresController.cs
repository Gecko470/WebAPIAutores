using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using WebAPIAutores.DTOs;
using WebAPIAutores.Models;
using WebAPIAutores.Utilities;

namespace WebAPIAutores.Controllers.v1
{
    [ApiController]
    [Route("api/autores")]
    //[Route("api/v1/autores")]
    [Cabecera("x-version", "1")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "EsAdmin")]
    [ApiConventionType(typeof(DefaultApiConventions))]
    public class AutoresController : ControllerBase
    {
        private readonly AppDBContext context;
        private readonly IMapper mapper;
        private readonly IAuthorizationService authorizationService;

        public AutoresController(AppDBContext appDBContext, IMapper mapper, IAuthorizationService authorizationService)
        {
            context = appDBContext;
            this.mapper = mapper;
            this.authorizationService = authorizationService;
        }


        [HttpGet(Name = "getAutoresv1")]
        [AllowAnonymous]
        [ServiceFilter(typeof(HateoasFilterAttributeAutor))]
        public async Task<ActionResult<List<AutorDTO>>> Get([FromQuery] PaginacionDTO paginacion)
        {
            var queryable = context.Autores.AsQueryable();
            await HttpContext.ParametrosPaginacionEnCabecera(queryable);

            var autoresBD = await queryable.OrderBy(x => x.Nombre).Paginar(paginacion).ToListAsync();
            return mapper.Map<List<AutorDTO>>(autoresBD);
        }


        [HttpGet("{id:int}", Name = "AutorByIdv1")]
        [AllowAnonymous]
        [ServiceFilter(typeof(HateoasFilterAttributeAutor))]
        public async Task<ActionResult<AutorDTOconLibros>> GetById(int id)
        {
            var autorBD = await context.Autores.Include(x => x.AutoresLibros).ThenInclude(x => x.Libro).FirstOrDefaultAsync(x => x.Id == id);

            if (autorBD == null)
            {
                return NotFound($"El Autor con Id: {id}, no existe en la BD..");
            }

            var autorDTO = mapper.Map<AutorDTOconLibros>(autorBD);

            return Ok(autorDTO);
        }

        [HttpGet("{nombre}", Name = "AutorByNombrev1")]
        public async Task<ActionResult<List<AutorDTO>>> GetByNombre([FromRoute] string nombre)
        {
            var autoresBD = await context.Autores.Where(x => x.Nombre.Contains(nombre)).ToListAsync();

            if (autoresBD.Count == 0)
            {
                return NotFound($"No existe ningún Autor con el nombre '{nombre}' en la BD..");
            }

            var autoresDTO = mapper.Map<List<AutorDTO>>(autoresBD);

            return Ok(autoresDTO);
        }

        [HttpPost(Name = "postAutorv1")]
        public async Task<ActionResult> Post(AutorCreacionDTO autorCracionDTO)
        {
            var existeAutor = await context.Autores.AnyAsync(x => x.Nombre == autorCracionDTO.Nombre);
            if (existeAutor)
            {
                return BadRequest("Ya existe un autor con ese nombre en la BD..");
            }

            var autor = mapper.Map<Autor>(autorCracionDTO);

            context.Add(autor);
            await context.SaveChangesAsync();

            var autorDTO = mapper.Map<AutorDTO>(autor);

            return CreatedAtRoute("AutorById", new { id = autor.Id }, autorDTO);
        }

        [HttpPut("{id:int}", Name = "putAutorv1")]
        public async Task<ActionResult<Autor>> Put(int id, AutorCreacionDTO autorCreacionDTO)
        {
            var existeAutor = await context.Autores.AnyAsync(x => x.Id == id);

            if (!existeAutor)
            {
                return NotFound($"No existe ese Autor en la BD con el Id: {id}..");
            }

            var autor = mapper.Map<Autor>(autorCreacionDTO);
            autor.Id = id;

            context.Update(autor);
            await context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id:int}", Name = "deleteAutorv1")]
        public async Task<ActionResult> Delete(int id)
        {
            var autorBd = await context.Autores.FirstOrDefaultAsync(x => x.Id == id);

            if (autorBd == null)
            {
                return NotFound();
            }

            context.Remove(new Autor() { Id = id });
            await context.SaveChangesAsync();

            return NoContent();
        }
    }
}
