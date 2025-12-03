CREATE TABLE [dbo].[TravelOrderTransport]
(
	[TransportId] INT NOT NULL , 
    [TravelOrderId] INT NOT NULL, 
    PRIMARY KEY ([TransportId], [TravelOrderId]), 
    CONSTRAINT [FK_TravelOrderTransport_Transport] FOREIGN KEY ([TransportId]) REFERENCES [Transport]([Id]),
    CONSTRAINT [FK_TravelOrderTransport_TravelOrder] FOREIGN KEY ([TravelOrderId]) REFERENCES [TravelOrder]([Id])
)
