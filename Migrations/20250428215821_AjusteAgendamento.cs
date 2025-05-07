using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace petmypet.Migrations
{
    /// <inheritdoc />
    public partial class AjusteAgendamento : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DataProximoVencimento",
                table: "Agendamentos",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DataUltimoPagamento",
                table: "Agendamentos",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DiaVencimentoPacote",
                table: "Agendamentos",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "PacoteEmDia",
                table: "Agendamentos",
                type: "tinyint(1)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PagamentosPacotes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    AgendamentoFixoId = table.Column<int>(type: "int", nullable: false),
                    DataPagamento = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    ReferenteAoMes = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    ValorPago = table.Column<decimal>(type: "decimal(65,30)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PagamentosPacotes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PagamentosPacotes_AgendamentosFixos_AgendamentoFixoId",
                        column: x => x.AgendamentoFixoId,
                        principalTable: "AgendamentosFixos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_PagamentosPacotes_AgendamentoFixoId",
                table: "PagamentosPacotes",
                column: "AgendamentoFixoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PagamentosPacotes");

            migrationBuilder.DropColumn(
                name: "DataProximoVencimento",
                table: "Agendamentos");

            migrationBuilder.DropColumn(
                name: "DataUltimoPagamento",
                table: "Agendamentos");

            migrationBuilder.DropColumn(
                name: "DiaVencimentoPacote",
                table: "Agendamentos");

            migrationBuilder.DropColumn(
                name: "PacoteEmDia",
                table: "Agendamentos");
        }
    }
}
