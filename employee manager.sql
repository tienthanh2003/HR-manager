create database EmployeeManagement
Use EmployeeManagement

drop database  EmployeeManagement

CREATE TABLE Departments (
    DepartmentID INT PRIMARY KEY IDENTITY(1,1),
    DepartmentName NVARCHAR(100) NOT NULL,
	NumberOfEmployees INT NOT NULL DEFAULT 0,
	Status NVARCHAR(20) NOT NULL DEFAULT 'Active' CHECK (Status IN ('Active', 'Inactive'))
);

CREATE TABLE Employees (
    EmployeeID INT PRIMARY KEY IDENTITY(1,1),
    FullName NVARCHAR(100) NOT NULL,
    DateOfBirth DATE NOT NULL,
    Gender NVARCHAR(10) NOT NULL,
    Address NVARCHAR(255),
    PhoneNumber NVARCHAR(15),
    DepartmentID INT FOREIGN KEY REFERENCES Departments(DepartmentID),
    Position NVARCHAR(50),
    Salary DECIMAL(18, 2),
    StartDate DATE,
    Avatar VARBINARY(MAX),
	RemainingLeaveDays INT NOT NULL DEFAULT 5
);

CREATE TABLE Users (
    UserID INT PRIMARY KEY IDENTITY(1,1),
    Username NVARCHAR(50) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(255) NOT NULL,
    Role NVARCHAR(20) NOT NULL CHECK (Role IN ('Admin', 'Employee')),
    EmployeeID INT FOREIGN KEY REFERENCES Employees(EmployeeID)  ON DELETE CASCADE
);

CREATE TABLE Salaries (
    SalaryID INT PRIMARY KEY IDENTITY(1,1),
    EmployeeID INT FOREIGN KEY REFERENCES Employees(EmployeeID) ON DELETE SET NULL,
    BaseSalary DECIMAL(18, 2) NOT NULL,
    Allowance DECIMAL(18, 2) NOT NULL,
    Bonus DECIMAL(18, 2) NOT NULL,
    Deduction DECIMAL(18, 2) NOT NULL,
    TotalSalary DECIMAL(18, 2), -- Computed Column
    StartDate DATE NOT NULL,
    PaymentDate DATE NOT NULL,
    SalaryStatus NVARCHAR(60) NOT NULL DEFAULT 'Pending' CHECK (SalaryStatus IN ('Pending', 'Complete'))
);

CREATE TABLE Attendance (
    AttendanceID INT PRIMARY KEY IDENTITY(1,1),
    EmployeeID INT FOREIGN KEY REFERENCES Employees(EmployeeID)  ON DELETE SET NULL,
    Date DATE NOT NULL,
    CheckInTime TIME,
    CheckOutTime TIME,
    OvertimeHours DECIMAL(5, 2),
	AttendStatus Nvarchar(250)  CHECK (AttendStatus IN ('Absent', 'Attend'))
);

CREATE TABLE Leaves (
    LeaveID INT PRIMARY KEY IDENTITY(1,1),
    EmployeeID INT FOREIGN KEY REFERENCES Employees(EmployeeID)  ON DELETE SET NULL,
    LeaveType NVARCHAR(50) NOT NULL,
    StartDate DATE NOT NULL,
    EndDate DATE NOT NULL,
    Status NVARCHAR(20) NOT NULL
);

CREATE TABLE Notifications (
    NotificationID INT PRIMARY KEY IDENTITY(1,1),
    Title NVARCHAR(100) NOT NULL,
    Content NVARCHAR(MAX) NOT NULL,
    SenderID INT FOREIGN KEY REFERENCES Users(UserID) ON DELETE SET NULL ,
    IsBroadcast BIT NOT NULL DEFAULT 0, -- 0: G?i d?n phòng ban c? th?, 1: G?i d?n t?t c? nhân viên
    SentDate DATETIME NOT NULL,
	EndDate DATETIME not null
);

CREATE TABLE ActivityLogs (
    LogID INT PRIMARY KEY IDENTITY(1,1),
    UserID INT FOREIGN KEY REFERENCES Users(UserID) ON DELETE SET NULL,
    Action NVARCHAR(100) NOT NULL,
    ActionDate DATETIME NOT NULL,
    Details NVARCHAR(MAX)
);
CREATE TABLE NotificationDepartments (
    NotificationDepartmentID INT PRIMARY KEY IDENTITY(1,1),
    NotificationID INT FOREIGN KEY REFERENCES Notifications(NotificationID) ON DELETE CASCADE,
    DepartmentID INT FOREIGN KEY REFERENCES Departments(DepartmentID) ON DELETE CASCADE
);

INSERT INTO Departments (DepartmentName)
VALUES 
('Human Resources'),
('IT'),
('Finance'),
('Marketing'),
('Sales');

INSERT INTO Employees (FullName, DateOfBirth, Gender, Address, PhoneNumber, DepartmentID, Position, Salary, StartDate)
VALUES 
('Nguyen Van A', '1985-05-15', 'Male', '123 Nguyen Trai, Ha Noi, VN', '0986754367', 1, 'HR Manager', 20000000.00, '2010-06-01'),
('Phan Thi B', '1990-08-25', 'Female', '456 Tran Thai Tong, Ha Noi, VN', '0768549836', 2, 'Software Developer', 20000000.00, '2015-07-15'),
('Tran Ha Anh', '1988-12-10', 'Female', '789 Nguyen Thai Hoc, Ha Noi, VN', '0965473528', 3, 'Financial Analyst', 15000000.00, '2012-03-20'),
('Tran Van D', '1992-03-30', 'Male', '321 Truong Trinh, Ha Noi, VN', '0346238564', 4, 'Marketing Specialist', 10000000.00, '2018-09-10'),
('Do Tien C', '1995-07-22', 'Male', '654 Lang, Ha Noi, VN', '0352718574', 5, 'Sales Representative', 8000000.00, '2019-11-05');



INSERT INTO Users (Username, PasswordHash, Role, EmployeeID)
VALUES 
('admin', 'admin', 'Admin', NULL), -- Admin không ph?i là nhân viên
('nguyenvana', '111', 'Employee', 1),   
('phanthib', '222', 'Employee', 2), 
('tranhaanh', '333', 'Employee', 3), 
('tranvand', '444', 'Employee', 4),  
('dotienc', '555', 'Employee', 5); 

INSERT INTO Salaries (EmployeeID, BaseSalary, Allowance, Bonus, Deduction, StartDate, PaymentDate, TotalSalary)
VALUES 
-- Nhân viên 1: Nguyen Van A
(1, 20000000.00, 5000000.00, 2000000.00, 1000000.00, '2025-03-02', '2025-04-01', 26000000.00),

-- Nhân viên 2: Phan Thi B
(2, 20000000.00, 3000000.00, 1500000.00, 800000.00, '2025-03-02', '2025-04-01', 23700000.00),

-- Nhân viên 3: Tran Ha Anh
(3, 15000000.00, 2000000.00, 1000000.00, 500000.00,  '2025-03-02', '2025-04-01', 17500000.00),

-- Nhân viên 4: Tran Van D
(4, 10000000.00, 1000000.00, 500000.00, 300000.00, '2025-03-02', '2025-04-01', 11200000.00),


-- Nhân viên 5: Do Tien C
(5, 8000000.00, 800000.00, 300000.00, 200000.00, '2025-03-02', '2025-04-01', 8900000.00);

INSERT INTO Attendance (EmployeeID, Date, CheckInTime, CheckOutTime, OvertimeHours, AttendStatus)
VALUES 
(1, '2025-03-01', '08:00:00', '17:00:00', 0.0, 'Attend'),
(2, '2025-03-01', '09:00:00', '18:00:00', 1.5, 'Attend'),
(3, '2025-03-01', '08:30:00', '17:30:00', 0.5, 'Attend'),
(4, '2025-03-01', '08:00:00', '16:30:00', 0.0, 'Attend'),
(5, '2025-03-01', '09:30:00', '19:00:00', 2.0, 'Attend');

INSERT INTO Leaves (EmployeeID, LeaveType, StartDate, EndDate, Status)
VALUES 
(1, 'Vacation', '2023-10-10', '2023-10-15', 'Approved'),
(2, 'Sick Leave', '2023-10-05', '2023-10-07', 'Approved'),
(3, 'Leave without pay', '2023-11-01', '2024-02-01', 'Pending'),
(4, 'Vacation', '2023-12-20', '2023-12-31', 'Pending'),
(5, 'Sick Leave', '2023-10-03', '2023-10-04', 'Approved');

INSERT INTO Notifications (Title, Content, SenderID, IsBroadcast, SentDate, EndDate)
VALUES 
('Company Meeting', 'All employees are required to attend the company meeting on 2023-10-15.', 1, 1, '2023-10-01 10:00:00', '2023-10-15 10:00:00'), 
('IT Maintenance', 'There will be a scheduled IT maintenance on 2023-10-10 from 10 PM to 2 AM.', 2, 0, '2023-10-02 09:00:00', '2023-10-10 02:00:00'); 

INSert Into NotificationDepartments(NotificationID, DepartmentID) Values(2,2)
UPDATE Users
SET PasswordHash = HASHBYTES('SHA2_256', '111') -- M?t kh?u g?c là '111'
WHERE Username = 'nguyenvana';
UPDATE Users
SET PasswordHash = HASHBYTES('SHA2_256', 'admin') -- M?t kh?u g?c là '111'
WHERE Username = 'admin';
UPDATE Users
SET PasswordHash = HASHBYTES('SHA2_256', '222') -- M?t kh?u g?c là '222'
WHERE Username = 'phanthib';

UPDATE Users
SET PasswordHash = HASHBYTES('SHA2_256', '333') -- M?t kh?u g?c là '333'
WHERE Username = 'tranhaanh';

UPDATE Users
SET PasswordHash = HASHBYTES('SHA2_256', '444') -- M?t kh?u g?c là '444'
WHERE Username = 'tranvand';

UPDATE Users
SET PasswordHash = HASHBYTES('SHA2_256', '555') -- M?t kh?u g?c là '555'
WHERE Username = 'dotienc';

select * from Salaries
select *from Departments
select * from Employees


UPDATE Users SET PasswordHash = CONVERT(NVARCHAR(64), HASHBYTES('SHA2_256', 'admin'), 2) WHERE Username = 'admin';
UPDATE Users SET PasswordHash = CONVERT(NVARCHAR(64), HASHBYTES('SHA2_256', '111'), 2) WHERE Username = 'nguyenvana';
UPDATE Users SET PasswordHash = CONVERT(NVARCHAR(64), HASHBYTES('SHA2_256', '222'), 2) WHERE Username = 'phanthib';
UPDATE Users SET PasswordHash = CONVERT(NVARCHAR(64), HASHBYTES('SHA2_256', '333'), 2) WHERE Username = 'tranhaanh';
UPDATE Users SET PasswordHash = CONVERT(NVARCHAR(64), HASHBYTES('SHA2_256', '444'), 2) WHERE Username = 'tranvand';
UPDATE Users SET PasswordHash = CONVERT(NVARCHAR(64), HASHBYTES('SHA2_256', '555'), 2) WHERE Username = 'dotienc';


CREATE TRIGGER trg_AfterInsertEmployee
ON Employees
AFTER INSERT
AS
BEGIN
    -- C?p nh?t s? lu?ng nhân viên trong phòng ban khi thêm nhân viên m?i
    UPDATE Departments
    SET NumberOfEmployees = NumberOfEmployees + 1
    WHERE DepartmentID IN (SELECT DepartmentID FROM inserted);
END;

CREATE TRIGGER trg_AfterDeleteEmployee
ON Employees
AFTER DELETE
AS
BEGIN
    -- C?p nh?t s? lu?ng nhân viên trong phòng ban khi xóa nhân viên
    UPDATE Departments
    SET NumberOfEmployees = NumberOfEmployees - 1
    WHERE DepartmentID IN (SELECT DepartmentID FROM deleted);
END;

CREATE TRIGGER trg_AfterUpdateEmployee
ON Employees
AFTER UPDATE
AS
BEGIN
    -- C?p nh?t s? lu?ng nhân viên trong phòng ban cu (n?u có thay d?i phòng ban)
    UPDATE Departments
    SET NumberOfEmployees = NumberOfEmployees - 1
    WHERE DepartmentID IN (SELECT DepartmentID FROM deleted);

    -- C?p nh?t s? lu?ng nhân viên trong phòng ban m?i (n?u có thay d?i phòng ban)
    UPDATE Departments
    SET NumberOfEmployees = NumberOfEmployees + 1
    WHERE DepartmentID IN (SELECT DepartmentID FROM inserted);
END;

UPDATE Departments
SET NumberOfEmployees = (SELECT COUNT(*) FROM Employees WHERE Employees.DepartmentID = Departments.DepartmentID);



ALTER TABLE Notifications
ADD DepartmentID INT NULL,
    EndDate DATETIME NULL;


-- Thêm ràng buộc khóa ngoại cho DepartmentID
ALTER TABLE Notifications
ADD CONSTRAINT FK_Notifications_Departments
FOREIGN KEY (DepartmentID) REFERENCES Departments(DepartmentID);



Select * from Users
select * from Notifications
select * from NotificationDepartments
select * from Salaries
select *from Attendance

CREATE PROCEDURE ResetRemainingLeaveDays
AS
BEGIN
    UPDATE Employees
    SET RemainingLeaveDays = 5; -- Reset về 5 ngày
END

Drop Procedure ResetRemainingLeaveDays

SELECT * 
FROM Leaves



INSERT INTO Attendance (EmployeeID, Date, CheckInTime, CheckOutTime, OvertimeHours, AttendStatus)
VALUES 
-- Ngày 08/03/2025
(1, '2025-03-08', '08:00:00', '17:00:00', 1.0, 'Attend'), -- 9h làm -> tăng ca 1h
(2, '2025-03-08', '09:00:00', '18:00:00', 1.0, 'Attend'), -- 9h làm -> tăng ca 1h
(3, '2025-03-08', NULL, NULL, 0.0, 'Absent'),
(4, '2025-03-08', '08:00:00', '16:30:00', 0.0, 'Attend'), -- 8.5h làm -> không tính tăng ca
(5, '2025-03-08', '09:30:00', '19:00:00', 1.5, 'Attend'); -- 9.5h làm -> tăng ca 1.5h
INSERT INTO Attendance (EmployeeID, Date, CheckInTime, CheckOutTime, OvertimeHours, AttendStatus)
VALUES 
-- Ngày 09/03/2025
(1, '2025-03-09', NULL, NULL, 0.0, 'Absent'),
(2, '2025-03-09', '09:00:00', '18:00:00', 1.0, 'Attend'),
(3, '2025-03-09', '08:30:00', '17:30:00', 1.0, 'Attend'), -- 9h làm -> tăng ca 1h
(4, '2025-03-09', NULL, NULL, 0.0, 'Absent'),
(5, '2025-03-09', '09:30:00', '19:00:00', 1.5, 'Attend'),

-- Ngày 10/03/2025
(1, '2025-03-10', '08:00:00', '17:00:00', 1.0, 'Attend'),
(2, '2025-03-10', NULL, NULL, 0.0, 'Absent'),
(3, '2025-03-10', '08:30:00', '17:30:00', 1.0, 'Attend'),
(4, '2025-03-10', '08:00:00', '16:30:00', 0.0, 'Attend'),
(5, '2025-03-10', NULL, NULL, 0.0, 'Absent'),

-- Ngày 11/03/2025
(1, '2025-03-11', NULL, NULL, 0.0, 'Absent'),
(2, '2025-03-11', '09:00:00', '18:00:00', 1.0, 'Attend'),
(3, '2025-03-11', '08:30:00', '17:30:00', 1.0, 'Attend'),
(4, '2025-03-11', NULL, NULL, 0.0, 'Absent'),
(5, '2025-03-11', '09:30:00', '19:00:00', 1.5, 'Attend'),

-- Ngày 12/03/2025
(1, '2025-03-12', '08:00:00', '17:00:00', 1.0, 'Attend'),
(2, '2025-03-12', NULL, NULL, 0.0, 'Absent'),
(3, '2025-03-12', '08:30:00', '17:30:00', 1.0, 'Attend'),
(4, '2025-03-12', '08:00:00', '16:30:00', 0.0, 'Attend'),
(5, '2025-03-12', NULL, NULL, 0.0, 'Absent'),

-- Ngày 13/03/2025
(1, '2025-03-13', NULL, NULL, 0.0, 'Absent'),
(2, '2025-03-13', '09:00:00', '18:00:00', 1.0, 'Attend'),
(3, '2025-03-13', '08:30:00', '17:30:00', 1.0, 'Attend'),
(4, '2025-03-13', NULL, NULL, 0.0, 'Absent'),
(5, '2025-03-13', '09:30:00', '19:00:00', 1.5, 'Attend'),

-- Ngày 14/03/2025
(1, '2025-03-14', '08:00:00', '17:00:00', 1.0, 'Attend'),
(2, '2025-03-14', NULL, NULL, 0.0, 'Absent'),
(3, '2025-03-14', '08:30:00', '17:30:00', 1.0, 'Attend'),
(4, '2025-03-14', '08:00:00', '16:30:00', 0.0, 'Attend'),
(5, '2025-03-14', NULL, NULL, 0.0, 'Absent'),

-- Ngày 15/03/2025
(1, '2025-03-15', NULL, NULL, 0.0, 'Absent'),
(2, '2025-03-15', '09:00:00', '18:00:00', 1.0, 'Attend'),
(3, '2025-03-15', '08:30:00', '17:30:00', 1.0, 'Attend'),
(4, '2025-03-15', NULL, NULL, 0.0, 'Absent'),
(5, '2025-03-15', '09:30:00', '19:00:00', 1.5, 'Attend'),

-- Ngày 16/03/2025
(1, '2025-03-16', '08:00:00', '17:00:00', 1.0, 'Attend'),
(2, '2025-03-16', NULL, NULL, 0.0, 'Absent'),
(3, '2025-03-16', '08:30:00', '17:30:00', 1.0, 'Attend'),
(4, '2025-03-16', '08:00:00', '16:30:00', 0.0, 'Attend'),
(5, '2025-03-16', NULL, NULL, 0.0, 'Absent');


INSERT INTO Attendance (EmployeeID, Date, CheckInTime, CheckOutTime, OvertimeHours, AttendStatus)
VALUES 
-- Ngày 02/03/2025
(1, '2025-03-02', '08:00:00', '17:00:00', 1.0, 'Attend'), -- 9 giờ làm, nhưng nghỉ 1 giờ => 8 giờ => Không có OT
(2, '2025-03-02', NULL, NULL, 0.0, 'Absent'), 
(3, '2025-03-02', '08:30:00', '17:30:00', 1.0, 'Attend'), -- 9 giờ làm => OT = 1
(4, '2025-03-02', '08:00:00', '16:30:00', 0.5, 'Attend'), -- 8.5 giờ nhưng nghỉ 1 giờ => 7.5 giờ => Không có OT
(5, '2025-03-02', NULL, NULL, 0.0, 'Absent'), 

-- Ngày 03/03/2025
(1, '2025-03-03', NULL, NULL, 0.0, 'Absent'), 
(2, '2025-03-03', '09:00:00', '18:00:00', 1.0, 'Attend'), -- 9 giờ làm => OT = 1
(3, '2025-03-03', '08:30:00', '17:30:00', 1.0, 'Attend'), 
(4, '2025-03-03', NULL, NULL, 0.0, 'Absent'), 
(5, '2025-03-03', '09:30:00', '19:00:00', 1.5, 'Attend'), -- 9.5 giờ làm => OT = 1.5

-- Ngày 04/03/2025
(1, '2025-03-04', '08:00:00', '17:00:00', 0.0, 'Attend'), 
(2, '2025-03-04', NULL, NULL, 0.0, 'Absent'), 
(3, '2025-03-04', '08:30:00', '17:30:00', 1.0, 'Attend'), 
(4, '2025-03-04', '08:00:00', '16:30:00', 0.0, 'Attend'), 
(5, '2025-03-04', NULL, NULL, 0.0, 'Absent'), 

-- Ngày 05/03/2025
(1, '2025-03-05', NULL, NULL, 0.0, 'Absent'), 
(2, '2025-03-05', '09:00:00', '18:00:00', 1.0, 'Attend'), 
(3, '2025-03-05', '08:30:00', '17:30:00', 1.0, 'Attend'), 
(4, '2025-03-05', NULL, NULL, 0.0, 'Absent'), 
(5, '2025-03-05', '09:30:00', '19:00:00', 1.5, 'Attend'), 

-- Ngày 06/03/2025
(1, '2025-03-06', '08:00:00', '17:00:00', 0.0, 'Attend'), 
(2, '2025-03-06', NULL, NULL, 0.0, 'Absent'), 
(3, '2025-03-06', '08:30:00', '17:30:00', 1.0, 'Attend'), 
(4, '2025-03-06', '08:00:00', '16:30:00', 0.0, 'Attend'), 
(5, '2025-03-06', NULL, NULL, 0.0, 'Absent'), 

-- Ngày 07/03/2025
(1, '2025-03-07', NULL, NULL, 0.0, 'Absent'), 
(2, '2025-03-07', '09:00:00', '18:00:00', 1.0, 'Attend'), 
(3, '2025-03-07', '08:30:00', '17:30:00', 1.0, 'Attend'), 
(4, '2025-03-07', NULL, NULL, 0.0, 'Absent'), 
(5, '2025-03-07', '09:30:00', '19:00:00', 1.5, 'Attend'), 

-- Ngày 17/03/2025
(1, '2025-03-17', '08:00:00', '17:00:00', 0.0, 'Attend'), 
(2, '2025-03-17', '09:00:00', '18:00:00', 1.0, 'Attend'), 
(3, '2025-03-17', NULL, NULL, 0.0, 'Absent'), 
(4, '2025-03-17', '08:00:00', '16:30:00', 0.0, 'Attend'), 
(5, '2025-03-17', '09:30:00', '19:00:00', 1.5, 'Attend'); 

INSERT INTO Salaries (EmployeeID, BaseSalary, Allowance, Bonus, Deduction, StartDate, PaymentDate, TotalSalary)
VALUES 
-- Tháng 1/2025
(1, 20000000.00, 5000000.00, 2000000.00, 1000000.00, '2025-01-02', '2025-02-01', 26000000.00),
(2, 20000000.00, 3000000.00, 1500000.00, 800000.00, '2025-01-02', '2025-02-01', 23700000.00),
(3, 15000000.00, 2000000.00, 1000000.00, 500000.00, '2025-01-02', '2025-02-01', 17500000.00),
(4, 10000000.00, 1000000.00, 500000.00, 300000.00, '2025-01-02', '2025-02-01', 11200000.00),
(5, 8000000.00, 800000.00, 300000.00, 200000.00, '2025-01-02', '2025-02-01', 8900000.00);
INSERT INTO Salaries (EmployeeID, BaseSalary, Allowance, Bonus, Deduction, StartDate, PaymentDate, TotalSalary)
VALUES 
-- Tháng 2/2025
(1, 20000000.00, 5000000.00, 2000000.00, 1000000.00, '2025-02-02', '2025-03-01', 26000000.00),
(2, 20000000.00, 3000000.00, 1500000.00, 800000.00, '2025-02-02', '2025-03-01', 23700000.00),
(3, 15000000.00, 2000000.00, 1000000.00, 500000.00, '2025-02-02', '2025-03-01', 17500000.00),
(4, 10000000.00, 1000000.00, 500000.00, 300000.00, '2025-02-02', '2025-03-01', 11200000.00),
(5, 8000000.00, 800000.00, 300000.00, 200000.00, '2025-02-02', '2025-03-01', 8900000.00);

   UPDATE Salaries 
SET SalaryStatus = 'Complete' 
WHERE PaymentDate < '2025-03-02';

