

USE [Stuff]
GO

/****** Object:  Table [dbo].[KvPairTable]    Script Date: 10/15/2014 10:01:05 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

--begin tran--commit--rollback

--DROP TABLE [RootObject]

CREATE TABLE [dbo].[RootObject](
	[Id] uniqueidentifier NOT NULL,
	[Description] nvarchar(500) NOT NULL,
	[OfferStartDate] datetime NULL,
	[OfferEndDate] datetime NULL,
	[ActiveFlag] bit NULL,
	[IsActive] AS ISNULL(ActiveFlag, CASE WHEN GETDATE() > offerstartdate AND GETDATE() < OfferEndDate THEN CAST(1 as bit) ELSE CAST(0 as bit) end),
 CONSTRAINT [PK_RootObject] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]



--SELECT
--CASE WHEN GETDATE() > offerstartdate OR GETDATE() < OfferEndDate THEN CAST(1 as bit) ELSE CAST(0 as bit) end
--FROM RootObject