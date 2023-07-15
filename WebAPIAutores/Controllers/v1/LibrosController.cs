using AutoMapper;
using Azure;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPIAutores.DTOs;
using WebAPIAutores.Models;

namespace WebAPIAutores.Controllers.v1
{
    [ApiController]
    [Route("api/v1/Libros")]
    public class LibrosController : ControllerBase
    {
        private readonly AppDBContext context;
        private readonly IMapper mapper;

        public LibrosController(AppDBContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet("{id:int}", Name = "LibroById")]
        public async Task<ActionResult<LibroDTOconAutores>> Get(int id)
        {
            Libro libroBd = await context.Libros.Include(x => x.Comentarios).Include(x => x.AutoresLibros).ThenInclude(x => x.Autor).FirstOrDefaultAsync(x => x.Id == id);

            if (libroBd == null)
            {
                return NotFound($"El libro con el Id: {id} no existe en la BD..");
            }

            libroBd.AutoresLibros = libroBd.AutoresLibros.OrderBy(x => x.Orden).ToList();

            return mapper.Map<LibroDTOconAutores>(libroBd);
        }

        [HttpPost(Name = "postLibro")]
        public async Task<ActionResult> Post(LibroCreacionDTO libroCreacionDTO)
        {
            if (libroCreacionDTO.AutoresIds == null)
            {
                return BadRequest("No se puede crear un libro sin su Autor/es..");

            }
            var autoresIds = await context.Autores.Where(x => libroCreacionDTO.AutoresIds.Contains(x.Id)).Select(x => x.Id).ToListAsync();

            if (libroCreacionDTO.AutoresIds.Count != autoresIds.Count)
            {
                return BadRequest("No todos los Ids de los autores existen en la BD..");
            }

            var libro = mapper.Map<Libro>(libroCreacionDTO);

            OrdenAutores(libro);

            context.Libros.Add(libro);
            await context.SaveChangesAsync();

            var libroDTO = mapper.Map<LibroDTO>(libro);

            return CreatedAtRoute("LibroById", new { id = libro.Id }, libroDTO);
        }

        [HttpPut("{id:int}", Name = "putLibro")]
        public async Task<ActionResult> Put(int id, LibroCreacionDTO libroCreacionDTO)
        {
            var libroBD = await context.Libros.Include(x => x.AutoresLibros).FirstOrDefaultAsync(x => x.Id == id);

            if (libroBD == null)
            {
                return NotFound($"No existe en la BD un libro con el id: {id}");
            }

            libroBD = mapper.Map(libroCreacionDTO, libroBD);

            OrdenAutores(libroBD);

            await context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPatch(Name = "patchLibro")]
        public async Task<ActionResult> Patch(int libroId, JsonPatchDocument<LibroPatchDTO> patchDocument)
        {
            if (patchDocument == null)
            {
                return BadRequest();
            }

            var libroBD = await context.Libros.FirstOrDefaultAsync(x => x.Id == libroId);

            if (libroBD == null)
            {
                return NotFound();
            }

            var libroPatchDTO = mapper.Map<LibroPatchDTO>(libroBD);

            patchDocument.ApplyTo(libroPatchDTO, ModelState);

            var esValido = TryValidateModel(libroPatchDTO);

            if (esValido == false)
            {
                return BadRequest(ModelState);
            }

            mapper.Map(libroPatchDTO, libroBD);
            await context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete(Name = "deleteLibro")]
        public async Task<ActionResult> Delete(int libroId)
        {
            var libroBd = await context.Libros.FirstOrDefaultAsync(x => x.Id == libroId);

            if (libroBd == null)
            {
                return NotFound();
            }

            context.Remove(new Libro() { Id = libroId });
            await context.SaveChangesAsync();

            return NoContent();
        }

        private void OrdenAutores(Libro libro)
        {
            if (libro.AutoresLibros != null)
            {
                for (int i = 0; i < libro.AutoresLibros.Count; i++)
                {
                    libro.AutoresLibros[i].Orden = i;
                }
            }
        }
    }

}
