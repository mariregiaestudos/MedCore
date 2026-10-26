using System;
using System.Collections.Generic;
using System.Text;

namespace MedCore.Business.Interfaces;

public interface IColetorErros
{
    bool PossuiErros { get; }

    void AdicionarErro(string mensagemErro);

    List<string> GerarErros();

    void LimparErros();
}
