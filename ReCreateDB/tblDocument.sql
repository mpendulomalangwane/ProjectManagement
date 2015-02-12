USE [nPM]
GO

/****** Object:  Table [dbo].[tblDocument]    Script Date: 5/19/2014 2:15:07 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[tblDocument](
	[Id] [uniqueidentifier] NOT NULL,
	[Name] [varchar](50) NOT NULL,
	[Description] [varchar](max) NULL,
	[Size] [int] NULL,
	[Type] [varchar](50) NULL,
	[Content] [image] NULL,
	[Longitude] [varchar](100) NULL,
	[Latitude] [varchar](50) NULL,
	[Location] [varchar](200) NULL,
	[TimeStamp] [datetime] NULL,
	[UploadedById] [uniqueidentifier] NOT NULL,
	[UploadedByName] [varchar](50) NOT NULL,
	[TaskId] [uniqueidentifier] NOT NULL,
	[TaskName] [varchar](50) NOT NULL,
	[StateId] [uniqueidentifier] NULL,
	[StateName] [varchar](20) NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[CreatedById] [uniqueidentifier] NOT NULL,
	[CreatedByName] [varchar](50) NOT NULL,
	[ModifiedOn] [datetime] NOT NULL,
	[ModifiedById] [uniqueidentifier] NOT NULL,
	[ModifiedByName] [varchar](50) NOT NULL,
 CONSTRAINT [PK_tblDocument] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[tblDocument]  WITH CHECK ADD  CONSTRAINT [FK_tblDocument_tblTask] FOREIGN KEY([TaskId])
REFERENCES [dbo].[tblTask] ([Id])
GO

ALTER TABLE [dbo].[tblDocument] CHECK CONSTRAINT [FK_tblDocument_tblTask]
GO

