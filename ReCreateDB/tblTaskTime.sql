USE [nPM]
GO

/****** Object:  Table [dbo].[tblTaskTime]    Script Date: 5/19/2014 2:19:09 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[tblTaskTime](
	[Id] [uniqueidentifier] NOT NULL,
	[Minutes] [int] NULL,
	[Hours] [int] NULL,
	[Duration] [float] NULL,
	[TaskId] [uniqueidentifier] NOT NULL,
	[TaskName] [varchar](50) NOT NULL,
	[StateId] [uniqueidentifier] NOT NULL,
	[StateName] [varchar](20) NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[CreatedById] [uniqueidentifier] NOT NULL,
	[CreatedByName] [varchar](50) NOT NULL,
	[ModifiedOn] [datetime] NOT NULL,
	[ModifiedById] [uniqueidentifier] NOT NULL,
	[ModifiedByName] [varchar](50) NOT NULL,
 CONSTRAINT [PK_tblTaskTime] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

