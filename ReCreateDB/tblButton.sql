USE [nPM]
GO

/****** Object:  Table [dbo].[tblButton]    Script Date: 5/19/2014 2:08:58 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[tblButton](
	[Id] [uniqueidentifier] NOT NULL,
	[Name] [varchar](50) NOT NULL,
	[Description] [varchar](50) NOT NULL,
	[EntityId] [uniqueidentifier] NOT NULL,
	[EntityName] [varchar](50) NOT NULL,
 CONSTRAINT [PK_tblButton] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[tblButton]  WITH CHECK ADD  CONSTRAINT [FK_tblButton_tblEntity] FOREIGN KEY([EntityId])
REFERENCES [dbo].[tblEntity] ([Id])
GO

ALTER TABLE [dbo].[tblButton] CHECK CONSTRAINT [FK_tblButton_tblEntity]
GO

