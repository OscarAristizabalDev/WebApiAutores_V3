
using AutoMapper;
using WebApiAutores.DTOs;
using WebApiAutores.Entidades;

namespace WebApiAutores.Utilidades;
public class AutoMapperProfiles: Profile
{
    public AutoMapperProfiles()
    {
        // Se confirgura el mapeo de la clase AutorCreacionDTO
        // a Autor
        CreateMap<AutorCreacionDTO, Autor>();
        // Mapear de la clase autor a autorDTO
        CreateMap<Autor, AutorDTO>();
        // Mapear la clase autor con AutorDTO
        CreateMap<Autor, AutorDTOConLibros>()
            .ForMember(autorDTO => autorDTO.libros
                , opciones => opciones.MapFrom(MapAutorDTOLibros));

        // Mapeo de LibroCreacionDTO a Libro
        CreateMap<LibroCreacionDTO, Libro>()
            .ForMember(libro => libro.AutoresLibros
                , opciones => opciones.MapFrom(MapAutoresLibros));
        // Mapeo de libro a libroDTO
        CreateMap<Libro, LibroDTO>();
        // Mapeo de libro con LibroDTOconAutores
        CreateMap<Libro, LibroDTOConAutores>()
            .ForMember(LibroDTO => LibroDTO.Autores
                , opciones => opciones.MapFrom(MapLibroDTOAutores));
        CreateMap<LibroPatchDTO, Libro>().ReverseMap();

        // Mapeo desde ComentarioCreacionDTO hasta comentario
        CreateMap<ComentarioCreacionDTO, Comentario>();
        // Mapeo desde comentario hasta comentarioDTO
        CreateMap<Comentario, ComentarioDTO>();
    }

    private List<LibroDTO> MapAutorDTOLibros(Autor autor, AutorDTO autorDTO)
    {
        // valores a retornar
        var resultado = new List<LibroDTO>();

        // Si el autor no tiene libros
        if(autor.AutoresLibros == null) { return resultado; }

        foreach(var autorLibro in autor.AutoresLibros)
        {
            resultado.Add(new LibroDTO()
            {
                Id = autorLibro.LibroId,
                Titulo = autorLibro.Libro.Titulo
            }) ;
        }

        return resultado;
    }

    /// <summary>
    /// Permite mapear los autores de un libro
    /// </summary>
    /// <param name="libro"></param>
    /// <param name="libroDTO"></param>
    /// <returns></returns>
    private List<AutorDTO> MapLibroDTOAutores(Libro libro, LibroDTO libroDTO)
    {
        // Variable a retornar
        var resultado = new List<AutorDTO>();
        // En caso se no existir autores en el libro
        if(libro.AutoresLibros == null) { return resultado; }
        // Se iteran los autores que tiene el libto
        foreach(var autorLibro in libro.AutoresLibros) 
        {
            // Se agrega un nuevo objeto de Autor DTO al listado
            resultado.Add(new AutorDTO()
            {
                Id = autorLibro.AutorId,
                Nombre = autorLibro.Autor.Nombre
            }) ;
        }
        return resultado;
    }

    /// <summary>
    /// Permite mapear las autores libros
    /// </summary>
    /// <param name="libroCreacionDTO"></param>
    /// <param name="libro"></param>
    /// <returns></returns>
    private List<AutorLibro> MapAutoresLibros(LibroCreacionDTO libroCreacionDTO, Libro libro)
    {
        var resultado = new List<AutorLibro>(); 

        // Si se esta creando un libro sin autores
        if(libroCreacionDTO.AutoresIds == null)
        {
            return resultado;
        }

        foreach(var autorId in libroCreacionDTO.AutoresIds) 
        {
            resultado.Add(new AutorLibro()
            {
                AutorId = autorId
            });
        }

        return resultado;
    }
}
