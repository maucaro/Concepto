CREATE PROCEDURE [dbo].[GetAllPermissionRoles]
AS
	SELECT Roles.TenantId as 'Tenant', Roles.Name As 'Role', Permissions.Name As 'Permission'
	FROM Roles
	JOIN RolePermissions ON Roles.Id = RolePermissions.RoleId
	JOIN Permissions ON Permissions.Id = RolePermissions.PermissionId