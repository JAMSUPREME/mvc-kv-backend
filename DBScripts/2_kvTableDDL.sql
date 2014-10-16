

USE [Stuff]
GO

/****** Object:  Table [dbo].[KvPairTable]    Script Date: 10/15/2014 10:01:05 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

begin tran--commit--rollback

--DROP TABLE [KvPairTable]

CREATE TABLE [dbo].[KvPairTable](
	[RootObjectId] uniqueidentifier NOT NULL,
	[ObjectId] uniqueidentifier NOT NULL DEFAULT NEWID(),
	[Key] nvarchar(500) NOT NULL,
	[Value] nvarchar(max) NULL,
	[Schema] nvarchar(100) NOT NULL,	--helps constraint fields and necessary to properly pivot
 CONSTRAINT [PK_KvPairTable] PRIMARY KEY NONCLUSTERED 
(
	[ObjectId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

CREATE CLUSTERED INDEX IDX_KvPairTable ON [dbo].[KvPairTable] (RootObjectId)


