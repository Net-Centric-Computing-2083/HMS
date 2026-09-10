-- ============================================================
-- Hospital/Clinic Management System - Database Schema
-- Group 4 | Version 1.0
-- ============================================================

IF DB_ID('HospitalClinicDB') IS NULL
BEGIN
    CREATE DATABASE HospitalClinicDB;
END
GO

USE HospitalClinicDB;
GO

-- 1. Patients
CREATE TABLE Patients (
    PatientID         INT IDENTITY(1,1) PRIMARY KEY,
    FirstName         NVARCHAR(50)    NOT NULL,
    LastName          NVARCHAR(50)    NOT NULL,
    DateOfBirth       DATE            NOT NULL,
    Gender            NVARCHAR(10)    NOT NULL,
    ContactNumber     NVARCHAR(15)    NOT NULL,
    Email             NVARCHAR(100)   NULL,
    Address           NVARCHAR(200)   NULL,
    BloodGroup        NVARCHAR(5)     NULL,
    RegistrationDate  DATETIME        NOT NULL DEFAULT GETDATE()
);
GO

-- 2. Doctors
CREATE TABLE Doctors (
    DoctorID          INT IDENTITY(1,1) PRIMARY KEY,
    FullName          NVARCHAR(100)   NOT NULL,
    Specialization    NVARCHAR(100)   NOT NULL,
    ContactNumber     NVARCHAR(15)    NOT NULL,
    Email             NVARCHAR(100)   NULL,
    AvailableDays     NVARCHAR(50)    NULL,
    ConsultationFee   DECIMAL(10,2)   NOT NULL,
    Status            NVARCHAR(20)    NOT NULL DEFAULT 'Active'
);
GO

-- 3. Appointments
CREATE TABLE Appointments (
    AppointmentID     INT IDENTITY(1,1) PRIMARY KEY,
    PatientID         INT NOT NULL,
    DoctorID          INT NOT NULL,
    AppointmentDate   DATE NOT NULL,
    AppointmentTime   TIME NOT NULL,
    Reason            NVARCHAR(200) NULL,
    Status            NVARCHAR(20) NOT NULL DEFAULT 'Scheduled',
    CreatedDate       DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_Appointments_Patients FOREIGN KEY (PatientID) REFERENCES Patients(PatientID),
    CONSTRAINT FK_Appointments_Doctors  FOREIGN KEY (DoctorID)  REFERENCES Doctors(DoctorID),
    CONSTRAINT UQ_Doctor_DateTime UNIQUE (DoctorID, AppointmentDate, AppointmentTime)
);
GO

-- 4. MedicalHistory
CREATE TABLE MedicalHistory (
    HistoryID         INT IDENTITY(1,1) PRIMARY KEY,
    PatientID         INT NOT NULL,
    DoctorID          INT NOT NULL,
    AppointmentID     INT NULL,
    VisitDate         DATE NOT NULL,
    Diagnosis         NVARCHAR(300) NULL,
    Notes             NVARCHAR(500) NULL,
    CONSTRAINT FK_History_Patients     FOREIGN KEY (PatientID) REFERENCES Patients(PatientID),
    CONSTRAINT FK_History_Doctors      FOREIGN KEY (DoctorID)  REFERENCES Doctors(DoctorID),
    CONSTRAINT FK_History_Appointments FOREIGN KEY (AppointmentID) REFERENCES Appointments(AppointmentID)
);
GO

-- 5. Prescriptions
CREATE TABLE Prescriptions (
    PrescriptionID    INT IDENTITY(1,1) PRIMARY KEY,
    AppointmentID     INT NOT NULL,
    PatientID         INT NOT NULL,
    DoctorID          INT NOT NULL,
    Medicines         NVARCHAR(500) NOT NULL,
    Dosage            NVARCHAR(200) NULL,
    DateIssued        DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_Prescriptions_Appointments FOREIGN KEY (AppointmentID) REFERENCES Appointments(AppointmentID),
    CONSTRAINT FK_Prescriptions_Patients     FOREIGN KEY (PatientID) REFERENCES Patients(PatientID),
    CONSTRAINT FK_Prescriptions_Doctors      FOREIGN KEY (DoctorID)  REFERENCES Doctors(DoctorID)
);
GO

-- 6. Bills
CREATE TABLE Bills (
    BillID            INT IDENTITY(1,1) PRIMARY KEY,
    PatientID         INT NOT NULL,
    AppointmentID     INT NOT NULL,
    Amount            DECIMAL(10,2) NOT NULL,
    PaymentStatus     NVARCHAR(20) NOT NULL DEFAULT 'Unpaid',
    PaymentMethod     NVARCHAR(30) NULL,
    BillDate          DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_Bills_Patients     FOREIGN KEY (PatientID) REFERENCES Patients(PatientID),
    CONSTRAINT FK_Bills_Appointments FOREIGN KEY (AppointmentID) REFERENCES Appointments(AppointmentID)
);
GO

-- Helpful indexes
CREATE INDEX IX_Patients_Name ON Patients(LastName, FirstName);
CREATE INDEX IX_Patients_Contact ON Patients(ContactNumber);
CREATE INDEX IX_Appointments_Date ON Appointments(AppointmentDate);
GO

-- Optional sample seed data
INSERT INTO Doctors (FullName, Specialization, ContactNumber, Email, AvailableDays, ConsultationFee)
VALUES
('Dr. Anil Shrestha', 'Cardiology', '9800000001', 'anil.shrestha@example.com', 'Mon-Fri', 1500.00),
('Dr. Priya Rai', 'Pediatrics', '9800000002', 'priya.rai@example.com', 'Mon-Sat', 1200.00);

INSERT INTO Patients (FirstName, LastName, DateOfBirth, Gender, ContactNumber, Email)
VALUES
('Ram', 'Karki', '1990-05-14', 'Male', '9811111111', 'ram.karki@example.com'),
('Sita', 'Gurung', '1985-11-02', 'Female', '9822222222', 'sita.gurung@example.com');
GO
