using MedCore.Dominio;

namespace MedCore.Test;

public class PacienteTeste
{
    [Fact]
    public void CriarPaciente_ComUsuarioValido_GeraIdEIniciaAtivo()
    {
        var usuarioId = Guid.NewGuid();

        var paciente = new Paciente(usuarioId);

        Assert.NotEqual(Guid.Empty, paciente.Id);
        Assert.Equal(usuarioId, paciente.UsuarioId);
        Assert.True(paciente.Ativo);
    }

    [Fact]
    public void CriarPaciente_ComUsuarioVazio_LancaExcecao()
    {
        var acao = () => new Paciente(Guid.Empty);

        Assert.Throws<ArgumentException>(acao);
    }

    [Fact]
    public void DesativarEAtivar_AtualizaEstado()
    {
        var paciente = new Paciente(Guid.NewGuid());

        paciente.Desativar();
        Assert.False(paciente.Ativo);

        paciente.Ativar();
        Assert.True(paciente.Ativo);
    }
}
