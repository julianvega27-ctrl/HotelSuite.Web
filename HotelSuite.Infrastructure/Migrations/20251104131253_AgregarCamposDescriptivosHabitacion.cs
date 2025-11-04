using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HotelSuite.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AgregarCamposDescriptivosHabitacion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Capacidad",
                table: "Habitaciones",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Descripcion",
                table: "Habitaciones",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MetrosCuadrados",
                table: "Habitaciones",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NumeroCamas",
                table: "Habitaciones",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Piso",
                table: "Habitaciones",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Servicios",
                table: "Habitaciones",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "TieneBalcon",
                table: "Habitaciones",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "TieneBanioPrivado",
                table: "Habitaciones",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "TieneVista",
                table: "Habitaciones",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "TipoCama",
                table: "Habitaciones",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TipoVista",
                table: "Habitaciones",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Capacidad",
                table: "Habitaciones");

            migrationBuilder.DropColumn(
                name: "Descripcion",
                table: "Habitaciones");

            migrationBuilder.DropColumn(
                name: "MetrosCuadrados",
                table: "Habitaciones");

            migrationBuilder.DropColumn(
                name: "NumeroCamas",
                table: "Habitaciones");

            migrationBuilder.DropColumn(
                name: "Piso",
                table: "Habitaciones");

            migrationBuilder.DropColumn(
                name: "Servicios",
                table: "Habitaciones");

            migrationBuilder.DropColumn(
                name: "TieneBalcon",
                table: "Habitaciones");

            migrationBuilder.DropColumn(
                name: "TieneBanioPrivado",
                table: "Habitaciones");

            migrationBuilder.DropColumn(
                name: "TieneVista",
                table: "Habitaciones");

            migrationBuilder.DropColumn(
                name: "TipoCama",
                table: "Habitaciones");

            migrationBuilder.DropColumn(
                name: "TipoVista",
                table: "Habitaciones");
        }
    }
}
