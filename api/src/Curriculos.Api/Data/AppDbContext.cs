using Curriculos.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Curriculos.Api.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Candidato> Candidatos => Set<Candidato>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Candidato>(e =>
        {
            e.ToTable("Candidatos");
            e.Property(c => c.NomeCompleto).HasMaxLength(150).IsRequired();
            e.Property(c => c.Email).HasMaxLength(150).IsRequired();
            e.HasIndex(c => c.Email).IsUnique();
            e.Property(c => c.Telefone).HasMaxLength(20);
            e.Property(c => c.AreaInteresse).HasMaxLength(100);
            e.Property(c => c.ResumoProfissional).HasMaxLength(2000);
            e.Property(c => c.CriadoEm).HasDefaultValueSql("SYSUTCDATETIME()");
        });
    }
}