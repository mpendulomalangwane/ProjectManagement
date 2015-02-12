USE [nPM]
GO

/****** Object:  Table [dbo].[tblTeamUser]    Script Date: 5/19/2014 2:19:27 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[tblTeamUser](
	[Id] [uniqueidentifier] NOT NULL,
	[UserId] [uniqueidentifier] NOT NULL,
	[TeamId] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_tblTeamUser] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[tblTeamUser]  WITH CHECK ADD  CONSTRAINT [FK_tblTeamUser_tblTeam] FOREIGN KEY([TeamId])
REFERENCES [dbo].[tblTeam] ([Id])
GO

ALTER TABLE [dbo].[tblTeamUser] CHECK CONSTRAINT [FK_tblTeamUser_tblTeam]
GO

ALTER TABLE [dbo].[tblTeamUser]  WITH CHECK ADD  CONSTRAINT [FK_tblTeamUser_tblUser] FOREIGN KEY([UserId])
REFERENCES [dbo].[tblUser] ([Id])
GO

ALTER TABLE [dbo].[tblTeamUser] CHECK CONSTRAINT [FK_tblTeamUser_tblUser]
GO

