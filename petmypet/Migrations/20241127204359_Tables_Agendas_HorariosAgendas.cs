using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace petmypet.Migrations
{
    /// <inheritdoc />
    public partial class Tables_Agendas_HorariosAgendas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Agendas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Nome = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Seg = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Ter = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Qua = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Qui = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Sex = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Sab = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Dom = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    HorarioInicial = table.Column<TimeSpan>(type: "time", nullable: false),
                    HorarioFinal = table.Column<TimeSpan>(type: "time", nullable: false),
                    DuracaoHorario = table.Column<TimeSpan>(type: "time", nullable: false),
                    InicioIntervalo = table.Column<TimeSpan>(type: "time", nullable: false),
                    FimIntervalo = table.Column<TimeSpan>(type: "time", nullable: false),
                    Ativo = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Agendas", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "HorariosAgendas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Horario = table.Column<TimeSpan>(type: "time", nullable: false),
                    AgendaId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HorariosAgendas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HorariosAgendas_Agendas_AgendaId",
                        column: x => x.AgendaId,
                        principalTable: "Agendas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_HorariosAgendas_AgendaId",
                table: "HorariosAgendas",
                column: "AgendaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HorariosAgendas");

            migrationBuilder.DropTable(
                name: "Agendas");
        }
    }
}
