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
INSERT INTO Users(FullName,UserName,Password,Address,BirthDate,Email,PhoneNumber,Role)--*Role: 1_admin,2_cus,3_staff,4_shipper
VALUES    

	   (N'Nguyễn Thị C', N'nguyenthic', N'passwordC', N'456 Nguyen Trai', '2002-07-15', 'nguyenthic@example.com', '0923456789', 2),
    (N'Hoàng Văn D', N'hoangvand', N'passwordD', NULL, '2001-06-30', 'hoangvand@example.com', '0934567890', 1),
    (N'Lê Thị E', N'lethie', N'passwordE', N'789 Tran Hung Dao', '2000-05-25', 'lethie@example.com', '0945678901', 3),
    (N'Phạm Văn F', N'phamvanf', N'passwordF', N'12 Pham Ngoc Thach', '1999-04-01', 'phamvanf@example.com', '0956789012', 4),
    (N'Nguyễn Văn G', N'nguyenvang', N'passwordG', NULL, '1998-03-15', 'nguyenvang@example.com', '0967890123', 2),
    (N'Trần Thị H', N'tranthih', N'passwordH', N'34 Tran Quoc Toan', '1997-02-20', 'tranthih@example.com', '0978901234', 3),
    (N'Đỗ Minh I', N'dominhi', N'passwordI', N'56 Ly Thuong Kiet', '1996-01-10', 'dominhi@example.com', '0989012345', 1);
GO
select * from Users
--Employee
INSERT INTO Employee(UserID,Salary,HiredDate,Status)
VALUES
	 (6, 3000000, '2024-04-24', N'Đang làm việc'),
    (7, 2500000, '2024-05-01', N'Đang nghỉ'),
    (8, 3500000, '2024-03-15', N'Đang làm việc'),
    (9, 2800000, '2024-02-20', N'Đang nghỉ'),
    (10, 4000000, '2024-01-10', N'Đang làm việc'),
    (11, 2200000, '2024-04-01', N'Đang nghỉ'),
    (12, 3200000, '2024-03-01', N'Đang làm việc'),
    (13, 2600000, '2024-02-15', N'Đang nghỉ');
GO

--Customer
INSERT INTO Customer(UserID,Ranking)
VALUES
	
	(2,N'Bạc '),
	(3,N'Vàng'),
	(4,N'Kim cương')
GO


--Category
INSERT INTO Category(Name)
VALUES
	(N'Bánh mì'),
	(N'Bánh ngọt'),
	(N'Bánh sinh nhật'),
	(N'Bánh quy'),
	(N'Bánh kem'),
    (N'Bánh trung thu'),
    (N'Bánh gạo lứt'),
    (N'Bánh bao'),
    (N'Bánh đậu xanh'),
    (N'Bánh gạo nếp');
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
	(1,2)
GO

--Feedback
INSERT INTO Feedback(CustomerID,BakeryID,ContentFB)
VALUES 
( 1, 1, 'Bánh rất ngon và phục vụ thân thiện'),
( 1, 2, 'Không gian quán rất ấm cúng, bánh mì tươi ngon'),
( 1, 3, 'Dịch vụ giao hàng nhanh, bánh đến nơi vẫn còn nóng')
GO
--OrderItem
INSERT INTO OrderItem ( BakeryID, BillID, Quantity, Price) VALUES
( 1, 1, 2, 50.00),
( 2, 2, 1, 30.00),
( 2, 3, 5, 15.00)
GO

--Orders
INSERT INTO Orders (ID, CustomerID, StaffID, ShipperID, VoucherID, DateTime, AdrDelivery, TotalPrice, Status) VALUES
(1, 1, 3, 3, 1, '2024-05-01 10:30:00', '123 Đường ABC, Quận 1, TP HCM', 150.00, 'Completed'),
(2, 1, 4, 4, 2, '2024-05-02 11:00:00', '456 Đường DEF, Quận 2, TP HCM', 200.00, 'Pending'),
(3, 1, 3, 3, 1, '2024-05-03 12:15:00', '789 Đường GHI, Quận 3, TP HCM', 75.00, 'Shipped')
GO

