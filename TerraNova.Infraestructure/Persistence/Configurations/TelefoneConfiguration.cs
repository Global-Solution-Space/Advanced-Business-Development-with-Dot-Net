using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TerraNova.Domain.Entities;

namespace TerraNova.Infraestructure.Persistence.Configurations;

public sealed class TelefoneConfiguration : IEntityTypeConfiguration<Telefone>
{
    public void Configure(EntityTypeBuilder<Telefone> builder)
    {
        builder.ToTable("telefone");
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id).HasColumnName("id_telefone");
 
        builder.Property(t => t.Ddd)
            .HasColumnName("ddd")
            .HasMaxLength(2)
            .IsFixedLength()
            .IsRequired();
 
        builder.Property(t => t.Numero)
            .HasColumnName("numero")
            .HasMaxLength(9)
            .IsRequired();
 
        builder.Property(t => t.ProdutorId)
            .HasColumnName("produtor_id_produtor")
            .IsRequired();
 
        // Índice único para garantir o 1:1 com Produtor
        builder.HasIndex(t => t.ProdutorId).IsUnique();
 
        // Relacionamento configurado no ProdutorConfiguration
    }
}