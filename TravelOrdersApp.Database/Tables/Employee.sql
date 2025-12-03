CREATE TABLE [dbo].[Employee]
(
    [Id] INT NOT NULL PRIMARY KEY IDENTITY,
	[PersonalNumber] NVARCHAR(10) NOT NULL, 
    [FirstName] NVARCHAR(150) COLLATE Latin1_General_CI_AI NOT NULL, 
    [LastName] NVARCHAR(150) COLLATE Latin1_General_CI_AI NOT NULL, 
    [DateOfBirth] DATETIME2 NOT NULL, 
    [PersonalIdentificationNumber] VARCHAR(11) NOT NULL,    

    -- Unique constraint on PersonalIdentificationNumber
    CONSTRAINT UQ_Employee_PersonalNumber 
        UNIQUE (PersonalNumber),

    -- Check constraint for basic format validation YYMMDD/XXXX
    CONSTRAINT CK_Employee_PersonalIdentificationNumber_Format
        CHECK (PersonalIdentificationNumber LIKE '[0-9][0-9][0-9][0-9][0-9][0-9]/[0-9][0-9][0-9][0-9]')
)