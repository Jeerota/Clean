CREATE TABLE [dbo].[Locations]
(
	[Id]			BIGINT NOT NULL IDENTITY(1, 1) CONSTRAINT [PK_Locaitons_Id] PRIMARY KEY,
	[Name]			VARCHAR(100) NULL,
	[AddressLine1]	VARCHAR(100) NOT NULL,
	[AddressLine2]	VARCHAR(100) NULL,
	[City]			VARCHAR(50) NOT NULL,
	[State]			VARCHAR(50) NULL,
	[Country]		VARCHAR(50) NOT NULL,
	[Zip]			VARCHAR(25) NULL,
	[CreatedDatetime]	DATETIME2(7) NOT NULL CONSTRAINT [DF_Locations_CreatedDate] DEFAULT(GETDATE()),
	[ModifiedDatetime]	DATETIME2(7) NULL
)