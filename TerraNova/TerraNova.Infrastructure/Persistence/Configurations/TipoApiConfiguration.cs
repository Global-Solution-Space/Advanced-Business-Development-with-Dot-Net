using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TerraNova.Domain.Entities;

namespace TerraNova.Infrastructure.Persistence.Configurations;

public sealed class TipoApiConfiguration : IEntityTypeConfiguration<TipoApi>
{
    public void Configure(EntityTypeBuilder<TipoApi> builder)
    {
        builder.ToTable("tipo_api");
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id).HasColumnName("id_tipo");
 
        builder.Property(t => t.NomeTipoApi)
            .HasColumnName("tipo_api")
            .HasMaxLength(10)
            .IsRequired();
 
        builder.HasMany(t => t.ReqApis)
            .WithOne(r => r.TipoApi)
            .HasForeignKey(r => r.TipoApiId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}