using Microsoft.EntityFrameworkCore;
using WebApiProducto.Data;
using WebApiProducto.Services;
using DotNetEnv;

// Cargar variables de entorno desde .env
DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

// Obtener la cadena de conexión desde las variables de entorno
var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");

// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddScoped<IProductoService, ProductoService>();

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
