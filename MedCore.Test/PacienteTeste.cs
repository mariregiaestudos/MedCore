using MedCore.Dominio;

namespace MedCore.Test;

public class PacienteTeste
{
    [Fact]
    public void CriarPaciente_ComDadosValidos_DeveCriarPaciente()
    {
        var nome = "Maria Silva";
        var email = "maria@email.com";
        var cpf = "52998224725";
        var dataNascimento = new DateTime(1995, 5, 10);

        var paciente = new Paciente(
            nome,
            email,
            cpf,
            dataNascimento);

        Assert.NotEqual(Guid.Empty, paciente.Id);
        Assert.Equal(nome, paciente.Nome);
        Assert.Equal(email, paciente.Email);
        Assert.Equal(cpf, paciente.CPF);
        Assert.Equal(dataNascimento, paciente.DataNascimento);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void CriarPaciente_SemNome_DeveLancarExcecao(string? nome)
    {
        var acao = () => new Paciente(
            nome!,
            "maria@email.com",
            "52998224725",
            new DateTime(1995, 5, 10));

        Assert.Throws<ArgumentException>(acao);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void CriarPaciente_SemEmail_DeveLancarExcecao(string? email)
    {
        var acao = () => new Paciente(
            "Maria Silva",
            email!,
            "52998224725",
            new DateTime(1995, 5, 10));

        Assert.Throws<ArgumentException>(acao);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void CriarPaciente_SemCpf_DeveLancarExcecao(string? cpf)
    {
        var acao = () => new Paciente(
            "Maria Silva",
            "maria@email.com",
            cpf!,
            new DateTime(1995, 5, 10));

        Assert.Throws<ArgumentException>(acao);
    }

    [Fact]
    public void CriarPaciente_ComEspacosNasExtremidades_DeveRemoverEspacos()
    {
        var paciente = new Paciente(
            " Maria Silva ",
            " maria@email.com ",
            " 52998224725 ",
            new DateTime(1995, 5, 10));

        Assert.Equal("Maria Silva", paciente.Nome);
        Assert.Equal("maria@email.com", paciente.Email);
        Assert.Equal("52998224725", paciente.CPF);
    }
}