using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TerraNova.Domain.Entities;

namespace TerraNova.Infrastructure.Persistence.Configurations;

public sealed class ProdutorConfiguration : IEntityTypeConfiguration<Produtor>
{
    public void Configure(EntityTypeBuilder<Produtor> builder)
    {
        builder.ToTable("produtor");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).HasColumnName("id_produtor");
 
        builder.Property(p => p.Nome)
            .HasColumnName("nome")
            .HasMaxLength(30)
            .IsRequired();
 
        builder.Property(p => p.Email)
            .HasColumnName("email")
            .HasMaxLength(30)
            .IsRequired();
 
        builder.HasIndex(p => p.Email).IsUnique();
 
        builder.Property(p => p.Senha)
            .HasColumnName("senha")
            .HasMaxLength(30)
            .IsRequired();
 
        // Navegação inversa para o Telefone detalhado (1:1, FK no lado Telefone)
        builder.HasOne(p => p.TelefoneDetalhado)
            .WithOne(t => t.Produtor)
            .HasForeignKey<Telefone>(t => t.ProdutorId)
            .OnDelete(DeleteBehavior.Cascade);
 
        builder.HasMany(p => p.Propriedades)
            .WithOne(pr => pr.Produtor)
            .HasForeignKey(pr => pr.ProdutorId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}