# MedCore

API REST para gerenciamento de pacientes, médicos, usuários e consultas. O projeto foi desenvolvido com ASP.NET Core, Entity Framework Core e SQL Server, organizado em camadas para separar a API, as regras de negócio, o domínio e a infraestrutura.

## Swagger para testes

Acesse a documentação interativa da API publicada:

[ Abrir Swagger da API ](https://medcore-api-dev2026-gwdvgqgahybud9bn.westus3-01.azurewebsites.net/swagger/index.html)

> Este ambiente está disponível para testes. As rotas protegidas exigem autenticação; use o endpoint de login para obter um token JWT e informe-o no botão **Authorize** do Swagger como `Bearer <token>`.

## Funcionalidades

- Cadastro e autenticação de usuários, pacientes e médicos.
- Consulta e busca de pacientes e médicos.
- Desativação de cadastros.
- Agendamento, consulta, reagendamento e cancelamento de consultas.
- Autenticação e autorização usando JWT.
- Persistência com Entity Framework Core e SQL Server.
- Validação de requisições com FluentValidation.

## Tecnologias

- C# e .NET 10
- ASP.NET Core Web API
- Entity Framework Core 10
- SQL Server
- Swagger / Swashbuckle
- JWT Bearer
- xUnit

## Estrutura da solução

| Projeto | Responsabilidade |
| --- | --- |
| `MedCore.Api` | Aplicação ASP.NET Core, controladores e configuração da API. |
| `MedCore.Business` | Serviços, DTOs, interfaces e validações de negócio. |
| `MedCore.Dominio` | Entidades e regras centrais do domínio. |
| `MedCore.Infraestrutura` | Persistência, repositórios e acesso ao SQL Server. |
| `MedCore.Test` | Testes automatizados com xUnit. |

## Executar localmente

### Pré-requisitos

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- Uma instância acessível do SQL Server

### Configuração

A API precisa de uma connection string chamada `MedCore` e das configurações JWT `Jwt:Key`, `Jwt:Issuer` e `Jwt:Audience`. Configure esses valores localmente usando User Secrets para não salvar credenciais no repositório:

```bash
dotnet user-secrets init --project MedCore.Api
dotnet user-secrets set "ConnectionStrings:MedCore" "Server=localhost;Database=MedCore;Trusted_Connection=True;TrustServerCertificate=True;" --project MedCore.Api
dotnet user-secrets set "Jwt:Key" "substitua-por-uma-chave-secreta-forte" --project MedCore.Api
dotnet user-secrets set "Jwt:Issuer" "MedCore" --project MedCore.Api
dotnet user-secrets set "Jwt:Audience" "MedCore" --project MedCore.Api
```

Ajuste a connection string conforme sua instalação do SQL Server. Para outros ambientes, configure os mesmos valores por meio do mecanismo seguro de configuração da plataforma de hospedagem.

### Compilar, testar e iniciar

Na raiz do repositório, execute:

```bash
dotnet restore MedCore.Api.slnx
dotnet build MedCore.Api.slnx
dotnet test MedCore.Api.slnx
dotnet run --project MedCore.Api
```

Quando a aplicação iniciar, abra a URL local indicada no terminal e acrescente `/swagger` para acessar o Swagger.

## Endpoints

Os caminhos abaixo usam o prefixo `/api`:

| Recurso | Operações |
| --- | --- |
| `Usuarios` | `POST /api/Usuario/login` — autenticação. |
| `Pacientes` | `POST`, `GET`, `GET /{codigo}`, `PATCH /{codigo}/desativar`. |
| `Medicos` | `POST`, `GET`, `GET /{codigo}`, `PUT /{codigo}`, `PATCH /{codigo}/desativar`. |
| `Consultas` | `POST`, `GET`, `GET /{codigo}`, `PUT /{codigo}`, `PATCH /{codigo}/cancelar`. |

Consulte o Swagger para ver os parâmetros, os formatos dos corpos das requisições e as respostas de cada operação. As rotas de cadastro de pacientes e médicos e o login permitem acesso sem autenticação; as demais rotas protegidas requerem token JWT.

## Contribuição

Contribuições são bem-vindas. Abra uma issue para relatar problemas ou propor melhorias e, se desejar contribuir com código, envie um pull request.
