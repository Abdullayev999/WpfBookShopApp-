CREATE DATABASE  BookShop

GO
USE BookShop

CREATE TABLE Authors
(
	Id INT PRIMARY KEY IDENTITY,
	[Name] NVARCHAR(50) NOT NULL
	
	CONSTRAINT CK_Authors_Name CHECK([Name] != ' ')
	CONSTRAINT UQ_Authors_Name UNIQUE([Name])
) 

CREATE TABLE Genres
(
	Id INT PRIMARY KEY IDENTITY,
	[Name] NVARCHAR(50) NOT NULL
	
	CONSTRAINT CK_Genres_Name CHECK([Name] != ' ')
	CONSTRAINT UQ_Genres_Name UNIQUE([Name])
)
 

CREATE TABLE Publishers
(
	Id INT PRIMARY KEY IDENTITY,
	[Name] NVARCHAR(50) NOT NULL
	
	CONSTRAINT CK_Publishers_Name CHECK([Name] != ' ')
	CONSTRAINT UQ_Publishers_Name UNIQUE([Name])
)

 CREATE TABLE Positions
 (	
	Id INT  PRIMARY KEY IDENTITY,
	[Name] nvarchar(30) NOT NULL,

    CONSTRAINT UQ_Positions_Name UNIQUE([Name])
 ) 

 INSERT INTO Positions([Name]) VALUES ('Admin')  
 INSERT INTO Positions([Name]) VALUES ('Client')
 INSERT INTO Positions([Name]) VALUES ('Moderator')



CREATE TABLE Users
(
	Id INT PRIMARY KEY IDENTITY, 
	IsModerator INT NOT NULL DEFAULT(0),
	PositionId INT NOT NULL ,
	FullName NVARCHAR(30) NOT NULL,
	DateOfBith DATE NOT NULL,
	Gender NVARCHAR(10) NOT NULL,
	Email NVARCHAR(30) NOT NULL,  
	Username NVARCHAR(30) NOT NULL,
	[Money] INT NOT NULL DEFAULT(1000),
   [Password] NVARCHAR(150) NOT NULL, 

	CONSTRAINT FK_Users_PositionId FOREIGN KEY (PositionId) REFERENCES Positions(Id),
	CONSTRAINT CK_Users_FullName CHECK(FullName!=' '),
	CONSTRAINT CK_Users_Email CHECK(Email!=' '),
	CONSTRAINT CK_Users_Money CHECK([Money]>=0),
	CONSTRAINT CK_Users_Gender CHECK(Gender IN ('Female','Male')),
	CONSTRAINT CK_Users_DateOfBith CHECK(18<=(Year(GETDATE())-Year(DateOfBith))),
	CONSTRAINT UQ_Users_FullName UNIQUE(FullName) ,
	CONSTRAINT UQ_Users_Username UNIQUE(Username) 
) 

CREATE TABLE Books
(
	Id INT PRIMARY KEY IDENTITY,
	[Name] NVARCHAR(50) NOT NULL,
	Pages INT NOT NULL,
	AuthorId INT NOT NULL,
	PublisherId INT NOT NULL,
	GenreId INT NOT NULL,
	PublishYear INT NOT NULL DEFAULT(Year(GETDATE())),
	PrimeCost DECIMAL NOT NULL,
	SellCost DECIMAL NOT NULL,
	AdderUserId  INT NOT NULL,
	
	
	CONSTRAINT CK_Books_PublishYear CHECK(PublishYear <= Year(GETDATE())),
	CONSTRAINT CK_Books_Name CHECK([Name] != ' '), 
	CONSTRAINT CK_Books_PrimeCost CHECK(PrimeCost >0),
	CONSTRAINT CK_Books_SellCost CHECK(SellCost >= PrimeCost),
	CONSTRAINT CK_Books_Pages CHECK(Pages > 0),
	CONSTRAINT UQ_Books_Name UNIQUE([Name]),
	CONSTRAINT FK_Books_AuthorId FOREIGN KEY (AuthorId) REFERENCES Authors(Id),
	CONSTRAINT FK_Books_GenreId FOREIGN KEY (GenreId) REFERENCES Genres(Id),
	CONSTRAINT FK_Books_PublisherId FOREIGN KEY (PublisherId) REFERENCES Publishers(Id),
	CONSTRAINT FK_Books_AdderUserId FOREIGN KEY (AdderUserId) REFERENCES Users(Id)  ON DELETE CASCADE,
)

  

CREATE TABLE Storage
(
    Id INT PRIMARY KEY IDENTITY,
	BooksId INT NOT NULL, 
	Quantity INT NOT NULL DEFAULT(0),

	CONSTRAINT FK_Storage_BooksId FOREIGN KEY (BooksId) REFERENCES Books(Id) ON DELETE CASCADE,
	CONSTRAINT CK_Storage_Quantity CHECK(Quantity>=0) 
) 



CREATE TABLE BuyingBooks
(  
    Id INT PRIMARY KEY IDENTITY,
	BooksId INT NOT NULL, 
	UserId INT NOT NULL ,
	Quantity INT NOT NULL DEFAULT(1),
	
	CONSTRAINT FK_BuyingBooks_BooksId FOREIGN KEY (BooksId) REFERENCES Books(Id) ON DELETE CASCADE,
	CONSTRAINT FK_BuyingBooks_UserId FOREIGN KEY (UserId) REFERENCES Users(Id),
	CONSTRAINT CK_BuyingBooks_Quantity CHECK(Quantity>=0) 
) 



CREATE TRIGGER DeleteBooks ON Storage
AFTER UPDATE
AS 
BEGIN
	DECLARE @quantity INT;
	DECLARE @id INT;
	SELECT @id = Id, @quantity = Quantity FROM inserted

	IF @quantity = 0
	DELETE FROM Storage WHERE Id=@id;
END


CREATE TRIGGER DeleteBooksBuying ON BuyingBooks
AFTER UPDATE
AS 
BEGIN
	DECLARE @quantity INT;
	DECLARE @id INT;
	SELECT @id = Id, @quantity = Quantity FROM inserted

	IF @quantity = 0
	DELETE FROM BuyingBooks WHERE Id=@id;
END
 