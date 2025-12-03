CREATE TABLE [dbo].[TravelOrder]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    CreatedDate DATETIME2 NOT NULL DEFAULT GETDATE(),
	EmployeeId Int NOT NULL,
	StartingLocationCityId Int NOT NULL,
	DestinationCityId Int NOT NULL,
	BusinessTripStart DATETIME2 NOT NULL,
	BusinessTripEnd DATETIME2 NOT NULL,
	TravelOrderStateId int NOT NULL,
	CONSTRAINT [FK_TravelOrder_Employee] FOREIGN KEY ([EmployeeId]) REFERENCES [Employee]([Id]),
	CONSTRAINT [FK_TravelOrder_StartingLocationCity] FOREIGN KEY ([StartingLocationCityId]) REFERENCES [City]([Id]),
	CONSTRAINT [FK_TravelOrder_DestinationCity] FOREIGN KEY ([DestinationCityId]) REFERENCES [City]([Id]),
	CONSTRAINT [FK_TravelOrder_TravelOrderState] FOREIGN KEY ([TravelOrderStateId]) REFERENCES [TravelOrderState]([Id])
)
