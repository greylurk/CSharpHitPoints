using Microsoft.EntityFrameworkCore.Migrations;

namespace HitPoints.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Damage",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Amount = table.Column<long>(nullable: false),
                    Type = table.Column<int>(nullable: false),
                    IsMagic = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Damage", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Modifier",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AffectedObject = table.Column<string>(nullable: true),
                    AffectedValue = table.Column<string>(nullable: true),
                    Value = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Modifier", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PlayerCharacter",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerCharacter", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CharacterClass",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    HitDiceValue = table.Column<int>(nullable: false),
                    ClassLevel = table.Column<int>(nullable: false),
                    PlayerCharacterId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacterClass", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CharacterClass_PlayerCharacter_PlayerCharacterId",
                        column: x => x.PlayerCharacterId,
                        principalTable: "PlayerCharacter",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Defense",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DefenseType = table.Column<int>(nullable: false),
                    DamageType = table.Column<int>(nullable: false),
                    PlayerCharacterId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Defense", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Defense_PlayerCharacter_PlayerCharacterId",
                        column: x => x.PlayerCharacterId,
                        principalTable: "PlayerCharacter",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Item",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    ModifierId = table.Column<long>(nullable: true),
                    PlayerCharacterId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Item", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Item_Modifier_ModifierId",
                        column: x => x.ModifierId,
                        principalTable: "Modifier",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Item_PlayerCharacter_PlayerCharacterId",
                        column: x => x.PlayerCharacterId,
                        principalTable: "PlayerCharacter",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CharacterClass_PlayerCharacterId",
                table: "CharacterClass",
                column: "PlayerCharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_Defense_PlayerCharacterId",
                table: "Defense",
                column: "PlayerCharacterId");

            migrationBuilder.CreateIndex(
                name: "IX_Item_ModifierId",
                table: "Item",
                column: "ModifierId");

            migrationBuilder.CreateIndex(
                name: "IX_Item_PlayerCharacterId",
                table: "Item",
                column: "PlayerCharacterId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CharacterClass");

            migrationBuilder.DropTable(
                name: "Damage");

            migrationBuilder.DropTable(
                name: "Defense");

            migrationBuilder.DropTable(
                name: "Item");

            migrationBuilder.DropTable(
                name: "Modifier");

            migrationBuilder.DropTable(
                name: "PlayerCharacter");
        }
    }
}
