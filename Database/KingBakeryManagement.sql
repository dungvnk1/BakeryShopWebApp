USE [master]
GO
alter database [KingBakeryManagement] set single_user with rollback immediate

IF EXISTS (SELECT * FROM sys.databases WHERE name = 'KingBakeryManagement')
	DROP DATABASE KingBakeryManagement
GO

CREATE DATABASE KingBakeryManagement
GO

USE KingBakeryManagement
GO

CREATE TABLE Users (
	ID INT IDENTITY(1,1) PRIMARY KEY,
	FullName NVARCHAR(100) NOT NULL,
	Username NVARCHAR(100) NOT NULL,
	Password NVARCHAR(100) NOT NULL,
	Address NVARCHAR(200),
	BirthDate DateTime NOT NULL,
	Email NVARCHAR(100),
    PhoneNumber NVARCHAR(100) NOT NULL,
	Role INT NOT NULL
)
GO

CREATE TABLE Employee (
	UserID INT PRIMARY KEY,
	Salary FLOAT NOT NULL,
	HiredDate DateTime NOT NULL,
	Status NVARCHAR(100) NOT NULL,
	FOREIGN KEY (UserID) REFERENCES Users(ID) on delete cascade
)
GO

CREATE TABLE Customer (
	UserID INT PRIMARY KEY,
	Ranking NVARCHAR(10) NOT NULL,
	FOREIGN KEY (UserID) REFERENCES Users(ID) ON DELETE CASCADE
)
GO

CREATE TABLE Category(
	ID INT Identity(1,1) PRIMARY KEY,
	Name NVARCHAR(255)
)
GO

CREATE TABLE BakeryDetail(
	ID INT Identity(1,1) PRIMARY KEY,
	Size INT,
	Quantity INT,
	Price FLOAT,
	Rating FLOAT,
	Discount INT
)
GO

CREATE TABLE Bakery(
	BakeryID INT PRIMARY KEY,
	Name NVARCHAR(255),
	Image VARCHAR(255),
	Description NVARCHAR(4000),
	CategoryID INT, 
	
	FOREIGN KEY (BakeryID) REFERENCES BakeryDetail(ID) ON DELETE CASCADE,
	FOREIGN KEY (CategoryID) REFERENCES Category(ID) ON DELETE CASCADE
)
GO

CREATE TABLE Vouchers(
	VoucherID INT Identity(1,1) PRIMARY KEY,
	Code VARCHAR(255),
	VPercent INT
)
GO

CREATE TABLE Orders(
	ID INT PRIMARY KEY,
	CustomerID INT,
	StaffID INT,
	ShipperID INT,
	VoucherID INT,
	DateTime DATETIME,
	AdrDelivery NVARCHAR(300),
	TotalPrice FLOAT,
	Status NVARCHAR(100),
	FOREIGN KEY (CustomerID) REFERENCES Customer(UserID) ON DELETE CASCADE,
	FOREIGN KEY (StaffID) REFERENCES Employee(UserID),
	FOREIGN KEY (ShipperID) REFERENCES Employee(UserID),
	FOREIGN KEY (VoucherID) REFERENCES Vouchers(VoucherID) ON DELETE CASCADE
)
GO

CREATE TABLE OrderItem(
	ID INT Identity(1,1) PRIMARY KEY,
	BakeryID INT,
	BillID INT,
	Quantity INT,
	Price FLOAT,
	FOREIGN KEY (BakeryID) REFERENCES BakeryDetail(ID) ON DELETE CASCADE,
	FOREIGN KEY (BillID) REFERENCES Orders(ID) ON DELETE CASCADE
)
GO

CREATE TABLE Feedback(
	ID INT Identity(1,1) PRIMARY KEY,
	CustomerID INT,
	BakeryID INT,
	ContentFB NVARCHAR(4000),
	FOREIGN KEY (CustomerID) REFERENCES Customer(UserID) ON DELETE CASCADE,
	FOREIGN KEY (BakeryID) REFERENCES BakeryDetail(ID) ON DELETE CASCADE
)
GO

CREATE TABLE Favourite(
	CustomerID INT,
	BakeryID INT,
	PRIMARY KEY(CustomerID,BakeryID),
	Foreign key (CustomerID) references Customer(UserID) on delete cascade,
	Foreign key (BakeryID) references BakeryDetail(ID) on delete cascade
)
GO


-----INSERT DATA-----
--Users
INSERT INTO Users(FullName,UserName,Password,Address,BirthDate,Email,PhoneNumber,Role)
VALUES    
	(N'Mạnh Hùng',N'hung123',N'123',NULL,'2004-01-08',NULL,'0123456789',2),  --*Role: 1_admin,2_cus,3_staff,4_shipper
	(N'Năng Dũng',N'dung123',N'123',NULL,'2004-05-12','dung@gmail.com','0123456789',1),
	(N'Chử Hồng Phúc',N'hongphuc',N'123',NULL,'2004-05-12',NULL,'0123456789',3),
	(N'Lê Trường Sơn',N'sonle123',N'123','81 QL21','2004-011-12','sonle@gmail.com','0987654321',3)
GO

--Employee
INSERT INTO Employee(UserID,Salary,HiredDate,Status)
VALUES
	(3,3000000,'2024-04-24',N'Đang làm việc'),
	(4,2500000,'2024-05-01',N'Đang nghỉ')
GO

--Customer
INSERT INTO Customer(UserID,Ranking)
VALUES
	(1,N'Đồng')
GO

--Category
INSERT INTO Category(Name)
VALUES
	(N'Bánh mì'),
	(N'Bánh ngọt'),
	(N'Bánh sinh nhật'),
	(N'Bánh quy')
GO

--BakeryDetail
INSERT INTO BakeryDetail(Size,Quantity,Price,Rating,Discount)
VALUES
	(28,5,200000,0,10),
	(30,6,250000,0,5),
	(5,10,50000,0,0)
GO

--Bakery
INSERT INTO Bakery(BakeryID,Name,Image,Description,CategoryID)
VALUES   --Image tạm thời để NULL hết nhé
	(1,N'Bánh kem Socola',NULL,N'Bánh sinh nhật phủ Socola và nhân chanh leo phù hợp với mọi lứa tuổi...',3),
	(2,N'Bánh kem Socola',NULL,N'Bánh sinh nhật phủ Socola và nhân chanh leo phù hợp với mọi lứa tuổi...',3),
	(3,N'Bánh quy nho khô',NULL,N'Món ăn vặt hấp dẫn...',4)
GO

--Vouchers
INSERT INTO Vouchers(Code,VPercent)
VALUES   
	('QUATANG55',15),
	('QUATANG66',10)
GO

--Favourite
INSERT INTO Favourite(CustomerID,BakeryID)
VALUES   
	(1,1),
	(1,3)
GO