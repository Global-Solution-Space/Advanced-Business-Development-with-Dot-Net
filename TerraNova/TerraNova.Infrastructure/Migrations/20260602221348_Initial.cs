using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TerraNova.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "localizacao",
                columns: table => new
                {
                    id_localizacao = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    loc_latitude = table.Column<decimal>(type: "NUMBER(8,6)", nullable: false),
                    loc_longitude = table.Column<decimal>(type: "NUMBER(9,6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_localizacao", x => x.id_localizacao);
                });

            migrationBuilder.CreateTable(
                name: "produtor",
                columns: table => new
                {
                    id_produtor = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    nome = table.Column<string>(type: "NVARCHAR2(30)", maxLength: 30, nullable: false),
                    email = table.Column<string>(type: "NVARCHAR2(30)", maxLength: 30, nullable: false),
                    senha = table.Column<string>(type: "NVARCHAR2(30)", maxLength: 30, nullable: false),
                    telefone = table.Column<string>(type: "NVARCHAR2(11)", maxLength: 11, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_produtor", x => x.id_produtor);
                });

            migrationBuilder.CreateTable(
                name: "tipo_plantacao",
                columns: table => new
                {
                    id_tipo_plant = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    tipo_plant = table.Column<string>(type: "NVARCHAR2(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tipo_plantacao", x => x.id_tipo_plant);
                });

            migrationBuilder.CreateTable(
                name: "propriedade",
                columns: table => new
                {
                    id_propriedade = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    nome = table.Column<string>(type: "NVARCHAR2(30)", maxLength: 30, nullable: false),
                    tamanho_total = table.Column<decimal>(type: "DECIMAL(18,4)", precision: 18, scale: 4, nullable: false),
                    produtor_id_produtor = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    localizacao_id_localizacao = table.Column<Guid>(type: "RAW(16)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_propriedade", x => x.id_propriedade);
                    table.ForeignKey(
                        name: "FK_propriedade_localizacao_localizacao_id_localizacao",
                        column: x => x.localizacao_id_localizacao,
                        principalTable: "localizacao",
                        principalColumn: "id_localizacao",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_propriedade_produtor_produtor_id_produtor",
                        column: x => x.produtor_id_produtor,
                        principalTable: "produtor",
                        principalColumn: "id_produtor",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "telefone",
                columns: table => new
                {
                    id_telefone = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    ddd = table.Column<string>(type: "NCHAR(2)", fixedLength: true, maxLength: 2, nullable: false),
                    numero = table.Column<string>(type: "NVARCHAR2(9)", maxLength: 9, nullable: false),
                    produtor_id_produtor = table.Column<Guid>(type: "RAW(16)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_telefone", x => x.id_telefone);
                    table.ForeignKey(
                        name: "FK_telefone_produtor_produtor_id_produtor",
                        column: x => x.produtor_id_produtor,
                        principalTable: "produtor",
                        principalColumn: "id_produtor",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "talhao",
                columns: table => new
                {
                    id_talhao = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    nome_talhao = table.Column<string>(type: "NVARCHAR2(30)", maxLength: 30, nullable: false),
                    volum_area = table.Column<decimal>(type: "DECIMAL(18,4)", precision: 18, scale: 4, nullable: false),
                    tipo_plantacao_id_tipo_plant = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    propriedade_id_propriedade = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    localizacao_id_localizacao = table.Column<Guid>(type: "RAW(16)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_talhao", x => x.id_talhao);
                    table.ForeignKey(
                        name: "FK_talhao_localizacao_localizacao_id_localizacao",
                        column: x => x.localizacao_id_localizacao,
                        principalTable: "localizacao",
                        principalColumn: "id_localizacao",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_talhao_propriedade_propriedade_id_propriedade",
                        column: x => x.propriedade_id_propriedade,
                        principalTable: "propriedade",
                        principalColumn: "id_propriedade",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_talhao_tipo_plantacao_tipo_plantacao_id_tipo_plant",
                        column: x => x.tipo_plantacao_id_tipo_plant,
                        principalTable: "tipo_plantacao",
                        principalColumn: "id_tipo_plant",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "nasapower",
                columns: table => new
                {
                    id_nasapower = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    data_inicio = table.Column<string>(type: "NVARCHAR2(8)", maxLength: 8, nullable: false),
                    data_fim = table.Column<string>(type: "NVARCHAR2(8)", maxLength: 8, nullable: false),
                    latitude = table.Column<decimal>(type: "NUMBER(9,6)", nullable: false),
                    longitude = table.Column<decimal>(type: "NUMBER(10,6)", nullable: false),
                    elevacao = table.Column<decimal>(type: "NUMBER(5,2)", nullable: false),
                    talhao_id_talhao = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    dados_json = table.Column<string>(type: "CLOB", nullable: false),
                    data_analise = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_nasapower", x => x.id_nasapower);
                    table.ForeignKey(
                        name: "FK_nasapower_talhao_talhao_id_talhao",
                        column: x => x.talhao_id_talhao,
                        principalTable: "talhao",
                        principalColumn: "id_talhao",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "satveg",
                columns: table => new
                {
                    id_satveg = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    tipo_perfil = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    satelite = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    pre_filtro = table.Column<int>(type: "NUMBER(1)", nullable: true),
                    filtro = table.Column<string>(type: "NVARCHAR2(3)", maxLength: 3, nullable: true),
                    parametro_filtro = table.Column<byte>(type: "NUMBER(2)", nullable: true),
                    poligono = table.Column<string>(type: "CLOB", nullable: false),
                    todas_estatisticas = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    data_analise = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    talhao_id_talhao = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    dados_json = table.Column<string>(type: "CLOB", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_satveg", x => x.id_satveg);
                    table.ForeignKey(
                        name: "FK_satveg_talhao_talhao_id_talhao",
                        column: x => x.talhao_id_talhao,
                        principalTable: "talhao",
                        principalColumn: "id_talhao",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "alerta_agricola",
                columns: table => new
                {
                    id_alerta = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    titulo = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    descricao = table.Column<string>(type: "NVARCHAR2(300)", maxLength: 300, nullable: false),
                    nivel_alerta = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: false),
                    resolvido = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    data_alerta = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    satveg_id_satveg = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    nasapower_id_nasapower = table.Column<Guid>(type: "RAW(16)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_alerta_agricola", x => x.id_alerta);
                    table.ForeignKey(
                        name: "FK_alerta_agricola_nasapower_nasapower_id_nasapower",
                        column: x => x.nasapower_id_nasapower,
                        principalTable: "nasapower",
                        principalColumn: "id_nasapower",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_alerta_agricola_satveg_satveg_id_satveg",
                        column: x => x.satveg_id_satveg,
                        principalTable: "satveg",
                        principalColumn: "id_satveg",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_alerta_agricola_nasapower_id_nasapower",
                table: "alerta_agricola",
                column: "nasapower_id_nasapower");

            migrationBuilder.CreateIndex(
                name: "IX_alerta_agricola_satveg_id_satveg",
                table: "alerta_agricola",
                column: "satveg_id_satveg");

            migrationBuilder.CreateIndex(
                name: "IX_nasapower_talhao_id_talhao",
                table: "nasapower",
                column: "talhao_id_talhao");

            migrationBuilder.CreateIndex(
                name: "IX_produtor_email",
                table: "produtor",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_propriedade_localizacao_id_localizacao",
                table: "propriedade",
                column: "localizacao_id_localizacao",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_propriedade_produtor_id_produtor",
                table: "propriedade",
                column: "produtor_id_produtor");

            migrationBuilder.CreateIndex(
                name: "IX_satveg_talhao_id_talhao",
                table: "satveg",
                column: "talhao_id_talhao");

            migrationBuilder.CreateIndex(
                name: "IX_talhao_localizacao_id_localizacao",
                table: "talhao",
                column: "localizacao_id_localizacao",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_talhao_propriedade_id_propriedade",
                table: "talhao",
                column: "propriedade_id_propriedade");

            migrationBuilder.CreateIndex(
                name: "IX_talhao_tipo_plantacao_id_tipo_plant",
                table: "talhao",
                column: "tipo_plantacao_id_tipo_plant");

            migrationBuilder.CreateIndex(
                name: "IX_telefone_produtor_id_produtor",
                table: "telefone",
                column: "produtor_id_produtor",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "alerta_agricola");

            migrationBuilder.DropTable(
                name: "telefone");

            migrationBuilder.DropTable(
                name: "nasapower");

            migrationBuilder.DropTable(
                name: "satveg");

            migrationBuilder.DropTable(
                name: "talhao");

            migrationBuilder.DropTable(
                name: "propriedade");

            migrationBuilder.DropTable(
                name: "tipo_plantacao");

            migrationBuilder.DropTable(
                name: "localizacao");

            migrationBuilder.DropTable(
                name: "produtor");
        }
    }
}
