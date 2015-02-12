USE [nPM]
GO

/****** Object:  Table [dbo].[tblTask]    Script Date: 5/19/2014 2:18:30 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[tblTask](
	[Id] [uniqueidentifier] NOT NULL,
	[Name] [varchar](50) NOT NULL,
	[Description] [varchar](max) NOT NULL,
	[TaskNumber] [varchar](10) NULL,
	[StartTime] [datetime] NOT NULL,
	[EndTime] [datetime] NOT NULL,
	[TaskStateId] [uniqueidentifier] NOT NULL,
	[TaskStateName] [varchar](50) NOT NULL,
	[ProjectId] [uniqueidentifier] NOT NULL,
	[ProjectName] [varchar](50) NOT NULL,
	[AssigneeId] [uniqueidentifier] NULL,
	[AssigneeName] [varchar](50) NULL,
	[ReporterId] [uniqueidentifier] NULL,
	[ReporterName] [varchar](50) NULL,
	[IsLinked] [bit] NULL,
	[StateId] [uniqueidentifier] NOT NULL,
	[StateName] [varchar](20) NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[CreatedById] [uniqueidentifier] NOT NULL,
	[CreatedByName] [varchar](50) NOT NULL,
	[ModifiedOn] [datetime] NOT NULL,
	[ModifiedById] [uniqueidentifier] NOT NULL,
	[ModifiedByName] [varchar](50) NOT NULL,
 CONSTRAINT [PK_tblTask] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[tblTask]  WITH CHECK ADD  CONSTRAINT [FK_tblTask_tblProject] FOREIGN KEY([ProjectId])
REFERENCES [dbo].[tblProject] ([Id])
GO

ALTER TABLE [dbo].[tblTask] CHECK CONSTRAINT [FK_tblTask_tblProject]
GO

ALTER TABLE [dbo].[tblTask]  WITH CHECK ADD  CONSTRAINT [FK_tblTask_tblTaskState] FOREIGN KEY([TaskStateId])
REFERENCES [dbo].[tblTaskState] ([Id])
GO

ALTER TABLE [dbo].[tblTask] CHECK CONSTRAINT [FK_tblTask_tblTaskState]
GO

ALTER TABLE [dbo].[tblTask]  WITH CHECK ADD  CONSTRAINT [FK_tblTask_tblUser] FOREIGN KEY([AssigneeId])
REFERENCES [dbo].[tblUser] ([Id])
GO

ALTER TABLE [dbo].[tblTask] CHECK CONSTRAINT [FK_tblTask_tblUser]
GO

ALTER TABLE [dbo].[tblTask]  WITH CHECK ADD  CONSTRAINT [FK_tblTask_tblUser1] FOREIGN KEY([ReporterId])
REFERENCES [dbo].[tblUser] ([Id])
GO

ALTER TABLE [dbo].[tblTask] CHECK CONSTRAINT [FK_tblTask_tblUser1]
GO

