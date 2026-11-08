using MedCore.Business.DTOs.Request;
using MedCore.Business.Servicos;
using MedCore.Dominio;

namespace MedCore.Test.Services;

public class MedicoServiceTests
{
    [Fact]
    public async Task BuscarPorCodigo_QuandoExiste_RetornaDadosDoMedico()
    {
        var fixture = new ServiceTestFixture();
        var medico = CriarMedico(fixture);
        var service = new MedicoService(fixture.Uow, fixture.Validator);

        var resultado = await service.BuscarPorCodigo(medico.Id);

        Assert.NotNull(resultado);
        Assert.Equal(medico.Id, resultado.Id);
        Assert.Equal("Ana Souza", resultado.Nome);
        Assert.Equal("123456", resultado.CRM);
        Assert.Equal("SP", resultado.UF);
        Assert.Equal("Cardiologia", resultado.Especialidade);
        Assert.True(resultado.Ativo);
    }

    [Fact]
    public async Task Buscar_RetornaMedicosFiltrados()
    {
        var fixture = new ServiceTestFixture();
        var esperado = CriarMedico(fixture);
        CriarMedico(fixture, nome: "Bruno Lima", crm: "654321");
        var service = new MedicoService(fixture.Uow, fixture.Validator);

        var resultado = await service.Buscar(new() { CRM = esperado.CRM });

        var medico = Assert.Single(resultado);
        Assert.Equal(esperado.Id, medico.Id);
    }

    [Fact]
    public async Task Atualizar_QuandoDadosSaoValidos_AlteraPerfilEConfirma()
    {
        var fixture = new ServiceTestFixture();
        var medico = CriarMedico(fixture);
        var service = new MedicoService(fixture.Uow, fixture.Validator);

        var resultado = await service.Atualizar(medico.Id, new()
        {
            CRM = "777777",
            UF = "rj",
            Especialidade = "Neurologia"
        });

        Assert.NotNull(resultado);
        Assert.Equal("777777", medico.CRM);
        Assert.Equal("RJ", medico.UF);
        Assert.Equal("Neurologia", medico.Especialidade);
        Assert.Equal(1, fixture.Uow.Confirmacoes);
    }

    [Fact]
    public async Task Atualizar_QuandoCrmJaEstaEmUso_NaoAlteraNemConfirma()
    {
        var fixture = new ServiceTestFixture();
        var medico = CriarMedico(fixture);
        CriarMedico(fixture, nome: "Bruno Lima", crm: "654321");
        var service = new MedicoService(fixture.Uow, fixture.Validator);

        var resultado = await service.Atualizar(medico.Id, new()
        {
            CRM = "654321",
            UF = "SP",
            Especialidade = "Neurologia"
        });

        Assert.Null(resultado);
        Assert.Equal("123456", medico.CRM);
        Assert.Equal(0, fixture.Uow.Confirmacoes);
        Assert.Contains(
            "Já existe outro médico cadastrado com esse CRM e UF.",
            fixture.Errors.GerarErros());
    }

    [Fact]
    public async Task Desativar_DesativaMedicoEConfirma()
    {
        var fixture = new ServiceTestFixture();
        var medico = CriarMedico(fixture);
        var service = new MedicoService(fixture.Uow, fixture.Validator);

        var resultado = await service.Desativar(medico.Id);

        Assert.True(resultado);
        Assert.False(medico.Ativo);
        Assert.Equal(1, fixture.Uow.Confirmacoes);
    }

    private static Medico CriarMedico(
        ServiceTestFixture fixture,
        string nome = "Ana Souza",
        string crm = "123456")
    {
        var usuario = new Usuario(
            nome,
            $"{Guid.NewGuid():N}@example.com",
            "52998224725",
            new DateTime(1990, 1, 1),
            "hash");
        fixture.Uow.Usuarios.Criar(usuario).GetAwaiter().GetResult();

        return fixture.Uow.Medicos.Add(
            new Medico(usuario.Id, crm, "SP", "Cardiologia"),
            usuario);
    }
}
