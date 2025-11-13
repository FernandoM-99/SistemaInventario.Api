using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using SistemaInventario.Api.Data;

// Definición de la política de CORS
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

var builder = WebApplication.CreateBuilder(args);

// --- 1. Servicio de CORS ---
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy =>
                      {
                          policy.WithOrigins("http://localhost:3000")
                                .AllowAnyHeader()
                                .AllowAnyMethod();
                      });
});

// --- 2. Cadena de Conexión y DbContext ---
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// --- 3. Controladores y JSON (Manejo de Ciclos) ---
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });


// --- 4. SERVICIOS DE SWAGGER 
builder.Services.AddSwaggerGen();
builder.Services.AddEndpointsApiExplorer();


// --- FIN DE LA CONFIGURACIÓN DE SERVICIOS ---
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // app.UseSwagger() necesita el servicio registrado arriba
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Usar CORS (debe ir ANTES de MapControllers)
app.UseCors(MyAllowSpecificOrigins);

app.MapControllers();

app.Run();