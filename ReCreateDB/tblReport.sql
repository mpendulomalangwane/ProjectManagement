USE [nPM]
GO

/****** Object:  Table [dbo].[tblReport]    Script Date: 5/19/2014 2:17:43 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[tblReport](
	[Id] [uniqueidentifier] NOT NULL,
	[Name] [varchar](50) NULL,
	[SchemaName] [varchar](50) NULL,
	[CreatedOn] [datetime] NULL,
	[CreatedById] [uniqueidentifier] NULL,
	[CreatedByName] [varchar](50) NULL,
	[ModifiedOn] [datetime] NULL,
	[ModifiedById] [uniqueidentifier] NULL,
	[ModifiedByName] [varchar](50) NULL
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

