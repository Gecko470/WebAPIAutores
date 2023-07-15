using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPIAutores.DTOs;
using WebAPIAutores.Models;

namespace WebAPIAutores.Controllers.v1
{
    [ApiController]
    [Route("api/v1/libros/{libroId:int}/comentarios")]
    public class ComentariosController : ControllerBase
    {
        private readonly AppDBContext context;
        private readonly IMapper mapper;
        private readonly UserManager<IdentityUser> userManager;

        public ComentariosController(AppDBContext context, IMapper mapper, UserManager<IdentityUser> userManager)
        {
            this.context = context;
            this.mapper = mapper;
            this.userManager = userManager;
        }

        [HttpGet(Name = "getComentarios")]
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

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost(Name = "postComentario")]
        public async Task<ActionResult> Post(int libroId, ComentarioCreacionDTO comentarioCreacionDTO)
        {
            var emailClaim = HttpContext.User.Claims.Where(x => x.Type == "Email").FirstOrDefault();
            var email = emailClaim.Value;
            var usuario = await userManager.FindByEmailAsync(email);
            var usuarioId = usuario.Id;

            var existeLibro = await context.Libros.AnyAsync(x => x.Id == libroId);

            if (!existeLibro)
            {
                return NotFound($"No existe un libro en la BD con el Id: {libroId}");
            }

            var comentario = mapper.Map<Comentario>(comentarioCreacionDTO);

            comentario.LibroId = libroId;
            comentario.UsuarioId = usuarioId;

            context.Add(comentario);
            await context.SaveChangesAsync();

            var comentarioDTO = mapper.Map<ComentarioDTO>(comentario);

            return CreatedAtRoute("ComentarioById", new { id = comentario.Id, libroId }, comentarioDTO);
        }

        [HttpPut("{id:int}", Name = "putComentario")]
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
