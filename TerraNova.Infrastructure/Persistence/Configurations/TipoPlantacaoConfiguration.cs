using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TerraNova.Domain.Entities;

namespace TerraNova.Infrastructure.Persistence.Configurations;

public sealed class TipoPlantacaoConfiguration : IEntityTypeConfiguration<TipoPlantacao>
{
    public void Configure(EntityTypeBuilder<TipoPlantacao> builder)
    {
        builder.ToTable("tipo_plantacao");
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id).HasColumnName("id_tipo_plant");
 
        builder.Property(t => t.TipoPlant)
            .HasColumnName("tipo_plant")
            .HasMaxLength(30)
            .IsRequired();
    }
}