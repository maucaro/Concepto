CREATE PROCEDURE [dbo].GetAllPermissionGroups
AS
	SELECT Groups.Name As 'Group', Permissions.Name As 'Permission'
	FROM Groups
	JOIN GroupPermissions ON Groups.Id = GroupPermissions.GroupId
	JOIN Permissions ON Permissions.Id = GroupPermissions.PermissionId