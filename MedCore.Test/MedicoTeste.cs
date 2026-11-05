using MedCore.Dominio;

namespace MedCore.Test;

public class MedicoTeste
{
    [Fact]
    public void CriarMedico_ComDadosValidos_DeveCriarMedico()
    {
        var nome = "Ana Souza";
        var crm = "123456";
        var uf = "SP";
        var especialidade = "Cardiologia";

        var medico = new Medico(nome, crm, uf, especialidade);

        Assert.NotEqual(Guid.Empty, medico.Id);
        Assert.Equal(nome, medico.Nome);
        Assert.Equal(crm, medico.CRM);
        Assert.Equal(uf, medico.UF);
        Assert.Equal(especialidade, medico.Especialidade);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void CriarMedico_SemNome_DeveLancarExcecao(string? nome)
    {
        var acao = () => new Medico(
            nome!,
            "123456",
            "SP",
            "Cardiologia");

        Assert.Throws<ArgumentException>(acao);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void CriarMedico_SemCrm_DeveLancarExcecao(string? crm)
    {
        var acao = () => new Medico(
            "Ana Souza",
            crm!,
            "SP",
            "Cardiologia");

        Assert.Throws<ArgumentException>(acao);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void CriarMedico_SemUf_DeveLancarExcecao(string? uf)
    {
        var acao = () => new Medico(
            "Ana Souza",
            "123456",
            uf!,
            "Cardiologia");

        Assert.Throws<ArgumentException>(acao);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void CriarMedico_SemEspecialidade_DeveLancarExcecao(
        string? especialidade)
    {
        var acao = () => new Medico(
            "Ana Souza",
            "123456",
            "SP",
            especialidade!);

        Assert.Throws<ArgumentException>(acao);
    }

    [Fact]
    public void CriarMedico_ComEspacosNasExtremidades_DeveRemoverEspacos()
    {
        var medico = new Medico(
            " Ana Souza ",
            " 123456 ",
            " SP ",
            " Cardiologia ");

        Assert.Equal("Ana Souza", medico.Nome);
        Assert.Equal("123456", medico.CRM);
        Assert.Equal("SP", medico.UF);
        Assert.Equal("Cardiologia", medico.Especialidade);
    }
}