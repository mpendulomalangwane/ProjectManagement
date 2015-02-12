USE [nPM]
GO

/****** Object:  Table [dbo].[tblProject]    Script Date: 5/19/2014 2:16:33 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[tblProject](
	[Id] [uniqueidentifier] NOT NULL,
	[Name] [varchar](50) NOT NULL,
	[Description] [varchar](200) NULL,
	[ProjectNumber] [varchar](10) NULL,
	[StartTime] [datetime] NOT NULL,
	[EndTime] [datetime] NOT NULL,
	[Budget] [money] NULL,
	[ManagerId] [uniqueidentifier] NOT NULL,
	[ManagerName] [varchar](50) NOT NULL,
	[ClientId] [uniqueidentifier] NOT NULL,
	[ClientName] [varchar](50) NOT NULL,
	[StateId] [uniqueidentifier] NOT NULL,
	[StateName] [varchar](50) NOT NULL,
	[StateCode] [int] NULL,
	[StateCodeName] [varchar](20) NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[CreatedById] [uniqueidentifier] NOT NULL,
	[CreatedByName] [varchar](50) NOT NULL,
	[ModifiedOn] [datetime] NOT NULL,
	[ModifiedById] [uniqueidentifier] NOT NULL,
	[ModifiedByName] [varchar](50) NOT NULL,
 CONSTRAINT [PK_tblProject] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[tblProject]  WITH CHECK ADD  CONSTRAINT [FK_tblProject_tblClient] FOREIGN KEY([ClientId])
REFERENCES [dbo].[tblClient] ([Id])
GO

ALTER TABLE [dbo].[tblProject] CHECK CONSTRAINT [FK_tblProject_tblClient]
GO

ALTER TABLE [dbo].[tblProject]  WITH CHECK ADD  CONSTRAINT [FK_tblProject_tblProjectState] FOREIGN KEY([StateId])
REFERENCES [dbo].[tblProjectState] ([Id])
GO

ALTER TABLE [dbo].[tblProject] CHECK CONSTRAINT [FK_tblProject_tblProjectState]
GO

