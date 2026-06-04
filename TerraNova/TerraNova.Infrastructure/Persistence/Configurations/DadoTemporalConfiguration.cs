using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TerraNova.Domain.Entities;

namespace TerraNova.Infrastructure.Persistence.Configurations;

public sealed class DadoTemporalConfiguration : IEntityTypeConfiguration<DadoTemporal>
{
    public void Configure(EntityTypeBuilder<DadoTemporal> builder)
    {
        builder.ToTable("dado_temporal");
        builder.HasKey(d => d.Id);
        builder.Property(d => d.Id).HasColumnName("id_dado");
 
        builder.Property(d => d.DataLeitura)
            .HasColumnName("data_leitura")
            .IsRequired();
 
        builder.Property(d => d.Valor)
            .HasColumnName("valor")
            .HasColumnType("NUMBER(18,6)")
            .IsRequired();
 
        builder.Property(d => d.TalhaoId)
            .HasColumnName("talhao_id_talhao")
            .IsRequired();
 
        // FK Talhao configurada no TalhaoConfiguration
 
        builder.Property(d => d.ReqApiId)
            .HasColumnName("req_api_id_api")
            .IsRequired();
 
        // FK ReqApi configurada no ReqApiConfiguration
    }
}
