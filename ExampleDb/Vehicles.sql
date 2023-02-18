CREATE TABLE [dbo].[Vehicles]
(
	[Id]			BIGINT NOT NULL IDENTITY(1, 1) CONSTRAINT [PK_Vehicles_Id] PRIMARY KEY,
	[LocationId]	BIGINT NOT NULL,
	[Name]			VARCHAR(100) NULL,
	[Make]			VARCHAR(100) NULL,
	[Model]			VARCHAR(100) NULL,
	[Plate]			VARCHAR(100) NULL,
	[VIN]			VARCHAR(100) NULL,
	[CreatedDate]	DATETIME2(7) NOT NULL CONSTRAINT [DF_Vehicles_CreatedDate] DEFAULT(GETDATE()),
	[ModifiedDate]	DATETIME2(7) NULL,
	Constraint [FK_Vehicles_Locations_LocationId_Id] FOREIGN KEY ([LocationId]) REFERENCES [dbo].[Locations]([Id])
		ON DELETE CASCADE
)