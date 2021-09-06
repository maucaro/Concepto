USE [DbUsers]
GO
INSERT [dbo].[Permissions] ([Id], [Name]) VALUES (N'013be2f2-9897-453c-b554-9c0d8d3f0115', N'AdministerUsers')
GO
INSERT [dbo].[Roles] ([Id], [Name]) VALUES (N'ed6a2371-b9b8-4288-8e54-1bbd62c56ba2', N'Operator')
GO
INSERT [dbo].[Roles] ([Id], [Name]) VALUES (N'c68d551f-dbaf-4dd4-8f87-2d022e0b5233', N'Admin')
GO
INSERT [dbo].[RolePermissions] ([RoleId], [PermissionId]) VALUES (N'c68d551f-dbaf-4dd4-8f87-2d022e0b5233', N'013be2f2-9897-453c-b554-9c0d8d3f0115')
GO
