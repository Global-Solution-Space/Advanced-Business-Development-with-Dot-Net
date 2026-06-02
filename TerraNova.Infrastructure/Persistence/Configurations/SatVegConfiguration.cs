using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TerraNova.Domain.Entities;

namespace TerraNova.Infrastructure.Persistence.Configurations;

public sealed class SatvegConfiguration : IEntityTypeConfiguration<Satveg>
{
    public void Configure(EntityTypeBuilder<Satveg> builder)
    {
        builder.ToTable("satveg");
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id).HasColumnName("id_satveg");
 
        builder.Property(s => s.TipoPerfil)
            .HasColumnName("tipo_perfil")
            .IsRequired();
 
        builder.Property(s => s.Satelite)
            .HasColumnName("satelite")
            .IsRequired();
 
        // Converte bool? para NUMBER(1)/null
        builder.Property(s => s.PreFiltro)
            .HasColumnName("pre_filtro")
            .HasColumnType("NUMBER(1)")
            .HasConversion(
                v => v.HasValue ? (v.Value ? 1 : 0) : (int?)null,
                v => v.HasValue ? v.Value == 1 : (bool?)null)
            .IsRequired(false);
 
        builder.Property(s => s.Filtro)
            .HasColumnName("filtro")
            .HasMaxLength(3)
            .IsRequired(false);
 
        builder.Property(s => s.ParametroFiltro)
            .HasColumnName("parametro_filtro")
            .HasColumnType("NUMBER(2)")
            .IsRequired(false);
 
        builder.Property(s => s.Poligono)
            .HasColumnName("poligono")
            .HasColumnType("CLOB")
            .IsRequired();
 
        // Converte bool para NUMBER
        builder.Property(s => s.TodasEstatisticas)
            .HasColumnName("todas_estatisticas")
            .HasConversion(v => v ? 1 : 0, v => v == 1)
            .IsRequired();
 
        builder.Property(s => s.DataAnalise)
            .HasColumnName("data_analise")
            .IsRequired();
 
        builder.Property(s => s.TalhaoId)
            .HasColumnName("talhao_id_talhao")
            .IsRequired();
 
        // FK para Talhao configurada no TalhaoConfiguration
        
        builder.Property(s => s.DadosJson)
            .HasColumnName("dados_json")
            .HasColumnType("CLOB")
            .IsRequired();
    }
}