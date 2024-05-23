CREATE DATABASE CSVDataDB;

USE CSVDataDB;

CREATE TABLE Users (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Firstname NVARCHAR(100) NOT NULL,
    Surname NVARCHAR(100) NOT NULL,
    Age INT NOT NULL,
    Sex CHAR(1) NOT NULL,
    Mobile NVARCHAR(15) NOT NULL,
    Active BIT NOT NULL
);

INSERT INTO Users (Firstname, Surname, Age, Sex, Mobile, Active)
VALUES ('John', 'Doe', 30, 'M', '1234567890', 1);
