using MedCore.Business.Servicos;
using MedCore.Dominio;

namespace MedCore.Test.Services;

public class PacienteServiceTests
{
    [Fact]
    public async Task BuscarPorCodigo_QuandoExiste_RetornaDadosDoPaciente()
    {
        var fixture = new ServiceTestFixture();
        var paciente = CriarPaciente(fixture);
        var service = new PacienteService(fixture.Uow, fixture.Validator);

        var resultado = await service.BuscarPorCodigo(paciente.Id);

        Assert.NotNull(resultado);
        Assert.Equal(paciente.Id, resultado.Id);
        Assert.Equal(paciente.Usuario.Nome, resultado.Nome);
        Assert.Equal(paciente.Usuario.Email, resultado.Email);
        Assert.False(fixture.Errors.PossuiErros);
    }

    [Fact]
    public async Task BuscarPorCodigo_QuandoNaoExiste_RegistraErroDeValidacao()
    {
        var fixture = new ServiceTestFixture();
        var service = new PacienteService(fixture.Uow, fixture.Validator);

        var resultado = await service.BuscarPorCodigo(Guid.NewGuid());

        Assert.Null(resultado);
        Assert.Contains("Paciente não encontrado.", fixture.Errors.GerarErros());
    }

    [Fact]
    public async Task Buscar_RetornaPacientesFiltradosPeloRepositorio()
    {
        var fixture = new ServiceTestFixture();
        var paciente = CriarPaciente(fixture, nome: "Ana Souza");
        CriarPaciente(fixture, nome: "Bruno Lima");
        var service = new PacienteService(fixture.Uow, fixture.Validator);

        var resultado = await service.Buscar(new()
        {
            Nome = "Ana"
        });

        var unicoResultado = Assert.Single(resultado);
        Assert.Equal(paciente.Id, unicoResultado.Id);
    }

    [Fact]
    public async Task Desativar_DesativaPacienteEConfirmaAlteracao()
    {
        var fixture = new ServiceTestFixture();
        var paciente = CriarPaciente(fixture);
        var service = new PacienteService(fixture.Uow, fixture.Validator);

        var resultado = await service.Desativar(paciente.Id);

        Assert.True(resultado);
        Assert.False(paciente.Ativo);
        Assert.Equal(1, fixture.Uow.Confirmacoes);
    }

    [Fact]
    public async Task Desativar_QuandoPacienteNaoExiste_NaoConfirmaAlteracao()
    {
        var fixture = new ServiceTestFixture();
        var service = new PacienteService(fixture.Uow, fixture.Validator);

        var resultado = await service.Desativar(Guid.NewGuid());

        Assert.False(resultado);
        Assert.Equal(0, fixture.Uow.Confirmacoes);
        Assert.Contains("Paciente não encontrado.", fixture.Errors.GerarErros());
    }

    private static Paciente CriarPaciente(
        ServiceTestFixture fixture,
        string nome = "Ana Souza")
    {
        var usuario = new Usuario(
            nome,
            $"{Guid.NewGuid():N}@example.com",
            "52998224725",
            new DateTime(1990, 1, 1),
            "hash");
        fixture.Uow.Usuarios.Criar(usuario).GetAwaiter().GetResult();

        return fixture.Uow.Pacientes.Add(
            new Paciente(usuario.Id),
            usuario);
    }
}
