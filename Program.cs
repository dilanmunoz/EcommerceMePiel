using EcommerceMePiel.Datos;
using EcommerceMePiel.Modelos;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

var builder = WebApplication.CreateBuilder(args);

//Aqui se configura el cache
builder.Services.AddResponseCaching();

//Aqui se configuran los CORS
builder.Services.AddCors(p => p.AddPolicy("politicaCors", build =>
{
    //http://localhost:3223
    build.WithOrigins("http://localhost:5046", "https://localhost:7237","http://172.16.101.20:81", "http://localhost:81").AllowAnyMethod().AllowAnyHeader().AllowCredentials();
}));

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = null; // Para respetar mayúsculas
    });

// Configuración de tamaño máximo para solicitudes (opcional)
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 500000000; // Ajusta el límite según tus necesidades
});

builder.Services.AddEndpointsApiExplorer();


builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Mi API", Version = "v1" });
    c.EnableAnnotations(); // Asegúrate de habilitar anotaciones
    // Agregar encabezado personalizado
    c.OperationFilter<AddCustomHeaderOperationFilter>();
    // Puedes agregar más configuraciones de Swagger aquí si es necesario
    //c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    //{
    //    In = ParameterLocation.Header,
    //    Description = "Por favor ingresa el token con el prefijo 'Bearer '",
    //    Name = "Authorization",
    //    Type = SecuritySchemeType.ApiKey
    //});
    //c.AddSecurityRequirement(new OpenApiSecurityRequirement
    //{
    //    {
    //        new OpenApiSecurityScheme
    //        {
    //            Reference = new OpenApiReference
    //            {
    //                Type = ReferenceType.SecurityScheme,
    //                Id = "Bearer"
    //            }
    //        },
    //        new string[] {}
    //    }
    //});
});

// Agregar la compresión de respuestas
//builder.Services.AddResponseCompression(options =>
//{
//    options.EnableForHttps = true; // Habilitar para HTTPS
//    options.MimeTypes = new[] { "text/plain", "text/json", "application/json" }; // Tipos MIME que se comprimirán
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

//// Habilitar la compresión
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