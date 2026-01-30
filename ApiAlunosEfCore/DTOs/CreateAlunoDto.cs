using System.ComponentModel.DataAnnotations;

namespace ApiAlunosEfCore.DTOs
{
    public class CreateAlunoDto
    {
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
    }
}
