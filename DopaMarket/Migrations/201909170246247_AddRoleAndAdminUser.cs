namespace DopaMarket.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddRoleAndAdminUser : DbMigration
    {
        public override void Up()
        {
            Sql(@"
INSERT INTO [dbo].[AspNetUsers] ([Id], [Email], [EmailConfirmed], [PasswordHash], [SecurityStamp], [PhoneNumber], [PhoneNumberConfirmed], [TwoFactorEnabled], [LockoutEndDateUtc], [LockoutEnabled], [AccessFailedCount], [UserName]) VALUES (N'3592db86-87f5-4504-80c0-4e57bd981925', N'admin@admin.com', 0, N'ACGC7mZ4Q5h/iMB1kVjwbYX/SYezImqa4spBqWUJbMy6d4Dy2SrEpn+ujg/PdSaq8A==', N'd9ec4cd5-f6d9-4195-8f01-7b49900ddf60', NULL, 0, 0, NULL, 1, 0, N'admin@admin.com')

INSERT INTO [dbo].[AspNetRoles] ([Id], [Name]) VALUES (N'a035c5ec-b2cb-44b3-8620-89a82e5be73e', N'Administrator')
INSERT INTO [dbo].[AspNetRoles] ([Id], [Name]) VALUES (N'be81dd21-2beb-4c9e-8115-e110d347ff52', N'CanManageItems')
INSERT INTO [dbo].[AspNetRoles] ([Id], [Name]) VALUES (N'16f8c5c2-e7cf-415e-b436-de34f7d76c1f', N'CanManageSales')

INSERT INTO [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'3592db86-87f5-4504-80c0-4e57bd981925', N'16f8c5c2-e7cf-415e-b436-de34f7d76c1f')
INSERT INTO [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'3592db86-87f5-4504-80c0-4e57bd981925', N'a035c5ec-b2cb-44b3-8620-89a82e5be73e')
INSERT INTO [dbo].[AspNetUserRoles] ([UserId], [RoleId]) VALUES (N'3592db86-87f5-4504-80c0-4e57bd981925', N'be81dd21-2beb-4c9e-8115-e110d347ff52')

SET IDENTITY_INSERT [dbo].[Addresses] ON
INSERT INTO [dbo].[Addresses] ([Id], [Street], [Street2], [City], [State], [PostalCode], [Country]) VALUES (9, N'4507  Don Jackson Lane', NULL, N'Cenon', NULL, N'33150', N'France')
SET IDENTITY_INSERT [dbo].[Addresses] OFF

SET IDENTITY_INSERT [dbo].[Clients] ON
INSERT INTO [dbo].[Clients] ([Id], [IdentityUserId], [Name], [Birthday], [PhoneNumber], [AddressId]) VALUES (6, N'3592db86-87f5-4504-80c0-4e57bd981925', N'Admin', NULL, NULL, 9)
SET IDENTITY_INSERT [dbo].[Clients] OFF
");
        }
        
        public override void Down()
        {
        }
    }
}
