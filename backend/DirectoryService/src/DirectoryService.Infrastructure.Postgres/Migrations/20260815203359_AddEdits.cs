using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DirectoryService.Infrastructure.Postgres.Migrations
{
    /// <inheritdoc />
    public partial class AddEdits : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_department_positions_departments_DepartmentId",
                table: "department_positions");

            migrationBuilder.DropForeignKey(
                name: "FK_department_positions_positions_PositionId",
                table: "department_positions");

            migrationBuilder.RenameColumn(
                name: "PositionId",
                table: "department_positions",
                newName: "position_id");

            migrationBuilder.RenameColumn(
                name: "DepartmentId",
                table: "department_positions",
                newName: "department_id");

            migrationBuilder.RenameIndex(
                name: "IX_department_positions_PositionId",
                table: "department_positions",
                newName: "IX_department_positions_position_id");

            migrationBuilder.RenameIndex(
                name: "IX_department_positions_DepartmentId",
                table: "department_positions",
                newName: "IX_department_positions_department_id");

            migrationBuilder.CreateIndex(
                name: "IX_departments_parent_id",
                table: "departments",
                column: "parent_id");

            migrationBuilder.AddForeignKey(
                name: "FK_department_positions_departments_department_id",
                table: "department_positions",
                column: "department_id",
                principalTable: "departments",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_department_positions_positions_position_id",
                table: "department_positions",
                column: "position_id",
                principalTable: "positions",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_departments_departments_parent_id",
                table: "departments",
                column: "parent_id",
                principalTable: "departments",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_department_positions_departments_department_id",
                table: "department_positions");

            migrationBuilder.DropForeignKey(
                name: "FK_department_positions_positions_position_id",
                table: "department_positions");

            migrationBuilder.DropForeignKey(
                name: "FK_departments_departments_parent_id",
                table: "departments");

            migrationBuilder.DropIndex(
                name: "IX_departments_parent_id",
                table: "departments");

            migrationBuilder.RenameColumn(
                name: "position_id",
                table: "department_positions",
                newName: "PositionId");

            migrationBuilder.RenameColumn(
                name: "department_id",
                table: "department_positions",
                newName: "DepartmentId");

            migrationBuilder.RenameIndex(
                name: "IX_department_positions_position_id",
                table: "department_positions",
                newName: "IX_department_positions_PositionId");

            migrationBuilder.RenameIndex(
                name: "IX_department_positions_department_id",
                table: "department_positions",
                newName: "IX_department_positions_DepartmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_department_positions_departments_DepartmentId",
                table: "department_positions",
                column: "DepartmentId",
                principalTable: "departments",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_department_positions_positions_PositionId",
                table: "department_positions",
                column: "PositionId",
                principalTable: "positions",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
