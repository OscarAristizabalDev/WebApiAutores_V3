using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAutores.DTOs;
using WebApiAutores.Entidades;

namespace WebApiAutores.Controllers;

[ApiController]
[Route("api/libros")] // ruta
public class LibrosController : ControllerBase
{
    private readonly ApplicationDbContext context;
    private readonly IMapper mapper;

    public LibrosController(ApplicationDbContext context, IMapper mapper)
    {
        this.context = context;
        this.mapper = mapper;
    }

    [HttpGet("{id:int}", Name = "ObtenerLibro")]
    public async Task<ActionResult<LibroDTOConAutores>> Get(int id)
    {
        // Permite consultar un libro incluyendo sus autores
        var libro = await context.Libros
            .Include(libroBD => libroBD.AutoresLibros)
            .ThenInclude(autorLibroDB => autorLibroDB.Autor)
            .FirstOrDefaultAsync(x => x.Id == id);

        // Permite consulta un libro sin incluir sus comentarios
        //var libro = await context.Libros.FirstOrDefaultAsync(x => x.Id == id);

        // Si no existe el autor
        if (libro == null)
        {
            // Retornar un 404
            return NotFound();
        }
        // Se ordena de manera ascendentes los autores del libro
        libro.AutoresLibros = libro.AutoresLibros.OrderBy(x => x.Orden).ToList();

        return mapper.Map<LibroDTOConAutores>(libro);
    }

    [HttpPost]
    public async Task<ActionResult> Post(LibroCreacionDTO libroCreacionDTO)
    {
        //var existeAutor = await context.Autores.AnyAsync(x => x.Id == libro.AutorId);
        //if (!existeAutor)
        //{
        //    return BadRequest($"No existe el autor de Id: {libro.AutorId}");
        //}

        // Se valida que el libro a crear tenga autores
        if (libroCreacionDTO.AutoresIds == null)
        {
            return BadRequest("No se puede crear un libro sin autores");
        }

        // Se consulta en la tabla de autores donde el Id de autores se encuentre en el listado
        // de autores "contains" y solo traiga el id del autor "select"
        var aurotesIds = await context.Autores.
            Where(autorBD => libroCreacionDTO.AutoresIds.Contains(autorBD.Id)).Select(x => x.Id).ToListAsync();

        // Se valida que los autores enviados en libro a crear existan en el listado actual de autores
        if (libroCreacionDTO.AutoresIds.Count != aurotesIds.Count)
        {
            return BadRequest("No existe uno de los autores enviados");
        }

        // Se realiza el mapeo 
        var libro = mapper.Map<Libro>(libroCreacionDTO);

        AsignarOrdenAutores(libro);

        // Se agrega el libro a crear
        context.Add(libro);
        // Se guardan los cambios en base de datos
        await context.SaveChangesAsync();
        // Se mapea de libro hacia LibroDTO
        var libroDTO = mapper.Map<LibroDTO>(libro);
        // Se retorna el libroDTO, con la ubicación del recurso
        return CreatedAtRoute("ObtenerLibro", new { id = libro.Id }, libroDTO);
    }

    /// <summary>
    /// Permite actualizar la entidad libro y sus autores libros
    /// </summary>
    /// <param name="id"></param>
    /// <param name="libroCreacionDTO"></param>
    /// <returns></returns>
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Put(int id, LibroCreacionDTO libroCreacionDTO)
    {
        // Se trea el libro por id, incluyendo los autores libros
        var libroDB = await context.Libros
            .Include(x => x.AutoresLibros)
            .FirstOrDefaultAsync(x => x.Id == id);

        // Si el libro no existe
        if(libroDB == null)
        {
            return NotFound();
        }
        // Se usa auto mapper para mapear las propiedades de libroCreacionDTO
        // a libroDB, utilizando la misma instancia
        libroDB = mapper.Map(libroCreacionDTO, libroDB);

        AsignarOrdenAutores(libroDB);

        await context.SaveChangesAsync();
        return Ok();
    }

    [HttpPatch("{id:int}")]
    public async Task<IActionResult> Patch(int id, JsonPatchDocument<LibroPatchDTO> patchDocument)
    {
        if(patchDocument == null)
        {
            return BadRequest();
        }
        // Se busca el libro actualizar
        var libroDB = await context.Libros.FirstOrDefaultAsync(x => x.Id == id);
    
        if(libroDB == null)
        {
            return NotFound();
        }

        // Se hace el mapeo de libroDB a LibroPatchDO
        var libroDTO = mapper.Map<LibroPatchDTO>(libroDB);
        // Se le aplica al libroDTO los cambios que llegan del patchDocument
        patchDocument.ApplyTo(libroDTO, ModelState);
        // Se valida que las reglas de validacion se esten cumpliendo
        var esValido = TryValidateModel(libroDTO);

        if (!esValido)
        {
            return BadRequest(ModelState);
        }
        // Se realiza el mapeo desde el LibroDTO hasta LibroDB
        mapper.Map(libroDTO, libroDB);

        await context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete(int id)
    {
        // Se busca el autor por Id
        var existe = context.Libros.Any(x => x.Id == id);
        // Si no existe 
        if (!existe)
        {
            return NotFound();
        }
        // Para remove un dato en base de datos, se debe enviar una instancia de ese objeto
        context.Remove(new Libro() { Id = id });
        // Se guardan los cambios
        await context.SaveChangesAsync();
        return Ok();
    }


    private void AsignarOrdenAutores(Libro libro)
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
