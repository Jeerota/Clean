CREATE TABLE [dbo].[Samples]
(
	[Id]			BIGINT NOT NULL IDENTITY(1, 1) CONSTRAINT [PK_Samples_Id] PRIMARY KEY,
	[ExampleId]		BIGINT NOT NULL,
	[Name]			VARCHAR(100) NULL,
	[CreatedDate]	DATETIME2(7) NOT NULL CONSTRAINT [DF_Samples_CreatedDate] DEFAULT(GETDATE()),
	[ModifiedDate]	DATETIME2(7) NULL,
	Constraint [FK_Samples_Examples_ExampleId_Id] FOREIGN KEY ([ExampleId]) REFERENCES [dbo].[Examples]([Id])
)