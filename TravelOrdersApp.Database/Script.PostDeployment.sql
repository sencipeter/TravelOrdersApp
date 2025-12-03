/*
Post-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.		
 Use SQLCMD syntax to include a file in the post-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the post-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/
INSERT INTO [dbo].[City] (Id, CityName, Country, Location)
VALUES
(1, N'Bratislava', N'Slovensko', geography::Point(48.1486, 17.1077, 4326)),
(2, N'Viedeň', N'Rakúsko', geography::Point(48.2082, 16.3738, 4326)),
(3, N'Praha', N'Česká republika', geography::Point(50.0755, 14.4378, 4326)),
(4, N'Budapešť', N'Maďarsko', geography::Point(47.4979, 19.0402, 4326)),
(5, N'Berlín', N'Nemecko', geography::Point(52.5200, 13.4050, 4326)),
(6, N'Paríž', N'Francúzsko', geography::Point(48.8566, 2.3522, 4326)),
(7, N'Londýn', N'Spojené kráľovstvo', geography::Point(51.5074, -0.1278, 4326)),
(8, N'Rím', N'Taliansko', geography::Point(41.9028, 12.4964, 4326)),
(9, N'Madrid', N'Španielsko', geography::Point(40.4168, -3.7038, 4326)),
(10, N'New York', N'Spojené štáty', geography::Point(40.7128, -74.0060, 4326));

INSERT INTO [dbo].[Transport] (Id, Name)
VALUES
(1, N'Služobné auto'),
(2, N'Autobus'),
(3, N'MHD'),
(4, N'Pešo'),
(5, N'Vlak'),
(6, N'Taxi'),
(7, N'Lietadlo');

INSERT INTO [dbo].[TravelOrderState] (Id, Name)
VALUES
(1, N'Vytvorený'),
(2, N'Schválený'),
(3, N'Vyúčtovaný'),
(4, N'Zrušený');

INSERT INTO [dbo].[Employee] (PersonalNumber, FirstName, LastName, DateOfBirth, PersonalIdentificationNumber)
VALUES
('A9X3B7', N'Ján', N'Novák', '1985-03-12', '850312/1234'),
('Q7W2Z1', N'Peter', N'Horváth', '1990-07-25', '900725/5678'),
('M4K8R2', N'Mária', N'Kováčová', '1982-11-05', '821105/4321'),
('Z1T9L5', N'Lucia', N'Bartošová', '1995-01-18', '950118/8765'),
('X5P0C8', N'Michal', N'Szabó', '1988-09-09', '880909/1111'),
('R8D6F3', N'Anna', N'Černá', '1979-04-30', '790430/2222'),
('T2V7N4', N'Tomáš', N'Varga', '1993-12-14', '931214/3333'),
('K6H1Y9', N'Zuzana', N'Králová', '1987-06-21', '870621/4444'),
('B3J5U2', N'Pavol', N'Kučera', '1980-02-02', '800202/5555'),
('W9E4S7', N'Katarína', N'Majerová', '1992-08-27', '920827/6666');

INSERT INTO [dbo].[TravelOrder] 
(EmployeeId, StartingLocationCityId, DestinationCityId, BusinessTripStart, BusinessTripEnd, TravelOrderStateId)
VALUES
(1, 1, 2, '2025-01-10 08:00', '2025-01-12 18:00', 1),
(2, 1, 3, '2025-02-05 07:30', '2025-02-07 20:00', 2),
(3, 3, 4, '2025-03-15 09:00', '2025-03-18 17:00', 3),
(4, 2, 5, '2025-04-01 06:00', '2025-04-04 19:00', 1),
(5, 4, 6, '2025-05-20 08:00', '2025-05-25 21:00', 2),
(6, 1, 7, '2025-06-10 07:00', '2025-06-15 22:00', 3),
(7, 3, 8, '2025-07-02 09:00', '2025-07-06 20:00', 1),
(8, 1, 9, '2025-08-12 08:30', '2025-08-16 18:30', 2),
(9, 5, 10, '2025-09-01 10:00', '2025-09-10 23:00', 3),
(10, 1, 6, '2025-10-05 07:00', '2025-10-09 20:00', 4);