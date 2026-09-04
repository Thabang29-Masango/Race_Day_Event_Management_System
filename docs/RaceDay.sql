--CREATE DATABASE RaceDayDB;
--GO

--USE RaceDayDB;
--GO

/* =========================================
   USERS TABLE
   ========================================= */

--CREATE TABLE Users
--(
--    UserId INT IDENTITY(1,1) PRIMARY KEY,
--    Email VARCHAR(255) NOT NULL UNIQUE,
--    PasswordHash VARCHAR(255) NOT NULL,
--    Role VARCHAR(20) NOT NULL,
--    DateCreated DATETIME2 NOT NULL DEFAULT GETDATE(),

--    CONSTRAINT CK_Users_Role
--        CHECK (Role IN ('Organiser', 'Participant'))
--);
--GO


/* =========================================
   ORGANISERS TABLE
   ========================================= */

--CREATE TABLE Organisers
--(
--    UserId INT PRIMARY KEY,
--    FirstName VARCHAR(100) NOT NULL,
--    LastName VARCHAR(100) NOT NULL,
--    PhoneNumber VARCHAR(20) NOT NULL,
--    OrganisationName VARCHAR(150) NOT NULL,

--    CONSTRAINT FK_Organisers_Users
--        FOREIGN KEY (UserId)
--        REFERENCES Users(UserId)
--        ON DELETE CASCADE
--);
--GO


/* =========================================
   PARTICIPANTS TABLE
   ========================================= */

--CREATE TABLE Participants
--(
--    UserId INT PRIMARY KEY,
--    FirstName VARCHAR(100) NOT NULL,
--    LastName VARCHAR(100) NOT NULL,
--    DateOfBirth DATE NOT NULL,
--    ProfilePictureUrl VARCHAR(500) NULL,

--    CONSTRAINT FK_Participants_Users
--        FOREIGN KEY (UserId)
--        REFERENCES Users(UserId)
--        ON DELETE CASCADE
--);
--GO


/* =========================================
   EVENTS TABLE
   ========================================= */

--CREATE TABLE Events
--(
--    EventId INT IDENTITY(1,1) PRIMARY KEY,
--    OrganiserId INT NOT NULL,
--    Name VARCHAR(150) NOT NULL,
--    Description VARCHAR(1000) NULL,
--    EventDate DATE NOT NULL,
--    Location VARCHAR(255) NOT NULL,
--    DistanceKm DECIMAL(6,2) NOT NULL,
--    EventType VARCHAR(100) NOT NULL,
--    BannerImageUrl VARCHAR(500) NULL,

--    CONSTRAINT FK_Events_Organisers
--        FOREIGN KEY (OrganiserId)
--        REFERENCES Organisers(UserId),

--    CONSTRAINT CK_Events_Distance
--        CHECK (DistanceKm > 0)
--);
--GO


/* =========================================
   CATEGORIES TABLE
   ========================================= */

--CREATE TABLE Categories
--(
--    CategoryId INT IDENTITY(1,1) PRIMARY KEY,
--    EventId INT NOT NULL,
--    Name VARCHAR(100) NOT NULL,
--    MinAge INT NOT NULL,
--    MaxAge INT NOT NULL,
--    DistanceKm DECIMAL(6,2) NOT NULL,

--    CONSTRAINT FK_Categories_Events
--        FOREIGN KEY (EventId)
--        REFERENCES Events(EventId)
--        ON DELETE CASCADE,

--    CONSTRAINT CK_Categories_Age
--        CHECK (MinAge >= 0 AND MaxAge >= MinAge),

--    CONSTRAINT CK_Categories_Distance
--        CHECK (DistanceKm > 0)
--);
--GO


/* =========================================
   ENROLMENTS TABLE
   ========================================= */

--CREATE TABLE Enrolments
--(
--    EnrolmentId INT IDENTITY(1,1) PRIMARY KEY,
--    ParticipantId INT NOT NULL,
--    EventId INT NOT NULL,
--    CategoryId INT NOT NULL,
--    EnrolmentDate DATETIME2 NOT NULL DEFAULT GETDATE(),
--    Status VARCHAR(30) NOT NULL DEFAULT 'Active',

--    CONSTRAINT FK_Enrolments_Participants
--        FOREIGN KEY (ParticipantId)
--        REFERENCES Participants(UserId),

--    CONSTRAINT FK_Enrolments_Events
--        FOREIGN KEY (EventId)
--        REFERENCES Events(EventId),

--    CONSTRAINT FK_Enrolments_Categories
--        FOREIGN KEY (CategoryId)
--        REFERENCES Categories(CategoryId),

--    CONSTRAINT CK_Enrolments_Status
--        CHECK (Status IN ('Active', 'Cancelled', 'Completed')),

--    CONSTRAINT UQ_Enrolments_Participant_Event
--        UNIQUE (ParticipantId, EventId)
--);
--GO


/* =========================================
   RESULTS TABLE
   ========================================= */

--CREATE TABLE Results
--(
--    ResultId INT IDENTITY(1,1) PRIMARY KEY,
--    EnrolmentId INT NOT NULL UNIQUE,
--    FinishTime TIME NULL,
--    FinishPosition INT NULL,
--    TotalFinishers INT NULL,
--    CapturedDate DATETIME2 NOT NULL DEFAULT GETDATE(),

--    CONSTRAINT FK_Results_Enrolments
--        FOREIGN KEY (EnrolmentId)
--        REFERENCES Enrolments(EnrolmentId)
--        ON DELETE CASCADE,

--    CONSTRAINT CK_Results_Position
--        CHECK (FinishPosition IS NULL OR FinishPosition > 0),

--    CONSTRAINT CK_Results_TotalFinishers
--        CHECK (TotalFinishers IS NULL OR TotalFinishers > 0)
--);
--GO

/* =========================================
   USERS
   ========================================= */

--INSERT INTO Users
--    (Email, PasswordHash, Role)
--VALUES
--    ('john.organiser@raceday.co.za', 'HASHED_PASSWORD_001', 'Organiser'),
--    ('sarah.organiser@raceday.co.za', 'HASHED_PASSWORD_002', 'Organiser'),
--    ('thabo.participant@example.com', 'HASHED_PASSWORD_003', 'Participant'),
--    ('lerato.participant@example.com', 'HASHED_PASSWORD_004', 'Participant');
--GO


/* =========================================
   ORGANISERS
   ========================================= */

--INSERT INTO Organisers
--    (UserId, FirstName, LastName, PhoneNumber, OrganisationName)
--VALUES
--    (1, 'John', 'Mokoena', '0712345678', 'RaceDay Events'),
--    (2, 'Sarah', 'Naidoo', '0723456789', 'Active South Africa');
--GO


/* =========================================
   PARTICIPANTS
   ========================================= */

--INSERT INTO Participants
--    (UserId, FirstName, LastName, DateOfBirth, ProfilePictureUrl)
--VALUES
--    (3, 'Thabo', 'Molefe', '2003-05-15', NULL),
--    (4, 'Lerato', 'Mthembu', '2005-08-22', NULL);
--GO


/* =========================================
   EVENTS
   ========================================= */

--INSERT INTO Events
--    (OrganiserId, Name, Description, EventDate, Location, DistanceKm, EventType, BannerImageUrl)
--VALUES
--    (
--        1,
--        'Pretoria City Run',
--        'Annual road running event in Pretoria.',
--        '2026-10-10',
--        'Pretoria',
--        10.00,
--        'Road Race',
--        NULL
--    ),
--    (
--        1,
--        'Johannesburg Charity Run',
--        'Community running event supporting local charities.',
--        '2026-11-07',
--        'Johannesburg',
--        15.00,
--        'Charity Run',
--        NULL
--    ),
--    (
--        2,
--        'Centurion Fun Run',
--        'Family-friendly running event.',
--        '2026-12-05',
--        'Centurion',
--        5.00,
--        'Fun Run',
--        NULL
--    );
--GO


/* =========================================
   CATEGORIES
   ========================================= */

--INSERT INTO Categories
--    (EventId, Name, MinAge, MaxAge, DistanceKm)
--VALUES
--    (1, 'Junior', 13, 17, 5.00),
--    (1, 'Open', 18, 39, 10.00),
--    (1, 'Veteran', 40, 99, 10.00),

--    (2, 'Open', 18, 39, 15.00),
--    (2, 'Veteran', 40, 99, 15.00),

--    (3, 'Junior', 13, 17, 5.00),
--    (3, 'Open', 18, 99, 5.00);
--GO


/* =========================================
   ENROLMENTS
   ========================================= */

--INSERT INTO Enrolments
--    (ParticipantId, EventId, CategoryId, Status)
--VALUES
--    (3, 1, 2, 'Active'),
--    (4, 1, 1, 'Active'),
--    (3, 2, 4, 'Active'),
--    (4, 3, 6, 'Active');
--GO


/* =========================================
   RESULTS
   ========================================= */

--INSERT INTO Results
--    (EnrolmentId, FinishTime, FinishPosition, TotalFinishers)
--VALUES
--    (1, '00:52:34', 15, 120),
--    (2, '00:31:45', 8, 85);
--GO

--SELECT * FROM Users;
--SELECT * FROM Organisers;
--SELECT * FROM Participants;
--SELECT * FROM Events;
--SELECT * FROM Categories;
--SELECT * FROM Enrolments;
--SELECT * FROM Results;
--GO