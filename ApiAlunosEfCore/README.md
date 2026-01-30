# API de Cadastro de Alunos

API RESTful desenvolvida com ASP.NET Core e Entity Framework Core para gerenciar cadastro de alunos em banco de dados SQLite.

## Tecnologias Utilizadas

- .NET 10.0
- ASP.NET Core Web API
- Entity Framework Core 10.0.1
- SQLite
- Swagger/OpenAPI

## Estrutura do Projeto

```
ApiAlunosEfCore/
├── Controllers/
│   └── AlunosController.cs      # Controller com endpoints CRUD
├── Data/
│   └── AlunosDbContext.cs       # Contexto do Entity Framework
├── Models/
│   └── Aluno.cs                 # Entidade Aluno
├── Migrations/                  # Migrations do EF Core
└── Program.cs                   # Configuração da aplicação
```

## Entidade Aluno

| Campo          | Tipo     | Validação                    |
|----------------|----------|------------------------------|
| Id             | int      | Chave primária (auto-gerada) |
| Nome           | string   | Obrigatório, máx. 100 chars  |
| Email          | string   | Obrigatório, único, formato de email, máx. 255 chars |
| Idade          | int      | Obrigatório, entre 18-100    |
| DataMatricula  | DateTime | Default: DateTime.Now        |

## Endpoints Disponíveis

### GET /api/alunos
Lista todos os alunos com filtros opcionais.

**Query Parameters:**
- `nome` (opcional): Filtrar por nome
- `idadeMinima` (opcional): Filtrar por idade mínima

**Resposta de Sucesso (200):**
```json
{
  "sucesso": true,
  "dados": [
    {
      "id": 1,
      "nome": "João Silva",
      "email": "joao@email.com",
      "idade": 25,
      "dataMatricula": "2026-01-06T00:00:00"
    }
  ]
}
```

### GET /api/alunos/{id}
Obtém um aluno específico pelo ID.

**Resposta de Sucesso (200):**
```json
{
  "sucesso": true,
  "dados": {
    "id": 1,
    "nome": "João Silva",
    "email": "joao@email.com",
    "idade": 25,
    "dataMatricula": "2026-01-06T00:00:00"
  }
}
```

**Resposta de Erro (404):**
```json
{
  "sucesso": false,
  "erro": "Aluno não encontrado"
}
```

### POST /api/alunos
Cria um novo aluno.

**Body:**
```json
{
  "nome": "Maria Santos",
  "email": "maria@email.com",
  "idade": 22
}
```

**Resposta de Sucesso (201):**
```json
{
  "sucesso": true,
  "dados": {
    "id": 2,
    "nome": "Maria Santos",
    "email": "maria@email.com",
    "idade": 22,
    "dataMatricula": "2026-01-06T17:30:00"
  }
}
```

**Resposta de Erro (409 - Email duplicado):**
```json
{
  "sucesso": false,
  "erro": "Já existe um aluno cadastrado com este email"
}
```

### PUT /api/alunos/{id}
Atualiza um aluno existente.

**Body:**
```json
{
  "nome": "Maria Santos Silva",
  "email": "maria.silva@email.com",
  "idade": 23
}
```

**Resposta de Sucesso (200):**
```json
{
  "sucesso": true,
  "dados": {
    "id": 2,
    "nome": "Maria Santos Silva",
    "email": "maria.silva@email.com",
    "idade": 23,
    "dataMatricula": "2026-01-06T17:30:00"
  }
}
```

### DELETE /api/alunos/{id}
Remove um aluno.

**Resposta de Sucesso (204 No Content)**

**Resposta de Erro (404):**
```json
{
  "sucesso": false,
  "erro": "Aluno não encontrado"
}
```

## Como Executar

### Pré-requisitos
- .NET 10.0 SDK instalado
- dotnet-ef tool instalado (para migrations)

### Passos

1. **Navegar até o diretório do projeto:**
   ```bash
   cd ApiAlunosEfCore
   ```

2. **Restaurar dependências:**
   ```bash
   dotnet restore
   ```

3. **Aplicar migrations (criar banco de dados):**
   ```bash
   dotnet ef database update
   ```
   > Nota: O banco de dados também é criado automaticamente ao executar a aplicação.

4. **Executar a aplicação:**
   ```bash
   dotnet run
   ```

5. **Acessar o Swagger UI:**
   ```
   http://localhost:5197/swagger
   ```

## Migrations

### Criar nova migration:
```bash
dotnet ef migrations add NomeDaMigration
```

### Aplicar migrations:
```bash
dotnet ef database update
```

### Reverter última migration:
```bash
dotnet ef migrations remove
```

## Banco de Dados

O projeto utiliza SQLite com o arquivo `alunos.db` criado na raiz do projeto.

**String de conexão padrão:** `Data Source=alunos.db`

Para customizar a string de conexão, adicione no `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=seu_banco.db"
  }
}
```

## Boas Práticas Implementadas

- **DbContext Scoped Lifetime**: Uma instância por requisição HTTP via DI
- **Métodos Assíncronos**: Uso de `async/await` para melhor performance
- **AsNoTracking**: Otimização em consultas somente leitura
- **Data Annotations**: Validações na entidade
- **Fluent API**: Configurações avançadas no `OnModelCreating`
- **LINQ Type-Safe**: Consultas fortemente tipadas
- **Change Tracking**: Detecção automática de modificações no EF Core
- **Tratamento de Exceções**: Respostas padronizadas para erros
- **CORS**: Configurado para desenvolvimento
- **Migrations**: Code-First para versionamento do schema

## Testando com curl

### Criar aluno:
```bash
curl -X POST http://localhost:5197/api/alunos \
  -H "Content-Type: application/json" \
  -d '{
    "nome": "Pedro Oliveira",
    "email": "pedro@email.com",
    "idade": 20
  }'
```

### Listar todos:
```bash
curl http://localhost:5197/api/alunos
```

### Buscar por ID:
```bash
curl http://localhost:5197/api/alunos/1
```

### Atualizar:
```bash
curl -X PUT http://localhost:5197/api/alunos/1 \
  -H "Content-Type: application/json" \
  -d '{
    "nome": "Pedro Oliveira Santos",
    "email": "pedro.santos@email.com",
    "idade": 21
  }'
```

### Deletar:
```bash
curl -X DELETE http://localhost:5197/api/alunos/1
```

## Próximos Passos (Extensões)

- Implementar Repository Pattern para abstrair acesso a dados
- Adicionar relacionamentos (ex: Aluno -> Curso, Matrícula)
- Implementar paginação nos endpoints de listagem
- Adicionar autenticação/autorização (JWT)
- Implementar testes unitários e de integração
- Adicionar logging estruturado (Serilog)
- Implementar versionamento de API
