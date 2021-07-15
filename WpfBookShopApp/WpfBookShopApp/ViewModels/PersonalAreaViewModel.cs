using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WpfBookShopApp.Messages;
using WpfBookShopApp.Models;
using WpfBookShopApp.Services;
using WpfBookShopApp.Views;

namespace WpfBookShopApp.ViewModels
{
    public class PersonalAreaViewModel : ViewModelBase
    {
        private IMessenger messanger;
        private readonly IBookShopRepository bookShopRepository;

        public User User { get; set; }

        public PersonalAreaViewModel(IMessenger messanger, IBookShopRepository bookShopRepository)
        {
            this.messanger = messanger;
            this.bookShopRepository = bookShopRepository;
            messanger.Register<LoginUserMessage>(this, msg=>
            {
                User = msg.User;
            });

            messanger.Register<UpdateData>(this, (obj) =>
            {
                UpdateDate();
                if (User.Position.Name.Equals("Admin")) IsAdmin = Visibility.Visible; 
                else  IsAdmin = Visibility.Collapsed;
                 

            }); 
        } 
        public Visibility IsAdmin { get; set; } 



        private RelayCommand<Book> sellCommand = null;

        public RelayCommand<Book> SellCommand => sellCommand ??= new RelayCommand<Book>(
        (Book) =>
        {
            bookShopRepository.DeleteBookByName(Book.Name);
            User.Money += Book.PrimeCost; 
            bookShopRepository.UpdateUser(User); 
            MessageBox.Show("You put the book up for sale money will be transferred as soon as the book is sold","Info",MessageBoxButton.OK,MessageBoxImage.Information);
            messanger.Send(new UpdateData());
        });

        private RelayCommand<string> removeCommand = null;

        public RelayCommand<string> RemoveCommand => removeCommand ??= new RelayCommand<string>(
        (name) =>
        {
            bookShopRepository.DeleteBookByName(name);
            messanger.Send(new UpdateData());
        });
        public void UpdateDate()
        {
            if (User != null)
            {
                AddedBooks = new ObservableCollection<Book>(bookShopRepository.GetBooksByUserOwner(User)); 
            }
        }
         
        public ObservableCollection<Book> AddedBooks { get; set; }

        private RelayCommand logoutCommand = null;

        public RelayCommand LogoutCommand => logoutCommand ??= new RelayCommand(
        () =>
        {
            messanger.Send(new NavigationMessage { ViewModelType = typeof(LoginViewModel) });
            messanger.Send(new UpdateData());
        });

        private RelayCommand shopCommand = null;

        public RelayCommand ShopCommand => shopCommand ??= new RelayCommand(
        () =>
        {
            messanger.Send(new NavigationMessage { ViewModelType = typeof(ShopViewModel) });
            messanger.Send(new UpdateData());
        });

        private RelayCommand adminCommand = null;

        public RelayCommand AdminCommand => adminCommand ??= new RelayCommand(
        () =>
        {
           messanger.Send(new UpdateData());
           messanger.Send(new NavigationMessage { ViewModelType = typeof(AdminViewModel) }); 
        });

        private RelayCommand favoritCommand = null;

        public RelayCommand FavoritCommand => favoritCommand ??= new RelayCommand(
        () =>
        {
            messanger.Send(new NavigationMessage { ViewModelType = typeof(FavoritViewModel) }); 
            messanger.Send(new UpdateData());
        });

        private RelayCommand addBookCommand = null;

        public RelayCommand AddBookCommand => addBookCommand ??= new RelayCommand(
        () =>
        {
            App.Services.GetInstance<AddBookViewModel>().UpdateData(); 
            messanger.Send(new NavigationMessage { ViewModelType = typeof(AddBookViewModel) }); 
        });

    }
}
