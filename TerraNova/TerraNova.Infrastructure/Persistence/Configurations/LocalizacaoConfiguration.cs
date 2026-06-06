using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TerraNova.Domain.Entities;

namespace TerraNova.Infrastructure.Persistence.Configurations;

public sealed class LocalizacaoConfiguration : IEntityTypeConfiguration<Localizacao>
{
    public void Configure(EntityTypeBuilder<Localizacao> builder)
    {
        builder.ToTable("localizacao");
        builder.HasKey(l => l.Id);
        builder.Property(l => l.Id).HasColumnName("id_localizacao");
 
        builder.Property(l => l.Coordenadas)
            .HasColumnName("coordenadas")
            .HasColumnType("SDO_GEOMETRY")
            .IsRequired();
    }
}