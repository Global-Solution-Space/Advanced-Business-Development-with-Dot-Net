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
                name: "tipo_api",
                columns: table => new
                {
                    id_tipo = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    tipo_api = table.Column<string>(type: "NVARCHAR2(10)", maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tipo_api", x => x.id_tipo);
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
                name: "req_api",
                columns: table => new
                {
                    id_api = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    tipo_param = table.Column<string>(type: "NVARCHAR2(15)", maxLength: 15, nullable: false),
                    data_analise = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    tipo_api_id_tipo = table.Column<Guid>(type: "RAW(16)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_req_api", x => x.id_api);
                    table.ForeignKey(
                        name: "FK_req_api_tipo_api_tipo_api_id_tipo",
                        column: x => x.tipo_api_id_tipo,
                        principalTable: "tipo_api",
                        principalColumn: "id_tipo",
                        onDelete: ReferentialAction.Restrict);
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
                name: "alerta_agricola",
                columns: table => new
                {
                    id_alerta = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    titulo = table.Column<string>(type: "NVARCHAR2(100)", maxLength: 100, nullable: false),
                    descricao = table.Column<string>(type: "NVARCHAR2(300)", maxLength: 300, nullable: false),
                    nivel_alerta = table.Column<string>(type: "NVARCHAR2(20)", maxLength: 20, nullable: false),
                    resolvido = table.Column<int>(type: "NUMBER(10)", nullable: false),
                    data_alerta = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    talhao_id_talhao = table.Column<Guid>(type: "RAW(16)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_alerta_agricola", x => x.id_alerta);
                    table.ForeignKey(
                        name: "FK_alerta_agricola_talhao_talhao_id_talhao",
                        column: x => x.talhao_id_talhao,
                        principalTable: "talhao",
                        principalColumn: "id_talhao",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "dado_temporal",
                columns: table => new
                {
                    id_dado = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    data_leitura = table.Column<DateTime>(type: "TIMESTAMP(7)", nullable: false),
                    valor = table.Column<decimal>(type: "NUMBER(18,6)", nullable: false),
                    talhao_id_talhao = table.Column<Guid>(type: "RAW(16)", nullable: false),
                    req_api_id_api = table.Column<Guid>(type: "RAW(16)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_dado_temporal", x => x.id_dado);
                    table.ForeignKey(
                        name: "FK_dado_temporal_req_api_req_api_id_api",
                        column: x => x.req_api_id_api,
                        principalTable: "req_api",
                        principalColumn: "id_api",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_dado_temporal_talhao_talhao_id_talhao",
                        column: x => x.talhao_id_talhao,
                        principalTable: "talhao",
                        principalColumn: "id_talhao",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_alerta_agricola_talhao_id_talhao",
                table: "alerta_agricola",
                column: "talhao_id_talhao");

            migrationBuilder.CreateIndex(
                name: "IX_dado_temporal_req_api_id_api",
                table: "dado_temporal",
                column: "req_api_id_api");

            migrationBuilder.CreateIndex(
                name: "IX_dado_temporal_talhao_id_talhao",
                table: "dado_temporal",
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
                name: "IX_req_api_tipo_api_id_tipo",
                table: "req_api",
                column: "tipo_api_id_tipo");

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
                name: "dado_temporal");

            migrationBuilder.DropTable(
                name: "telefone");

            migrationBuilder.DropTable(
                name: "req_api");

            migrationBuilder.DropTable(
                name: "talhao");

            migrationBuilder.DropTable(
                name: "tipo_api");

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
