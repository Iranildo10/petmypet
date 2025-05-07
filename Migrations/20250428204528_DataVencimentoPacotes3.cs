using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace petmypet.Migrations
{
    /// <inheritdoc />
    public partial class DataVencimentoPacotes3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DataProximoVencimento",
                table: "AgendamentosFixos",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DataUltimoPagamento",
                table: "AgendamentosFixos",
                type: "datetime(6)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DataProximoVencimento",
                table: "AgendamentosFixos");

            migrationBuilder.DropColumn(
                name: "DataUltimoPagamento",
                table: "AgendamentosFixos");
        }
    }
}
