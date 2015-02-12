USE [nPM]
GO

/****** Object:  Table [dbo].[tblRibbon]    Script Date: 5/19/2014 2:17:54 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[tblRibbon](
	[Id] [uniqueidentifier] NOT NULL,
	[ButtonId] [uniqueidentifier] NOT NULL,
	[ButtonName] [varchar](50) NOT NULL,
	[Page] [varchar](50) NOT NULL,
 CONSTRAINT [PK_tblRibbon] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[tblRibbon]  WITH CHECK ADD  CONSTRAINT [FK_tblRibbon_tblButton] FOREIGN KEY([ButtonId])
REFERENCES [dbo].[tblButton] ([Id])
GO

ALTER TABLE [dbo].[tblRibbon] CHECK CONSTRAINT [FK_tblRibbon_tblButton]
GO

