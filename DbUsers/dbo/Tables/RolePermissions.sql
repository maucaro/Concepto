CREATE TABLE [dbo].[RolePermissions]
(
	[RoleId] UNIQUEIDENTIFIER NOT NULL , 
    [PermissionId] UNIQUEIDENTIFIER NOT NULL, 
    PRIMARY KEY ([RoleId], [PermissionId]), 
    CONSTRAINT [FK_RolePermissions_ToRoles] FOREIGN KEY ([RoleId]) REFERENCES [Roles]([Id]), 
    CONSTRAINT [FK_RolePermissions_ToPermissions] FOREIGN KEY ([PermissionId]) REFERENCES [Permissions]([Id])
)
