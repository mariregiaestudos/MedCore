using MedCore.Dominio;
using MedCore.Dominio.Enums;

namespace MedCore.Test;

public class MedicoTeste
{
    [Fact]
    public void CriarMedico_ComDadosValidos_DeveCriarMedico()
    {
        // Arrange
        var nome = "Ana Souza";
        var crm = "123456-SP";
        var especialidade = "Cardiologia";

        // Act
        var medico = new Medico(nome, crm, especialidade);

        // Assert
        Assert.NotEqual(Guid.Empty, medico.Codigo);
        Assert.Equal(nome, medico.Nome);
        Assert.Equal(crm, medico.Crm);
        Assert.Equal(especialidade, medico.Especialidade);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void CriarMedico_SemNome_DeveLancarExcecao(
        string? nome)
    {
        // Arrange
        var crm = "123456-SP";
        var especialidade = "Cardiologia";

        // Act
        var acao = () => new Medico(nome!, crm, especialidade);

        // Assert
        Assert.Throws<ArgumentException>(acao);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void CriarMedico_SemCrm_DeveLancarExcecao(
        string? crm)
    {
        // Arrange
        var nome = "Ana Souza";
        var especialidade = "Cardiologia";

        // Act
        var acao = () => new Medico(nome, crm!, especialidade);

        // Assert
        Assert.Throws<ArgumentException>(acao);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void CriarMedico_SemEspecialidade_DeveLancarExcecao(
        string? especialidade)
    {
        // Arrange
        var nome = "Ana Souza";
        var crm = "123456-SP";

        // Act
        var acao = () => new Medico(nome, crm, especialidade!);

        // Assert
        Assert.Throws<ArgumentException>(acao);
    }

    [Fact]
    public void CriarMedico_ComEspacosNasExtremidades_DeveRemoverEspacos()
    {
        // Arrange
        var nome = " Ana Souza ";
        var crm = " 123456-SP ";
        var especialidade = " Cardiologia ";

        // Act
        var medico = new Medico(nome, crm, especialidade);

        // Assert
        Assert.Equal("Ana Souza", medico.Nome);
        Assert.Equal("123456-SP", medico.Crm);
        Assert.Equal("Cardiologia", medico.Especialidade);
    }
}
