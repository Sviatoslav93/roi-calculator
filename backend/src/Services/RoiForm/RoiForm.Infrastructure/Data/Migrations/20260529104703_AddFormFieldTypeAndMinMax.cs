using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RoiForm.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddFormFieldTypeAndMinMax : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Max",
                table: "roi_form_fields",
                type: "numeric(18,6)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Min",
                table: "roi_form_fields",
                type: "numeric(18,6)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "roi_form_fields",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Max",
                table: "roi_form_fields");

            migrationBuilder.DropColumn(
                name: "Min",
                table: "roi_form_fields");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "roi_form_fields");
        }
    }
}
