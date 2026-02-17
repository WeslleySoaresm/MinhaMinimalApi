

```markdown
# MinhaMinimalApi 🚀

Uma Web API construída com .NET 9 utilizando o conceito de **Minimal APIs**, focada em performance, código limpo e testes automatizados. O projeto simula um sistema de administração com autenticação JWT e persistência em base de dados MySQL.

## 🏗️ Estrutura do Projeto

O projeto está dividido em duas partes principais:

- **Api/**: Contém a lógica de negócio, controladores (endpoints), infraestrutura de dados e configurações da API.
- **Test/**: Suite de testes utilizando **xUnit**, cobrindo testes unitários de domínio e testes de integração (Requests).

## 🛠️ Tecnologias Utilizadas

- **C# / .NET 9**
- **Entity Framework Core**: ORM para persistência de dados.
- **MySQL / MariaDB**: Base de dados relacional.
- **JWT (JSON Web Tokens)**: Para autenticação e autorização de utilizadores.
- **xUnit & Bogus**: Para criação de testes automatizados e geração de dados fictícios.
- **WebApplicationFactory**: Para testes de integração que simulam chamadas HTTP.

## ⚙️ Configuração do Ambiente

### Pré-requisitos
- .NET 9 SDK instalado.
- MySQL Server (ou Docker com imagem MySQL).

### Base de Dados
No arquivo `appsettings.json` da pasta **Api**, configure a sua string de conexão:
```json
"ConnectionStrings": {
    "mysql": "Server=localhost;Database=MinhaMinimalApi;Uid=root;Pwd=sua_senha;"
}

```

### Executar a API

```bash
cd Api
dotnet watch run

```

## 🧪 Testes

Os testes foram configurados para rodar de forma isolada, utilizando um **Banco de Dados em Memória (InMemory)** para evitar dependências externas.

Para rodar todos os testes:

```bash
dotnet test

```

## 🔐 Endpoints Principais (Exemplos)

| Método | Endpoint | Descrição | Requer Token |
| --- | --- | --- | --- |
| POST | `/administradores/login` | Autentica um admin e retorna o token JWT | Não |
| POST | `/administradores` | Regista um novo administrador | Sim |
| GET | `/administradores` | Lista administradores (paginado) | Sim |
| GET | `/veiculos` | Lista veículos registados | Sim |

## 🛠️ Solução de Problemas Comuns (Mac/Linux)

Se encontrar erros de build relacionados a dependências de teste no macOS, utilize o comando de limpeza profunda:

```bash
dotnet clean && rm -rf **/bin **/obj && dotnet restore && dotnet build

```

---

Desenvolvido por [Weslley Soares]()

```
