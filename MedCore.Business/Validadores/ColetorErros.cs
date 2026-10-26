using MedCore.Business.Interfaces;

namespace MedCore.Business.Validadores;

public class ColetorErros : IColetorErros
{
    private readonly List<string> _erros = [];

    public bool PossuiErros => _erros.Count > 0;

    public void AdicionarErro(string mensagemErro)
    {
        _erros.Add(mensagemErro);
    }

    public List<string> GerarErros()
    {
        return [.. _erros];
    }

    public void LimparErros()
    {
        _erros.Clear();
    }
}