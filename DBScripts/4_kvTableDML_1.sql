

--[RootObjectId] uniqueidentifier NOT NULL,
--	[ObjectId] uniqueidentifier NOT NULL DEFAULT NEWID(),
--	[Key] nvarchar(500) NOT NULL,
--	[Value]

TRUNCATE TABLE [KvPairTable]

INSERT INTO [dbo].[KvPairTable]
(
	[RootObjectId],
	[Key],
	[Value],
	[Schema]
)
VALUES
(
	'DE3AA882-CC88-456A-A2CB-155D477D00A2',
	'Name',
	'JJ',
	'Schem1'
)
INSERT INTO [dbo].[KvPairTable]
(
	[RootObjectId],
	[Key],
	[Value],
	[Schema]
)
VALUES
(
	'DE3AA882-CC88-456A-A2CB-155D477D00A2',
	'Cat[0].Name',
	'whiskers',
	'Schem1'
)
INSERT INTO [dbo].[KvPairTable]
(
	[RootObjectId],
	[Key],
	[Value],
	[Schema]
)
VALUES
(
	'DE3AA882-CC88-456A-A2CB-155D477D00A2',
	'Cat[1].Name',
	'raphael',
	'Schem1'
)
INSERT INTO [dbo].[KvPairTable]
(
	[RootObjectId],
	[Key],
	[Value],
	[Schema]
)
VALUES
(
	'DE3AA882-CC88-456A-A2CB-155D477D00A2',
	'Hobby.Category.Name',
	'Outdoors',
	'Schem1'
)