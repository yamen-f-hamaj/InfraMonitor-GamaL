---- InfraMonitor Initial Schema
---- Create database if it doesn't exist
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'InfraMonitor')
BEGIN
    PRINT 'Creating InfraMonitor database...';
    CREATE DATABASE InfraMonitor;
END
GO

--USE InfraMonitor;
--GO

----PRINT 'Creating Roles table...';
----IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Roles' AND type = 'U')
----BEGIN
----    CREATE TABLE Roles (
----        RoleId INT IDENTITY PRIMARY KEY,
----        Name NVARCHAR(50) NOT NULL UNIQUE,
----        Description NVARCHAR(250) NULL
----    );
----END
----GO

--PRINT 'Creating Users table...';
--IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Users' AND type = 'U')
--BEGIN
--    CREATE TABLE Users (
--        UserId INT IDENTITY PRIMARY KEY,
--        UserName NVARCHAR(50) NOT NULL UNIQUE,
--        Email NVARCHAR(100) NOT NULL UNIQUE,
--        PasswordHash NVARCHAR(500) NOT NULL,
--        RoleId INT NOT NULL,
--        RefreshToken NVARCHAR(500) NULL,
--        RefreshTokenExpiry DATETIME NULL,
--        CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),

--        CONSTRAINT FK_Users_Roles FOREIGN KEY (RoleId) REFERENCES Roles(RoleId)
--    );
--END
--GO

--PRINT 'Creating Servers table...';
--IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Servers' AND type = 'U')
--BEGIN
--    CREATE TABLE Servers (
--        ServerId INT IDENTITY PRIMARY KEY,
--        Name NVARCHAR(100) NOT NULL,
--        IPAddress NVARCHAR(50),
--        Status NVARCHAR(20) NOT NULL DEFAULT 'Up',
--        Description NVARCHAR(250),
--        CreatedAt DATETIME NOT NULL DEFAULT GETDATE()
--    );
--END
--GO

--PRINT 'Creating Metrics table...';
--IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Metrics' AND type = 'U')
--BEGIN
--    CREATE TABLE Metrics (
--        MetricId INT IDENTITY PRIMARY KEY,
--        ServerId INT NOT NULL,
--        CpuUsage FLOAT NOT NULL,
--        MemoryUsage FLOAT NOT NULL,
--        DiskUsage FLOAT NOT NULL,
--        ResponseTime FLOAT NOT NULL,
--        Status NVARCHAR(20) NOT NULL,
--        Timestamp DATETIME NOT NULL DEFAULT GETDATE(),

--        CONSTRAINT FK_Metrics_Servers FOREIGN KEY (ServerId) REFERENCES Servers(ServerId)
--    );
--END
--GO

--PRINT 'Creating Disks table...';
--IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Disks' AND type = 'U')
--BEGIN
--    CREATE TABLE Disks (
--        DiskId INT IDENTITY PRIMARY KEY,
--        ServerId INT NOT NULL,
--        DriveLetter NVARCHAR(5) NOT NULL,
--        FreeSpaceMB BIGINT NOT NULL,
--        TotalSpaceMB BIGINT NOT NULL,
--        UsedPercentage FLOAT NOT NULL,
--        Timestamp DATETIME NOT NULL DEFAULT GETDATE(),

--        CONSTRAINT FK_Disks_Servers FOREIGN KEY (ServerId) REFERENCES Servers(ServerId)
--    );
--END
--GO

--PRINT 'Creating Alerts table...';
--IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Alerts' AND type = 'U')
--BEGIN
--    CREATE TABLE Alerts (
--        AlertId INT IDENTITY PRIMARY KEY,
--        ServerId INT NOT NULL,
--        MetricType NVARCHAR(50) NOT NULL,
--        MetricValue FLOAT NOT NULL,
--        Threshold FLOAT NOT NULL,
--        Status NVARCHAR(20) NOT NULL DEFAULT 'Triggered',
--        CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
--        ResolvedAt DATETIME NULL,

--        CONSTRAINT FK_Alerts_Servers FOREIGN KEY (ServerId) REFERENCES Servers(ServerId)
--    );
--END
--GO

--PRINT 'Creating Reports table...';
--IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Reports' AND type = 'U')
--BEGIN
--    CREATE TABLE Reports (
--        ReportId INT IDENTITY PRIMARY KEY,
--        ServerId INT NOT NULL,
--        StartTime DATETIME NOT NULL,
--        EndTime DATETIME NOT NULL,
--        Status NVARCHAR(20) NOT NULL DEFAULT 'Pending',
--        FilePath NVARCHAR(500) NULL,
--        CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
--        CompletedAt DATETIME NULL,

--        CONSTRAINT FK_Reports_Servers FOREIGN KEY (ServerId) REFERENCES Servers(ServerId)
--    );
--END
--GO

--PRINT 'Creating indexes for performance...';

--IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Metrics_ServerId_Timestamp')
--BEGIN
--    CREATE INDEX IX_Metrics_ServerId_Timestamp
--    ON Metrics(ServerId, Timestamp);
--END
--GO

--IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Disks_ServerId_Timestamp')
--BEGIN
--    CREATE INDEX IX_Disks_ServerId_Timestamp
--    ON Disks(ServerId, Timestamp);
--END
--GO

--IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Alerts_ServerId')
--BEGIN
--    CREATE INDEX IX_Alerts_ServerId
--    ON Alerts(ServerId);
--END
--GO

--PRINT 'InfraMonitor initial schema creation complete!';
