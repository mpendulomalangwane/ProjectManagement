USE [nPM]
GO

/****** Object:  Table [dbo].[tblLinkedTask]    Script Date: 5/19/2014 2:15:54 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[tblLinkedTask](
	[Id] [uniqueidentifier] NOT NULL,
	[Name] [varchar](50) NULL,
	[TaskId1] [uniqueidentifier] NOT NULL,
	[TaskName1] [varchar](50) NOT NULL,
	[TaskId2] [uniqueidentifier] NOT NULL,
	[TaskName2] [varchar](50) NOT NULL
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[tblLinkedTask]  WITH CHECK ADD  CONSTRAINT [FK_tblLinkedTask_tblTask] FOREIGN KEY([TaskId1])
REFERENCES [dbo].[tblTask] ([Id])
GO

ALTER TABLE [dbo].[tblLinkedTask] CHECK CONSTRAINT [FK_tblLinkedTask_tblTask]
GO

ALTER TABLE [dbo].[tblLinkedTask]  WITH CHECK ADD  CONSTRAINT [FK_tblLinkedTask_tblTask1] FOREIGN KEY([TaskId2])
REFERENCES [dbo].[tblTask] ([Id])
GO

ALTER TABLE [dbo].[tblLinkedTask] CHECK CONSTRAINT [FK_tblLinkedTask_tblTask1]
GO

