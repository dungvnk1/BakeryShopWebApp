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
    PhoneNumber NVARCHAR(100) NOT NULL,
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
	Quantity INT
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
	(N'Mạnh Hùng',N'hung123',N'123','Ha Noi','2004-01-08','hung@gmail.com','0123456789',2, '', 0),  --*Role: 1_admin,2_cus,3_staff,4_shipper
	(N'Năng Dũng',N'dung123',N'123','Ha Noi','2004-05-12','dung@gmail.com','0123456789',1, '', 0),
	(N'Chử Hồng Phúc',N'hongphuc',N'123','Ha Noi','2004-05-12','phuc@gmail.com','0123456789',3, '', 0),
	(N'Lê Trường Sơn',N'sonle123',N'123','81 QL21','2004-11-12','sonle@gmail.com','0987654321',3, '', 0),
	(N'Nguyễn Thị C', N'nguyenthic', N'passwordC', N'456 Nguyen Trai', '2002-07-15', 'nguyenthic@example.com', '0923456789', 2, '', 0),
    (N'Hoàng Văn D', N'hoangvand', N'passwordD', 'Ha Noi', '2001-06-30', 'hoangvand@example.com', '0934567890', 1, '', 0),
    (N'Lê Thị E', N'lethie', N'passwordE', N'789 Tran Hung Dao', '2000-05-25', 'lethie@example.com', '0945678901', 3, '', 0),
    (N'Phạm Văn F', N'phamvanf', N'passwordF', N'12 Pham Ngoc Thach', '1999-04-01', 'phamvanf@example.com', '0956789012', 4, '', 0),
    (N'Nguyễn Văn G', N'nguyenvang', N'passwordG', 'Ha Noi', '1998-03-15', 'nguyenvang@example.com', '0967890123', 2, '', 0),
    (N'Trần Thị H', N'tranthih', N'passwordH', N'34 Tran Quoc Toan', '1997-02-20', 'tranthih@example.com', '0978901234', 3, '', 0),
    (N'Đỗ Minh I', N'dominhi', N'passwordI', N'56 Ly Thuong Kiet', '1996-01-10', 'dominhi@example.com', '0989012345', 1, '', 0);
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
	(5,N'Đồng'),
	(9,N'Đồng')
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
INSERT INTO Bakery(Name,Image,Description,ReleaseDate,CategoryID)
VALUES   --Image tạm thời để NULL hết nhé
(N'Bánh kem Socola','/BakeryImg/ChocoSanta.png',N'Bánh sinh nhật phủ Socola và nhân chanh leo phù hợp với mọi lứa tuổi...','2024-05-29',3),
(N'Bánh quy nho khô','/BakeryImg/BanhQuyNhoKho.png',N'Món ăn vặt hấp dẫn...','2024-05-29',4),
(N'Apple Crepe','/BakeryImg/AppleCrepe.png',N'Bánh crepe ngọt có nhân táo thơm ngon và phủ kem ngọt lên trên.','2024-05-29',2),
(N'Apple Danish','/BakeryImg/AppleDanish.png',N'Món ăn thú vị kết hợp hương vị dễ chịu của táo và gia vị với kết cấu mềm mại, dễ chịu của bánh ngọt Đan Mạch.', '2024-05-29',2),
(N'Bánh Croissant','/BakeryImg/BanhCroissant.png',N'Lớp vỏ giòn tan và hình dáng hình lưỡi liềm. Được làm từ bột mì, men, sữa, bơ, đường và muối, croissant là một món bánh yêu thích vào bữa sáng hay bữa ăn nhẹ','2024-05-29',2),
(N'Bánh Cuộn socola','/BakeryImg/BanhCuonChocolate.png',N'Bánh ngọt hấp dẫn với lớp bánh bông lan mềm mại cuộn quanh lớp nhân socola thơm ngon', '2024-05-29', 4),
(N'Bánh Dứa','/BakeryImg/BanhDua.png',N'Hương vị đặc trưng của dừa, với lớp bánh mềm mại kết hợp cùng lớp dừa nạo giòn ngọt hoặc dừa khô', '2024-05-29', 3),
(N'Bánh Mỳ Ruốc','/BakeryImg/BanhMyRuoc.png',N'Đây là loại bánh mỳ mềm, thường có nhân hoặc phủ ruốc, hương vị mặn ngọt hài hòa, với lớp bánh mềm mịn kết hợp cùng ruốc thơm ngon.', '2024-05-29', 2),
(N'Bánh Ốc Quế Kem','/BakeryImg/BanhOcQueKem.png',N'Một món ăn độc đáo và hấp dẫn, kết hợp giữa hai yếu tố: lớp bánh mì mềm mại và phần kem ngọt mát bên trong', '2024-05-29', 4),
(N'Bánh Paté chaud','/BakeryImg/BanhPateChaud.png',N'Được làm từ lớp vỏ bánh giòn và nhân pâté thơm ngon bên trong', '2024-05-29', 1),
(N'Black Croffle Chocolate','/BakeryImg/BlackCroffleChocolate.png',N'Sự kết hợp độc đáo giữa croissant và sô cô la', '2024-05-29', 2),
(N'Bánh Caramel','/BakeryImg/Caramel.png',N'Lớp bánh ẩm mịn, phủ trên cùng bởi lớp caramel sệt và thường được trang trí bằng các hạt đậu, hạt dẻ cười hoặc sô cô la.', '2024-05-29', 2),
(N'Bánh Choco Santa','/BakeryImg/ChocoSanta.png',N'Hình dạng và trang trí giống như xe quà của ông già Noel, được làm từ sô cô la và bánh ngọt.', '2024-05-29', 4),
(N'Bánh Crepe Xoài','/BakeryImg/CrepeXoai.png',N'Món ăn vừa ngon miệng, vừa hấp dẫn, kết hợp giữa chiếc crepe mềm mịn và mùi vị ngọt ngào của xoài', '2024-05-29', 2),
(N'Bánh Plan','/BakeryImg/FlanCake.png',N'Nước sốt caramel mịn mượt được làm từ đường và nước, lót dưới đáy khuôn, được chế biến với hỗn hợp trứng, sữa đặc có đường và đôi khi là nước cốt dừa, mang lại kết cấu béo ngậy', '2024-05-29', 2),
(N'Bánh Madeleines','/BakeryImg/MadeleinesPie.png',N'Bánh nhỏ, có hình dạng và kích thước tương tự như chiếc vỏ sò,được làm từ bột mì, bơ, đường, trứng và hương vani', '2024-05-29', 2),
(N'NewYork Cheese Rolls','/BakeryImg/NewYorkCheeseRolls.png',N'Bánh cuộn được làm từ bột mì mềm mịn và nhân phô mai đậm đà', '2024-05-29', 3),
(N'NewYork Strawberry Rolls','/BakeryImg/NewYorkStrawberryRolls.png',N'Hình dạng cuộn tròn, với lớp vỏ ngoài giòn và màu vàng đẹp, bên trong là lớp nhân dâu ngọt ngào và mềm mịn', '2024-05-29', 3),
(N'Bánh Ruốc Cheese Croffle','/BakeryImg/RuocCheeseCroffle.png',N'Sự kết hợp giữa hai món ăn ngon là ruốc (ruột cá) và croffle (bánh waffle được làm từ croissant),Ruốc được phối trộn với phô mai và các gia vị khác, sau đó được đặt giữa hai lớp croffle và nướng cho đến khi phô mai tan chảy và bánh croffle giòn rụm.', '2024-05-29', 1),
(N'Salted Egg Pastry','/BakeryImg/SaltedEggPastry.png',N'Một món ăn phổ biến trong nhiều nền văn hóa, thường được làm từ bột mì và nhân trứng muối', '2024-05-29', 1),
(N'Swedish Princess Pie','/BakeryImg/SwedishPrincessPie.png',N'Được làm từ mứt, trứng, sữa, kem và cốt bánh bông lan, bao phủ phía trên là lớp bánh hạnh nhân (thường có màu xanh)', '2024-05-29', 2);
GO

--BakeryDetail
INSERT INTO BakeryOption(Size,Quantity,Price,Rating,Discount,BakeryID)
VALUES
	(28,5,200000,0,10,1),
	(30,6,250000,0,5,1),
	(5,10,50000,0,0,2)
GO

--Vouchers
INSERT INTO Vouchers(Code,VPercent,Quantity)
VALUES
	('QUATANG55',15,1),
	('QUATANG66',10,2)
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
INSERT INTO Orders (StaffID, ShipperID, VoucherID, DateTime, AdrDelivery, PhoneNumber, TotalPrice, Status, Note, DenyReason, Payment) VALUES
(3, 8, NULL, '2024-05-01 10:30:00', N'123 Đường ABC, Quận 1, TP HCM', '0123456789', 470000, N'Đã giao hàng',N'Shop vui lòng gửi thêm thìa nhựa nhé',NULL, 'COD'),
(NULL, NULL, 2, '2024-05-02 11:00:00', N'456 Đường DEF, Quận 2, TP HCM', '0123456789', 378000, N'Đã đặt hàng',NULL,NULL, 'COD'),
(3, 8, NULL, '2024-05-03 12:15:00', N'789 Đường GHI, Quận 3, TP HCM', '0123456789', 220000, N'Đang giao hàng',NULL,NULL, 'COD'),
(NULL, NULL, 2, '2024-05-02 11:00:00', N'456 Đường DEF, Quận 2, TP HCM', '0123456789', 108000, N'Bị từ chối',NULL,N'Xin lỗi quý khách, hiện tại shop không thể ship hàng. Mong quý khách thông cảm', 'COD'),
(3, 8, NULL, '2024-06-02', N'123 Đường ABC, Quận 2, TP HCM', '0123456789', 490000, N'Đã giao hàng',N'Cảm ơn shop',NULL, 'COD'),
(4, 8, NULL, '2024-06-12', N'456 Đường XYZ, Quận 3, TP HCM', '0123456789', 120000, N'Đã giao hàng',N'Cảm ơn shop',NULL, 'COD'),
(7, 8, NULL, getdate(), N'789 Đường ABC, Quận 4, TP HCM', '0123456789', 220000, N'Đã giao hàng',N'Cảm ơn shop',NULL, 'COD')
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
( 1, 1, 7, 1, 200000)
Go
--select * from OrderItem
--select * from Orders
--select * from Vouchers
--select * from Users

--BlogPosts
INSERT INTO BlogPosts (Title, Content, PublishedDate, ModifiedDate, Author, Image)
VALUES
    ('Welcome to King Bakery!', 'This is our first blog post! We are excited to share our baking adventures with you.', '2022-01-01 10:00:00', NULL, 'John Doe', 'blog-post-1.jpg'),
    ('Our Favorite Recipes', 'In this post, we share our top 5 favorite recipes that you must try!', '2022-01-15 14:00:00', '2022-01-20 12:00:00', 'Jane Smith', 'blog-post-2.jpg'),
    ('Baking Tips and Tricks', 'Get ready to take your baking skills to the next level with these expert tips and tricks!', '2022-02-01 08:00:00', NULL, 'Bob Johnson', 'blog-post-3.jpg'),
    ('New Arrivals: Spring Collection', 'Check out our new spring collection of baked goods, featuring fresh flavors and ingredients!', '2022-03-01 12:00:00', '2022-03-05 10:00:00', 'Emily Chen', 'blog-post-4.jpg'),
    ('Behind the Scenes: Our Bakery', 'Ever wondered what goes on behind the scenes of our bakery? Take a peek at our latest blog post to find out!', '2022-04-01 10:00:00', NULL, 'Michael Brown', 'blog-post-5.jpg');

