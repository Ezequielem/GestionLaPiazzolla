using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GestionLaPiazzolla.Data.Migrations
{
    public partial class Cambios : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Monto",
                table: "Alumnos_X_Cursos",
                type: "money",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "Clases",
                columns: table => new
                {
                    ClaseId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(maxLength: 50, nullable: false),
                    FechaYHora = table.Column<DateTime>(nullable: false),
                    Observacion = table.Column<string>(nullable: true),
                    CursoId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clases", x => x.ClaseId);
                    table.ForeignKey(
                        name: "FK_Clases_Cursos_CursoId",
                        column: x => x.CursoId,
                        principalTable: "Cursos",
                        principalColumn: "CursoId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DetallesDePagos",
                columns: table => new
                {
                    DetalleDePagoId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Concepto = table.Column<string>(nullable: false),
                    Cantidad = table.Column<int>(nullable: false),
                    PagoId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DetallesDePagos", x => x.DetalleDePagoId);
                    table.ForeignKey(
                        name: "FK_DetallesDePagos_Pagos_PagoId",
                        column: x => x.PagoId,
                        principalTable: "Pagos",
                        principalColumn: "PagoId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Alumnos_X_Clases",
                columns: table => new
                {
                    AlumnoId = table.Column<int>(nullable: false),
                    ClaseId = table.Column<int>(nullable: false),
                    Asistencia = table.Column<bool>(nullable: false),
                    Observacion = table.Column<string>(maxLength: 512, nullable: true),
                    FechaYHora = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Alumnos_X_Clases", x => new { x.AlumnoId, x.ClaseId });
                    table.ForeignKey(
                        name: "FK_Alumnos_X_Clases_Alumnos_AlumnoId",
                        column: x => x.AlumnoId,
                        principalTable: "Alumnos",
                        principalColumn: "AlumnoId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Alumnos_X_Clases_Clases_ClaseId",
                        column: x => x.ClaseId,
                        principalTable: "Clases",
                        principalColumn: "ClaseId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Alumnos_X_Clases_ClaseId",
                table: "Alumnos_X_Clases",
                column: "ClaseId");

            migrationBuilder.CreateIndex(
                name: "IX_Clases_CursoId",
                table: "Clases",
                column: "CursoId");

            migrationBuilder.CreateIndex(
                name: "IX_DetallesDePagos_PagoId",
                table: "DetallesDePagos",
                column: "PagoId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Alumnos_X_Clases");

            migrationBuilder.DropTable(
                name: "DetallesDePagos");

            migrationBuilder.DropTable(
                name: "Clases");

            migrationBuilder.DropColumn(
                name: "Monto",
                table: "Alumnos_X_Cursos");
        }
    }
}
