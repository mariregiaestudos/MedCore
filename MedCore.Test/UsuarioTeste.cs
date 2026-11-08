using MedCore.Dominio;

namespace MedCore.Test;

public class UsuarioTeste
{
    [Fact]
    public void CriarUsuario_ComDadosValidos_NormalizaDadosEIniciaAtivo()
    {
        var usuario = CriarUsuario(
            " Ana Souza ",
            " ANA@EXAMPLE.COM ",
            "529.982.247-25");

        Assert.NotEqual(Guid.Empty, usuario.Id);
        Assert.Equal("Ana Souza", usuario.Nome);
        Assert.Equal("ana@example.com", usuario.Email);
        Assert.Equal("52998224725", usuario.CPF);
        Assert.Equal(new DateTime(1990, 1, 1), usuario.DataNascimento);
        Assert.Equal("hash-senha", usuario.SenhaHash);
        Assert.True(usuario.Ativo);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void AlterarDadosPessoais_SemNome_LancaExcecao(string? nome)
    {
        var usuario = CriarUsuario();

        var acao = () => usuario.AlterarDadosPessoais(
            nome!, "ana@example.com", "52998224725", new DateTime(1990, 1, 1));

        Assert.Throws<ArgumentException>(acao);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void AlterarDadosPessoais_SemEmail_LancaExcecao(string? email)
    {
        var usuario = CriarUsuario();

        var acao = () => usuario.AlterarDadosPessoais(
            "Ana Souza", email!, "52998224725", new DateTime(1990, 1, 1));

        Assert.Throws<ArgumentException>(acao);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void AlterarDadosPessoais_SemCpf_LancaExcecao(string? cpf)
    {
        var usuario = CriarUsuario();

        var acao = () => usuario.AlterarDadosPessoais(
            "Ana Souza", "ana@example.com", cpf!, new DateTime(1990, 1, 1));

        Assert.Throws<ArgumentException>(acao);
    }

    [Fact]
    public void AlterarDadosPessoais_ComDataNascimentoFutura_LancaExcecao()
    {
        var usuario = CriarUsuario();

        var acao = () => usuario.AlterarDadosPessoais(
            "Ana Souza",
            "ana@example.com",
            "52998224725",
            DateTime.Today.AddDays(1));

        Assert.Throws<ArgumentException>(acao);
    }

    [Fact]
    public void AlterarSenhaHash_ComHashVazio_LancaExcecao()
    {
        var usuario = CriarUsuario();

        var acao = () => usuario.AlterarSenhaHash(" ");

        Assert.Throws<ArgumentException>(acao);
    }

    [Fact]
    public void AtivarEDesativar_AtualizaEstado()
    {
        var usuario = CriarUsuario();

        usuario.Desativar();
        Assert.False(usuario.Ativo);

        usuario.Ativar();
        Assert.True(usuario.Ativo);
    }

    private static Usuario CriarUsuario(
        string nome = "Ana Souza",
        string email = "ana@example.com",
        string cpf = "52998224725") =>
        new(
            nome,
            email,
            cpf,
            new DateTime(1990, 1, 1),
            "hash-senha");
}
