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
	Rating FLOAT
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

CREATE TABLE Orders(
	ID INT PRIMARY KEY,
	CustomerID INT,
	StaffID INT,
	ShipperID INT,
	DateTime DATETIME,
	AdrDelivery NVARCHAR(300),
	TotalPrice FLOAT,
	Status NVARCHAR(100),
	FOREIGN KEY (CustomerID) REFERENCES Customer(UserID) ON DELETE CASCADE,
	FOREIGN KEY (StaffID) REFERENCES Employee(UserID),
	FOREIGN KEY (ShipperID) REFERENCES Employee(UserID)
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


