using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

#nullable disable

namespace TerraNova.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSpatialLocalizacao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "loc_latitude",
                table: "localizacao");

            migrationBuilder.DropColumn(
                name: "loc_longitude",
                table: "localizacao");

            migrationBuilder.AddColumn<Point>(
                name: "coordenadas",
                table: "localizacao",
                type: "SDO_GEOMETRY",
                nullable: false);

            migrationBuilder.Sql("INSERT INTO USER_SDO_GEOM_METADATA (TABLE_NAME, COLUMN_NAME, DIMINFO, SRID) VALUES ('localizacao', 'coordenadas', SDO_DIM_ARRAY(SDO_DIM_ELEMENT('X', -180, 180, 0.005), SDO_DIM_ELEMENT('Y', -90, 90, 0.005)), 4326)");
            migrationBuilder.Sql("CREATE INDEX idx_localizacao_coordenadas ON localizacao(coordenadas) INDEXTYPE IS MDSYS.SPATIAL_INDEX_V2");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP INDEX idx_localizacao_coordenadas");
            migrationBuilder.Sql("DELETE FROM USER_SDO_GEOM_METADATA WHERE TABLE_NAME = 'localizacao' AND COLUMN_NAME = 'coordenadas'");

            migrationBuilder.DropColumn(
                name: "coordenadas",
                table: "localizacao");

            migrationBuilder.AddColumn<decimal>(
                name: "loc_latitude",
                table: "localizacao",
                type: "NUMBER(8,6)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "loc_longitude",
                table: "localizacao",
                type: "NUMBER(9,6)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
