using MedCore.Business.DTOs.Request;
using MedCore.Business.Servicos;
using MedCore.Dominio;
using MedCore.Dominio.Enums;

namespace MedCore.Test.Services;

public class ConsultaServiceTests
{
    [Fact]
    public async Task Agendar_UsaPacienteDaContaAutenticada()
    {
        var fixture = new ServiceTestFixture();
        var paciente = CriarPaciente(fixture);
        var medico = CriarMedico(fixture);
        var service = new ConsultaService(fixture.Uow, fixture.Validator);

        var resultado = await service.Agendar(paciente.UsuarioId, new()
        {
            MedicoId = medico.Id,
            DataHora = DateTime.Now.AddDays(1),
            DuracaoMinutos = 30
        });

        Assert.NotNull(resultado);
        Assert.Equal(paciente.Id, resultado.PacienteId);
        Assert.Equal(medico.Id, resultado.MedicoId);
        Assert.Equal(StatusConsulta.Agendada, resultado.Status);
        Assert.Equal(1, fixture.Uow.Confirmacoes);
    }

    [Fact]
    public async Task Agendar_QuandoPacienteEOMedicoSaoAMesmaConta_Rejeita()
    {
        var fixture = new ServiceTestFixture();
        var usuario = CriarUsuario(fixture);
        var paciente = fixture.Uow.Pacientes.Add(new Paciente(usuario.Id), usuario);
        var medico = fixture.Uow.Medicos.Add(
            new Medico(usuario.Id, "123456", "SP", "Cardiologia"),
            usuario);
        var service = new ConsultaService(fixture.Uow, fixture.Validator);

        var resultado = await service.Agendar(usuario.Id, new()
        {
            MedicoId = medico.Id,
            DataHora = DateTime.Now.AddDays(1),
            DuracaoMinutos = 30
        });

        Assert.Null(resultado);
        Assert.Empty(await fixture.Uow.Consultas.Buscar(new()
        {
            PacienteId = paciente.Id
        }));
        Assert.Equal(0, fixture.Uow.Confirmacoes);
        Assert.Contains(
            "Não é permitido agendar uma consulta com o próprio médico.",
            fixture.Errors.GerarErros());
    }

    [Fact]
    public async Task Reagendar_AlteraMedicoHorarioEDuracao()
    {
        var fixture = new ServiceTestFixture();
        var paciente = CriarPaciente(fixture);
        var medicoOriginal = CriarMedico(fixture);
        var medicoNovo = CriarMedico(fixture, "Bruno Lima", "654321");
        var consulta = new Consulta(
            paciente.Id,
            medicoOriginal.Id,
            DateTime.Now.AddDays(1),
            30);
        await fixture.Uow.Consultas.Criar(consulta);
        var service = new ConsultaService(fixture.Uow, fixture.Validator);
        var novoHorario = DateTime.Now.AddDays(2);

        var resultado = await service.Reagendar(
            consulta.Id,
            paciente.UsuarioId,
            new ReagendarConsultaRequest
            {
                MedicoId = medicoNovo.Id,
                DataHora = novoHorario,
                DuracaoMinutos = 45
            });

        Assert.NotNull(resultado);
        Assert.Equal(medicoNovo.Id, consulta.MedicoId);
        Assert.Equal(novoHorario, consulta.DataHora);
        Assert.Equal(45, consulta.DuracaoMinutos);
        Assert.Equal(StatusConsulta.Agendada, consulta.Status);
        Assert.Equal(1, fixture.Uow.Confirmacoes);
    }

    [Fact]
    public async Task Buscar_LimitaResultadosAoPacienteAutenticado()
    {
        var fixture = new ServiceTestFixture();
        var paciente = CriarPaciente(fixture);
        var outroPaciente = CriarPaciente(fixture);
        var medico = CriarMedico(fixture);
        await fixture.Uow.Consultas.Criar(new Consulta(
            paciente.Id,
            medico.Id,
            DateTime.Now.AddDays(1),
            30));
        await fixture.Uow.Consultas.Criar(new Consulta(
            outroPaciente.Id,
            medico.Id,
            DateTime.Now.AddDays(2),
            30));
        var service = new ConsultaService(fixture.Uow, fixture.Validator);

        var resultado = await service.Buscar(
            new BuscarConsultaRequest { PacienteId = outroPaciente.Id },
            paciente.UsuarioId);

        var consulta = Assert.Single(resultado);
        Assert.Equal(paciente.Id, consulta.PacienteId);
    }

    [Fact]
    public async Task Cancelar_SomenteCancelaConsultaDoPacienteAutenticado()
    {
        var fixture = new ServiceTestFixture();
        var paciente = CriarPaciente(fixture);
        var outroPaciente = CriarPaciente(fixture);
        var medico = CriarMedico(fixture);
        var consulta = new Consulta(
            outroPaciente.Id,
            medico.Id,
            DateTime.Now.AddDays(1),
            30);
        await fixture.Uow.Consultas.Criar(consulta);
        var service = new ConsultaService(fixture.Uow, fixture.Validator);

        var resultado = await service.Cancelar(consulta.Id, paciente.UsuarioId);

        Assert.False(resultado);
        Assert.Equal(StatusConsulta.Agendada, consulta.Status);
        Assert.Equal(0, fixture.Uow.Confirmacoes);
        Assert.Contains("Consulta não encontrada.", fixture.Errors.GerarErros());
    }

    [Fact]
    public async Task ConfirmarEConcluir_AtualizamStatusEConfirmam()
    {
        var fixture = new ServiceTestFixture();
        var paciente = CriarPaciente(fixture);
        var medico = CriarMedico(fixture);
        var consulta = new Consulta(
            paciente.Id,
            medico.Id,
            DateTime.Now.AddDays(1),
            30);
        await fixture.Uow.Consultas.Criar(consulta);
        var service = new ConsultaService(fixture.Uow, fixture.Validator);

        Assert.True(await service.Confirmar(consulta.Id));
        Assert.Equal(StatusConsulta.Confirmada, consulta.Status);
        Assert.True(await service.Concluir(consulta.Id));
        Assert.Equal(StatusConsulta.Concluida, consulta.Status);
        Assert.Equal(2, fixture.Uow.Confirmacoes);
    }

    private static Paciente CriarPaciente(ServiceTestFixture fixture)
    {
        var usuario = CriarUsuario(fixture);
        return fixture.Uow.Pacientes.Add(new Paciente(usuario.Id), usuario);
    }

    private static Medico CriarMedico(
        ServiceTestFixture fixture,
        string nome = "Ana Médica",
        string crm = "111111")
    {
        var usuario = CriarUsuario(fixture, nome);
        return fixture.Uow.Medicos.Add(
            new Medico(usuario.Id, crm, "SP", "Cardiologia"),
            usuario);
    }

    private static Usuario CriarUsuario(
        ServiceTestFixture fixture,
        string nome = "Paciente Teste")
    {
        var usuario = new Usuario(
            nome,
            $"{Guid.NewGuid():N}@example.com",
            "52998224725",
            new DateTime(1990, 1, 1),
            "hash");
        fixture.Uow.Usuarios.Criar(usuario).GetAwaiter().GetResult();
        return usuario;
    }
}
