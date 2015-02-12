USE [nPM]
GO

/****** Object:  Table [dbo].[tblGroupItem]    Script Date: 5/19/2014 2:15:39 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[tblGroupItem](
	[Id] [uniqueidentifier] NOT NULL,
	[Name] [varchar](50) NOT NULL,
	[Order] [int] NULL,
	[GroupId] [uniqueidentifier] NOT NULL,
	[GroupName] [varchar](50) NOT NULL,
 CONSTRAINT [PK_tblGroupItem] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[tblGroupItem]  WITH CHECK ADD  CONSTRAINT [FK_tblGroupItem_tblGroup] FOREIGN KEY([GroupId])
REFERENCES [dbo].[tblGroup] ([Id])
GO

ALTER TABLE [dbo].[tblGroupItem] CHECK CONSTRAINT [FK_tblGroupItem_tblGroup]
GO

