USE [nPM]
GO

/****** Object:  Table [dbo].[tblRoleUser]    Script Date: 5/19/2014 2:18:12 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[tblRoleUser](
	[Id] [uniqueidentifier] NOT NULL,
	[UserId] [uniqueidentifier] NOT NULL,
	[RoleId] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_tblRoleUser] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[tblRoleUser]  WITH CHECK ADD  CONSTRAINT [FK_tblRoleUser_tblRole] FOREIGN KEY([RoleId])
REFERENCES [dbo].[tblRole] ([Id])
GO

ALTER TABLE [dbo].[tblRoleUser] CHECK CONSTRAINT [FK_tblRoleUser_tblRole]
GO

ALTER TABLE [dbo].[tblRoleUser]  WITH CHECK ADD  CONSTRAINT [FK_tblRoleUser_tblUser1] FOREIGN KEY([UserId])
REFERENCES [dbo].[tblUser] ([Id])
GO

ALTER TABLE [dbo].[tblRoleUser] CHECK CONSTRAINT [FK_tblRoleUser_tblUser1]
GO

