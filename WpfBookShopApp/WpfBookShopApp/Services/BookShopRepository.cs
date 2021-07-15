using Dapper;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WpfBookShopApp.Models;

namespace WpfBookShopApp.Services
{
    public class BookShopRepository : IBookShopRepository
    {
        private readonly string ConnectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=BookShop;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

        

        public void AddAuthor(string name)
        {
            using var db = new SqlConnection(ConnectionString);
            var query = @"INSERT INTO Authors ([Name]) VALUES (@name)";
            db.Execute(query, new { name });
        }

        public int AddBook(Book book)
        {
            using var db = new SqlConnection(ConnectionString);
            var query = @"INSERT INTO Books(Name,Pages,AuthorId,PublisherId,GenreId,PublishYear,PrimeCost,SellCost,AdderUserId) 
                          VALUES (@Name,@Pages,@AuthorId,@PublisherId,@GenreId,@PublishYear,@PrimeCost,@SellCost,@AdderUserId);SET @id=SCOPE_IDENTITY(); ";

            var param = new DynamicParameters();

            param.Add("@Name", book.Name, DbType.String, ParameterDirection.Input);
            param.Add("@Pages", book.Pages, DbType.Int32, ParameterDirection.Input);
            param.Add("@AuthorId", book.AuthorId, DbType.Int32, ParameterDirection.Input);
            param.Add("@PublisherId", book.PublisherId, DbType.Int32, ParameterDirection.Input);
            param.Add("@GenreId", book.GenreId, DbType.Int32, ParameterDirection.Input);
            param.Add("@PublishYear", book.PublishYear, DbType.Int32, ParameterDirection.Input);
            param.Add("@PrimeCost", book.PrimeCost, DbType.Int32, ParameterDirection.Input);
            param.Add("@SellCost", book.SellCost, DbType.Int32, ParameterDirection.Input);
            param.Add("@AdderUserId", book.AdderUserId, DbType.Int32, ParameterDirection.Input);
            param.Add("@id", 0, DbType.Int32, ParameterDirection.Output);
           
            db.Execute(query, param);
            return param.Get<int>("@id");
        }

        public void AddBookStorage(int BooksId, int quantity)
        { 
            using var db = new SqlConnection(ConnectionString);
            var query = @"INSERT INTO Storage(BooksId, Quantity) VALUES(@BooksId, @quantity)";
            db.Execute(query, new { BooksId, quantity });
        }

        public void AddGenre(string name)
        {
            using var db = new SqlConnection(ConnectionString);
            var query = @"INSERT INTO Genres ([Name]) VALUES (@name)";
            db.Execute(query, new { name });
        }

        public void AddPublisher(string name)
        {
            using var db = new SqlConnection(ConnectionString);
            var query = @"INSERT INTO Publishers ([Name]) VALUES (@name)";
            db.Execute(query, new { name });
        }

        public void DeleteAuthor(string name)
        {
            using var db = new SqlConnection(ConnectionString);
            var query = @"DELETE FROM Authors WHERE Name = @name";
            db.Execute(query, new { name });
        }

        public void DeleteGenre(string name)
        {
            using var db = new SqlConnection(ConnectionString);
            var query = @"DELETE FROM Genres WHERE Name = @name";
            db.Execute(query, new { name });
        }

        public void DeletePublisher(string name)
        {
            using var db = new SqlConnection(ConnectionString);
            var query = @"DELETE FROM Publishers WHERE Name = @name";
            db.Execute(query, new { name });
        }

        public int GetAuthorIdByName(string name)
        {
            using var db = new SqlConnection(ConnectionString);
            var query = @"SELECT * FROM Authors WHERE Name = @name";

            return db.QueryFirst<int>(query, new { name });
        }

        public List<Author> GetAuthors()
        {
            using var db = new SqlConnection(ConnectionString);
            var query = @"SELECT* FROM Authors";
            return db.Query<Author>(query).ToList();
        }

        public int GetGenreIdByName(string name)
        {
            using var db = new SqlConnection(ConnectionString);
            var query = @"SELECT * FROM Genres WHERE Name = @name";

            return db.QueryFirst<int>(query, new { name });
        }

        public List<Genre> GetGenres()
        {
            using var db = new SqlConnection(ConnectionString);
            var query = @"SELECT* FROM Genres";
            return db.Query<Genre>(query).ToList();
        }

        public int GetPublisherIdByName(string name)
        {
            using var db = new SqlConnection(ConnectionString);
            var query = @"SELECT * FROM Publishers WHERE Name = @name";

            return db.QueryFirst<int>(query, new { name });
        }

        public List<Publisher> GetPublishers()
        {
            using var db = new SqlConnection(ConnectionString);
            var query = @"SELECT* FROM Publishers";
            return db.Query<Publisher>(query).ToList();
        }

        public void AddUser(User user)
        {
            using var db = new SqlConnection(ConnectionString);
            var query = @"INSERT INTO Users(FullName,DateOfBith,Gender,Email,Username,[Password],PositionId) 
                          VALUES (@FullName,@DateOfBith,@Gender,@Email,@Username,@Password,@PositionId)";
            db.Execute(query, user);
        }

        public User GetUserByLoginAndPassword(string login, string password)
        {
            using var db = new SqlConnection(ConnectionString);
            var query = @" SELECT * FROM Users
                           Join Positions ON Users.PositionId = Positions.Id
                           WHERE Username LIKE @login AND [Password] LIKE @password";
            return db.Query<User, Position, User>(query,
            (user, position) =>
            {
                user.Position = position;
                return user;
            }, new { login, password }).FirstOrDefault();
        }


        public List<Book> GetBooksByUserOwner(User user)
        {
            using var db = new SqlConnection(ConnectionString);
            var query = @" SELECT * FROM Books
                           WHERE Books.AdderUserId = @Id";
            return db.Query<Book>(query, new { user.Id }).ToList();
        }

        public void DeleteBookByName(string name)
        {
            using var db = new SqlConnection(ConnectionString);
            var query = " DELETE FROM Books WHERE Name = @name";
            db.Execute(query, new { name });
        }


        public List<Storage> GetBooksSearchByName(string name)
        {
            using var db = new SqlConnection(ConnectionString);
            var query = @"SELECT * FROM Storage 
                          JOIN Books ON Books.Id = Storage.BooksId
                          WHERE Books.Name LIKE @name";
            return db.Query<Storage,Book, Storage>(query,
             (storage,book) => 
            {
                storage.Book = book;
                return storage;
            } ,new { name = name + "%" }).ToList();
        }
         

        public List<Storage> GetBooksSearchByAuthor(string name)
        {
            using var db = new SqlConnection(ConnectionString);
            var query = @"SELECT * FROM Storage 
                          JOIN Books ON Books.Id = Storage.BooksId
                          JOIN Authors ON Books.AuthorId = Authors.Id
                          WHERE Authors.Name LIKE @name";
            return db.Query<Storage, Book,Author ,Storage>(query,
            (storage,book,author) =>
            {
                book.Author = author;
                storage.Book = book;
                return storage;
            },new { name = name + "%" }).ToList();
        }

        public List<Storage> GetBooksSearchByGenre(string name)
        {
            using var db = new SqlConnection(ConnectionString);
            var query = @"SELECT * FROM Storage 
                          JOIN Books ON Books.Id = Storage.BooksId
                          JOIN Genres ON Books.GenreId = Genres.Id
                          WHERE Genres.Name LIKE @name";
            return db.Query<Storage, Book, Genre, Storage>(query,
            (storage, book, genre) =>
            {
                book.Genre = genre;
                storage.Book = book;
                return storage;
            }, new { name = name + "%" }).ToList();
        }
         
        public void BuyBook(Book book, User user) 
        {
            using var db = new SqlConnection(ConnectionString);

            var query = @"  Select * from Storage where BooksId = @id";
            var storage = db.QueryFirst<Storage>(query, new { id = book.Id });

            if (storage!=null )
            {
                if (storage.Quantity>0)
                {
                    storage.Quantity -= 1;
                    var query2 = @"UPDATE Storage SET Quantity=@Quantity WHERE Id = @Id ";
                    db.Execute(query2, new { storage.Id, storage.Quantity });

                    var query3 = @"SELECT * FROM BuyingBooks WHERE UserId = @UserId AND BooksId = @BooksId";
                    var buying = db.QueryFirstOrDefault<BuyingBooks>(query3, new { BooksId = storage.BooksId, UserId = user.Id });

                    if (buying!=null)
                    {
                        buying.Quantity += 1;
                        var query4 = @"UPDATE BuyingBooks SET Quantity=@Quantity WHERE Id = @Id ";
                        db.Execute(query4, new { buying.Id, buying.Quantity });
                    }
                    else
                    {
                        var query5 = @"INSERT INTO BuyingBooks (BooksId,UserId,Quantity) VALUES (@BooksId,@UserId,@Quantity)";
                        db.Execute(query5, new { storage.BooksId, UserId = user.Id, Quantity = 1 });
                    }
                }

            } 
        }

        public List<BuyingBooks> GetBooksByUserBuying(User user)
        {
            using var db = new SqlConnection(ConnectionString);
            var query = @" SELECT * FROM BuyingBooks
                           JOIN Books ON BuyingBooks.BooksId = Books.Id
                           WHERE BuyingBooks.UserId = @Id";
            return db.Query<BuyingBooks, Book, BuyingBooks>(query,
            (buying, book) =>
            {
                buying.Book = book;
                return buying;
            }
            , new { Id = user.Id }).ToList();
        }

        public void DeleteBookBuying(int Id)
        {
            using var db = new SqlConnection(ConnectionString);
            var query = @"DELETE FROM BuyingBooks 
                          WHERE BuyingBooks.BooksId = @Id";
            db.Execute(query, new { Id });
        }

        public List<Book> GetTopSellBook()
        {
            using var db = new SqlConnection(ConnectionString);
            var query = @"SELECT Books.Name,SUM(BuyingBooks.Quantity)as Sum FROM BuyingBooks
                          JOIN Books ON BuyingBooks.BooksId = Books.Id
                          Group by Books.Name
                          Order by SUM(BuyingBooks.Quantity) DESC";
            return db.Query<Book>(query).ToList();
        }

        public List<Book> GetTopNewBook()
        {
            using var db = new SqlConnection(ConnectionString);
            var query = @"SELECT * FROM  Books 
                          Order by Books.Id DESC";
            return db.Query<Book>(query).ToList();
        }

        public List<Book> GetTopByAuthorBook()
        {
            using var db = new SqlConnection(ConnectionString);
            var query = @"SELECT Authors.Name ,Count(*) as Count FROM  Books 
                          JOIN Authors ON Books.AuthorId = Authors.Id
                          Group by Authors.Name
                          Order by Count(*) DESC";
            return db.Query<Book>(query).ToList();
        }

        public List<Book> GetTopByGenreBook()
        {
            using var db = new SqlConnection(ConnectionString);
            var query = @"SELECT Genres.Name ,Count(*) as Count FROM  Books 
                          JOIN Genres ON Books.GenreId = Genres.Id
                          Group by Genres.Name
                          Order by Count(*) DESC";
            return db.Query<Book>(query).ToList();
        }

        public void UpdateUser(User user)
        { 
            using var db = new SqlConnection(ConnectionString);
            var query = @"UPDATE Users SET Money = @Money,IsModerator = @IsModerator WHERE Id = @Id";
            db.Execute(query, user); 
        }

        public List<User> GetUsers()
        {
            using var db = new SqlConnection(ConnectionString);
            var query = @"SELECT * FROM Users
                           Join Positions ON Users.PositionId = Positions.Id";
            return db.Query<User,Position,User>(query,
            (user,position) =>
            {
                user.Position = position;
                return user;
            }).ToList();
        }

        public void DeleteUserById(int id)
        {
            using var db = new SqlConnection(ConnectionString);
            var query = @"DELETE FROM Users WHERE Id = @id";
            db.Execute(query, new { id });
        }
         
    }
}
