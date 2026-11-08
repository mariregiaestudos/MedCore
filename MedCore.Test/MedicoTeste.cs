using MedCore.Dominio;

namespace MedCore.Test;

public class MedicoTeste
{
    [Fact]
    public void CriarMedico_ComDadosValidos_GeraIdEIniciaAtivo()
    {
        var usuarioId = Guid.NewGuid();
        var medico = new Medico(
            usuarioId,
            "123456",
            "sp",
            "Cardiologia");

        Assert.NotEqual(Guid.Empty, medico.Id);
        Assert.Equal(usuarioId, medico.UsuarioId);
        Assert.Equal("123456", medico.CRM);
        Assert.Equal("SP", medico.UF);
        Assert.Equal("Cardiologia", medico.Especialidade);
        Assert.True(medico.Ativo);
    }

    [Fact]
    public void CriarMedico_ComUsuarioVazio_LancaExcecao()
    {
        var acao = () => new Medico(
            Guid.Empty,
            "123456",
            "SP",
            "Cardiologia");

        Assert.Throws<ArgumentException>(acao);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void CriarMedico_SemCrm_LancaExcecao(string? crm)
    {
        var acao = () => new Medico(crm!, "SP", "Cardiologia");

        Assert.Throws<ArgumentException>(acao);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("S")]
    [InlineData("SPX")]
    [InlineData(null)]
    public void CriarMedico_ComUfDeTamanhoInvalido_LancaExcecao(string? uf)
    {
        var acao = () => new Medico("123456", uf!, "Cardiologia");

        Assert.Throws<ArgumentException>(acao);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void CriarMedico_SemEspecialidade_LancaExcecao(string? especialidade)
    {
        var acao = () => new Medico("123456", "SP", especialidade!);

        Assert.Throws<ArgumentException>(acao);
    }

    [Fact]
    public void AlterarDados_NormalizaValores()
    {
        var medico = CriarMedico();

        medico.AlterarDados(" 654321 ", " rj ", " Neurologia ");

        Assert.Equal("654321", medico.CRM);
        Assert.Equal("RJ", medico.UF);
        Assert.Equal("Neurologia", medico.Especialidade);
    }

    [Fact]
    public void AtivarEDesativar_AtualizaEstado()
    {
        var medico = CriarMedico();

        medico.Desativar();
        Assert.False(medico.Ativo);

        medico.Ativar();
        Assert.True(medico.Ativo);
    }

    private static Medico CriarMedico() =>
        new("123456", "SP", "Cardiologia");
}
