using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Products.Infrastructure.Migrations
{
  /// <inheritdoc />
  public partial class AddStockMoventProductTotal : Migration
  {
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.RenameColumn(
          name: "StockQuantity",
          table: "StockMovements",
          newName: "RunningTotal");

      migrationBuilder.AddColumn<bool>(
          name: "IsActive",
          table: "StockMovements",
          type: "boolean",
          nullable: false,
          defaultValue: false);

      migrationBuilder.AddColumn<int>(
          name: "MovementQuantity",
          table: "StockMovements",
          type: "integer",
          nullable: false,
          defaultValue: 0);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.DropColumn(
          name: "IsActive",
          table: "StockMovements");

      migrationBuilder.DropColumn(
          name: "MovementQuantity",
          table: "StockMovements");

      migrationBuilder.RenameColumn(
          name: "RunningTotal",
          table: "StockMovements",
          newName: "StockQuantity");
    }
  }
}