using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RoiForm.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "roi_form_templates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    FormulaExpression = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: false),
                    PostfixNotation = table.Column<string>(type: "jsonb", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_roi_form_templates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "roi_form_fields",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Identifier = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Label = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    IsRequired = table.Column<bool>(type: "boolean", nullable: false),
                    RoiFormId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_roi_form_fields", x => x.Id);
                    table.ForeignKey(
                        name: "FK_roi_form_fields_roi_form_templates_RoiFormId",
                        column: x => x.RoiFormId,
                        principalTable: "roi_form_templates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_roi_form_fields_RoiFormId",
                table: "roi_form_fields",
                column: "RoiFormId");

            migrationBuilder.CreateIndex(
                name: "IX_roi_form_templates_Name",
                table: "roi_form_templates",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "roi_form_fields");

            migrationBuilder.DropTable(
                name: "roi_form_templates");
        }
    }
}
