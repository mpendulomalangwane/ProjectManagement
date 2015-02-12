USE [nPM]
GO

ALTER TABLE [dbo].[tblButtonRole] DROP CONSTRAINT [FK_tblButtonRole_tblRole]
GO

ALTER TABLE [dbo].[tblButtonRole] DROP CONSTRAINT [FK_tblButtonRole_tblButton]
GO

/****** Object:  Table [dbo].[tblButtonRole]    Script Date: 5/19/2014 2:09:10 PM ******/
DROP TABLE [dbo].[tblButtonRole]
GO

/****** Object:  Table [dbo].[tblButtonRole]    Script Date: 5/19/2014 2:09:10 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[tblButtonRole](
	[Id] [uniqueidentifier] NOT NULL,
	[ButtonId] [uniqueidentifier] NOT NULL,
	[ButtonName] [varchar](50) NOT NULL,
	[RoleId] [uniqueidentifier] NOT NULL,
	[RoleName] [varchar](50) NOT NULL,
 CONSTRAINT [PK_tblButtonRole] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[tblButtonRole]  WITH CHECK ADD  CONSTRAINT [FK_tblButtonRole_tblButton] FOREIGN KEY([ButtonId])
REFERENCES [dbo].[tblButton] ([Id])
GO

ALTER TABLE [dbo].[tblButtonRole] CHECK CONSTRAINT [FK_tblButtonRole_tblButton]
GO

ALTER TABLE [dbo].[tblButtonRole]  WITH CHECK ADD  CONSTRAINT [FK_tblButtonRole_tblRole] FOREIGN KEY([RoleId])
REFERENCES [dbo].[tblRole] ([Id])
GO

ALTER TABLE [dbo].[tblButtonRole] CHECK CONSTRAINT [FK_tblButtonRole_tblRole]
GO

