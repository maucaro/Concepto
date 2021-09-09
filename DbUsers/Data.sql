USE [DbUsers]
GO
INSERT [dbo].[Permissions] ([Id], [Name]) VALUES (N'013be2f2-9897-453c-b554-9c0d8d3f0115', N'AdministerUsers')
GO
INSERT [dbo].[Tenants] ([Id], [Name]) VALUES (N'vida-75frq', N'Tenant de Vida18')
GO
INSERT [dbo].[Roles] ([Id], [Name], [TenantId]) VALUES (N'ed6a2371-b9b8-4288-8e54-1bbd62c56ba2', N'Operator', N'vida-75frq')
GO
INSERT [dbo].[Roles] ([Id], [Name], [TenantId]) VALUES (N'c68d551f-dbaf-4dd4-8f87-2d022e0b5233', N'Admin', N'vida-75frq')
GO
INSERT [dbo].[RolePermissions] ([RoleId], [PermissionId]) VALUES (N'c68d551f-dbaf-4dd4-8f87-2d022e0b5233', N'013be2f2-9897-453c-b554-9c0d8d3f0115')
GO
INSERT [dbo].[RolePermissions] ([RoleId], [PermissionId]) VALUES (N'ed6a2371-b9b8-4288-8e54-1bbd62c56ba2', N'013be2f2-9897-453c-b554-9c0d8d3f0115')
GO

