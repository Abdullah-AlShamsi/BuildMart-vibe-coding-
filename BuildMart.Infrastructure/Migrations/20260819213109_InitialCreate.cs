using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BuildMart.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ImageUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Carts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Carts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Carts_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    OrderDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ShippingAddress = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    PaymentMethod = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    PaymentStatus = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    OrderStatus = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Orders_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DiscountPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    StockQuantity = table.Column<int>(type: "int", nullable: false),
                    SKU = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    Brand = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Unit = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Weight = table.Column<decimal>(type: "decimal(10,3)", nullable: true),
                    IsAvailable = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Products_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CartItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CartId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CartItems_Carts_CartId",
                        column: x => x.CartId,
                        principalTable: "Carts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CartItems_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OrderItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderItems_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderItems_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "CreatedAt", "Description", "ImageUrl", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Electric and battery-powered tools for professional and DIY work.", "/images/categories/power-tools.jpg", "Power Tools", null },
                    { 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Manual tools for everyday construction and repair tasks.", "/images/categories/hand-tools.jpg", "Hand Tools", null },
                    { 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Cement, concrete mix, adhesives and waterproofing materials.", "/images/categories/construction-materials.jpg", "Construction Materials", null },
                    { 4, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Personal protective equipment for job sites.", "/images/categories/safety-equipment.jpg", "Safety Equipment", null },
                    { 5, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Rollers, brushes, trays and surface prep tools.", "/images/categories/painting-tools.jpg", "Painting Tools", null },
                    { 6, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Tools and accessories for electrical installation work.", "/images/categories/electrical-tools.jpg", "Electrical Tools", null },
                    { 7, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Tools and fittings for plumbing and pipework.", "/images/categories/plumbing-tools.jpg", "Plumbing Tools", null },
                    { 8, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Fasteners, hinges and general hardware supplies.", "/images/categories/hardware.jpg", "Hardware", null }
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "Brand", "CategoryId", "CreatedAt", "Description", "DiscountPrice", "ImageUrl", "IsAvailable", "Name", "Price", "SKU", "StockQuantity", "Unit", "UpdatedAt", "Weight" },
                values: new object[,]
                {
                    { 1, "Bosch", 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Variable-speed corded electric drill with keyless chuck.", 39.900m, "/images/products/electric-drill.jpg", true, "Electric Drill 750W", 45.900m, "PWR-DRL-001", 40, "Piece", null, 1.800m },
                    { 2, "Makita", 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "850W angle grinder for cutting and grinding metal.", null, "/images/products/angle-grinder.jpg", true, "Angle Grinder 115mm", 38.500m, "PWR-GRD-002", 25, "Piece", null, 2.100m },
                    { 3, "DeWalt", 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Corded circular saw with laser guide for straight cuts.", 54.900m, "/images/products/circular-saw.jpg", true, "Circular Saw 1200W", 62.000m, "PWR-SAW-003", 15, "Piece", null, 3.400m },
                    { 4, "Makita", 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Cordless impact driver with 2 batteries and fast charger.", null, "/images/products/impact-driver.jpg", true, "Impact Driver 18V", 58.750m, "PWR-IMD-004", 30, "Piece", null, 1.500m },
                    { 5, "Bosch", 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Battery-powered jigsaw for curved and straight cuts.", null, "/images/products/jigsaw.jpg", true, "Cordless Jigsaw 18V", 41.200m, "PWR-JIG-005", 20, "Piece", null, 1.900m },
                    { 6, "Stanley", 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Fiberglass handle claw hammer with shock-absorbing grip.", null, "/images/products/claw-hammer.jpg", true, "Claw Hammer 16oz", 8.900m, "HND-HMR-006", 100, "Piece", null, 0.600m },
                    { 7, "Stanley", 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Assorted flathead and Phillips screwdrivers with magnetic tips.", 11.900m, "/images/products/screwdriver-set.jpg", true, "Screwdriver Set 12-Piece", 14.500m, "HND-SCD-007", 60, "Set", null, 0.900m },
                    { 8, "Irwin", 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Chrome vanadium steel adjustable wrench.", null, "/images/products/adjustable-wrench.jpg", true, "Adjustable Wrench 10-inch", 9.750m, "HND-WRN-008", 80, "Piece", null, 0.400m },
                    { 9, "Irwin", 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Heavy-duty combination pliers with insulated grip.", null, "/images/products/combination-pliers.jpg", true, "Combination Pliers 8-inch", 7.200m, "HND-PLR-009", 90, "Piece", null, 0.300m },
                    { 10, "Stanley", 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Retractable steel measuring tape with locking mechanism.", null, "/images/products/measuring-tape.jpg", true, "Measuring Tape 8m", 6.500m, "HND-TAP-010", 120, "Piece", null, 0.250m },
                    { 11, "LafargeHolcim", 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "General-purpose Portland cement, high-strength grade.", null, "/images/products/cement-bag.jpg", true, "Portland Cement 50kg", 4.200m, "MAT-CEM-011", 500, "Bag", null, 50.000m },
                    { 12, "Quikrete", 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Pre-mixed concrete, just add water.", null, "/images/products/concrete-mix.jpg", true, "Ready Concrete Mix 40kg", 5.800m, "MAT-CON-012", 300, "Bag", null, 40.000m },
                    { 13, "Weber", 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Cement-based adhesive for ceramic and porcelain tiles.", null, "/images/products/tile-adhesive.jpg", true, "Tile Adhesive 25kg", 6.900m, "MAT-ADH-013", 200, "Bag", null, 25.000m },
                    { 14, "Sika", 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Liquid-applied waterproofing membrane for roofs and terraces.", 27.500m, "/images/products/waterproofing.jpg", true, "Waterproofing Membrane 10L", 32.000m, "MAT-WPF-014", 60, "Liter", null, 10.500m },
                    { 15, "Knauf", 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Interior wall and ceiling finishing plaster.", null, "/images/products/gypsum-plaster.jpg", true, "Gypsum Plaster 25kg", 5.100m, "MAT-GYP-015", 250, "Bag", null, 25.000m },
                    { 16, "3M", 4, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "ANSI-rated hard hat with adjustable ratchet suspension.", null, "/images/products/safety-helmet.jpg", true, "Safety Helmet - Yellow", 6.900m, "SAF-HLM-016", 150, "Piece", null, 0.400m },
                    { 17, "3M", 4, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Level 5 cut-resistant work gloves, size L.", null, "/images/products/safety-gloves.jpg", true, "Safety Gloves - Cut Resistant", 4.500m, "SAF-GLV-017", 200, "Piece", null, 0.150m },
                    { 18, "3M", 4, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Anti-fog, anti-scratch polycarbonate safety glasses.", null, "/images/products/safety-glasses.jpg", true, "Safety Glasses - Clear", 3.200m, "SAF-GLS-018", 180, "Piece", null, 0.050m },
                    { 19, "Generic", 4, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Reflective high-visibility vest, orange, one size fits most.", null, "/images/products/safety-vest.jpg", true, "Hi-Vis Safety Vest", 5.400m, "SAF-VST-019", 160, "Piece", null, 0.200m },
                    { 20, "Caterpillar", 4, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Slip-resistant steel toe cap safety boots, size 42.", 24.900m, "/images/products/work-boots.jpg", true, "Steel Toe Work Boots", 28.900m, "SAF-BOT-020", 70, "Piece", null, 1.200m },
                    { 21, "Generic", 5, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Roller frame with two medium-nap sleeves.", null, "/images/products/paint-roller.jpg", true, "Paint Roller Set 9-inch", 6.200m, "PNT-ROL-021", 140, "Set", null, 0.350m },
                    { 22, "Generic", 5, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Angled sash paint brush with synthetic bristles.", null, "/images/products/paint-brush.jpg", true, "Paint Brush 2-inch", 2.900m, "PNT-BRS-022", 220, "Piece", null, 0.080m },
                    { 23, "Generic", 5, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Plastic paint tray with ribbed roller-off ramp.", null, "/images/products/paint-tray.jpg", true, "Paint Tray 9-inch", 3.500m, "PNT-TRY-023", 130, "Piece", null, 0.220m },
                    { 24, "Stanley", 5, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Flexible stainless steel wall scraper for surface prep.", null, "/images/products/scraper.jpg", true, "Wall Scraper 4-inch", 4.100m, "PNT-SCR-024", 110, "Piece", null, 0.150m },
                    { 25, "Klein Tools", 6, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Automatic wire stripper for 10-24 AWG cable.", null, "/images/products/wire-stripper.jpg", true, "Wire Stripper Tool", 9.900m, "ELC-WST-025", 75, "Piece", null, 0.200m },
                    { 26, "Fluke", 6, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Auto-ranging digital multimeter for voltage, current and resistance.", null, "/images/products/multimeter.jpg", true, "Digital Multimeter", 22.500m, "ELC-MTM-026", 45, "Piece", null, 0.350m },
                    { 27, "Rigid", 7, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Cast iron pipe wrench with serrated jaws.", null, "/images/products/pipe-wrench.jpg", true, "Pipe Wrench 14-inch", 16.800m, "PLB-WRN-027", 55, "Piece", null, 1.100m },
                    { 28, "Generic", 7, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "12m roll of PTFE tape for sealing pipe threads.", null, "/images/products/ptfe-tape.jpg", true, "PTFE Thread Seal Tape", 1.200m, "PLB-TPE-028", 300, "Piece", null, 0.020m },
                    { 29, "Generic", 8, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Zinc-plated wood screws, assorted sizes, 200-piece box.", null, "/images/products/wood-screws.jpg", true, "Assorted Wood Screws 200pc", 7.500m, "HRD-SCR-029", 160, "Box", null, 1.000m },
                    { 30, "Generic", 8, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Stainless steel butt hinges, pair, with mounting screws.", null, "/images/products/door-hinges.jpg", true, "Door Hinges 4-inch (Pair)", 5.900m, "HRD-HNG-030", 100, "Set", null, 0.400m }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_CartId_ProductId",
                table: "CartItems",
                columns: new[] { "CartId", "ProductId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_ProductId",
                table: "CartItems",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Carts_UserId",
                table: "Carts",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Categories_Name",
                table: "Categories",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_OrderId",
                table: "OrderItems",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_ProductId",
                table: "OrderItems",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_OrderDate",
                table: "Orders",
                column: "OrderDate");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_OrderStatus",
                table: "Orders",
                column: "OrderStatus");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_UserId",
                table: "Orders",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_Brand",
                table: "Products",
                column: "Brand");

            migrationBuilder.CreateIndex(
                name: "IX_Products_CategoryId",
                table: "Products",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_IsAvailable",
                table: "Products",
                column: "IsAvailable");

            migrationBuilder.CreateIndex(
                name: "IX_Products_Name",
                table: "Products",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Products_Price",
                table: "Products",
                column: "Price");

            migrationBuilder.CreateIndex(
                name: "IX_Products_SKU",
                table: "Products",
                column: "SKU",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "CartItems");

            migrationBuilder.DropTable(
                name: "OrderItems");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "Carts");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Categories");
        }
    }
}
