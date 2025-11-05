using HoloSimpID;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace RuTakingTooLong.Migrations {
  /// <inheritdoc />
  public partial class InitialCreate : Migration {
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder) {
      migrationBuilder.CreateTable(
          name: "Simps",
          columns: table =>
              new { uDex = table.Column<int>(type: "integer", nullable: false)
                               .Annotation("Npgsql:ValueGenerationStrategy",
                                           NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    dcUserName     = table.Column<string>(type: "text", nullable: false),
                    simpName       = table.Column<string>(type: "text", nullable: false),
                    profilePicPath = table.Column<string>(type: "text", nullable: false) },
          constraints: table => { table.PrimaryKey("PK_Simps", x => x.uDex); });

      migrationBuilder.CreateTable(
          name: "Carts",
          columns: table =>
              new { uDex = table.Column<int>(type: "integer", nullable: false)
                               .Annotation("Npgsql:ValueGenerationStrategy",
                                           NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CartName      = table.Column<string>(type: "text", nullable: false),
                    OwnerDex      = table.Column<int>(type: "integer", nullable: false),
                    DateOpen      = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    DatePlan      = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    DateClose     = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    DateDelivered = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    ShippingCost  = table.Column<decimal>(type: "numeric", nullable: false) },
          constraints: table => {
            table.PrimaryKey("PK_Carts", x => x.uDex);
            table.ForeignKey(name: "FK_Carts_Simps_OwnerDex", column: x => x.OwnerDex,
                             principalTable: "Simps", principalColumn: "uDex",
                             onDelete: ReferentialAction.Cascade);
          });

      migrationBuilder.CreateTable(
          name: "CartItems",
          columns: table =>
              new { cartDex    = table.Column<int>(type: "integer", nullable: false),
                    simpDex    = table.Column<int>(type: "integer", nullable: false),
                    Items      = table.Column<List<Item>>(type: "jsonb", nullable: false),
                    Quantities = table.Column<List<int>>(type: "integer[]", nullable: false) },
          constraints: table => {
            table.PrimaryKey("PK_CartItems", x => new { x.cartDex, x.simpDex });
            table.ForeignKey(name: "FK_CartItems_Carts_cartDex", column: x => x.cartDex,
                             principalTable: "Carts", principalColumn: "uDex",
                             onDelete: ReferentialAction.Cascade);
            table.ForeignKey(name: "FK_CartItems_Simps_simpDex", column: x => x.simpDex,
                             principalTable: "Simps", principalColumn: "uDex",
                             onDelete: ReferentialAction.Cascade);
          });

      migrationBuilder.CreateIndex(name: "IX_CartItems_simpDex", table: "CartItems",
                                   column: "simpDex");

      migrationBuilder.CreateIndex(name: "IX_Carts_OwnerDex", table: "Carts", column: "OwnerDex");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder) {
      migrationBuilder.DropTable(name: "CartItems");

      migrationBuilder.DropTable(name: "Carts");

      migrationBuilder.DropTable(name: "Simps");
    }
  }
}
