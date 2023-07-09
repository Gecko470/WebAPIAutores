using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPIAutores.DTOs;
using WebAPIAutores.Models;

namespace WebAPIAutores.Controllers
{
    [ApiController]
    [Route("api/libros/{libroId:int}/comentarios")]
    public class ComentariosController : ControllerBase
    {
        private readonly AppDBContext context;
        private readonly IMapper mapper;

        public ComentariosController(AppDBContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<ComentarioDTO>>> Get(int libroId)
        {
            var existeLibro = await context.Libros.AnyAsync(x => x.Id == libroId);

            if (!existeLibro)
            {
                return NotFound($"No existe un libro en la BD con el Id: {libroId}");
            }

            var comentarios = await context.Comentarios.Where(x => x.LibroId == libroId).ToListAsync();

            return mapper.Map<List<ComentarioDTO>>(comentarios);
        }

        [HttpGet("{id:int}", Name = "ComentarioById")]
        public async Task<ActionResult<ComentarioDTO>> GetById(int id)
        {
            var comentario = await context.Comentarios.FirstOrDefaultAsync(x => x.Id == id);

            if (comentario == null)
            {
                return NotFound($"No existe en la BD un comentario con el Id: {id}..");
            }

            var comentarioDTO = mapper.Map<ComentarioDTO>(comentario);

            return comentarioDTO;
        }

        [HttpPost]
        public async Task<ActionResult> Post(int libroId, ComentarioCreacionDTO comentarioCreacionDTO)
        {
            var existeLibro = await context.Libros.AnyAsync(x => x.Id == libroId);

            if (!existeLibro)
            {
                return NotFound($"No existe un libro en la BD con el Id: {libroId}");
            }

            var comentario = mapper.Map<Comentario>(comentarioCreacionDTO);

            comentario.LibroId = libroId;

            context.Add(comentario);
            await context.SaveChangesAsync();

            var comentarioDTO = mapper.Map<ComentarioDTO>(comentario);

            return CreatedAtRoute("ComentarioById", new { id = comentario.Id, libroId = libroId }, comentarioDTO);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(ComentarioCreacionDTO comentarioCreacionDTO, int libroId, int id)
        {
            var existeLibro = await context.Libros.AnyAsync(x => x.Id == libroId);

            if (!existeLibro)
            {
                return NotFound($"No existe un libro en la BD con el Id: {libroId}");
            }

            var existeComentario = await context.Comentarios.AnyAsync(x => x.Id == id);

            if (!existeComentario)
            {
                return NotFound($"No existe un Comentario en la BD con el Id: {id}");
            }

            var comentario = mapper.Map<Comentario>(comentarioCreacionDTO);
            comentario.Id = id;
            comentario.LibroId = libroId;

            context.Update(comentario);
            await context.SaveChangesAsync();

            return NoContent();
        }
    }
}
