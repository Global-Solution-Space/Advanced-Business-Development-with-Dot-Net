using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TerraNova.Domain.Entities;

namespace TerraNova.Infrastructure.Persistence.Configurations;

public sealed class PropriedadeConfiguration : IEntityTypeConfiguration<Propriedade>
{
    public void Configure(EntityTypeBuilder<Propriedade> builder)
    {
        builder.ToTable("propriedade");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).HasColumnName("id_propriedade");
 
        builder.Property(p => p.Nome)
            .HasColumnName("nome")
            .HasMaxLength(30)
            .IsRequired();
 
        builder.Property(p => p.TamanhoTotal)
            .HasColumnName("tamanho_total")
            .IsRequired();
 
        builder.Property(p => p.ProdutorId)
            .HasColumnName("produtor_id_produtor")
            .IsRequired();
 
        // FK para Produtor configurada no ProdutorConfiguration
 
        builder.Property(p => p.LocalizacaoId)
            .HasColumnName("localizacao_id_localizacao")
            .IsRequired();
 
        builder.HasOne(p => p.Localizacao)
            .WithOne(l => l.Propriedade)
            .HasForeignKey<Propriedade>(p => p.LocalizacaoId)
            .OnDelete(DeleteBehavior.Restrict);
 
        // Índice único garante 1:1 com Localizacao
        builder.HasIndex(p => p.LocalizacaoId).IsUnique();
 
        builder.HasMany(p => p.Talhoes)
            .WithOne(t => t.Propriedade)
            .HasForeignKey(t => t.PropriedadeId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}