CREATE TABLE [dbo].[UserGroups]
(
	[UserId] UNIQUEIDENTIFIER NOT NULL , 
    [GroupId] UNIQUEIDENTIFIER NOT NULL, 
    CONSTRAINT [FK_UserGroups_ToTable] FOREIGN KEY (UserId) REFERENCES Users(Id), 
    CONSTRAINT [FK_UserGroups_ToTable_1] FOREIGN KEY (GroupId) REFERENCES Groups(Id), 
    PRIMARY KEY ([UserId], [GroupId])
)
