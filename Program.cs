var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Esto permite que la aplicación reconozca los controladores que creamos
builder.Services.AddControllers();

// Configuración de Swagger/OpenAPI para documentar y probar la API
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
// Habilitar Swagger siempre en entorno de desarrollo
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1");
        c.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

// Mapea las rutas a los controladores
app.MapControllers();

app.Run();

