using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeBanking.Migrations
{
    /// <inheritdoc />
    public partial class IntentoArregloTransaccion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "transactionType",
                table: "Transactions",
                newName: "Type");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Type",
                table: "Transactions",
                newName: "transactionType");
        }
    }
}
