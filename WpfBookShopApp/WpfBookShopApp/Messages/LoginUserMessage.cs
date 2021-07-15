using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfBookShopApp.Models;

namespace WpfBookShopApp.Messages
{
    public class LoginUserMessage : Messenger
    {
        public User User { get; set; }
    }
}
