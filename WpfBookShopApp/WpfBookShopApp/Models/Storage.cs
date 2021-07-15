using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfBookShopApp.Models
{
    public class Storage
    {
        public int Id { get; set; }
        public int BooksId { get; set; }
        public Book Book { get; set; }
        public int Quantity { get; set; } 
    }

}
