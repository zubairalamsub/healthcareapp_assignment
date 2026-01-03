-- =============================================
-- Healthcare App - Complete Database Setup Script
-- =============================================

-- =============================================
-- STEP 0: Create Database
-- =============================================
PRINT '========================================';
PRINT 'Healthcare Database Setup';
PRINT '========================================';
PRINT '';

IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'HealthcareDB')
BEGIN
    PRINT 'Creating HealthcareDB database...';
    CREATE DATABASE HealthcareDB;
    PRINT 'Database created successfully.';
END
ELSE
BEGIN
    PRINT 'Database HealthcareDB already exists.';
END
PRINT '';

USE HealthcareDB;
GO

PRINT 'Using HealthcareDB database.';
PRINT '';

-- =============================================
-- STEP 1: Drop Existing Tables (in reverse dependency order)
-- =============================================
PRINT 'STEP 1: Dropping existing tables...';

IF OBJECT_ID('PatientCaregivers', 'U') IS NOT NULL
    DROP TABLE PatientCaregivers;

IF OBJECT_ID('SearchAudits', 'U') IS NOT NULL
    DROP TABLE SearchAudits;

IF OBJECT_ID('Patients', 'U') IS NOT NULL
    DROP TABLE Patients;

IF OBJECT_ID('Caregivers', 'U') IS NOT NULL
    DROP TABLE Caregivers;

IF OBJECT_ID('Users', 'U') IS NOT NULL
    DROP TABLE Users;

IF OBJECT_ID('Offices', 'U') IS NOT NULL
    DROP TABLE Offices;

IF OBJECT_ID('Roles', 'U') IS NOT NULL
    DROP TABLE Roles;

PRINT 'Tables dropped successfully.';
PRINT '';

-- =============================================
-- STEP 2: Create Tables
-- =============================================
PRINT 'STEP 2: Creating tables...';

-- Table: Roles
PRINT 'Creating Roles table...';
CREATE TABLE Roles (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(50) NOT NULL UNIQUE,
    Description NVARCHAR(200)
);

-- Table: Offices
PRINT 'Creating Offices table...';
CREATE TABLE Offices (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL,
    Address NVARCHAR(200) NOT NULL,
    Phone NVARCHAR(20) NOT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NULL
);

-- Table: Users
PRINT 'Creating Users table...';
CREATE TABLE Users (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Username NVARCHAR(50) NOT NULL UNIQUE,
    Email NVARCHAR(100) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(255) NOT NULL,
    RoleId INT NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    LastLoginAt DATETIME2 NULL,
    CONSTRAINT FK_Users_Roles FOREIGN KEY (RoleId) REFERENCES Roles(Id)
);

-- Table: Caregivers
PRINT 'Creating Caregivers table...';
CREATE TABLE Caregivers (
    Id INT PRIMARY KEY IDENTITY(1,1),
    OfficeId INT NOT NULL,
    FirstName NVARCHAR(50) NOT NULL,
    LastName NVARCHAR(50) NOT NULL,
    Phone NVARCHAR(20) NOT NULL,
    Email NVARCHAR(100) NOT NULL,
    Specialization NVARCHAR(100) NOT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NULL,
    CONSTRAINT FK_Caregivers_Offices FOREIGN KEY (OfficeId) REFERENCES Offices(Id)
);

-- Table: Patients
PRINT 'Creating Patients table...';
CREATE TABLE Patients (
    Id INT PRIMARY KEY IDENTITY(1,1),
    OfficeId INT NOT NULL,
    FirstName NVARCHAR(50) NOT NULL,
    LastName NVARCHAR(50) NOT NULL,
    DateOfBirth DATE NOT NULL,
    Phone NVARCHAR(20) NOT NULL,
    Email NVARCHAR(100) NOT NULL,
    Address NVARCHAR(200) NOT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 NULL,
    CONSTRAINT FK_Patients_Offices FOREIGN KEY (OfficeId) REFERENCES Offices(Id)
);

-- Table: PatientCaregivers (Many-to-Many)
PRINT 'Creating PatientCaregivers table...';
CREATE TABLE PatientCaregivers (
    PatientId INT NOT NULL,
    CaregiverId INT NOT NULL,
    AssignedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    PRIMARY KEY (PatientId, CaregiverId),
    CONSTRAINT FK_PatientCaregivers_Patients FOREIGN KEY (PatientId) REFERENCES Patients(Id) ON DELETE CASCADE,
    CONSTRAINT FK_PatientCaregivers_Caregivers FOREIGN KEY (CaregiverId) REFERENCES Caregivers(Id) ON DELETE CASCADE
);

-- Table: SearchAudits
PRINT 'Creating SearchAudits table...';
CREATE TABLE SearchAudits (
    Id INT PRIMARY KEY IDENTITY(1,1),
    UserId NVARCHAR(50) NOT NULL,
    EntityType NVARCHAR(50) NOT NULL,
    SearchTerm NVARCHAR(200),
    ResultCount INT NOT NULL,
    SearchedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    IpAddress NVARCHAR(50)
);

PRINT 'All tables created successfully.';
PRINT '';

-- =============================================
-- STEP 3: Create Indexes for Performance
-- =============================================
PRINT 'STEP 3: Creating indexes...';

-- Indexes on Users
CREATE INDEX IX_Users_Username ON Users(Username);
CREATE INDEX IX_Users_Email ON Users(Email);
CREATE INDEX IX_Users_RoleId ON Users(RoleId);

-- Indexes on Caregivers
CREATE INDEX IX_Caregivers_OfficeId ON Caregivers(OfficeId);
CREATE INDEX IX_Caregivers_LastName ON Caregivers(LastName);
CREATE INDEX IX_Caregivers_FirstName ON Caregivers(FirstName);
CREATE INDEX IX_Caregivers_Email ON Caregivers(Email);

-- Indexes on Patients
CREATE INDEX IX_Patients_OfficeId ON Patients(OfficeId);
CREATE INDEX IX_Patients_LastName ON Patients(LastName);
CREATE INDEX IX_Patients_FirstName ON Patients(FirstName);
CREATE INDEX IX_Patients_Email ON Patients(Email);
CREATE INDEX IX_Patients_DateOfBirth ON Patients(DateOfBirth);

-- Indexes on PatientCaregivers
CREATE INDEX IX_PatientCaregivers_CaregiverId ON PatientCaregivers(CaregiverId);
CREATE INDEX IX_PatientCaregivers_PatientId ON PatientCaregivers(PatientId);

-- Indexes on SearchAudits
CREATE INDEX IX_SearchAudits_UserId ON SearchAudits(UserId);
CREATE INDEX IX_SearchAudits_SearchedAt ON SearchAudits(SearchedAt);
CREATE INDEX IX_SearchAudits_EntityType ON SearchAudits(EntityType);

PRINT 'Indexes created successfully.';
PRINT '';

-- =============================================
-- STEP 4: Seed Data
-- =============================================
PRINT 'STEP 4: Seeding data...';

-- Seed Roles
PRINT 'Seeding Roles...';
INSERT INTO Roles (Name, Description) VALUES
('Admin', 'Full system access with all permissions'),
('Manager', 'Can manage patients, caregivers, and view reports'),
('User', 'Read-only access to view data');

-- Seed Offices (Bangladesh Locations)
PRINT 'Seeding Offices...';
INSERT INTO Offices (Name, Address, Phone) VALUES
('Dhaka Medical Center', 'House-10, Road-5, Dhanmondi, Dhaka-1205', '+880-2-9661234'),
('Chittagong Health Clinic', 'Plot-15, CDA Avenue, Nasirabad, Chittagong-4200', '+880-31-2556789'),
('Sylhet Family Care', 'Zindabazar Road, Sylhet-3100', '+880-821-715432'),
('Rajshahi Wellness Center', 'Saheb Bazar, Rajshahi-6100', '+880-721-772345'),
('Khulna Medical Group', 'Khan-E-Sabur Road, Khulna-9100', '+880-41-761890');

-- Seed Users (Password: Password123! for all users)
PRINT 'Seeding Users...';
INSERT INTO Users (Username, Email, PasswordHash, RoleId, IsActive) VALUES
('admin', 'admin@healthcare.bd', '$2a$11$9AK4w/kY1iP.hF/p/DYhHuhHNJYkIUSoIgwBZdTz5Hmtxs15xL8za', 1, 1),
('manager', 'manager@healthcare.bd', '$2a$11$9AK4w/kY1iP.hF/p/DYhHuhHNJYkIUSoIgwBZdTz5Hmtxs15xL8za', 2, 1),
('user1', 'user1@healthcare.bd', '$2a$11$9AK4w/kY1iP.hF/p/DYhHuhHNJYkIUSoIgwBZdTz5Hmtxs15xL8za', 3, 1),
('karim.admin', 'karim.admin@healthcare.bd', '$2a$11$9AK4w/kY1iP.hF/p/DYhHuhHNJYkIUSoIgwBZdTz5Hmtxs15xL8za', 1, 1),
('nasrin.manager', 'nasrin.manager@healthcare.bd', '$2a$11$9AK4w/kY1iP.hF/p/DYhHuhHNJYkIUSoIgwBZdTz5Hmtxs15xL8za', 2, 1);

-- Seed Caregivers (Bangladeshi Names)
PRINT 'Seeding Caregivers...';
INSERT INTO Caregivers (OfficeId, FirstName, LastName, Phone, Email, Specialization) VALUES
-- Dhaka Medical Center
(1, 'Abdul', 'Karim', '+880-1711-234567', 'abdul.karim@healthcare.bd', 'General Practice'),
(1, 'Fatema', 'Begum', '+880-1712-234568', 'fatema.begum@healthcare.bd', 'Pediatrics'),
(1, 'Mohammad', 'Rahman', '+880-1713-234569', 'mohammad.rahman@healthcare.bd', 'Cardiology'),
(1, 'Salma', 'Khatun', '+880-1714-234570', 'salma.khatun@healthcare.bd', 'Nursing'),
-- Chittagong Health Clinic
(2, 'Tarek', 'Ahmed', '+880-1715-234571', 'tarek.ahmed@healthcare.bd', 'Orthopedics'),
(2, 'Rumana', 'Akter', '+880-1716-234572', 'rumana.akter@healthcare.bd', 'Physical Therapy'),
(2, 'Jahangir', 'Alam', '+880-1717-234573', 'jahangir.alam@healthcare.bd', 'General Practice'),
(2, 'Nazma', 'Parveen', '+880-1718-234574', 'nazma.parveen@healthcare.bd', 'Nursing'),
-- Sylhet Family Care
(3, 'Rafiq', 'Islam', '+880-1719-234575', 'rafiq.islam@healthcare.bd', 'Family Medicine'),
(3, 'Shahnaz', 'Ali', '+880-1720-234576', 'shahnaz.ali@healthcare.bd', 'Pediatrics'),
(3, 'Anowar', 'Hossain', '+880-1721-234577', 'anowar.hossain@healthcare.bd', 'Internal Medicine'),
(3, 'Rokeya', 'Sultana', '+880-1722-234578', 'rokeya.sultana@healthcare.bd', 'Nursing'),
-- Rajshahi Wellness Center
(4, 'Mahbub', 'Chowdhury', '+880-1723-234579', 'mahbub.chowdhury@healthcare.bd', 'Dermatology'),
(4, 'Shirin', 'Nahar', '+880-1724-234580', 'shirin.nahar@healthcare.bd', 'General Practice'),
(4, 'Kamal', 'Uddin', '+880-1725-234581', 'kamal.uddin@healthcare.bd', 'Psychiatry'),
-- Khulna Medical Group
(5, 'Ibrahim', 'Khan', '+880-1726-234582', 'ibrahim.khan@healthcare.bd', 'Surgery'),
(5, 'Nasrin', 'Jahan', '+880-1727-234583', 'nasrin.jahan@healthcare.bd', 'Radiology'),
(5, 'Hasan', 'Mahmud', '+880-1728-234584', 'hasan.mahmud@healthcare.bd', 'Nursing');

-- Seed Patients (Bangladeshi Names)
PRINT 'Seeding Patients...';
INSERT INTO Patients (OfficeId, FirstName, LastName, DateOfBirth, Phone, Email, Address) VALUES
-- Dhaka Medical Center patients
(1, 'Ali', 'Hasan', '1985-03-15', '+880-1812-345678', 'ali.hasan@email.bd', 'House-20, Road-7, Gulshan-2, Dhaka-1212'),
(1, 'Ayesha', 'Siddika', '1978-07-22', '+880-1813-345679', 'ayesha.siddika@email.bd', 'Flat-5B, Banani, Dhaka-1213'),
(1, 'Rashed', 'Mia', '1992-11-08', '+880-1814-345680', 'rashed.mia@email.bd', 'House-15, Mirpur-10, Dhaka-1216'),
(1, 'Sabina', 'Yasmin', '1965-01-30', '+880-1815-345681', 'sabina.yasmin@email.bd', 'Block-C, Mohammadpur, Dhaka-1207'),
(1, 'Tanvir', 'Rahman', '1988-11-06', '+880-1816-345682', 'tanvir.rahman@email.bd', 'Road-3, Uttara, Dhaka-1230'),
(1, 'Shaheen', 'Akter', '1950-12-12', '+880-1817-345683', 'shaheen.akter@email.bd', 'House-8, Baridhara DOHS, Dhaka-1229'),
-- Chittagong Health Clinic patients
(2, 'Noor', 'Mohammad', '1995-05-20', '+880-1818-345684', 'noor.mohammad@email.bd', 'Hazari Goli, Chittagong-4000'),
(2, 'Moriom', 'Begum', '1973-09-14', '+880-1819-345685', 'moriom.begum@email.bd', 'Panchlaish, Chittagong-4203'),
(2, 'Jamal', 'Uddin', '1990-02-28', '+880-1820-345686', 'jamal.uddin@email.bd', 'Agrabad, Chittagong-4100'),
(2, 'Rubina', 'Khanam', '1982-06-17', '+880-1821-345687', 'rubina.khanam@email.bd', 'Khulshi, Chittagong-4225'),
(2, 'Sohel', 'Rana', '1987-04-19', '+880-1822-345688', 'sohel.rana@email.bd', 'Bakolia, Chittagong-4210'),
-- Sylhet Family Care patients
(3, 'Babor', 'Ali', '1960-08-25', '+880-1823-345689', 'babor.ali@email.bd', 'Zindabazar, Sylhet-3100'),
(3, 'Jesmin', 'Sultana', '1993-10-10', '+880-1824-345690', 'jesmin.sultana@email.bd', 'Ambarkhana, Sylhet-3100'),
(3, 'Foysal', 'Ahmad', '1985-07-07', '+880-1825-345691', 'foysal.ahmad@email.bd', 'Mirabazar, Sylhet-3100'),
(3, 'Popy', 'Akter', '1991-12-03', '+880-1826-345692', 'popy.akter@email.bd', 'Uposhohor, Sylhet-3100'),
-- Rajshahi Wellness Center patients
(4, 'Kabir', 'Hossain', '1998-08-10', '+880-1827-345693', 'kabir.hossain@email.bd', 'Saheb Bazar, Rajshahi-6100'),
(4, 'Lucky', 'Sarkar', '1986-03-22', '+880-1828-345694', 'lucky.sarkar@email.bd', 'Ranibazar, Rajshahi-6100'),
(4, 'Munir', 'Hasan', '1989-05-05', '+880-1829-345695', 'munir.hasan@email.bd', 'Kazla, Rajshahi-6204'),
-- Khulna Medical Group patients
(5, 'Salman', 'Sheikh', '1977-11-30', '+880-1830-345696', 'salman.sheikh@email.bd', 'Daulatpur, Khulna-9200'),
(5, 'Dilruba', 'Parveen', '1955-04-15', '+880-1831-345697', 'dilruba.parveen@email.bd', 'Khan Jahan Ali Road, Khulna-9100'),
(5, 'Riaz', 'Mahmud', '1983-09-27', '+880-1832-345698', 'riaz.mahmud@email.bd', 'New Market, Khulna-9100');

-- Seed Patient-Caregiver Assignments
PRINT 'Seeding Patient-Caregiver assignments...';
INSERT INTO PatientCaregivers (PatientId, CaregiverId) VALUES
-- Dhaka Medical Center assignments
(1, 1), (1, 4),  -- Ali Hasan -> Abdul Karim (GP) + Salma Khatun (Nursing)
(2, 1), (2, 4),  -- Ayesha Siddika -> Abdul Karim (GP) + Salma Khatun (Nursing)
(3, 2), (3, 4),  -- Rashed Mia -> Fatema Begum (Pediatrics) + Salma Khatun (Nursing)
(4, 3), (4, 4),  -- Sabina Yasmin -> Mohammad Rahman (Cardiology) + Salma Khatun (Nursing)
(5, 1), (5, 2),  -- Tanvir Rahman -> Abdul Karim (GP) + Fatema Begum (Pediatrics)
(6, 3),          -- Shaheen Akter -> Mohammad Rahman (Cardiology)
-- Chittagong Health Clinic assignments
(7, 5), (7, 8),  -- Noor Mohammad -> Tarek Ahmed (Orthopedics) + Nazma Parveen (Nursing)
(8, 7), (8, 8),  -- Moriom Begum -> Jahangir Alam (GP) + Nazma Parveen (Nursing)
(9, 6), (9, 8),  -- Jamal Uddin -> Rumana Akter (PT) + Nazma Parveen (Nursing)
(10, 7), (10, 8), -- Rubina Khanam -> Jahangir Alam (GP) + Nazma Parveen (Nursing)
(11, 5), (11, 6), -- Sohel Rana -> Tarek Ahmed (Orthopedics) + Rumana Akter (PT)
-- Sylhet Family Care assignments
(12, 9), (12, 12), -- Babor Ali -> Rafiq Islam (Family Med) + Rokeya Sultana (Nursing)
(13, 10), (13, 12), -- Jesmin Sultana -> Shahnaz Ali (Pediatrics) + Rokeya Sultana (Nursing)
(14, 11), (14, 12), -- Foysal Ahmad -> Anowar Hossain (Internal Med) + Rokeya Sultana (Nursing)
(15, 9), (15, 10),  -- Popy Akter -> Rafiq Islam (Family Med) + Shahnaz Ali (Pediatrics)
-- Rajshahi Wellness Center assignments
(16, 14), (16, 15), -- Kabir Hossain -> Shirin Nahar (GP) + Kamal Uddin (Psychiatry)
(17, 13), (17, 14), -- Lucky Sarkar -> Mahbub Chowdhury (Dermatology) + Shirin Nahar (GP)
(18, 14), (18, 15), -- Munir Hasan -> Shirin Nahar (GP) + Kamal Uddin (Psychiatry)
-- Khulna Medical Group assignments
(19, 16), (19, 18), -- Salman Sheikh -> Ibrahim Khan (Surgery) + Hasan Mahmud (Nursing)
(20, 17), (20, 18), -- Dilruba Parveen -> Nasrin Jahan (Radiology) + Hasan Mahmud (Nursing)
(21, 16), (21, 17); -- Riaz Mahmud -> Ibrahim Khan (Surgery) + Nasrin Jahan (Radiology)

PRINT 'Sample data seeded successfully.';
PRINT '';

