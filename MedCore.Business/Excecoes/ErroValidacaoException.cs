namespace MedCore.Business.Excecoes;

public class ErroValidacaoException(
    IDictionary<string, string[]> erros)
    : Exception("Um ou mais erros de validação ocorreram.")
{
    public IDictionary<string, string[]> Erros { get; } = erros;
}