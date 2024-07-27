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
	BirthDate Date NOT NULL,
	Email NVARCHAR(100),
    PhoneNumber NVARCHAR(11) NOT NULL,
	Role INT NOT NULL,
	VertificationCode NVARCHAR(200),
	IsBanned INT
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
	ReleaseDate Date,
	CategoryID INT NOT NULL,
	[isDeleted] [bit] NULL,
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
	VPercent INT,
	Quantity INT,
	StartDate datetime,
	EndDate datetime,
	UserID INT,
)
GO

CREATE TABLE Orders(
	ID INT Identity(1,1) PRIMARY KEY,
	StaffID INT,
	ShipperID INT,
	VoucherID INT,
	DateTime DATETIME,
	AdrDelivery NVARCHAR(300),
	PhoneNumber NVARCHAR(100),
	Note NVARCHAR(2000),
	TotalPrice FLOAT,
	Payment VARCHAR(10),
	Status NVARCHAR(100),
	DenyReason NVARCHAR(2000),
	HasFB BIT DEFAULT 0,
	FOREIGN KEY (StaffID) REFERENCES Employee(UserID),
	FOREIGN KEY (ShipperID) REFERENCES Employee(UserID),
	FOREIGN KEY (VoucherID) REFERENCES Vouchers(VoucherID) ON DELETE CASCADE
)
GO

CREATE TABLE OrderItem(
	ID INT Identity(1,1) PRIMARY KEY,
	BakeryID INT,
	CustomerID INT,
	OrderID INT,
	Quantity INT,
	Price FLOAT,
	FOREIGN KEY (CustomerID) REFERENCES Customer(UserID) ON DELETE CASCADE,
	FOREIGN KEY (BakeryID) REFERENCES BakeryOption(ID) ON DELETE CASCADE,
	FOREIGN KEY (OrderID) REFERENCES Orders(ID) ON DELETE CASCADE
)
GO

CREATE TABLE Feedback(
	ID INT Identity(1,1) PRIMARY KEY,
	CustomerID INT,
	BakeryID INT,
	ContentFB NVARCHAR(4000),
	Time DATETIME,
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

CREATE TABLE BlogPosts (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Title NVARCHAR(50) NOT NULL,
    Content NVARCHAR(MAX) NOT NULL,
    PublishedDate DATETIME NOT NULL,
    ModifiedDate DATETIME NULL,
    Author NVARCHAR(50) NOT NULL,
    Image NVARCHAR(255) NULL
);
GO


-----INSERT DATA-----
--Users
INSERT INTO Users(FullName,UserName,Password,Address,BirthDate,Email,PhoneNumber,Role, VertificationCode, IsBanned)--*Role: 1_admin,2_cus,3_staff,4_shipper
VALUES
	(N'Phạm Mạnh Hùng',N'hung123',N'123456','Ha Noi','2004-01-08','hung080104@gmail.com','0975861472',2, '', 0),  --*Role: 1_admin,2_cus,3_staff,4_shipper
	(N'Trịnh Năng Dũng',N'dung123',N'123456','Ha Noi','2004-05-12','admin.kingbakery@gmail.com','0926553412',1, '', 0),
	(N'Chử Hồng Phúc',N'hongphuc',N'123456','Ha Noi','2004-05-12','phuc@gmail.com','0945673661',3, '', 0),
	(N'Lê Trường Sơn',N'sonle123',N'123456','81 QL21','2004-11-12','sonle@gmail.com','0966882113',4, '', 0),
	(N'Nguyễn Thị Hoa', N'nguyenthihoa', N'passwordhoa', N'456 Nguyen Trai', '2002-07-15', 'hungpmhe181830@fpt.edu.vn', '0973332441', 2, '', 0),
    (N'Hoàng Văn Đan', N'hoangvandan', N'passworddan', 'Ha Noi', '2001-06-30', 'minhdangtai2422004@gmail.com', '0934567890', 2, '', 0),
    (N'Lê Thị Lan', N'lethilan', N'passwordlan', N'789 Tran Hung Dao', '2000-05-25', 'lethilan@gmail.com', '0945678901', 3, '', 1),
    (N'Phạm Văn Minh', N'phamvanminh', N'passwordminh', N'12 Pham Ngoc Thach', '1999-04-01', 'phamvanminh@gmail.com', '0956789012', 4, '', 0),
    (N'Nguyễn Văn Dũng', N'nguyenvandung', N'passworddung', 'Ha Noi', '1998-03-15', 'dungvnhe181036@fpt.edu.vn', '0967890123', 2, '', 0),
    (N'Trần Thị Hồng', N'tranthihong', N'passwordhong', N'34 Tran Quoc Toan', '1997-02-20', 'tranthihong@gmail.com', '0978901234', 3, '', 0),
    (N'Đỗ Minh Yến', N'dominhyen', N'passwordyen', N'56 Ly Thuong Kiet', '1996-01-10', 'dominhyen@gmail.com', '0989012345', 3, '', 1)
GO
--select * from Users

--Employee
INSERT INTO Employee(UserID,Salary,HiredDate,Status)
VALUES
	(3, 3000000, '2024-04-24', N'Đang làm việc'),
    (4, 2200000, '2024-05-01', N'Đang làm việc'),
    (7, 3200000, '2024-03-15', N'Đã nghỉ'),
    (8, 2200000, '2024-02-20', N'Đang làm việc'),
    (10, 2000000, '2024-01-10', N'Đang làm việc'),
	(11, 1500000, '2024-01-10', N'Đã nghỉ')
GO

--Customer
INSERT INTO Customer(UserID,Ranking)
VALUES
	(1,N'Vàng'),
	(5,N'Bạc'),
	(6,N'Đồng'),
	(9,N'Đồng')
GO


--Category
INSERT INTO Category(Name)
VALUES
	(N'Bánh mì'),
	(N'Bánh ngọt'),
	(N'Bánh quy'),
	(N'Bánh kem'),
    (N'Bánh bao'),
    (N'Bánh gạo nếp')
GO

--Bakery
SET IDENTITY_INSERT [dbo].[Bakery] ON 

INSERT [dbo].[Bakery] ([ID], [Name], [Image], [Description], [ReleaseDate], [CategoryID], [isDeleted]) VALUES (1, N'Bánh kem Socola', N'/BakeryImg/ChocoSanta.png', N'Bánh sinh nhật phủ Socola và nhân chanh leo phù hợp với mọi lứa tuổi...', CAST(N'2024-05-29' AS Date), 4, 0)
INSERT [dbo].[Bakery] ([ID], [Name], [Image], [Description], [ReleaseDate], [CategoryID], [isDeleted]) VALUES (2, N'Bánh quy nho khô', N'/BakeryImg/BanhQuyNhoKho.png', N'Món ăn vặt hấp dẫn...', CAST(N'2024-05-29' AS Date), 3, 0)
INSERT [dbo].[Bakery] ([ID], [Name], [Image], [Description], [ReleaseDate], [CategoryID], [isDeleted]) VALUES (3, N'Apple Crepe', N'/BakeryImg/AppleCrepe.png', N'Bánh crepe ngọt có nhân táo thơm ngon và phủ kem ngọt lên trên.', CAST(N'2024-05-29' AS Date), 2, 0)
INSERT [dbo].[Bakery] ([ID], [Name], [Image], [Description], [ReleaseDate], [CategoryID], [isDeleted]) VALUES (4, N'Apple Danish', N'/BakeryImg/AppleDanish.png', N'Món ăn thú vị kết hợp hương vị dễ chịu của táo và gia vị với kết cấu mềm mại, dễ chịu của bánh ngọt Đan Mạch.', CAST(N'2024-05-29' AS Date), 2, 0)
INSERT [dbo].[Bakery] ([ID], [Name], [Image], [Description], [ReleaseDate], [CategoryID], [isDeleted]) VALUES (5, N'Bánh Croissant', N'/BakeryImg/BanhCroissant.png', N'Lớp vỏ giòn tan và hình dáng hình lưỡi liềm. Được làm từ bột mì, men, sữa, bơ, đường và muối, croissant là một món bánh yêu thích vào bữa sáng hay bữa ăn nhẹ', CAST(N'2024-05-29' AS Date), 1, 0)
INSERT [dbo].[Bakery] ([ID], [Name], [Image], [Description], [ReleaseDate], [CategoryID], [isDeleted]) VALUES (6, N'Bánh Cuộn socola', N'/BakeryImg/BanhCuonChocolate.png', N'Bánh ngọt hấp dẫn với lớp bánh bông lan mềm mại cuộn quanh lớp nhân socola thơm ngon', CAST(N'2024-05-29' AS Date), 1, 0)
INSERT [dbo].[Bakery] ([ID], [Name], [Image], [Description], [ReleaseDate], [CategoryID], [isDeleted]) VALUES (7, N'Bánh Dứa', N'/BakeryImg/BanhDua.png', N'Hương vị đặc trưng của dừa, với lớp bánh mềm mại kết hợp cùng lớp dừa nạo giòn ngọt hoặc dừa khô', CAST(N'2024-05-29' AS Date), 1, 0)
INSERT [dbo].[Bakery] ([ID], [Name], [Image], [Description], [ReleaseDate], [CategoryID], [isDeleted]) VALUES (8, N'Bánh Mỳ Ruốc', N'/BakeryImg/BanhMyRuoc.png', N'Đây là loại bánh mỳ mềm, thường có nhân hoặc phủ ruốc, hương vị mặn ngọt hài hòa, với lớp bánh mềm mịn kết hợp cùng ruốc thơm ngon.', CAST(N'2024-05-29' AS Date), 1, 0)
INSERT [dbo].[Bakery] ([ID], [Name], [Image], [Description], [ReleaseDate], [CategoryID], [isDeleted]) VALUES (9, N'Bánh Ốc Quế Kem', N'/BakeryImg/BanhOcQueKem.png', N'Một món ăn độc đáo và hấp dẫn, kết hợp giữa hai yếu tố: lớp bánh mì mềm mại và phần kem ngọt mát bên trong', CAST(N'2024-05-29' AS Date), 2, 0)
INSERT [dbo].[Bakery] ([ID], [Name], [Image], [Description], [ReleaseDate], [CategoryID], [isDeleted]) VALUES (10, N'Bánh Paté chaud', N'/BakeryImg/BanhPateChaud.png', N'Được làm từ lớp vỏ bánh giòn và nhân pâté thơm ngon bên trong', CAST(N'2024-05-29' AS Date), 1, 0)
INSERT [dbo].[Bakery] ([ID], [Name], [Image], [Description], [ReleaseDate], [CategoryID], [isDeleted]) VALUES (11, N'Black Croffle Chocolate', N'/BakeryImg/BlackCroffleChocolate.png', N'Sự kết hợp độc đáo giữa croissant và sô cô la', CAST(N'2024-05-29' AS Date), 3, 0)
INSERT [dbo].[Bakery] ([ID], [Name], [Image], [Description], [ReleaseDate], [CategoryID], [isDeleted]) VALUES (12, N'Bánh Caramel', N'/BakeryImg/Caramel.png', N'Lớp bánh ẩm mịn, phủ trên cùng bởi lớp caramel sệt và thường được trang trí bằng các hạt đậu, hạt dẻ cười hoặc sô cô la.', CAST(N'2024-05-29' AS Date), 2, 0)
INSERT [dbo].[Bakery] ([ID], [Name], [Image], [Description], [ReleaseDate], [CategoryID], [isDeleted]) VALUES (13, N'Bánh Choco Santa', N'/BakeryImg/Choco.png', N'Hình dạng và trang trí giống như xe quà của ông già Noel, được làm từ sô cô la và bánh ngọt.', CAST(N'2024-05-29' AS Date), 2, 0)
INSERT [dbo].[Bakery] ([ID], [Name], [Image], [Description], [ReleaseDate], [CategoryID], [isDeleted]) VALUES (14, N'Bánh Crepe Xoài', N'/BakeryImg/CrepeXoai.png', N'Món ăn vừa ngon miệng, vừa hấp dẫn, kết hợp giữa chiếc crepe mềm mịn và mùi vị ngọt ngào của xoài', CAST(N'2024-05-29' AS Date), 2, 0)
INSERT [dbo].[Bakery] ([ID], [Name], [Image], [Description], [ReleaseDate], [CategoryID], [isDeleted]) VALUES (15, N'Bánh Plan', N'/BakeryImg/FlanCake.png', N'Nước sốt caramel mịn mượt được làm từ đường và nước, lót dưới đáy khuôn, được chế biến với hỗn hợp trứng, sữa đặc có đường và đôi khi là nước cốt dừa, mang lại kết cấu béo ngậy', CAST(N'2024-05-29' AS Date), 2, 0)
INSERT [dbo].[Bakery] ([ID], [Name], [Image], [Description], [ReleaseDate], [CategoryID], [isDeleted]) VALUES (16, N'Bánh Madeleines', N'/BakeryImg/MadeleinesPie.png', N'Bánh nhỏ, có hình dạng và kích thước tương tự như chiếc vỏ sò,được làm từ bột mì, bơ, đường, trứng và hương vani', CAST(N'2024-05-29' AS Date), 3, 0)
INSERT [dbo].[Bakery] ([ID], [Name], [Image], [Description], [ReleaseDate], [CategoryID], [isDeleted]) VALUES (17, N'NewYork Cheese Rolls', N'/BakeryImg/NewYorkCheeseRolls.png', N'Bánh cuộn được làm từ bột mì mềm mịn và nhân phô mai đậm đà', CAST(N'2024-05-29' AS Date), 3, 0)
INSERT [dbo].[Bakery] ([ID], [Name], [Image], [Description], [ReleaseDate], [CategoryID], [isDeleted]) VALUES (18, N'NewYork Strawberry Rolls', N'/BakeryImg/NewYorkStrawberryRolls.png', N'Hình dạng cuộn tròn, với lớp vỏ ngoài giòn và màu vàng đẹp, bên trong là lớp nhân dâu ngọt ngào và mềm mịn', CAST(N'2024-05-29' AS Date), 3, 0)
INSERT [dbo].[Bakery] ([ID], [Name], [Image], [Description], [ReleaseDate], [CategoryID], [isDeleted]) VALUES (19, N'Bánh Ruốc Cheese Croffle', N'/BakeryImg/RuocCheeseCroffle.png', N'Sự kết hợp giữa hai món ăn ngon là ruốc (ruột cá) và croffle (bánh waffle được làm từ croissant),Ruốc được phối trộn với phô mai và các gia vị khác, sau đó được đặt giữa hai lớp croffle và nướng cho đến khi phô mai tan chảy và bánh croffle giòn rụm.', CAST(N'2024-05-29' AS Date), 1, 0)
INSERT [dbo].[Bakery] ([ID], [Name], [Image], [Description], [ReleaseDate], [CategoryID], [isDeleted]) VALUES (20, N'Salted Egg Pastry', N'/BakeryImg/SaltedEggPastry.png', N'Một món ăn phổ biến trong nhiều nền văn hóa, thường được làm từ bột mì và nhân trứng muối', CAST(N'2024-05-29' AS Date), 1, 0)
INSERT [dbo].[Bakery] ([ID], [Name], [Image], [Description], [ReleaseDate], [CategoryID], [isDeleted]) VALUES (21, N'Swedish Princess Pie', N'/BakeryImg/SwedishPrincessPie.png', N'Được làm từ mứt, trứng, sữa, kem và cốt bánh bông lan, bao phủ phía trên là lớp bánh hạnh nhân (thường có màu xanh)', CAST(N'2024-05-29' AS Date), 4, 0)
SET IDENTITY_INSERT [dbo].[Bakery] OFF
GO

--BakeryDetail
INSERT INTO BakeryOption(Size,Quantity,Price,Rating,Discount,BakeryID)
VALUES
	(28,5,200000,0,0,1),
	(30,6,250000,0,0,1),
	(5,10,50000,0,0,2),
	(3,10,35000,0,0,2),
    (3,15,5000,0,0,3),
    (5,15,7000,0,0,3),
    (3,15,5000,0,0,4),
    (5,15,7000,0,0,4),
    (10,10,15000,0,0,5),
    (13,10,20000,0,0,5),
    (10,15,15000,0,0,6),
    (13,15,20000,0,0,6),
    (10,10,25000,0,0,7),
    (23,10,30000,0,0,7),
    (10,10,15000,0,0,8),
    (13,10,20000,0,0,8),
    (15,10,30000,0,0,9),
    (17,10,35000,0,0,9),
    (10,10,15000,0,0,10),
    (13,10,20000,0,0,10),
    (10,10,15000,0,0,11),
    (13,10,20000,0,0,11),
    (5,10,10000,0,0,12),
    (10,10,20000,0,0,12),
    (3,5,60000,0,10,13),
    (4,6,80000,0,0,13),
	(7,12,99000,0,0,14),
	(4,12,19000,0,0,15),
	(3,12,49000,0,0,16),
	(5,12,50000,0,0,17),
	(5,12,50000,0,0,18),
	(10,12,15000,0,0,19),
	(10,12,20000,0,0,20),
	(25,12,250000,0,0,21)
GO
--select * from BakeryOption

--Vouchers
INSERT INTO Vouchers(Code,VPercent,Quantity,StartDate,EndDate,UserID)
VALUES
	('QUATANG11',15,1, CAST(N'2024-06-12T00:00:00.000' AS DateTime), CAST(N'2024-07-12T00:00:00.000' AS DateTime),NULL),
	('QUATANG33',10,0, CAST(N'2024-06-12T00:00:00.000' AS DateTime), CAST(N'2024-12-12T00:00:00.000' AS DateTime),NULL),
	('QUATANG55',15,1, CAST(N'2024-06-12T00:00:00.000' AS DateTime), CAST(N'2024-12-12T00:00:00.000' AS DateTime),NULL),
	('QUATANG66',10,2, CAST(N'2024-06-12T00:00:00.000' AS DateTime), CAST(N'2024-12-12T00:00:00.000' AS DateTime),NULL),
	(N'KINGBAKERY', 30, 4, CAST(N'2024-07-17T00:00:00.000' AS DateTime), CAST(N'2024-07-26T00:00:00.000' AS DateTime),NULL),
	(N'KBGIFT88', 25, 2, CAST(N'2024-07-17T00:00:00.000' AS DateTime), CAST(N'2024-07-17T00:00:00.000' AS DateTime),NULL)
GO

--Favourite
INSERT INTO Favourite(CustomerID,BakeryID)
VALUES
	(1,1),
	(1,2)
GO

--Feedback
INSERT INTO Feedback(CustomerID,BakeryID,ContentFB,Time)
VALUES
( 1, 1, N'Bánh rất ngon và phục vụ thân thiện',GETDATE()),
( 1, 2, N'Không gian quán rất ấm cúng, bánh mì tươi ngon',GETDATE()),
( 1, 2, N'Mình đã mua bánh này nhiều lần và chất lượng thực sự rất tốt',GETDATE()),
( 1, 3, N'Dịch vụ giao hàng nhanh, bánh đến nơi vẫn còn ấm',GETDATE()),
( 6, 3, N'Tôi là người yêu thích những sản phẩm bánh quy kết hợp cùng với hoa quả, những chiếc bánh quy này chính là lựa chọn hoàn hảo',GETDATE()),
( 6, 1, N'Mình cảm thấy là một món tráng miệng mà hầu như ai cũng yêu thích, từ trẻ em đến người lớn, và nên được thưởng thức trong các dịp đặc biệt như sinh nhật, kỷ niệm, hay ngày lễ',GETDATE()),
( 5, 1, N'Không giống như những mẫu bánh khác, chiếc bánh có hương vị khác lạ, cuốn hút. Lớp chocolate giòn vụn, kết hợp cùng kem trứng ngọt béo tạo nên một hương vị tổng thể rất hài hòa và đưa miệng',GETDATE())
GO

--Orders
INSERT INTO Orders (StaffID, ShipperID, VoucherID, DateTime, AdrDelivery, PhoneNumber, TotalPrice, Status, Note, DenyReason, Payment) VALUES
(3, 8, NULL, '2024-05-01 10:30:00', N'123 Đường ABC, Quận 1, TP HCM', '0123456789', 470000, N'Đã giao hàng',N'Shop vui lòng gửi thêm thìa nhựa nhé',NULL, 'COD'),
(NULL, NULL, 4, getdate(), N'456 Đường DEF, Quận 2, TP HCM', '0123456789', 378000, N'Đã đặt hàng',NULL,NULL, 'COD'),
(3, 8, NULL, getdate(), N'789 Đường GHI, Quận 3, TP HCM', '0123456789', 220000, N'Đang giao hàng',NULL,NULL, 'COD'),
(NULL, NULL, 4, '2024-05-02 11:00:00', N'456 Đường DEF, Quận 2, TP HCM', '0123456789', 108000, N'Bị từ chối',NULL,N'Xin lỗi quý khách, hiện tại shop không thể ship hàng. Mong quý khách thông cảm', 'COD'),
(3, 8, NULL, '2024-06-02', N'123 Đường ABC, Quận 2, TP HCM', '0123456789', 470000, N'Đã giao hàng',N'Cảm ơn shop',NULL, 'COD'),
(10, 8, NULL, '2024-06-12', N'456 Đường XYZ, Quận 3, TP HCM', '0123456789', 120000, N'Đã giao hàng',N'Cảm ơn shop',NULL, 'COD'),
(7, 8, NULL, '2024-07-10', N'789 Đường ABC, Quận 4, TP HCM', '0123456789', 220000, N'Đã giao hàng',N'Cảm ơn shop',NULL, 'COD'),
(7, 8, NULL, '2024-07-12', N'JQKA Đường ABC, Quận 5, HN', '0123456789', 520000, N'Đã giao hàng',N'Cảm ơn shop',NULL, 'COD'),
(7, 8, NULL, '2024-07-15', N'28 Đường Lê Hồng Phong, Quận 5, TP HCM', '0975861471', 1485000, N'Đã giao hàng',N'Shop cố gắng giao hàng nhanh giúp mình nhé',NULL, 'VNP'),
(7, 8, NULL, '2024-07-15', N'74 Đường Trần Hưng Đạo, Quận 2, TP HCM', '0927188192', 1085000, N'Đã giao hàng',NULL,NULL, 'COD'),
(NULL, NULL, NULL, getdate(), N'456 Đường DEF, Quận 2, TP HCM', '0123456789', 420000, N'Đã đặt hàng',NULL,NULL, 'COD'),
(NULL, NULL, NULL, '2024-05-03 11:00:00', N'456 Đường DEF, Quận 2, TP HCM', '0123456789', 120000, N'Bị từ chối',NULL,N'Xin lỗi quý khách rất nhiều', 'VNP')
GO
SET IDENTITY_INSERT Orders ON;
INSERT INTO Orders (ID, DateTime) VALUES
(0,'2000-01-01')
GO
SET IDENTITY_INSERT Orders OFF;
--delete from Orders where id = 

--OrderItem
INSERT INTO OrderItem ( BakeryID, CustomerID, OrderID, Quantity, Price) VALUES
( 1, 1, 1, 2, 400000),
( 1, 1, 2, 2, 400000),
( 3, 1, 1, 1, 50000),
( 1, 1, 3, 1, 200000),
( 3, 1, 4, 2, 100000),
( 3, 1, 5, 1, 50000),
( 1, 1, 5, 2, 400000),
( 3, 1, 6, 2, 100000),
( 1, 1, 7, 1, 200000),
( 2, 1, 8, 2, 500000),
( 1, 1, 9, 2, 400000),
( 34, 1, 9, 3, 750000),
( 26, 1, 9, 3, 240000),
( 9, 1, 9, 5, 75000),
( 34, 5, 10, 3, 750000),
( 26, 5, 10, 3, 240000),
( 9, 5, 10, 5, 75000),
( 1, 1, 11, 2, 400000),
( 3, 1, 12, 2, 100000)
Go
--select * from OrderItem
--select * from Orders
--select * from Vouchers
--select * from Users
--select * from Feedback
--select * from Favourite

--BlogPosts
INSERT INTO BlogPosts (Title, Content, PublishedDate, ModifiedDate, Author, Image)
VALUES
    ('Welcome to King Bakery!', 'This is our first blog post! We are excited to share our baking adventures with you.', '2022-01-01 10:00:00', NULL, 'John Doe', '/BlogImg/blog-post-1.jpg'),
    ('Our Favorite Recipes', 'In this post, we share our top 5 favorite recipes that you must try!', '2022-01-15 14:00:00', '2022-01-20 12:00:00', 'Jane Smith', '/BlogImg/blog-post-2.jpg'),
    ('Baking Tips and Tricks', 'Get ready to take your baking skills to the next level with these expert tips and tricks!', '2022-02-01 08:00:00', NULL, 'Bob Johnson', '/BlogImg/blog-post-3.jpg'),
    ('New Arrivals: Spring Collection', 'Check out our new spring collection of baked goods, featuring fresh flavors and ingredients!', '2022-03-01 12:00:00', '2022-03-05 10:00:00', 'Emily Chen', '/BlogImg/blog-post-4.jpg'),
    ('Behind the Scenes: Our Bakery', 'Ever wondered what goes on behind the scenes of our bakery? Take a peek at our latest blog post to find out!', '2022-04-01 10:00:00', NULL, 'Michael Brown', '/BlogImg/blog-post-5.jpg')