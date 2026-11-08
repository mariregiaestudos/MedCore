using MedCore.Business.DTOs.Request;
using MedCore.Business.DTOs.Response;
using System;
using System.Collections.Generic;
using System.Text;

namespace MedCore.Business.Interfaces;

public interface IUsuarioService
{
    Task<PacienteResponse?> CadastrarPaciente(UsuarioRequest request);
    Task<MedicoResponse?> CadastrarMedico(MedicoRequest request);
    Task<LoginResponse?> Login(LoginRequest request);
}
