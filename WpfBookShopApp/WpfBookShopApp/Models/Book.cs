using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfBookShopApp.Models
{
    public class Book
    {   
        public int Id { get; set; }
        public string Name { get; set; }
        public int Pages { get; set; } 
        public int AuthorId { get; set; }
        public int PublisherId { get; set; }
        public int GenreId { get; set; }
        public int PublishYear { get; set; }
        public int PrimeCost { get; set; }
        public int SellCost { get; set; }
        public int AdderUserId { get; set; }
        public User User { get; set; }

        public Author Author { get; set; }
        public Genre Genre { get; set; }
        public Publisher Publisher { get; set; }
    }
}
