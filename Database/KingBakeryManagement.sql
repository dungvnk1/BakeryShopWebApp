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

	FOREIGN KEY (UserID) REFERENCES Users(ID) on delete cascade
)
GO

CREATE TABLE Category(
	ID INT Identity(1,1) PRIMARY KEY,
	Name NVARCHAR(255)
)
GO

CREATE TABLE Bakery(
	ID INT Identity(1,1) PRIMARY KEY,
	Size INT,
	Quantity INT,
	Price FLOAT
)
GO

CREATE TABLE BakeryDetail(
	BakeryID INT PRIMARY KEY,
	Name NVARCHAR(255),
	Image VARCHAR(255),
	Description NVARCHAR(4000),
	CategoryID INT, 
	FOREIGN KEY (BakeryID) REFERENCES Bakery(ID) on delete cascade,
	FOREIGN KEY (CategoryID) REFERENCES Category(ID) on delete cascade
)
GO

CREATE TABLE Orders(
	ID int primary key,
	BakeryID int,
	CustomerID int,
	Quantity int
);
GO

create table Bill(
	ID int primary key,
	DateTime date,
	Status nvarchar(100)
)

GO

create table BillDetail(
	BillID int foreign key references Bill on delete cascade,
	OrderID int foreign key references Orders on delete cascade,
	StaffID int foreign key references Employee on delete cascade,
	CustomerAddress nvarchar(100),
	Price float
)

CREATE TABLE Feedback(
	ID int primary key,
	ContentFB nvarchar(4000),
	CustomerID int,
	BillID int,
	Foreign key (CustomerID) references Customer(UserID) on delete cascade,
	Foreign key (BillID) references Bill(ID) on delete cascade
);
GO


