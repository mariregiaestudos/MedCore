using FluentValidation;
using MedCore.Business.DTOs.Request;
using MedCore.Business.Interfaces;
using MedCore.Business.Interfaces.IRepositorios;
using MedCore.Business.Servicos;
using MedCore.Business.Validadores;
using MedCore.Dominio;
using MedCore.Dominio.Enums;
using System.Reflection;

namespace MedCore.Test.Services;

internal sealed class FakeUnitOfWork : IUnityOfWork
{
    public FakeUnitOfWork()
    {
        Usuarios = new FakeUsuarioRepositorio();
        Pacientes = new FakePacienteRepositorio(Usuarios.BuscarPorIdInterno);
        Consultas = new FakeConsultaRepositorio();
        Medicos = new FakeMedicoRepositorio(Usuarios.BuscarPorIdInterno);
    }

    public FakePacienteRepositorio Pacientes { get; }
    IPacienteRepositorio IUnityOfWork.Pacientes => Pacientes;

    public FakeUsuarioRepositorio Usuarios { get; }
    IUsuarioRepositorio IUnityOfWork.Usuarios => Usuarios;

    public FakeConsultaRepositorio Consultas { get; }
    IConsultaRepositorio IUnityOfWork.Consultas => Consultas;

    public FakeMedicoRepositorio Medicos { get; }
    IMedicoRepositorio IUnityOfWork.Medicos => Medicos;

    public int Confirmacoes { get; private set; }

    public Task<int> Confirmar()
    {
        Confirmacoes++;
        return Task.FromResult(1);
    }
}

internal sealed class FakeUsuarioRepositorio : IUsuarioRepositorio
{
    private readonly List<Usuario> _usuarios = [];

    public Task<Usuario?> Buscar(BuscarUsuarioRequest request)
    {
        var resultado = _usuarios.SingleOrDefault(usuario =>
            (!request.Id.HasValue || usuario.Id == request.Id) &&
            (string.IsNullOrWhiteSpace(request.Email) ||
             usuario.Email == request.Email.Trim().ToLowerInvariant()) &&
            (string.IsNullOrWhiteSpace(request.CPF) ||
             Digitos(usuario.CPF) == Digitos(request.CPF)) &&
            (string.IsNullOrWhiteSpace(request.Nome) ||
             usuario.Nome.Contains(request.Nome.Trim(), StringComparison.OrdinalIgnoreCase)));

        return Task.FromResult(resultado);
    }

    public Task<Usuario?> BuscarPorId(Guid id) =>
        Task.FromResult(BuscarPorIdInterno(id));

    internal Usuario? BuscarPorIdInterno(Guid id) =>
        _usuarios.SingleOrDefault(usuario => usuario.Id == id);

    public Task<bool> ExisteEmail(string email) =>
        Task.FromResult(_usuarios.Any(usuario => usuario.Email == email));

    public Task Criar(Usuario usuario)
    {
        _usuarios.Add(usuario);
        return Task.CompletedTask;
    }

    public Task Atualizar(Usuario usuario) => Task.CompletedTask;

    private static string Digitos(string valor) =>
        new(valor.Where(char.IsAsciiDigit).ToArray());
}

internal sealed class FakePacienteRepositorio(
    Func<Guid, Usuario?> buscarUsuario) : IPacienteRepositorio
{
    private readonly List<Paciente> _pacientes = [];

    public Task Criar(Paciente paciente)
    {
        typeof(Paciente).GetProperty(nameof(Paciente.Usuario))!
            .SetValue(paciente, buscarUsuario(paciente.UsuarioId));
        _pacientes.Add(paciente);
        return Task.CompletedTask;
    }

    public Task<Paciente?> BuscarPorCodigo(Guid codigo) =>
        Task.FromResult(_pacientes.SingleOrDefault(paciente => paciente.Id == codigo));

    public Task<Paciente?> BuscarPorUsuarioId(Guid usuarioId) =>
        Task.FromResult(_pacientes.SingleOrDefault(paciente => paciente.UsuarioId == usuarioId));

    public Task<List<Paciente>> Buscar(BuscarPacienteRequest filtros)
    {
        IEnumerable<Paciente> consulta = _pacientes;

        if (filtros.Id.HasValue)
            consulta = consulta.Where(paciente => paciente.Id == filtros.Id);
        if (filtros.Ativo.HasValue)
            consulta = consulta.Where(paciente => paciente.Ativo == filtros.Ativo);
        if (!string.IsNullOrWhiteSpace(filtros.Nome))
            consulta = consulta.Where(paciente => paciente.Usuario.Nome.Contains(filtros.Nome));
        if (!string.IsNullOrWhiteSpace(filtros.Email))
            consulta = consulta.Where(paciente => paciente.Usuario.Email == filtros.Email);

        return Task.FromResult(consulta.ToList());
    }

    public Task Atualizar(Paciente paciente) => Task.CompletedTask;

    public Paciente Add(Paciente paciente, Usuario usuario)
    {
        SetNavigation(paciente, nameof(Paciente.Usuario), usuario);
        _pacientes.Add(paciente);
        return paciente;
    }

    private static void SetNavigation<T>(T entity, string propertyName, object value)
    {
        typeof(T).GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public)!
            .SetValue(entity, value);
    }
}

internal sealed class FakeMedicoRepositorio(
    Func<Guid, Usuario?> buscarUsuario) : IMedicoRepositorio
{
    private readonly List<Medico> _medicos = [];

    public Task Criar(Medico medico)
    {
        typeof(Medico).GetProperty(nameof(Medico.Usuario))!
            .SetValue(medico, buscarUsuario(medico.UsuarioId));
        _medicos.Add(medico);
        return Task.CompletedTask;
    }

    public Task<Medico?> BuscarPorCodigo(Guid codigo) =>
        Task.FromResult(_medicos.SingleOrDefault(medico => medico.Id == codigo));

    public Task<Medico?> BuscarPorUsuarioId(Guid usuarioId) =>
        Task.FromResult(_medicos.SingleOrDefault(medico => medico.UsuarioId == usuarioId));

    public Task<List<Medico>> Buscar(BuscarMedicoRequest filtros)
    {
        IEnumerable<Medico> consulta = _medicos;

        if (filtros.Ativo.HasValue)
            consulta = consulta.Where(medico => medico.Ativo == filtros.Ativo);
        if (!string.IsNullOrWhiteSpace(filtros.Nome))
            consulta = consulta.Where(medico => medico.Usuario.Nome.Contains(filtros.Nome));
        if (!string.IsNullOrWhiteSpace(filtros.CRM))
            consulta = consulta.Where(medico => medico.CRM == filtros.CRM);
        if (!string.IsNullOrWhiteSpace(filtros.UF))
            consulta = consulta.Where(medico => medico.UF == filtros.UF.ToUpperInvariant());
        if (!string.IsNullOrWhiteSpace(filtros.Especialidade))
            consulta = consulta.Where(medico => medico.Especialidade.Contains(filtros.Especialidade));

        return Task.FromResult(consulta.ToList());
    }

    public Task Atualizar(Medico medico) => Task.CompletedTask;

    public Task<bool> ExisteCRM(string crm, string uf) =>
        Task.FromResult(_medicos.Any(medico =>
            medico.CRM == crm.Trim() && medico.UF == uf.Trim().ToUpperInvariant()));

    public Medico Add(Medico medico, Usuario usuario)
    {
        typeof(Medico).GetProperty(nameof(Medico.Usuario))!
            .SetValue(medico, usuario);
        _medicos.Add(medico);
        return medico;
    }
}

internal sealed class FakeConsultaRepositorio : IConsultaRepositorio
{
    private readonly List<Consulta> _consultas = [];

    public Task<bool> ExisteConflitoAsync(
        Guid medicoId,
        Guid pacienteId,
        DateTime dataHora,
        DateTime dataHoraFim,
        Guid? consultaIdIgnorada = null) =>
        Task.FromResult(_consultas.Any(consulta =>
            consulta.Id != consultaIdIgnorada &&
            consulta.Status != StatusConsulta.Cancelada &&
            (consulta.MedicoId == medicoId || consulta.PacienteId == pacienteId) &&
            dataHora < consulta.DataHoraFim &&
            dataHoraFim > consulta.DataHora));

    public Task Criar(Consulta consulta)
    {
        _consultas.Add(consulta);
        return Task.CompletedTask;
    }

    public Task<Consulta?> BuscarPorCodigo(Guid codigo) =>
        Task.FromResult(_consultas.SingleOrDefault(consulta => consulta.Id == codigo));

    public Task<List<Consulta>> Buscar(BuscarConsultaRequest filtros)
    {
        IEnumerable<Consulta> consulta = _consultas;

        if (filtros.PacienteId.HasValue)
            consulta = consulta.Where(item => item.PacienteId == filtros.PacienteId);
        if (filtros.MedicoId.HasValue)
            consulta = consulta.Where(item => item.MedicoId == filtros.MedicoId);
        if (filtros.DataInicio.HasValue)
            consulta = consulta.Where(item => item.DataHora >= filtros.DataInicio);
        if (filtros.DataFim.HasValue)
            consulta = consulta.Where(item => item.DataHora <= filtros.DataFim);
        if (filtros.Status.HasValue)
            consulta = consulta.Where(item => item.Status == filtros.Status);

        return Task.FromResult(consulta.ToList());
    }

    public Task Atualizar(Consulta consulta) => Task.CompletedTask;
}

internal sealed class RecordingErrorCollector : IColetorErros
{
    private readonly List<string> _errors = [];

    public bool PossuiErros => _errors.Count > 0;

    public void AdicionarErro(string mensagemErro) => _errors.Add(mensagemErro);

    public List<string> GerarErros() => [.. _errors];

    public void LimparErros() => _errors.Clear();
}

internal sealed class ValidatorServiceProvider : IServiceProvider
{
    private readonly Dictionary<Type, object> _services;

    public ValidatorServiceProvider(FakeUnitOfWork uow)
    {
        _services = new Dictionary<Type, object>
        {
            [typeof(IValidator<UsuarioRequest>)] = new UsuarioValidator(),
            [typeof(IValidator<MedicoRequest>)] = new MedicoValidator(),
            [typeof(IValidator<ValidarCadastroPerfilRequest>)] = new CadastroPerfilValidator(uow),
            [typeof(IValidator<LoginRequest>)] = new LoginRequestValidator(uow),
            [typeof(IValidator<AgendarConsultaValidationRequest>)] = new AgendarConsultaValidator(uow),
            [typeof(IValidator<ReagendarConsultaValidationRequest>)] = new ReagendarConsultaValidator(uow),
            [typeof(IValidator<ValidarConsultaRequest>)] = new ConsultaOperacaoValidator(uow),
            [typeof(IValidator<ValidarPacienteRequest>)] = new PacienteOperacaoValidator(uow),
            [typeof(IValidator<ValidarMedicoRequest>)] = new MedicoOperacaoValidator(uow)
        };
    }

    public object? GetService(Type serviceType) =>
        _services.TryGetValue(serviceType, out var service) ? service : null;
}

internal sealed class ServiceTestFixture
{
    public ServiceTestFixture()
    {
        Uow = new FakeUnitOfWork();
        Errors = new RecordingErrorCollector();
        Validator = new ValidadorService(
            new ValidatorServiceProvider(Uow),
            Errors);
    }

    public FakeUnitOfWork Uow { get; }
    public RecordingErrorCollector Errors { get; }
    public ValidadorService Validator { get; }
}
