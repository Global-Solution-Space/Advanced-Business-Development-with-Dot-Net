using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TerraNova.Domain.Entities;

namespace TerraNova.Infrastructure.Persistence.Configurations;

public sealed class TalhaoConfiguration : IEntityTypeConfiguration<Talhao>
{
    public void Configure(EntityTypeBuilder<Talhao> builder)
    {
        builder.ToTable("talhao");
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id).HasColumnName("id_talhao");

        builder.Property(t => t.NomeTalhao)
            .HasColumnName("nome_talhao").HasMaxLength(30).IsRequired();

        builder.Property(t => t.VolumArea)
            .HasColumnName("volum_area").IsRequired();

        builder.Property(t => t.TipoPlantacaoId)
            .HasColumnName("tipo_plantacao_id_tipo_plant").IsRequired();

        builder.HasOne(t => t.TipoPlantacao)
            .WithMany(tp => tp.Talhoes)
            .HasForeignKey(t => t.TipoPlantacaoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(t => t.PropriedadeId)
            .HasColumnName("propriedade_id_propriedade").IsRequired();

        // FK Propriedade configurada no PropriedadeConfiguration

        builder.Property(t => t.LocalizacaoId)
            .HasColumnName("localizacao_id_localizacao").IsRequired();

        builder.HasOne(t => t.Localizacao)
            .WithOne(l => l.Talhao)
            .HasForeignKey<Talhao>(t => t.LocalizacaoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(t => t.LocalizacaoId).IsUnique();

        builder.HasMany(t => t.DadosTemporais)
            .WithOne(d => d.Talhao)
            .HasForeignKey(d => d.TalhaoId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(t => t.Alertas)
            .WithOne(a => a.Talhao)
            .HasForeignKey(a => a.TalhaoId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}