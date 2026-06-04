using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TerraNova.Domain.Entities;
using TerraNova.Domain.Enums;

namespace TerraNova.Infrastructure.Persistence.Configurations;

public sealed class AlertaAgricolaConfiguration : IEntityTypeConfiguration<AlertaAgricola>
{
    public void Configure(EntityTypeBuilder<AlertaAgricola> builder)
    {
        builder.ToTable("alerta_agricola");
        builder.HasKey(a => a.Id);
        builder.Property(a => a.Id).HasColumnName("id_alerta");

        builder.Property(a => a.Titulo)
            .HasColumnName("titulo").HasMaxLength(100).IsRequired();

        builder.Property(a => a.Descricao)
            .HasColumnName("descricao").HasMaxLength(300).IsRequired();

        builder.Property(a => a.NivelAlerta)
            .HasColumnName("nivel_alerta")
            .HasMaxLength(20)
            .HasConversion(
                v => v.ToString().ToUpperInvariant(),
                v => Enum.Parse<NivelAlerta>(v, true))
            .IsRequired();

        builder.Property(a => a.Resolvido)
            .HasColumnName("resolvido")
            .HasConversion(v => v ? 1 : 0, v => v == 1)
            .IsRequired();

        builder.Property(a => a.DataAlerta)
            .HasColumnName("data_alerta").IsRequired();

        builder.Property(a => a.TalhaoId)
            .HasColumnName("talhao_id_talhao").IsRequired();

        // FK para Talhao configurada no TalhaoConfiguration
    }
}