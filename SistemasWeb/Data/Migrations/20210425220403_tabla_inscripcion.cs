using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SistemasWeb.Data.Migrations
{
    public partial class tabla_inscripcion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "_TInscripcion",
                columns: table => new
                {
                    InscripcionID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EstudianteID = table.Column<string>(nullable: true),
                    Fecha = table.Column<DateTime>(nullable: false),
                    Pago = table.Column<decimal>(nullable: false),
                    CursoID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__TInscripcion", x => x.InscripcionID);
                    table.ForeignKey(
                        name: "FK__TInscripcion__TCursos_CursoID",
                        column: x => x.CursoID,
                        principalTable: "_TCursos",
                        principalColumn: "CursoID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX__TInscripcion_CursoID",
                table: "_TInscripcion",
                column: "CursoID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "_TInscripcion");
        }
    }
}
