using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace petmypet.Migrations
{
    /// <inheritdoc />
    public partial class RemovePacoteMensalFromPet : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DiaVencimentoPacote",
                table: "Pets");

            migrationBuilder.DropColumn(
                name: "PacoteEmDia",
                table: "Pets");

            migrationBuilder.DropColumn(
                name: "PacoteMensal",
                table: "Pets");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DiaVencimentoPacote",
                table: "Pets",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "PacoteEmDia",
                table: "Pets",
                type: "tinyint(1)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "PacoteMensal",
                table: "Pets",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }
    }
}
