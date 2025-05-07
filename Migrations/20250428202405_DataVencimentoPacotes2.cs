using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace petmypet.Migrations
{
    /// <inheritdoc />
    public partial class DataVencimentoPacotes2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PacoteMensal",
                table: "AgendamentosFixos");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "PacoteMensal",
                table: "AgendamentosFixos",
                type: "tinyint(1)",
                nullable: true);
        }
    }
}
