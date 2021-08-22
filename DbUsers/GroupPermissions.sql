CREATE TABLE [dbo].[GroupPermissions]
(
	[GroupId] UNIQUEIDENTIFIER NOT NULL , 
    [PermissionId] UNIQUEIDENTIFIER NOT NULL, 
    PRIMARY KEY ([GroupId], [PermissionId])
)
