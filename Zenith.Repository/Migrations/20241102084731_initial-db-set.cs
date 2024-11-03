using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zenith.Repository.Migrations
{
    /// <inheritdoc />
    public partial class initialdbset : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Discriminator = table.Column<string>(type: "nvarchar(21)", maxLength: 21, nullable: false),
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
                    IsApproved = table.Column<bool>(type: "bit", nullable: false),
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
                name: "Attachments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Attachments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Brands",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BrandCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ShortName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LogoFilePath = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Brands", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DropdownLists",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DropdownLists", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DropdownValues",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DropdownParentNameId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DropdownValues", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SecurityGroups",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SecurityGroups", x => x.Id);
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
                        onDelete: ReferentialAction.Restrict);
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
                        onDelete: ReferentialAction.Restrict);
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
                        onDelete: ReferentialAction.Restrict);
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
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
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
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AccountDetails",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BeneficiaryNameId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BankNameId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BranchNameId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BankCountryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AccountNumber = table.Column<int>(type: "int", nullable: false),
                    AccountCurrencyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IBANId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SwiftCodeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IfscCodeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BankAddressId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BankLetter = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccountDetails_DropdownValues_AccountCurrencyId",
                        column: x => x.AccountCurrencyId,
                        principalTable: "DropdownValues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AccountDetails_DropdownValues_BankAddressId",
                        column: x => x.BankAddressId,
                        principalTable: "DropdownValues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AccountDetails_DropdownValues_BankCountryId",
                        column: x => x.BankCountryId,
                        principalTable: "DropdownValues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AccountDetails_DropdownValues_BankNameId",
                        column: x => x.BankNameId,
                        principalTable: "DropdownValues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AccountDetails_DropdownValues_BeneficiaryNameId",
                        column: x => x.BeneficiaryNameId,
                        principalTable: "DropdownValues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AccountDetails_DropdownValues_BranchNameId",
                        column: x => x.BranchNameId,
                        principalTable: "DropdownValues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AccountDetails_DropdownValues_IBANId",
                        column: x => x.IBANId,
                        principalTable: "DropdownValues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AccountDetails_DropdownValues_IfscCodeId",
                        column: x => x.IfscCodeId,
                        principalTable: "DropdownValues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AccountDetails_DropdownValues_SwiftCodeId",
                        column: x => x.SwiftCodeId,
                        principalTable: "DropdownValues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Address",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FullAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OfficeNo = table.Column<int>(type: "int", nullable: false),
                    BuildingName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Street = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LocalTownId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DistrictId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StateId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CountryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<int>(type: "int", nullable: false),
                    CodeTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GeorgraphicLocationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Address", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Address_DropdownValues_CodeTypeId",
                        column: x => x.CodeTypeId,
                        principalTable: "DropdownValues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Address_DropdownValues_CountryId",
                        column: x => x.CountryId,
                        principalTable: "DropdownValues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Address_DropdownValues_DistrictId",
                        column: x => x.DistrictId,
                        principalTable: "DropdownValues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Address_DropdownValues_GeorgraphicLocationId",
                        column: x => x.GeorgraphicLocationId,
                        principalTable: "DropdownValues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Address_DropdownValues_LocalTownId",
                        column: x => x.LocalTownId,
                        principalTable: "DropdownValues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Address_DropdownValues_StateId",
                        column: x => x.StateId,
                        principalTable: "DropdownValues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Contacts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GenderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Phone = table.Column<int>(type: "int", nullable: false),
                    Mobile = table.Column<int>(type: "int", nullable: false),
                    Whatsapp = table.Column<int>(type: "int", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PositionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ContactProfileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PrimaryContactId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contacts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Contacts_DropdownValues_ContactProfileId",
                        column: x => x.ContactProfileId,
                        principalTable: "DropdownValues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Contacts_DropdownValues_GenderId",
                        column: x => x.GenderId,
                        principalTable: "DropdownValues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Contacts_DropdownValues_PositionId",
                        column: x => x.PositionId,
                        principalTable: "DropdownValues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Contacts_DropdownValues_PrimaryContactId",
                        column: x => x.PrimaryContactId,
                        principalTable: "DropdownValues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Manufacturer",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ManufacturerCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ShortName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Website = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RegisteredSinceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HeadQuarterId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    RevisionNumer = table.Column<int>(type: "int", nullable: false),
                    ApprovalStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RejectionReason = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AddressId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Manufacturer", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Manufacturer_DropdownValues_HeadQuarterId",
                        column: x => x.HeadQuarterId,
                        principalTable: "DropdownValues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Manufacturer_DropdownValues_RegisteredSinceId",
                        column: x => x.RegisteredSinceId,
                        principalTable: "DropdownValues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OtherDocuments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DocumentName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DocumentTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Document = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ExpiryDateId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OtherDocuments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OtherDocuments_DropdownValues_DocumentTypeId",
                        column: x => x.DocumentTypeId,
                        principalTable: "DropdownValues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OtherDocuments_DropdownValues_ExpiryDateId",
                        column: x => x.ExpiryDateId,
                        principalTable: "DropdownValues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PaymentTerms",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreditTermsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreditLimit = table.Column<int>(type: "int", nullable: false),
                    CreditLimitCurrencyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DocumentPathURL = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentTerms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaymentTerms_DropdownValues_CreditLimitCurrencyId",
                        column: x => x.CreditLimitCurrencyId,
                        principalTable: "DropdownValues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PaymentTerms_DropdownValues_CreditTermsId",
                        column: x => x.CreditTermsId,
                        principalTable: "DropdownValues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductNameId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductFamilyCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductFamilyNameId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductNameCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BrandId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Products_Brands_BrandId",
                        column: x => x.BrandId,
                        principalTable: "Brands",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Products_DropdownValues_ProductFamilyNameId",
                        column: x => x.ProductFamilyNameId,
                        principalTable: "DropdownValues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Products_DropdownValues_ProductNameId",
                        column: x => x.ProductNameId,
                        principalTable: "DropdownValues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "VendorsInitializationForm",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RequestedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PriorityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RequiredBy = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SupplierName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SupplierTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Scope = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContactName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContactPhone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContactEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContactCountryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BusinessCard = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Website = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RequestNum = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StatusId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsCritical = table.Column<bool>(type: "bit", nullable: false),
                    IsApproved = table.Column<bool>(type: "bit", nullable: false),
                    RejectionReasonId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Comments = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VendorsInitializationForm", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VendorsInitializationForm_DropdownValues_ContactCountryId",
                        column: x => x.ContactCountryId,
                        principalTable: "DropdownValues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VendorsInitializationForm_DropdownValues_PriorityId",
                        column: x => x.PriorityId,
                        principalTable: "DropdownValues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VendorsInitializationForm_DropdownValues_RejectionReasonId",
                        column: x => x.RejectionReasonId,
                        principalTable: "DropdownValues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VendorsInitializationForm_DropdownValues_StatusId",
                        column: x => x.StatusId,
                        principalTable: "DropdownValues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VendorsInitializationForm_DropdownValues_SupplierTypeId",
                        column: x => x.SupplierTypeId,
                        principalTable: "DropdownValues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SecurityGroupsRoles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SecurityGroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SecurityGroupsRoles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SecurityGroupsRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SecurityGroupsRoles_SecurityGroups_SecurityGroupId",
                        column: x => x.SecurityGroupId,
                        principalTable: "SecurityGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "VendorQualificationWorkFlow",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SecurityGroupId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StepOrder = table.Column<int>(type: "int", nullable: false),
                    StepName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsCriticalOnly = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VendorQualificationWorkFlow", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VendorQualificationWorkFlow_SecurityGroups_SecurityGroupId",
                        column: x => x.SecurityGroupId,
                        principalTable: "SecurityGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "QualityCertification",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LicenseById = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LicenseStandardId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LicenseNumber = table.Column<int>(type: "int", nullable: false),
                    RegistrationValidityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ValidityStart = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ValidityEnd = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Document = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LicenseScope = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    VendorsInitializationFormId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QualityCertification", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QualityCertification_DropdownValues_LicenseById",
                        column: x => x.LicenseById,
                        principalTable: "DropdownValues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QualityCertification_DropdownValues_LicenseStandardId",
                        column: x => x.LicenseStandardId,
                        principalTable: "DropdownValues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QualityCertification_DropdownValues_RegistrationValidityId",
                        column: x => x.RegistrationValidityId,
                        principalTable: "DropdownValues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_QualityCertification_VendorsInitializationForm_VendorsInitializationFormId",
                        column: x => x.VendorsInitializationFormId,
                        principalTable: "VendorsInitializationForm",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Registrations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LicenseTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LicenseNumber = table.Column<int>(type: "int", nullable: false),
                    RegisteredSince = table.Column<int>(type: "int", nullable: false),
                    RegistrationCategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RegisteredCountryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RegistrationValidityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ValidityStart = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ValidityEnd = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Document = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ActivityCode = table.Column<int>(type: "int", nullable: false),
                    ActivityNameId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VendorsInitializationFormId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Registrations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Registrations_DropdownValues_ActivityNameId",
                        column: x => x.ActivityNameId,
                        principalTable: "DropdownValues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Registrations_DropdownValues_LicenseTypeId",
                        column: x => x.LicenseTypeId,
                        principalTable: "DropdownValues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Registrations_DropdownValues_RegisteredCountryId",
                        column: x => x.RegisteredCountryId,
                        principalTable: "DropdownValues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Registrations_DropdownValues_RegistrationCategoryId",
                        column: x => x.RegistrationCategoryId,
                        principalTable: "DropdownValues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Registrations_DropdownValues_RegistrationValidityId",
                        column: x => x.RegistrationValidityId,
                        principalTable: "DropdownValues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Registrations_VendorsInitializationForm_VendorsInitializationFormId",
                        column: x => x.VendorsInitializationFormId,
                        principalTable: "VendorsInitializationForm",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "VendorQualificationWorkFlowExecution",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VendorQualificationWorkFlowId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssignedUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    VendorsInitializationFormId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StatusId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VendorQualificationWorkFlowExecution", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VendorQualificationWorkFlowExecution_AspNetUsers_AssignedUserId",
                        column: x => x.AssignedUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VendorQualificationWorkFlowExecution_DropdownValues_StatusId",
                        column: x => x.StatusId,
                        principalTable: "DropdownValues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VendorQualificationWorkFlowExecution_VendorQualificationWorkFlow_VendorQualificationWorkFlowId",
                        column: x => x.VendorQualificationWorkFlowId,
                        principalTable: "VendorQualificationWorkFlow",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccountDetails_AccountCurrencyId",
                table: "AccountDetails",
                column: "AccountCurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountDetails_BankAddressId",
                table: "AccountDetails",
                column: "BankAddressId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountDetails_BankCountryId",
                table: "AccountDetails",
                column: "BankCountryId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountDetails_BankNameId",
                table: "AccountDetails",
                column: "BankNameId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountDetails_BeneficiaryNameId",
                table: "AccountDetails",
                column: "BeneficiaryNameId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountDetails_BranchNameId",
                table: "AccountDetails",
                column: "BranchNameId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountDetails_IBANId",
                table: "AccountDetails",
                column: "IBANId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountDetails_IfscCodeId",
                table: "AccountDetails",
                column: "IfscCodeId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountDetails_SwiftCodeId",
                table: "AccountDetails",
                column: "SwiftCodeId");

            migrationBuilder.CreateIndex(
                name: "IX_Address_CodeTypeId",
                table: "Address",
                column: "CodeTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Address_CountryId",
                table: "Address",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_Address_DistrictId",
                table: "Address",
                column: "DistrictId");

            migrationBuilder.CreateIndex(
                name: "IX_Address_GeorgraphicLocationId",
                table: "Address",
                column: "GeorgraphicLocationId");

            migrationBuilder.CreateIndex(
                name: "IX_Address_LocalTownId",
                table: "Address",
                column: "LocalTownId");

            migrationBuilder.CreateIndex(
                name: "IX_Address_StateId",
                table: "Address",
                column: "StateId");

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
                name: "IX_Contacts_ContactProfileId",
                table: "Contacts",
                column: "ContactProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_Contacts_GenderId",
                table: "Contacts",
                column: "GenderId");

            migrationBuilder.CreateIndex(
                name: "IX_Contacts_PositionId",
                table: "Contacts",
                column: "PositionId");

            migrationBuilder.CreateIndex(
                name: "IX_Contacts_PrimaryContactId",
                table: "Contacts",
                column: "PrimaryContactId");

            migrationBuilder.CreateIndex(
                name: "IX_Manufacturer_HeadQuarterId",
                table: "Manufacturer",
                column: "HeadQuarterId");

            migrationBuilder.CreateIndex(
                name: "IX_Manufacturer_RegisteredSinceId",
                table: "Manufacturer",
                column: "RegisteredSinceId");

            migrationBuilder.CreateIndex(
                name: "IX_OtherDocuments_DocumentTypeId",
                table: "OtherDocuments",
                column: "DocumentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_OtherDocuments_ExpiryDateId",
                table: "OtherDocuments",
                column: "ExpiryDateId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentTerms_CreditLimitCurrencyId",
                table: "PaymentTerms",
                column: "CreditLimitCurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentTerms_CreditTermsId",
                table: "PaymentTerms",
                column: "CreditTermsId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_BrandId",
                table: "Products",
                column: "BrandId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_ProductFamilyNameId",
                table: "Products",
                column: "ProductFamilyNameId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_ProductNameId",
                table: "Products",
                column: "ProductNameId");

            migrationBuilder.CreateIndex(
                name: "IX_QualityCertification_LicenseById",
                table: "QualityCertification",
                column: "LicenseById");

            migrationBuilder.CreateIndex(
                name: "IX_QualityCertification_LicenseStandardId",
                table: "QualityCertification",
                column: "LicenseStandardId");

            migrationBuilder.CreateIndex(
                name: "IX_QualityCertification_RegistrationValidityId",
                table: "QualityCertification",
                column: "RegistrationValidityId");

            migrationBuilder.CreateIndex(
                name: "IX_QualityCertification_VendorsInitializationFormId",
                table: "QualityCertification",
                column: "VendorsInitializationFormId");

            migrationBuilder.CreateIndex(
                name: "IX_Registrations_ActivityNameId",
                table: "Registrations",
                column: "ActivityNameId");

            migrationBuilder.CreateIndex(
                name: "IX_Registrations_LicenseTypeId",
                table: "Registrations",
                column: "LicenseTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Registrations_RegisteredCountryId",
                table: "Registrations",
                column: "RegisteredCountryId");

            migrationBuilder.CreateIndex(
                name: "IX_Registrations_RegistrationCategoryId",
                table: "Registrations",
                column: "RegistrationCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Registrations_RegistrationValidityId",
                table: "Registrations",
                column: "RegistrationValidityId");

            migrationBuilder.CreateIndex(
                name: "IX_Registrations_VendorsInitializationFormId",
                table: "Registrations",
                column: "VendorsInitializationFormId");

            migrationBuilder.CreateIndex(
                name: "IX_SecurityGroupsRoles_RoleId",
                table: "SecurityGroupsRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_SecurityGroupsRoles_SecurityGroupId",
                table: "SecurityGroupsRoles",
                column: "SecurityGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_VendorQualificationWorkFlow_SecurityGroupId",
                table: "VendorQualificationWorkFlow",
                column: "SecurityGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_VendorQualificationWorkFlowExecution_AssignedUserId",
                table: "VendorQualificationWorkFlowExecution",
                column: "AssignedUserId");

            migrationBuilder.CreateIndex(
                name: "IX_VendorQualificationWorkFlowExecution_StatusId",
                table: "VendorQualificationWorkFlowExecution",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_VendorQualificationWorkFlowExecution_VendorQualificationWorkFlowId",
                table: "VendorQualificationWorkFlowExecution",
                column: "VendorQualificationWorkFlowId");

            migrationBuilder.CreateIndex(
                name: "IX_VendorsInitializationForm_ContactCountryId",
                table: "VendorsInitializationForm",
                column: "ContactCountryId");

            migrationBuilder.CreateIndex(
                name: "IX_VendorsInitializationForm_PriorityId",
                table: "VendorsInitializationForm",
                column: "PriorityId");

            migrationBuilder.CreateIndex(
                name: "IX_VendorsInitializationForm_RejectionReasonId",
                table: "VendorsInitializationForm",
                column: "RejectionReasonId");

            migrationBuilder.CreateIndex(
                name: "IX_VendorsInitializationForm_StatusId",
                table: "VendorsInitializationForm",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_VendorsInitializationForm_SupplierTypeId",
                table: "VendorsInitializationForm",
                column: "SupplierTypeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccountDetails");

            migrationBuilder.DropTable(
                name: "Address");

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
                name: "Attachments");

            migrationBuilder.DropTable(
                name: "Contacts");

            migrationBuilder.DropTable(
                name: "DropdownLists");

            migrationBuilder.DropTable(
                name: "Manufacturer");

            migrationBuilder.DropTable(
                name: "OtherDocuments");

            migrationBuilder.DropTable(
                name: "PaymentTerms");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "QualityCertification");

            migrationBuilder.DropTable(
                name: "Registrations");

            migrationBuilder.DropTable(
                name: "SecurityGroupsRoles");

            migrationBuilder.DropTable(
                name: "VendorQualificationWorkFlowExecution");

            migrationBuilder.DropTable(
                name: "Brands");

            migrationBuilder.DropTable(
                name: "VendorsInitializationForm");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "VendorQualificationWorkFlow");

            migrationBuilder.DropTable(
                name: "DropdownValues");

            migrationBuilder.DropTable(
                name: "SecurityGroups");
        }
    }
}
