using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WpfBookShopApp.Messages;
using WpfBookShopApp.Models;
using WpfBookShopApp.Services;

namespace WpfBookShopApp.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        private IMessenger messanger;
        private IBookShopRepository IBookShopRepository;

        private string login;
        private string password;

        public string Login
        {
            get { return login; }
            set { login = value; Check(); }
        }
        

        public string Password
        {
            get { return password; }
            set { password = value; Check(); }
        }
         
        public Visibility Visibility { get; set; } 
        public bool IsEnable { get; set; }
        public LoginViewModel(IMessenger messanger, IBookShopRepository IBookShopRepository)
        {
            this.messanger = messanger;
            this.IBookShopRepository = IBookShopRepository;  
        }

        private RelayCommand loginCommand = null;

        public RelayCommand LoginCommand => loginCommand ??= new RelayCommand(
        () =>
        {
            UnicodeEncoding unicodeEncoding = new UnicodeEncoding();
            var sha256 = SHA256.Create();
            byte[] secret = sha256.ComputeHash(unicodeEncoding.GetBytes(Password));
            Password = Convert.ToBase64String(secret);

            var user = IBookShopRepository.GetUserByLoginAndPassword(Login, Password);

            if (user != null)
            {
                messanger.Send(new LoginUserMessage() { User = user });

                messanger.Send(new UpdateData());
                messanger.Send(new NavigationMessage() { ViewModelType = typeof(PersonalAreaViewModel) }); 
                Clear();
            }
            else
            {
                MessageBox.Show("Incorrect Login or Password!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            
        });

        private RelayCommand registrationCommand = null;

        public RelayCommand RegistrationCommand => registrationCommand ??= new RelayCommand(
        () =>
        {
            Clear();
            messanger.Send(new NavigationMessage { ViewModelType = typeof(RegistrationViewModel) });
        });

        void Clear()
        {
            Password = string.Empty;
            Login = string.Empty;
        }

        void Check()
        {
            bool isCorrect = true;

            if (string.IsNullOrWhiteSpace(Password))
            {
                isCorrect = false;
                Visibility = Visibility.Visible;
            }

            if (string.IsNullOrWhiteSpace(Login))
            {
                isCorrect = false;
                Visibility = Visibility.Visible;
            }

            if (isCorrect)
                Visibility = Visibility.Collapsed;

            IsEnable = isCorrect;
        }

    }
}
