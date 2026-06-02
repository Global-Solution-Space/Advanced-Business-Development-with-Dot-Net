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
            .HasColumnName("titulo")
            .HasMaxLength(100)
            .IsRequired();
 
        builder.Property(a => a.Descricao)
            .HasColumnName("descricao")
            .HasMaxLength(300)
            .IsRequired();
 
        // Persiste NivelAlerta como string (VARCHAR2(20)) para legibilidade no banco
        builder.Property(a => a.NivelAlerta)
            .HasColumnName("nivel_alerta")
            .HasMaxLength(20)
            .HasConversion(
                v => v.ToString().ToUpperInvariant(),
                v => Enum.Parse<NivelAlerta>(v, true))
            .IsRequired();
 
        // Persiste bool como NUMBER (0/1)
        builder.Property(a => a.Resolvido)
            .HasColumnName("resolvido")
            .HasConversion(v => v ? 1 : 0, v => v == 1)
            .IsRequired();
 
        builder.Property(a => a.DataAlerta)
            .HasColumnName("data_alerta")
            .IsRequired();
 
        builder.Property(a => a.SatvegId)
            .HasColumnName("satveg_id_satveg")
            .IsRequired();
 
        builder.HasOne(a => a.Satveg)
            .WithMany(s => s.Alertas)
            .HasForeignKey(a => a.SatvegId)
            .OnDelete(DeleteBehavior.Restrict);
 
        builder.Property(a => a.NasaPowerId)
            .HasColumnName("nasapower_id_nasapower")
            .IsRequired();
 
        builder.HasOne(a => a.NasaPower)
            .WithMany(n => n.Alertas)
            .HasForeignKey(a => a.NasaPowerId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}