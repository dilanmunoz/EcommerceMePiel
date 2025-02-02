using EcommerceMePiel.Datos;
using EcommerceMePiel.Modelos;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using EcommerceMePiel.Filtros;

var builder = WebApplication.CreateBuilder(args);

//Aqui se configura el cache
builder.Services.AddResponseCaching();

//Aqui se configuran los CORS
builder.Services.AddCors(p => p.AddPolicy("politicaCors", build =>
{
    build.WithOrigins("http://localhost:5046", "https://localhost:7237", "http://172.16.101.20:81", "http://localhost:81")
         .AllowAnyMethod()
         .AllowAnyHeader()
         .AllowCredentials(); // No uses AllowAnyOrigin()
}));

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = null; // Para respetar may�sculas
    });

// Configuraci�n de tama�o m�ximo para solicitudes (opcional)
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 500000000; // Ajusta el l�mite seg�n tus necesidades
});

builder.Services.AddEndpointsApiExplorer();


builder.Services.AddSwaggerGen(c =>
{
    //c.SwaggerDoc("v1", new OpenApiInfo { Title = "Mi API", Version = "v1" });
    c.EnableAnnotations(); // Aseg�rate de habilitar anotaciones
    // Agregar encabezado personalizado
    //c.OperationFilter<AddCustomHeaderOperationFilter>();
    //c.OperationFilter<Authorization>();
    // Puedes agregar m�s configuraciones de Swagger aqu� si es necesario
    c.AddSecurityDefinition("Authorization", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Por favor ingresa el token con el prefijo {token}",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer"

    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Authorization"
                }
            },
            new string[] {}
        }
    });
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1.0",
        Title = "Api E-Commerce Mepiel",
        Description = "Primera version",
        //TermsOfService = new Uri("https://google.com.mx"),
        Contact = new OpenApiContact
        {
            Name = "Abraham Jimenez",
            Email = "abraham.jimenez@mepiel.com.mx",

        },
        License = new OpenApiLicense
        {
            Name = "Licencia Personal",
            //Url = new Uri("https://google.com.mx")
        }
    });
});

// Agregar la compresi�n de respuestas
//builder.Services.AddResponseCompression(options =>
//{
//    options.EnableForHttps = true; // Habilitar para HTTPS
//    options.MimeTypes = new[] { "text/plain", "text/json", "application/json" }; // Tipos MIME que se comprimir�n
//});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{

    app.UseDeveloperExceptionPage();

}

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Api V1");
});

//// Habilitar la compresi�n
//app.UseResponseCompression();

//app.UseHttpsRedirection();

//soporte para CORS
app.UseCors("politicaCors");

app.UseRouting();

app.UseAuthorization();

app.MapControllers();

app.Run();



// Filtro para agregar encabezados personalizados en Swagger
//public class AddCustomHeaderOperationFilter : IOperationFilter
//{
//    public void Apply(OpenApiOperation operation, OperationFilterContext context)
//    {
//        operation.Parameters.Add(new OpenApiParameter
//        {
//            Name = "Authorization",
//            In = ParameterLocation.Header,
//            Required = false, // Cambia a true si es obligatorio
//            Description = "Token personalizado",
//            Schema = new OpenApiSchema { Type = "string" }
//        });
//    }
//}