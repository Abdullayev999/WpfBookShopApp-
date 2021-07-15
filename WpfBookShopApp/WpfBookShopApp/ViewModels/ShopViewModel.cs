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
    public class ShopViewModel : ViewModelBase
    {
        private IMessenger messanger;
        private IBookShopRepository bookShopRepository;
        public User User { get; set; } 
        public Visibility IsAdmin { get; set; }
        public ShopViewModel(IMessenger messanger, IBookShopRepository bookShopRepository)
        {
            this.messanger = messanger;
            this.bookShopRepository = bookShopRepository;
            Selects = new ObservableCollection<string>() { "Book","Author" , "Ganre"};
            messanger.Register<LoginUserMessage>(this, msg =>
            {
                User = msg.User;
                if (User.Position.Name.Equals("Admin")) IsAdmin = Visibility.Visible;
                else IsAdmin = Visibility.Collapsed;
            });
            ResultTop = new ObservableCollection<Book>();
            Sorting = new ObservableCollection<string>() { "Sels", "New", "Author", "Genre" };
        }
        public ObservableCollection<string> Sorting { get; set; }
        public ObservableCollection<Book> ResultTop { get; set; }
        public ObservableCollection<Storage> Books { get; set; } 
        public Visibility visName { get; set; }
        private string name;
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                if (string.IsNullOrWhiteSpace(name)) visName = Visibility.Visible;
                else visName = Visibility.Collapsed; Check();
            }
        }
         

        private int selectSearch = -1;

        public int SelectSearch
        {
            get { return selectSearch; }
            set { selectSearch = value; Check(); }
        }

        private int selectIndex = -1;

        public int SelectIndex
        {
            get { return selectIndex; }
            set { selectIndex = value; TopBooks(); }
        }


        private void TopBooks()
        { 
            if (SelectIndex == 0)
            { 
                ResultTop = new ObservableCollection<Book>(bookShopRepository.GetTopSellBook());
            }
            else if (SelectIndex == 1)
            { 
                ResultTop = new ObservableCollection<Book>(bookShopRepository.GetTopNewBook());
            }
            else if (SelectIndex == 2)
            { 
                ResultTop = new ObservableCollection<Book>(bookShopRepository.GetTopByAuthorBook());
            }
            else if (SelectIndex == 3)
            {
                ResultTop = new ObservableCollection<Book>(bookShopRepository.GetTopByGenreBook());
            }
        }


        public bool isEnable { get; set; }

        public void Check()
        { 
            if ( visName == Visibility.Collapsed && SelectSearch >= 0)
            {
                isEnable = true;
            }
            else
                isEnable = false;
        }

        public void Clear()
        {
            Name = string.Empty;
            SelectSearch = -1;
        }
        public ObservableCollection<string> Selects { get; set; }
        private RelayCommand searchCommand = null;

        public RelayCommand SearchCommand => searchCommand ??= new RelayCommand(
        () =>  UpdateData());


        private RelayCommand<Book> buyCommand = null;

        public RelayCommand<Book> BuyCommand => buyCommand ??= new RelayCommand<Book>(
        (book) =>
        {
            try
            {
                if (book.SellCost <= User.Money)
                {
                    bookShopRepository.BuyBook(book, User); 

                    User.Money -= book.SellCost;
                    bookShopRepository.UpdateUser(User);
                    UpdateData();
                    messanger.Send(new UpdateData()); 
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        });

        public void UpdateData()
        {
            switch (selectSearch)
            {
                case 0:
                    Books = new ObservableCollection<Storage>(bookShopRepository.GetBooksSearchByName(Name));
                    break;
                case 1:
                    Books = new ObservableCollection<Storage>(bookShopRepository.GetBooksSearchByAuthor(Name));
                    break;
                case 2:
                    Books = new ObservableCollection<Storage>(bookShopRepository.GetBooksSearchByGenre(Name));
                    break;
                default:
                    break;
            };
        }

        private RelayCommand logoutCommand = null;

        public RelayCommand LogoutCommand => logoutCommand ??= new RelayCommand(
        () =>
        { 
            messanger.Send(new NavigationMessage { ViewModelType = typeof(LoginViewModel) });
        });


        private RelayCommand favoritCommand = null;

        public RelayCommand FavoritCommand => favoritCommand ??= new RelayCommand(
        () =>
        {
            UpdateData();
            messanger.Send(new NavigationMessage { ViewModelType = typeof(FavoritViewModel) });
        });

        private RelayCommand personalAreaCommand = null;

        public RelayCommand PersonalAreaCommand => personalAreaCommand ??= new RelayCommand(
        () =>
        {
            UpdateData();
            messanger.Send(new NavigationMessage { ViewModelType = typeof(PersonalAreaViewModel) });
        });

        private RelayCommand adminCommand = null;

        public RelayCommand AdminCommand => adminCommand ??= new RelayCommand(
        () =>
        {
            messanger.Send(new UpdateData());
            messanger.Send(new NavigationMessage { ViewModelType = typeof(AdminViewModel) });
        });
    }
}
