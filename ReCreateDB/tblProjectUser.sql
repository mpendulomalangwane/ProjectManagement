USE [nPM]
GO

/****** Object:  Table [dbo].[tblProjectUser]    Script Date: 5/19/2014 2:17:23 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[tblProjectUser](
	[Id] [uniqueidentifier] NOT NULL,
	[UserId] [uniqueidentifier] NOT NULL,
	[ProjectId] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_tblProjectUser] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[tblProjectUser]  WITH CHECK ADD  CONSTRAINT [FK_tblProjectUser_tblProject] FOREIGN KEY([ProjectId])
REFERENCES [dbo].[tblProject] ([Id])
GO

ALTER TABLE [dbo].[tblProjectUser] CHECK CONSTRAINT [FK_tblProjectUser_tblProject]
GO

ALTER TABLE [dbo].[tblProjectUser]  WITH CHECK ADD  CONSTRAINT [FK_tblProjectUser_tblUser] FOREIGN KEY([UserId])
REFERENCES [dbo].[tblUser] ([Id])
GO

ALTER TABLE [dbo].[tblProjectUser] CHECK CONSTRAINT [FK_tblProjectUser_tblUser]
GO

