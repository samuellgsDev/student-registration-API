using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApiAlunosEfCore.Models
{
    public class Aluno
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "A matrícula é obrigatória")]
        [StringLength(20, ErrorMessage = "A matrícula deve ter no máximo 20 caracteres")]
        public string Matricula { get; set; } = string.Empty;

        [Required(ErrorMessage = "O nome é obrigatório")]
        [StringLength(100, ErrorMessage = "O nome deve ter no máximo 100 caracteres")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "O email é obrigatório")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        [StringLength(255, ErrorMessage = "O email deve ter no máximo 255 caracteres")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "A idade é obrigatória")]
        [Range(18, 100, ErrorMessage = "A idade deve estar entre 18 e 100 anos")]
        public int Idade { get; set; }

        [Required]
        public DateTime DataMatricula { get; set; } = DateTime.Now;
    }
}
