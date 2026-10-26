using FluentValidation;
using MedCore.Business.DTOs.Request;
using MedCore.Business.Interfaces;
using MedCore.Business.Servicos;
using MedCore.Business.Validadores;
using MedCore.Infraestrutura.Contexto;
using MedCore.Infraestrutura.Repositorios;
using MedCore.Infraestrutura.UoW;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

// Controllers
builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Informe o token JWT."
    });

    options.AddSecurityRequirement(document =>
        new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference(
                "Bearer", document)] = []
        });
});
// Banco de dados
var conexao = builder.Configuration
    .GetConnectionString("MedCore");

builder.Services.AddDbContext<MedCoreContexto>(opcoes =>
    opcoes.UseSqlServer(conexao));

// Injeção de dependência
builder.Services.AddScoped<IPacienteRepositorio, PacienteRepository>();
builder.Services.AddScoped<IUnityOfWork, UnityOfWork>();
builder.Services.AddScoped<IPacienteService, PacienteService>();
builder.Services.AddScoped<
    IValidator<PacienteRequest>,
    PacienteValidator>();
builder.Services.AddScoped<IColetorErros, ColetorErros>();
builder.Services.AddScoped<ValidadorService>();
builder.Services.AddScoped<IRespostaPadrao, RespostaPadrao>();

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer();

builder.Services.AddAuthorization();

var app = builder.Build();

// Pipeline HTTP
// Pipeline HTTP
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();