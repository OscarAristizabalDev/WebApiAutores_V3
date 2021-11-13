
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using WebApiAutores.Filtros;
using WebApiAutores.Middlewares;

namespace WebApiAutores;
public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }
    public IConfiguration Configuration {  get; set; }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers(opciones =>
        {
            // permite agregar un filtro de manera global
            // es decir, se va a ejecutar en todos los controladores y
            // se ejecuta cuando se capture una excepción
            //opciones.Filters.Add(typeof(FiltroDeExcepcion));
        }).AddJsonOptions(x =>
            x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles).AddNewtonsoftJson();
        // se configura el servicio para conexión y manipulación de la base de datos
        services.AddDbContext<ApplicationDbContext>(options 
            => options.UseSqlServer(Configuration.GetConnectionString("defaultConnection")));

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();

        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new() { Title = "WebApiAutores", Version = "v1" });
        });
        // Se agrega el servicio de automapper para el mapeo entre clases
        services.AddAutoMapper(typeof(Startup));

        // Se configura el servicio de AddIdentity
        services.AddIdentity<IdentityUser, IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();
    }
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
    {
        // Permite crear un middleware personalizado
        //  Use indica que se puede ejecutar los demás middleware
        app.UseLoguearRespuestaHTTP();

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebApiAutores v1"));
        }
        app.UseDeveloperExceptionPage();

        app.UseHttpsRedirection();

        app.UseRouting();
        
        // Permita aplicar autorización para accerder a los servicios
        app.UseAuthorization();
        // Permite acceder a los EndPoints (Servicios) de la aplicación
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();   
        });
    }
}
