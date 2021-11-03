using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAutores.DTOs;
using WebApiAutores.Entidades;

namespace WebApiAutores.Controllers;

[ApiController]
[Route("api/libros/{libroId:int}/comentarios")]
public class ComentariosController : ControllerBase
{
    private readonly ApplicationDbContext context;
    private readonly IMapper mapper;

    public ComentariosController(ApplicationDbContext context, IMapper mapper)
    {
        this.context = context;
        this.mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<List<ComentarioDTO>>> Get(int libroId)
    {
        var existeLibro = await context.Libros.AnyAsync(libroDB => libroDB.Id == libroId);

        if (!existeLibro)
        {
            return NotFound();
        }

        var comentarios = await context.Comentarios
            .Where(comentarioDB => comentarioDB.LibroId == libroId).ToListAsync();

        return mapper.Map<List<ComentarioDTO>>(comentarios);
    }

    [HttpGet("{id:int}", Name = "ObtenerComentario")]
    public async Task<ActionResult<ComentarioDTO>> GetComentarioByID(int id)
    {
        // Se busca el comentario Por ID
        var comentario = await context.Comentarios.FirstOrDefaultAsync(comentarioDB => comentarioDB.Id == id);
        // En caso de no existir
        if (comentario == null)
        {
            // Retornar un 404
            return NotFound();
        }
        // Se mapea de comentario a comentarioDTO
        var comentarioDTO = mapper.Map<ComentarioDTO>(comentario);
        // Se retorna el comentario
        return comentarioDTO;

    }

    [HttpPost]
    public async Task<ActionResult> Post(int libroId, ComentarioCreacionDTO comentarioCreacionDTO)
    {
        var existeLibro = await context.Libros.AnyAsync(libroDB => libroDB.Id == libroId);

        // Si el libro no exisite
        if (!existeLibro)
        {
            // Se retorno un 404
            return NotFound();
        }

        // Se realiza el mapeo de comentarioCreacionDTO a comentario
        var comentario = mapper.Map<Comentario>(comentarioCreacionDTO);
        comentario.LibroId = libroId;
        // Se agrega el comentario a guardar
        context.Add(comentario);
        // Se guardan los cambios en base de datos
        await context.SaveChangesAsync();
        // Se realiza el mapeo de comentario a comentario DTO
        var comentarioDTO = mapper.Map<ComentarioDTO>(comentario);
        // Se retorna el comentario creado, con la ruta de acceso
        return CreatedAtRoute("ObtenerComentario", new { id = comentario.Id, libroID = comentario.LibroId }, comentarioDTO);

    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult> Put(int libroId, int id, ComentarioCreacionDTO comentarioCreacionDTO)
    {
        var existeLibro = await context.Libros.AnyAsync(libroDB => libroDB.Id == libroId);

        // Si el libro no exisite
        if (!existeLibro)
        {
            // Se retorno un 404
            return NotFound();
        }

        var existeComentario = await context.Comentarios.AnyAsync(comentarioDB => comentarioDB.Id == id);

        if (!existeComentario)
        {
            return NotFound();
        }

        // Se realiza el mapeo de comentarioCreacionDTO a comentario
        var comentario = mapper.Map<Comentario>(comentarioCreacionDTO);
        comentario.Id = id;
        comentario.LibroId = libroId;
        context.Update(comentario);
        // Se guardan los cambios
        await context.SaveChangesAsync();

        return Ok();
    }

}
