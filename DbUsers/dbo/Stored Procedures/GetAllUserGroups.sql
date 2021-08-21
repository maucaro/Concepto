CREATE PROCEDURE [dbo].GetAllUserGroups
AS
	SELECT Users.Email AS 'Email', Groups.Name As 'Group'
	FROM Users
	JOIN UserGroups ON Users.Id = UserGroups.UserId
	JOIN Groups ON UserGroups.GroupId = Groups.Id