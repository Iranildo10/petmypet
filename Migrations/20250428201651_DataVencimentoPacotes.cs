using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace petmypet.Migrations
{
    /// <inheritdoc />
    public partial class DataVencimentoPacotes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DiaVencimentoPacote",
                table: "AgendamentosFixos",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "PacoteEmDia",
                table: "AgendamentosFixos",
                type: "tinyint(1)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "PacoteMensal",
                table: "AgendamentosFixos",
                type: "tinyint(1)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DiaVencimentoPacote",
                table: "AgendamentosFixos");

            migrationBuilder.DropColumn(
                name: "PacoteEmDia",
                table: "AgendamentosFixos");

            migrationBuilder.DropColumn(
                name: "PacoteMensal",
                table: "AgendamentosFixos");
        }
    }
}
