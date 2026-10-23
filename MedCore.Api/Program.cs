using MedCore.Business.Interfaces;
using MedCore.Business.Servicos;
using MedCore.Infraestrutura.Contexto;
using MedCore.Infraestrutura.Repositorios;
using MedCore.Infraestrutura.UoW;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Controllers
builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Banco de dados
var conexao = builder.Configuration
    .GetConnectionString("MedCore");

builder.Services.AddDbContext<MedCoreContexto>(opcoes =>
    opcoes.UseSqlServer(conexao));

// Injeção de dependência
builder.Services.AddScoped<IPacienteRepositorio, PacienteRepository>();
builder.Services.AddScoped<IUnityOfWork, UnityOfWork>();
builder.Services.AddScoped<IPacienteService, PacienteService>();

var app = builder.Build();

// Pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();