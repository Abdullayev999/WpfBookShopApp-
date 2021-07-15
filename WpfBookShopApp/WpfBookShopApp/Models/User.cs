using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfBookShopApp.Models
{
    public class User
    {
        public int Id { get; set; }
        public int PositionId { get; set; }
        public string FullName { get; set; }
        public DateTime DateOfBith { get; set; }
        public string Gender { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string Password { get; set; } 
        public  Position Position { get; set; }
        public int Money { get; set; }


        public bool Moderator { get; set; } = false;
         


        private int isModerator;

        public int IsModerator
        {
            get { return isModerator; }
            set { isModerator = value;
                if (isModerator == 1)
                    Moderator = true;
                else
                    Moderator = false;
            }
        }


        //
        //
        //
        //

    }
}
