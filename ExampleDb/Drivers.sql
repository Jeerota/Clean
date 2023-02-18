CREATE TABLE [dbo].[Drivers]
(
	[Id]			BIGINT NOT NULL IDENTITY(1, 1) CONSTRAINT [PK_Drivers_Id] PRIMARY KEY,
	[LocationId]	BIGINT NULL,
	[VehicleId]		BIGINT NULL,
	[FirstName]		VARCHAR(100) NULL,
	[LastName]		VARCHAR(100) NULL,
	[CreatedDate]	DATETIME2(7) NOT NULL CONSTRAINT [DF_Drivers_CreatedDate] DEFAULT(GETDATE()),
	[ModifiedDate]	DATETIME2(7) NULL,
	Constraint [FK_Drivers_Locations_LocationId_Id] FOREIGN KEY ([LocationId]) REFERENCES [dbo].[Locations]([Id])
		ON DELETE CASCADE,
	Constraint [FK_Drivers_Vehicles_VehicleId_Id] FOREIGN KEY ([VehicleId]) REFERENCES [dbo].[Vehicles]([Id])
		ON DELETE CASCADE
)