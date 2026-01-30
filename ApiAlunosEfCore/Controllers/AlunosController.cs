using ApiAlunosEfCore.Data;
using ApiAlunosEfCore.DTOs;
using ApiAlunosEfCore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiAlunosEfCore.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AlunosController : ControllerBase
    {
        private readonly AlunosDbContext _context;

        public AlunosController(AlunosDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Lista todos os alunos com filtros opcionais
        /// </summary>
        /// <param name="nome">Filtro opcional por nome</param>
        /// <param name="idadeMinima">Filtro opcional por idade mínima</param>
        /// <returns>Lista de alunos</returns>
        [HttpGet]
        public async Task<IActionResult> GetAlunos([FromQuery] string? nome, [FromQuery] int? idadeMinima)
        {
            try
            {
                var query = _context.Alunos.AsNoTracking();

                // Aplicar filtros opcionais usando LINQ
                if (!string.IsNullOrWhiteSpace(nome))
                {
                    query = query.Where(a => a.Nome.Contains(nome));
                }

                if (idadeMinima.HasValue)
                {
                    query = query.Where(a => a.Idade >= idadeMinima.Value);
                }

                // Ordenar por nome
                var alunos = await query.OrderBy(a => a.Nome).ToListAsync();

                return Ok(new
                {
                    sucesso = true,
                    dados = alunos
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    sucesso = false,
                    erro = $"Erro ao buscar alunos: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// Obtém um aluno pelo ID
        /// </summary>
        /// <param name="id">ID do aluno</param>
        /// <returns>Dados do aluno</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAluno(int id)
        {
            try
            {
                var aluno = await _context.Alunos
                    .AsNoTracking()
                    .FirstOrDefaultAsync(a => a.Id == id);

                if (aluno == null)
                {
                    return NotFound(new
                    {
                        sucesso = false,
                        erro = "Aluno não encontrado"
                    });
                }

                return Ok(new
                {
                    sucesso = true,
                    dados = aluno
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    sucesso = false,
                    erro = $"Erro ao buscar aluno: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// Cria um novo aluno
        /// </summary>
        /// <param name="dto">Dados do aluno</param>
        /// <returns>Aluno criado</returns>
        [HttpPost]
        public async Task<IActionResult> CreateAluno([FromBody] CreateAlunoDto dto)
        {
            try
            {
                // Validação do modelo
                if (!ModelState.IsValid)
                {
                    var erros = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();

                    return BadRequest(new
                    {
                        sucesso = false,
                        erro = "Dados inválidos",
                        detalhes = erros
                    });
                }

                // Mapeia o DTO para a entidade Aluno
                var aluno = new Aluno
                {
                    Nome = dto.Nome,
                    Email = dto.Email,
                    Idade = dto.Idade,
                    Matricula = await GerarMatricula(),
                    DataMatricula = DateTime.Now
                };

                _context.Alunos.Add(aluno);
                await _context.SaveChangesAsync();

                return CreatedAtAction(
                    nameof(GetAluno),
                    new { id = aluno.Id },
                    new
                    {
                        sucesso = true,
                        dados = aluno
                    });
            }
            catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("UNIQUE") == true)
            {
                return Conflict(new
                {
                    sucesso = false,
                    erro = "Já existe um aluno cadastrado com este email"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    sucesso = false,
                    erro = $"Erro ao criar aluno: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// Atualiza um aluno existente
        /// </summary>
        /// <param name="id">ID do aluno</param>
        /// <param name="dto">Dados atualizados do aluno</param>
        /// <returns>Aluno atualizado</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAluno(int id, [FromBody] UpdateAlunoDto dto)
        {
            try
            {
                // Validação do modelo
                if (!ModelState.IsValid)
                {
                    var erros = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();

                    return BadRequest(new
                    {
                        sucesso = false,
                        erro = "Dados inválidos",
                        detalhes = erros
                    });
                }

                var aluno = await _context.Alunos.FindAsync(id);

                if (aluno == null)
                {
                    return NotFound(new
                    {
                        sucesso = false,
                        erro = "Aluno não encontrado"
                    });
                }

                // Atualizar apenas propriedades permitidas
                // Id, Matricula e DataMatricula são protegidos
                aluno.Nome = dto.Nome;
                aluno.Email = dto.Email;
                aluno.Idade = dto.Idade;

                await _context.SaveChangesAsync();

                return Ok(new
                {
                    sucesso = true,
                    dados = aluno
                });
            }
            catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("UNIQUE") == true)
            {
                return Conflict(new
                {
                    sucesso = false,
                    erro = "Já existe um aluno cadastrado com este email"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    sucesso = false,
                    erro = $"Erro ao atualizar aluno: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// Remove um aluno
        /// </summary>
        /// <param name="id">ID do aluno</param>
        /// <returns>Confirmação de remoção</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAluno(int id)
        {
            try
            {
                var aluno = await _context.Alunos.FindAsync(id);

                if (aluno == null)
                {
                    return NotFound(new
                    {
                        sucesso = false,
                        erro = "Aluno não encontrado"
                    });
                }

                _context.Alunos.Remove(aluno);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    sucesso = true,
                    mensagem = $"Aluno '{aluno.Nome}' (Matrícula: {aluno.Matricula}) foi removido com sucesso"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    sucesso = false,
                    erro = $"Erro ao deletar aluno: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// Gera uma matrícula única no formato ANOSEQUENCIAL (ex: 2026001)
        /// </summary>
        /// <returns>Matrícula gerada</returns>
        private async Task<string> GerarMatricula()
        {
            var ano = DateTime.Now.Year;

            // Busca a última matrícula do ano atual
            var ultimaMatriculaDoAno = await _context.Alunos
                .Where(a => a.Matricula.StartsWith(ano.ToString()))
                .OrderByDescending(a => a.Matricula)
                .Select(a => a.Matricula)
                .FirstOrDefaultAsync();

            int proximoNumero = 1;

            if (!string.IsNullOrEmpty(ultimaMatriculaDoAno) && ultimaMatriculaDoAno.Length > 4)
            {
                // Extrai o número sequencial da última matrícula
                var numeroStr = ultimaMatriculaDoAno.Substring(4);
                if (int.TryParse(numeroStr, out int numero))
                {
                    proximoNumero = numero + 1;
                }
            }

            // Formata: ANO + número sequencial com 4 dígitos (ex: 2026001)
            return $"{ano}{proximoNumero:D4}";
        }
    }
}
