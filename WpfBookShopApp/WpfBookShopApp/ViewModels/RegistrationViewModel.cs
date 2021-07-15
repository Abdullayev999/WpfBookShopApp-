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
using System.Windows.Controls;
using System.Windows.Input;
using WpfBookShopApp.Messages;
using WpfBookShopApp.Models;
using WpfBookShopApp.Services;

namespace WpfBookShopApp.ViewModels
{
    public class RegistrationViewModel : ViewModelBase
    {
        private IMessenger messanger;
        private IBookShopRepository bookShopRepository;

        private RelayCommand registrationCommand = null;
        private RelayCommand backCommand = null;

        private string password; 
        private string forwardPassword;
        private string email;
        private string fullName;
        private string username;
        public bool IsEnable { get; set; }
        public bool IsMale { get; set; }
        public DateTime DateOfBith { get; set; }

        public Visibility VisFullName { get; set; }
        public Visibility VisEmail { get; set; }
        public Visibility VisPassword { get; set; }
        public Visibility VisLogin { get; set; }

        public string Password
        {
            get => password;
            set { password = value; Check(); }
        } 
        public string ForwardPassword
        {
            get => forwardPassword; 
            set { forwardPassword = value; Check(); }
        }  
        public string Email
        {
            get => email;
            set { email = value; Check(); }
        } 
        public string FullName
        {
            get => fullName;
            set { fullName = value; Check();  }
        } 
        public string Username
        {
            get => username;
            set { username = value; Check(); }
        }
            
        public RegistrationViewModel(IMessenger messanger, IBookShopRepository bookShopRepository)
        {
            this.messanger = messanger;
            this.bookShopRepository = bookShopRepository;
            Clear();
        }
         
        public RelayCommand RegistrationCommand => registrationCommand ??= new RelayCommand(
        () =>
        {
            if (Password != null && ForwardPassword != null && Password.Equals(ForwardPassword))
            {
                try
                {  
                    string gender;
                    if (IsMale)
                        gender = "Male";
                    else
                        gender = "Female";

                     
                    UnicodeEncoding unicodeEncoding = new UnicodeEncoding();
                    var sha256 = SHA256.Create();
                    byte[] secret = sha256.ComputeHash(unicodeEncoding.GetBytes(Password));
                    string cryptedPassword = Convert.ToBase64String(secret);


                   
                    var user = new User() { FullName = FullName, Username = Username, DateOfBith = DateOfBith, Gender = gender, Password = cryptedPassword, Email = Email };
                    if (bookShopRepository.GetUsers().Count()==0)
                    {
                        user.PositionId = 1;
                        user.IsModerator = 1;
                    }
                    else
                    {

                        user.PositionId = 2;
                    }


                    bookShopRepository.AddUser(user);
                    messanger.Send(new NavigationMessage() { ViewModelType = typeof(LoginViewModel) });
                    Clear();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else MessageBox.Show("Passwords not equel!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

        });
         
        void Check()
        { 
            var correct = true;
            if (string.IsNullOrWhiteSpace(Password) || string.IsNullOrWhiteSpace(ForwardPassword) || !Password.Equals(ForwardPassword) )
            {
                VisPassword = Visibility.Visible;
                correct = false;
            }
            else  VisPassword = Visibility.Collapsed;
           
            if (string.IsNullOrWhiteSpace(Email))
            {
                VisEmail = Visibility.Visible;
                correct = false;
            }
            else  VisEmail = Visibility.Collapsed; 

            if (string.IsNullOrWhiteSpace(FullName))
            {
                VisFullName = Visibility.Visible;
                correct = false;
            } 
            else   VisFullName = Visibility.Collapsed; 

            if (string.IsNullOrWhiteSpace(Username))
            {
                VisLogin = Visibility.Visible;
                correct = false;
            }
            else  VisLogin = Visibility.Collapsed;  
             
            IsEnable = correct;
        }
        void Clear()
        {
            IsMale = true;
            FullName = string.Empty;
            Username = string.Empty;
            Email = string.Empty;
            IsEnable = false;
            Password = string.Empty;
            ForwardPassword = string.Empty;
            DateOfBith = DateTime.Now.AddYears(-18); 
        }

       

        public RelayCommand BackCommand => backCommand ??= new RelayCommand(
        () =>
        {
            Clear();
            messanger.Send(new NavigationMessage() { ViewModelType = typeof(LoginViewModel) });
        });
    }
}
