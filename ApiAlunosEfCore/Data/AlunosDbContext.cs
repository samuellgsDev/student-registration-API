using ApiAlunosEfCore.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiAlunosEfCore.Data
{
    public class AlunosDbContext : DbContext
    {
        public AlunosDbContext(DbContextOptions<AlunosDbContext> options) : base(options)
        {
        }

        public DbSet<Aluno> Alunos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuração da entidade Aluno
            modelBuilder.Entity<Aluno>(entity =>
            {
                // Configuração da chave primária
                entity.HasKey(a => a.Id);

                // Configuração da matrícula como única
                entity.HasIndex(a => a.Matricula)
                    .IsUnique();

                // Configuração do email como único
                entity.HasIndex(a => a.Email)
                    .IsUnique();

                // Configuração de precisão para DateTime
                entity.Property(a => a.DataMatricula)
                    .HasDefaultValueSql("datetime('now')");

                // Configurações adicionais de propriedades
                entity.Property(a => a.Matricula)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(a => a.Nome)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(a => a.Email)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(a => a.Idade)
                    .IsRequired();
            });
        }
    }
}
