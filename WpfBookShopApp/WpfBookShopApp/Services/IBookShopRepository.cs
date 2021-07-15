using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfBookShopApp.Models;

namespace WpfBookShopApp.Services
{
    public interface IBookShopRepository
    {
        List<Author> GetAuthors();
        List<Genre> GetGenres();
        List<Publisher> GetPublishers();
        void AddAuthor(string name);
        void AddPublisher(string name);
        void AddGenre(string name);
        void UpdateUser(User user); 

        void DeleteAuthor(string name);
        void DeletePublisher(string name);
        void DeleteGenre(string name);

        List<Book> GetTopSellBook();
        List<Book> GetTopByAuthorBook();
        List<Book> GetTopByGenreBook();
        List<Book> GetTopNewBook();
        int GetAuthorIdByName(string name);
        int GetGenreIdByName(string name);
        int GetPublisherIdByName(string name);

        int AddBook(Book book);

        void AddBookStorage(int BooksId, int quantity);

        void AddUser(User user);
        User GetUserByLoginAndPassword(string login, string password);
        List<Book> GetBooksByUserOwner(User user);
        List<BuyingBooks> GetBooksByUserBuying(User user);
        void DeleteBookByName(string name);
        void DeleteBookBuying(int Id);
        
        List<Storage> GetBooksSearchByName(string name);
        List<Storage> GetBooksSearchByAuthor(string name);
        List<Storage> GetBooksSearchByGenre(string name);

        void BuyBook(Book book, User user);

        List<User> GetUsers();
        void DeleteUserById(int id);
         
    }
}
