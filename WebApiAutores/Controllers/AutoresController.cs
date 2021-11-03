
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using WebApiAutores.DTOs;
using WebApiAutores.Entidades;
using WebApiAutores.Filtros;

namespace WebApiAutores.Controllers;

[ApiController]
[Route("api/autores")] // ruta 
public class AutoresController: ControllerBase
{
    private readonly ApplicationDbContext context;
    private readonly IMapper mapper;
    private readonly IConfiguration configuration;

    public AutoresController(ApplicationDbContext _context, IMapper mapper,
        IConfiguration configuration)
    {
        this.context = _context;
        this.mapper = mapper;
        this.configuration = configuration;
    }

    [HttpGet("configuraciones")]
    public ActionResult<string> ObtenerConfiguracion()
    {
        //return configuration["connectionStrings:defaultConnection"];
        return configuration["Apellido"];
    }

    // devuelve un listado de autores
    [HttpGet] // api/autores
    public async Task<List<AutorDTO>> Get()
    {
        // Se buscan los autores incluyendos los libros
        // return await context.Autores.Include(x => x.Libros).ToListAsync();
        var autores = await context.Autores.ToListAsync();
        return mapper.Map<List<AutorDTO>>(autores);
    }

    [HttpGet("{id:int}", Name = "obtenerAutor")] // indica que el Id ingresado debe ser un entero
    public async Task<ActionResult<AutorDTOConLibros>> Get(int id)
    {
        // Se busca el autor por el Id con sus libros
        var autor = await context.Autores
            .Include(autorDB => autorDB.AutoresLibros)
            .ThenInclude(autorLibroDB => autorLibroDB.Libro)
            .FirstOrDefaultAsync(autorBD => autorBD.Id == id);
        // Si no existe el autor
        if(autor == null)
        {
            // Retornar un 404
            return NotFound();
        }
        return mapper.Map<AutorDTOConLibros>(autor);
    }

    [HttpGet("{nombre}")] // indica que el Id ingresado debe ser un entero
    // [FromRoute] indica que el valor viene desde la ruta
    public async Task<ActionResult<List<AutorDTO>>> Get([FromRoute] string nombre)
    {
        // Se busca el autor cuyo nombre contenga el valor ingresado
        var autores = await context.Autores.Where(autorBD => autorBD.Nombre.Contains(nombre)).ToListAsync();

        return mapper.Map<List<AutorDTO>>(autores);
    }

    [HttpPost]
    // [FromBody] indica que el valor viene desde el cuerpo de la petición http
    public async Task<ActionResult> Post([FromBody] AutorCreacionDTO autorCreacionDTO)
    {
        var existeAutor = await context.Autores.AnyAsync(x => x.Nombre == autorCreacionDTO.Nombre);
        // Si existe el autor
        if (existeAutor)
        {
            return BadRequest($"Ya existe un autor con el nombre {autorCreacionDTO.Nombre}");
        }

        // se mapen el DTO autorCreacionDTO a la autor
        var autor = mapper.Map<Autor>(autorCreacionDTO);

        // Paso previo para insertar en base de datos
        context.Add(autor);
        // Se guardan los cambios
        await context.SaveChangesAsync();

        var autorDTO = mapper.Map<AutorDTO>(autor);
        // Se retorna el objeto AutorDTO, y se indica la ruta del recurso
        return CreatedAtRoute("obtenerAutor", new { id = autor.Id}, autorDTO);
    }

    [HttpPut("{id:int}")] //"api/autores/1
    public async Task<ActionResult> Put(AutorCreacionDTO autorCreacionDTO, int id)
    {
        // Se busca el autor por Id
        var existe = context.Autores.Any(x => x.Id == id);
        // Si no existe 
        if (!existe)
        {
            return NotFound();
        }

        var autor = mapper.Map<Autor>(autorCreacionDTO);
        autor.Id = id;

        context.Update(autor);
        // Se guardan los cambios
        await context.SaveChangesAsync();
        return Ok();
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete(int id)
    {
        // Se busca el autor por Id
        var existe = context.Autores.Any(x => x.Id == id);
        // Si no existe 
        if (!existe)
        {
            return NotFound();
        }
        // Para remove un dato en base de datos, se debe enviar una instancia de ese objeto
        context.Remove(new Autor() { Id = id });
        // Se guardan los cambios
        await context.SaveChangesAsync();
        return Ok();
    }
}
