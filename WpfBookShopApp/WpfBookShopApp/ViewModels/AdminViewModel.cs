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
using WpfBookShopApp.Views;

namespace WpfBookShopApp.ViewModels
{
    public class AdminViewModel : ViewModelBase
    {
        private IMessenger messanger;
        private readonly IBookShopRepository bookShopRepository;
        public ObservableCollection<User> Users { get; set; }
        public AdminViewModel(IBookShopRepository bookShopRepository, IMessenger messanger)
        {
            this.messanger = messanger;
            this.bookShopRepository = bookShopRepository;
            messanger.Register<UpdateData>(this, (obj) =>
            {
                UpdateData();
            }); 
        }
         

        void UpdateData()
        {
            Users = new ObservableCollection<User>(bookShopRepository.GetUsers());
        }

        private RelayCommand<User> deleteCommand = null;

        public RelayCommand<User> DeleteCommand => deleteCommand ??= new RelayCommand<User>(
        (user) =>
        {
            if (user.Position.Name.Equals("Admin"))
            {
                MessageBox.Show("You cannot delete the admin", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                bookShopRepository.DeleteUserById(user.Id);
                UpdateData();
            }
        });

        private RelayCommand backCommand = null;

        public RelayCommand BackCommand => backCommand ??= new RelayCommand(
        () =>
        {
           
            messanger.Send(new NavigationMessage() { ViewModelType = typeof(PersonalAreaViewModel) }) ;
        });


        private RelayCommand<User> moderatorCommand = null;

        public RelayCommand<User> ModeratorCommand => moderatorCommand ??= new RelayCommand<User>(
        (user) =>
        { 
            if (!user.Position.Equals("Admin"))
            {
                user.Moderator = !user.Moderator;
                if (user.Moderator)
                    user.IsModerator = 1;
                else
                    user.IsModerator = 0;


                bookShopRepository.UpdateUser(user);
                UpdateData();
            }
            else
            {
                MessageBox.Show("You cannot change moderator to admin", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }); 
    }
}
