USE [nPM]
GO

/****** Object:  Table [dbo].[tblAddress]    Script Date: 5/19/2014 2:08:36 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[tblAddress](
	[Id] [uniqueidentifier] NOT NULL,
	[StreetName] [varchar](100) NULL,
	[Suburb] [varchar](100) NULL,
	[CityId] [uniqueidentifier] NULL,
	[CityName] [varchar](50) NULL,
	[ProvinceId] [uniqueidentifier] NULL,
	[ProvinceName] [varchar](50) NULL,
	[CountryId] [uniqueidentifier] NULL,
	[CountryName] [varchar](50) NULL
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[tblAddress]  WITH CHECK ADD  CONSTRAINT [FK_tblAddress_tblCity] FOREIGN KEY([CityId])
REFERENCES [dbo].[tblCity] ([Id])
GO

ALTER TABLE [dbo].[tblAddress] CHECK CONSTRAINT [FK_tblAddress_tblCity]
GO

ALTER TABLE [dbo].[tblAddress]  WITH CHECK ADD  CONSTRAINT [FK_tblAddress_tblCity1] FOREIGN KEY([CityId])
REFERENCES [dbo].[tblCity] ([Id])
GO

ALTER TABLE [dbo].[tblAddress] CHECK CONSTRAINT [FK_tblAddress_tblCity1]
GO

ALTER TABLE [dbo].[tblAddress]  WITH CHECK ADD  CONSTRAINT [FK_tblAddress_tblCountry] FOREIGN KEY([CountryId])
REFERENCES [dbo].[tblCountry] ([Id])
GO

ALTER TABLE [dbo].[tblAddress] CHECK CONSTRAINT [FK_tblAddress_tblCountry]
GO

ALTER TABLE [dbo].[tblAddress]  WITH CHECK ADD  CONSTRAINT [FK_tblAddress_tblProvince] FOREIGN KEY([ProvinceId])
REFERENCES [dbo].[tblProvince] ([Id])
GO

ALTER TABLE [dbo].[tblAddress] CHECK CONSTRAINT [FK_tblAddress_tblProvince]
GO

