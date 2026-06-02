using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TerraNova.Domain.Entities;

namespace TerraNova.Infraestructure.Persistence.Configurations;

public sealed class NasaPowerConfiguration : IEntityTypeConfiguration<NasaPower>
{
    public void Configure(EntityTypeBuilder<NasaPower> builder)
    {
        builder.ToTable("nasapower");
        builder.HasKey(n => n.Id);
        builder.Property(n => n.Id).HasColumnName("id_nasapower");
 
        builder.Property(n => n.DataInicio)
            .HasColumnName("data_inicio")
            .HasMaxLength(8)
            .IsRequired();
 
        builder.Property(n => n.DataFim)
            .HasColumnName("data_fim")
            .HasMaxLength(8)
            .IsRequired();
 
        builder.Property(n => n.Latitude)
            .HasColumnName("latitude")
            .HasColumnType("NUMBER(9,6)")
            .IsRequired();
 
        builder.Property(n => n.Longitude)
            .HasColumnName("longitude")
            .HasColumnType("NUMBER(10,6)")
            .IsRequired();
 
        builder.Property(n => n.Elevacao)
            .HasColumnName("elevacao")
            .HasColumnType("NUMBER(5,2)")
            .IsRequired();
 
        builder.Property(n => n.TalhaoId)
            .HasColumnName("talhao_id_talhao")
            .IsRequired();
 
        // FK para Talhao configurada no TalhaoConfiguration
    }
}