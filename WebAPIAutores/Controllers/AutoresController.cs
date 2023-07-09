using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using WebAPIAutores.DTOs;
using WebAPIAutores.Models;

namespace WebAPIAutores.Controllers
{
    [ApiController]
    [Route("api/autores")]
    public class AutoresController : ControllerBase
    {
        private readonly AppDBContext context;
        private readonly IMapper mapper;

        public AutoresController(AppDBContext appDBContext, IMapper mapper)
        {
            this.context = appDBContext;
            this.mapper = mapper;
        }
        [HttpGet]
        public async Task<ActionResult<List<AutorDTO>>> Get()
        {
            //return new List<Autor>()
            //{
            //    new Autor()
            //    {
            //        Id = 1,
            //        Nombre = "Juan"
            //    },
            //    new Autor()
            //    {
            //        Id = 2,
            //        Nombre = "Marina"
            //    }
            //};
            var autoresBD = await context.Autores.ToListAsync();
            var autoresDTO = mapper.Map<List<AutorDTO>>(autoresBD);

            return autoresDTO;

        }

        [HttpGet("{id:int}", Name = "AutorById")]
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

        [HttpGet("{nombre}")]
        public async Task<ActionResult<List<AutorDTO>>> GetById([FromRoute] string nombre)
        {
            var autoresBD = await context.Autores.Where(x => x.Nombre.Contains(nombre)).ToListAsync();

            if (autoresBD.Count == 0)
            {
                return NotFound($"No existe ningún Autor con el nombre '{nombre}' en la BD..");
            }

            var autoresDTO = mapper.Map<List<AutorDTO>>(autoresBD);

            return Ok(autoresDTO);
        }

        [HttpPost]
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

        [HttpPut("{id:int}")]
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

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var autorBd = await context.Autores.FirstOrDefaultAsync(x => x.Id == id);

            if (autorBd == null)
            {
                return NotFound();
            }

            context.Remove(new Autor() { Id = id });
            await context.SaveChangesAsync();

            return Ok();
        }
    }
}
