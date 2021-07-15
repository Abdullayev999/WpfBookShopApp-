using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WpfBookShopApp.Messages;
using WpfBookShopApp.Models;
using WpfBookShopApp.Services;

namespace WpfBookShopApp.ViewModels
{
    public class FavoritViewModel : ViewModelBase
    {
        private IMessenger messanger;
        private IBookShopRepository bookShopRepository;
        public User User { get; set; }

        private RelayCommand adminCommand = null;

        public RelayCommand AdminCommand => adminCommand ??= new RelayCommand(
        () =>
        {
            messanger.Send(new UpdateData());
            messanger.Send(new NavigationMessage { ViewModelType = typeof(AdminViewModel) });
        });

        public FavoritViewModel(IMessenger messanger, IBookShopRepository bookShopRepository)
        {
            this.messanger = messanger;
            this.bookShopRepository = bookShopRepository;

            messanger.Register<LoginUserMessage>(this, msg =>
            {
                User = msg.User;
            });

            messanger.Register<UpdateData>(this, (obj) =>
            {
                UpdateDate();
                if (User.Position.Name.Equals("Admin")) IsAdmin = Visibility.Visible;
                else IsAdmin = Visibility.Collapsed;
            });
        }


              
        public Visibility IsAdmin { get; set; } 

        private RelayCommand<int> removeCommand = null;

        public RelayCommand<int> RemoveCommand => removeCommand ??= new RelayCommand<int>(
        (id) =>
        { 
            bookShopRepository.DeleteBookBuying(id);
            messanger.Send(new UpdateData());
        });
        private RelayCommand logoutCommand = null;

        public RelayCommand LogoutCommand => logoutCommand ??= new RelayCommand(
        () =>
        {
            messanger.Send(new NavigationMessage { ViewModelType = typeof(LoginViewModel) });
        });


        private RelayCommand personalAreaCommand = null;

        public RelayCommand PersonalAreaCommand => personalAreaCommand ??= new RelayCommand(
        () =>
        {
            messanger.Send(new NavigationMessage { ViewModelType = typeof(PersonalAreaViewModel) });
        });

        private RelayCommand<BuyingBooks> sellCommand = null;

        public RelayCommand<BuyingBooks> SellCommand => sellCommand ??= new RelayCommand<BuyingBooks>(
        (Book) =>
        {
            bookShopRepository.DeleteBookBuying(Book.BooksId);
            User.Money += Book.Book.PrimeCost;
            bookShopRepository.UpdateUser(User);
            MessageBox.Show("You put the book up for sale money will be transferred as soon as the book is sold", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
            UpdateDate();
        });

        private RelayCommand shopCommand = null;

        public RelayCommand ShopCommand => shopCommand ??= new RelayCommand(
        () =>
        {
            messanger.Send(new NavigationMessage { ViewModelType = typeof(ShopViewModel) });
        });

        public void UpdateDate()
        {
            if (User != null)
            { 

                BuyingBooks = new ObservableCollection<BuyingBooks>(bookShopRepository.GetBooksByUserBuying(User));
            }
        }

        public ObservableCollection<BuyingBooks> BuyingBooks { get; set; }
    }
}
