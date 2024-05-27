﻿USE [master]
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
	BirthDate Date NOT NULL,
	Email NVARCHAR(100),
    PhoneNumber NVARCHAR(100) NOT NULL,
	Role INT NOT NULL
)
GO

CREATE TABLE Employee (
	UserID INT PRIMARY KEY,
	Salary FLOAT NOT NULL,
	HiredDate Date NOT NULL,
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
	Name NVARCHAR(255) NOT NULL
)
GO

CREATE TABLE Bakery(
	ID INT Identity(1,1) PRIMARY KEY,
	Name NVARCHAR(255) NOT NULL,
	Image VARCHAR(255),
	Description NVARCHAR(4000),
	CategoryID INT NOT NULL,
	
	FOREIGN KEY (CategoryID) REFERENCES Category(ID) ON DELETE CASCADE
)
GO

CREATE TABLE BakeryOption(
	ID INT Identity(1,1) PRIMARY KEY,
	Size INT,
	Quantity INT,
	Price FLOAT,
	Rating FLOAT,
	Discount INT,
	BakeryID INT NOT NULL,
	
	FOREIGN KEY (BakeryID) REFERENCES Bakery(ID) ON DELETE CASCADE
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
	FOREIGN KEY (BakeryID) REFERENCES BakeryOption(ID) ON DELETE CASCADE,
	FOREIGN KEY (BillID) REFERENCES Orders(ID) ON DELETE CASCADE
)
GO

CREATE TABLE Feedback(
	ID INT Identity(1,1) PRIMARY KEY,
	CustomerID INT,
	BakeryID INT,
	ContentFB NVARCHAR(4000),
	FOREIGN KEY (CustomerID) REFERENCES Customer(UserID) ON DELETE CASCADE,
	FOREIGN KEY (BakeryID) REFERENCES BakeryOption(ID) ON DELETE CASCADE
)
GO

CREATE TABLE Favourite(
	ID INT Identity(1,1) PRIMARY KEY,
	CustomerID INT,
	BakeryID INT,
	Foreign key (CustomerID) references Customer(UserID) on delete cascade,
	Foreign key (BakeryID) references BakeryOption(ID) on delete cascade
)
GO


-----INSERT DATA-----
--Users
INSERT INTO Users(FullName,UserName,Password,Address,BirthDate,Email,PhoneNumber,Role)--*Role: 1_admin,2_cus,3_staff,4_shipper
VALUES    
	(N'Mạnh Hùng',N'hung123',N'123',NULL,'2004-01-08',NULL,'0123456789',2),  --*Role: 1_admin,2_cus,3_staff,4_shipper
	(N'Năng Dũng',N'dung123',N'123',NULL,'2004-05-12','dung@gmail.com','0123456789',1),
	(N'Chử Hồng Phúc',N'hongphuc',N'123',NULL,'2004-05-12',NULL,'0123456789',3),
	(N'Lê Trường Sơn',N'sonle123',N'123','81 QL21','2004-11-12','sonle@gmail.com','0987654321',3),
	(N'Nguyễn Thị C', N'nguyenthic', N'passwordC', N'456 Nguyen Trai', '2002-07-15', 'nguyenthic@example.com', '0923456789', 2),
    (N'Hoàng Văn D', N'hoangvand', N'passwordD', NULL, '2001-06-30', 'hoangvand@example.com', '0934567890', 1),
    (N'Lê Thị E', N'lethie', N'passwordE', N'789 Tran Hung Dao', '2000-05-25', 'lethie@example.com', '0945678901', 3),
    (N'Phạm Văn F', N'phamvanf', N'passwordF', N'12 Pham Ngoc Thach', '1999-04-01', 'phamvanf@example.com', '0956789012', 4),
    (N'Nguyễn Văn G', N'nguyenvang', N'passwordG', NULL, '1998-03-15', 'nguyenvang@example.com', '0967890123', 2),
    (N'Trần Thị H', N'tranthih', N'passwordH', N'34 Tran Quoc Toan', '1997-02-20', 'tranthih@example.com', '0978901234', 3),
    (N'Đỗ Minh I', N'dominhi', N'passwordI', N'56 Ly Thuong Kiet', '1996-01-10', 'dominhi@example.com', '0989012345', 1);
GO

--Employee
INSERT INTO Employee(UserID,Salary,HiredDate,Status)
VALUES
	(3, 3000000, '2024-04-24', N'Đang làm việc'),
    (4, 2500000, '2024-05-01', N'Đang nghỉ'),
    (7, 3500000, '2024-03-15', N'Đang làm việc'),
    (8, 2800000, '2024-02-20', N'Đang nghỉ'),
    (9, 4000000, '2024-01-10', N'Đang làm việc')
GO

--Customer
INSERT INTO Customer(UserID,Ranking)
VALUES
	(1,N'Đồng'),
	(5,N'Bạc '),
	(9,N'Vàng')
GO


--Category
INSERT INTO Category(Name)
VALUES
	(N'Bánh mì'),
	(N'Bánh ngọt'),
	(N'Bánh quy'),
	(N'Bánh kem'),
    (N'Bánh trung thu'),
    (N'Bánh gạo lứt'),
    (N'Bánh bao'),
    (N'Bánh đậu xanh'),
    (N'Bánh gạo nếp');
GO

--Bakery
INSERT INTO Bakery(Name,Image,Description,CategoryID)
VALUES   --Image tạm thời để NULL hết nhé
	(N'Bánh kem Socola',NULL,N'Bánh sinh nhật phủ Socola và nhân chanh leo phù hợp với mọi lứa tuổi...',3),
	(N'Bánh quy nho khô',NULL,N'Món ăn vặt hấp dẫn...',4)
GO

--BakeryDetail
INSERT INTO BakeryOption(Size,Quantity,Price,Rating,Discount,BakeryID)
VALUES
	(28,5,200000,0,10,1),
	(30,6,250000,0,5,1),
	(5,10,50000,0,0,2)
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
( 1, 1, N'Bánh rất ngon và phục vụ thân thiện'),
( 1, 2, N'Không gian quán rất ấm cúng, bánh mì tươi ngon'),
( 1, 3, N'Dịch vụ giao hàng nhanh, bánh đến nơi vẫn còn nóng')
GO

--Orders
INSERT INTO Orders (ID, CustomerID, StaffID, ShipperID, VoucherID, DateTime, AdrDelivery, TotalPrice, Status) VALUES
(1, 1, 3, 8, NULL, '2024-05-01 10:30:00', N'123 Đường ABC, Quận 1, TP HCM', 450000, N'Đã giao hàng'),
(2, 1, 7, 8, 2, '2024-05-02 11:00:00', N'456 Đường DEF, Quận 2, TP HCM', 200.00, N'Đã đặt hàng'),
(3, 1, 3, 8, NULL, '2024-05-03 12:15:00', N'789 Đường GHI, Quận 3, TP HCM', 200000, N'Đang giao hàng')
GO

--OrderItem
INSERT INTO OrderItem ( BakeryID, BillID, Quantity, Price) VALUES
( 1, 1, 2, 400000),
( 3, 1, 1, 50000),
( 1, 3, 1, 200000)
Go

SET IDENTITY_INSERT Bakery ON;

-- Insert into the Bakery table
INSERT INTO Bakery (ID, Name, Image, Description, CategoryID) VALUES
(3, 'Apple Crepe', NULL, 'Bánh crepe ngọt có nhân táo thơm ngon và phủ kem ngọt lên trên', 2),
(4, 'Apple Danish', NULL, 'Món ăn thú vị kết hợp hương vị dễ chịu của táo và gia vị với kết cấu mềm mại, dễ chịu của bánh ngọt Đan Mạch', 2),
(6, 'Bánh Croissant', NULL, 'Lớp vỏ giòn tan và hình dáng hình lưỡi liềm. Được làm từ bột mì, men, sữa, bơ, đường và muối, croissant là một món bánh yêu thích vào bữa sáng hay bữa ăn nhẹ', 2),
(7, 'Bánh Cuộn socola', NULL, 'Bánh ngọt hấp dẫn với lớp bánh bông lan mềm mại cuộn quanh lớp nhân socola thơm ngon', 4),
(8, 'Bánh Dứa', NULL, 'Hương vị đặc trưng của dừa, với lớp bánh mềm mại kết hợp cùng lớp dừa nạo giòn ngọt hoặc dừa khô', 3),
(9, 'Bánh Mỳ Ruốc', NULL, 'Đây là loại bánh mỳ mềm, thường có nhân hoặc phủ ruốc, hương vị mặn ngọt hài hòa, với lớp bánh mềm mịn kết hợp cùng ruốc thơm ngon.', 2),
(10, 'Bánh Ốc Quế Kem', NULL, 'Một món ăn độc đáo và hấp dẫn, kết hợp giữa hai yếu tố: lớp bánh mì mềm mại và phần kem ngọt mát bên trong', 4),
(11, 'Bánh Paté chaud', NULL, 'Được làm từ lớp vỏ bánh giòn và nhân pâté thơm ngon bên trong', 1),
(12, 'Black Croffle Chocolate', NULL, 'Sự kết hợp độc đáo giữa croissant và sô cô la', 2),
(13, 'Bánh Caramel', NULL, 'Lớp bánh ẩm mịn, phủ trên cùng bởi lớp caramel sệt và thường được trang trí bằng các hạt đậu, hạt dẻ cười hoặc sô cô la.', 2),
(14, 'Bánh Choco Santa', NULL, 'Hình dạng và trang trí giống như xe quà của ông già Noel, được làm từ sô cô la và bánh ngọt.', 4),
(15, 'Bánh Crepe Xoài', NULL, 'Món ăn vừa ngon miệng, vừa hấp dẫn, kết hợp giữa chiếc crepe mềm mịn và mùi vị ngọt ngào của xoài', 2),
(16, 'Bánh Plan', NULL, 'Nước sốt caramel mịn mượt được làm từ đường và nước, lót dưới đáy khuôn, được chế biến với hỗn hợp trứng, sữa đặc có đường và đôi khi là nước cốt dừa, mang lại kết cấu béo ngậy', 2),
(17, 'Bánh Madeleines', NULL, 'Bánh nhỏ, có hình dạng và kích thước tương tự như chiếc vỏ sò,được làm từ bột mì, bơ, đường, trứng và hương vani', 2),
(18, 'NewYork Cheese Rolls', NULL, 'Bánh cuộn được làm từ bột mì mềm mịn và nhân phô mai đậm đà', 3),
(19, 'NewYork Strawberry Rolls', NULL, 'hình dạng cuộn tròn, với lớp vỏ ngoài giòn và màu vàng đẹp, bên trong là lớp nhân dâu ngọt ngào và mềm mịn', 3),
(20, 'Bánh Ruốc Cheese Croffle', NULL, 'sự kết hợp giữa hai món ăn ngon là ruốc (ruột cá) và croffle (bánh waffle được làm từ croissant),Ruốc được phối trộn với phô mai và các gia vị khác, sau đó được đặt giữa hai lớp croffle và nướng cho đến khi phô mai tan chảy và bánh croffle giòn rụm.', 1),
(21, 'Salted Egg Pastry', NULL, 'Một món ăn phổ biến trong nhiều nền văn hóa, thường được làm từ bột mì và nhân trứng muối', 1),
(22, 'Swedish Princess Pie', NULL, 'Được làm từ mứt, trứng, sữa, kem và cốt bánh bông lan, bao phủ phía trên là lớp bánh hạnh nhân (thường có màu xanh)', 2);
-- Disable IDENTITY_INSERT for Bakery
SET IDENTITY_INSERT Bakery OFF;