using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TerraNova.Domain.Entities;
using TerraNova.Domain.Enums;

namespace TerraNova.Infrastructure.Persistence.Configurations;

public sealed class ReqApiConfiguration : IEntityTypeConfiguration<ReqApi>
{
    public void Configure(EntityTypeBuilder<ReqApi> builder)
    {
        builder.ToTable("req_api");
        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id).HasColumnName("id_api");
 
        // Persiste como string para respeitar o CHECK ('NVDI','PRECTOTCORR')
        builder.Property(r => r.TipoParam)
            .HasColumnName("tipo_param")
            .HasMaxLength(15)
            .HasConversion(
                v => v.ToString().ToUpperInvariant(),
                v => Enum.Parse<TipoParamReqApi>(v, true))
            .IsRequired();
 
        builder.Property(r => r.DataAnalise)
            .HasColumnName("data_analise")
            .IsRequired();
 
        builder.Property(r => r.TipoApiId)
            .HasColumnName("tipo_api_id_tipo")
            .IsRequired();
 
        // FK para TipoApi configurada no TipoApiConfiguration
 
        builder.HasMany(r => r.DadosTemporais)
            .WithOne(d => d.ReqApi)
            .HasForeignKey(d => d.ReqApiId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}