using MedCore.Business.DTOs.Request;
using MedCore.Business.Servicos;
using MedCore.Dominio;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Identity;

namespace MedCore.Test.Services;

public class UsuarioServiceTests
{
    [Fact]
    public async Task CadastrarPaciente_CriaUsuarioComCpfNumericoEPerfil()
    {
        var fixture = new ServiceTestFixture();
        var service = CriarService(fixture);

        var resultado = await service.CadastrarPaciente(CriarRequest());

        Assert.NotNull(resultado);
        Assert.NotNull(resultado.Id);
        Assert.Equal("Ana Souza", resultado.Nome);
        Assert.Equal("ana@example.com", resultado.Email);
        var usuario = await fixture.Uow.Usuarios.Buscar(
            new BuscarUsuarioRequest { Email = "ana@example.com" });
        Assert.NotNull(usuario);
        Assert.Equal("52998224725", usuario.CPF);
        Assert.NotNull(await fixture.Uow.Pacientes.BuscarPorUsuarioId(usuario.Id));
        Assert.Equal(1, fixture.Uow.Confirmacoes);
    }

    [Fact]
    public async Task CadastrarMedico_QuandoContaJaTemPerfilPaciente_ReutilizaUsuario()
    {
        var fixture = new ServiceTestFixture();
        var service = CriarService(fixture);
        var requestPaciente = CriarRequest();
        var paciente = await service.CadastrarPaciente(requestPaciente);

        var medico = await service.CadastrarMedico(new MedicoRequest
        {
            Nome = requestPaciente.Nome,
            Email = requestPaciente.Email,
            CPF = "529.982.247-25",
            DataNascimento = requestPaciente.DataNascimento,
            Senha = requestPaciente.Senha,
            CRM = "123456",
            UF = "sp",
            Especialidade = "Cardiologia"
        });

        Assert.NotNull(paciente);
        Assert.NotNull(medico);
        Assert.Equal("123456", medico.CRM);
        Assert.Equal("SP", medico.UF);
        var usuario = await fixture.Uow.Usuarios.Buscar(
            new BuscarUsuarioRequest { Email = requestPaciente.Email });
        Assert.NotNull(usuario);
        Assert.Equal(paciente.Id, (await fixture.Uow.Pacientes.BuscarPorUsuarioId(usuario.Id))!.Id);
        Assert.NotNull(await fixture.Uow.Medicos.BuscarPorUsuarioId(usuario.Id));
        Assert.Equal(2, fixture.Uow.Confirmacoes);
    }

    [Fact]
    public async Task CadastrarPaciente_QuandoPerfilJaExiste_RejeitaSemConfirmar()
    {
        var fixture = new ServiceTestFixture();
        var service = CriarService(fixture);
        var request = CriarRequest();
        Assert.NotNull(await service.CadastrarPaciente(request));
        fixture.Errors.LimparErros();

        var resultado = await service.CadastrarPaciente(request);

        Assert.Null(resultado);
        Assert.Equal(1, fixture.Uow.Confirmacoes);
        Assert.Contains(
            "Este usuário já possui cadastro de paciente.",
            fixture.Errors.GerarErros());
    }

    [Fact]
    public async Task CadastrarMedico_CrmInvalidoNaoCriaPerfil()
    {
        var fixture = new ServiceTestFixture();
        var service = CriarService(fixture);

        var resultado = await service.CadastrarMedico(new MedicoRequest
        {
            Nome = "Ana Souza",
            Email = "ana@example.com",
            CPF = "52998224725",
            DataNascimento = new DateTime(1990, 1, 1),
            Senha = "SenhaSegura123",
            CRM = "",
            UF = "SP",
            Especialidade = "Cardiologia"
        });

        Assert.Null(resultado);
        Assert.Null(await fixture.Uow.Usuarios.Buscar(
            new BuscarUsuarioRequest { Email = "ana@example.com" }));
        Assert.Equal(0, fixture.Uow.Confirmacoes);
        Assert.Contains("O CRM é obrigatório.", fixture.Errors.GerarErros());
    }

    [Fact]
    public async Task Login_ComCredenciaisValidas_RetornaTokenJwt()
    {
        var fixture = new ServiceTestFixture();
        var service = CriarService(fixture);
        await service.CadastrarPaciente(CriarRequest());

        var resposta = await service.Login(new()
        {
            Email = "ana@example.com",
            Senha = "SenhaSegura123"
        });

        Assert.NotNull(resposta);
        Assert.False(string.IsNullOrWhiteSpace(resposta.AccessToken));
        Assert.True(resposta.ExpiraEm > DateTime.UtcNow);
        Assert.False(fixture.Errors.PossuiErros);
    }

    [Fact]
    public async Task Login_ComSenhaIncorreta_RejeitaCredenciais()
    {
        var fixture = new ServiceTestFixture();
        var service = CriarService(fixture);
        await service.CadastrarPaciente(CriarRequest());
        fixture.Errors.LimparErros();

        var resposta = await service.Login(new()
        {
            Email = "ana@example.com",
            Senha = "SenhaErrada123"
        });

        Assert.Null(resposta);
        Assert.Contains("E-mail ou senha inválidos.", fixture.Errors.GerarErros());
    }

    private static UsuarioService CriarService(ServiceTestFixture fixture)
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Key"] = "chave-de-teste-com-tamanho-suficiente-para-assinatura",
                ["Jwt:Issuer"] = "MedCore.Tests",
                ["Jwt:Audience"] = "MedCore.Tests.Client",
                ["Jwt:ExpirationMinutes"] = "30"
            })
            .Build();

        return new UsuarioService(fixture.Uow, fixture.Validator, config);
    }

    private static UsuarioRequest CriarRequest() => new()
    {
        Nome = "Ana Souza",
        Email = "ana@example.com",
        CPF = "529.982.247-25",
        DataNascimento = new DateTime(1990, 1, 1),
        Senha = "SenhaSegura123"
    };
}
