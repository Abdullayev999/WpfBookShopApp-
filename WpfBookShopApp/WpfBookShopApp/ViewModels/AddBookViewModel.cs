using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System; 
using System.Collections.ObjectModel; 
using System.Windows;
using WpfBookShopApp.Messages;
using WpfBookShopApp.Models;
using WpfBookShopApp.Services;
using WpfBookShopApp.Views;

namespace WpfBookShopApp.ViewModels
{
    public class AddBookViewModel : ViewModelBase
    {
        private Author author;
        private Publisher publisher;
        private Genre genre;
        private string name;
        private int date;
        private int primCost;
        private int sellCost;
        private int quantity;
        private int pages;

        public int Pages
        {
            get { return pages; }
            set { pages = value;
                if (pages <= 0) visPages = Visibility.Visible;
                else visPages = Visibility.Collapsed; Check(); }
        }

        public int Quantity
        {
            get { return quantity; }
            set { quantity = value;
                if (quantity <= 0) visQuantity = Visibility.Visible;
                else visQuantity = Visibility.Collapsed; Check(); }
        }
        public int SellCost
        {
            get { return sellCost; }
            set { sellCost = value;
                if (sellCost <= 0) visSellCost = Visibility.Visible;
                else visSellCost = Visibility.Collapsed; Check(); }
        }
        public int PrimCost
        {
            get { return primCost; }
            set { primCost = value;
                if (primCost <= 0) visPrimCost = Visibility.Visible;
                else visPrimCost = Visibility.Collapsed; Check(); }
        }
        public int Date
        {
            get { return date; }
            set { date = value;
                if (date <= 0) visDate = Visibility.Visible;
                else visDate = Visibility.Collapsed; Check();
            }
        }
        public string Name
        {
            get { return name; }
            set { name = value;
                if (string.IsNullOrWhiteSpace(name)) visName = Visibility.Visible;
                else visName = Visibility.Collapsed; Check(); }
        }
        public Author Author
        {
            get { return author; }
            set { author = value;
                if (string.IsNullOrWhiteSpace(author?.Name)) visAuthor = Visibility.Visible;
                else visAuthor = Visibility.Collapsed; Check(); }
        }
        public Genre Genre
        {
            get { return genre; }
            set { genre = value;
                if (string.IsNullOrWhiteSpace(genre?.Name)) visGenre = Visibility.Visible;
                else visGenre = Visibility.Collapsed;
                Check(); }
        }
        public Publisher Publisher
        {
            get { return publisher; }
            set { publisher = value;
                if (string.IsNullOrWhiteSpace(publisher?.Name)) visPublisher = Visibility.Visible;
                else visPublisher = Visibility.Collapsed; Check(); }
        }

        public bool isEnable { get; set; }

        public Visibility visAuthor { get; set; }
        public Visibility visPublisher { get; set; }
        public Visibility visGenre { get; set; }
        public Visibility visName { get; set; }
        public Visibility visDate { get; set; }
        public Visibility visPrimCost { get; set; }
        public Visibility visSellCost { get; set; }
        public Visibility visQuantity { get; set; }
        public Visibility visPages { get; set; }

        public void Check()
        {
            var Collapsed = Visibility.Collapsed;
            if (visAuthor == Collapsed && visDate == Collapsed && visGenre == Collapsed &&
                visName == Collapsed && visPages == Collapsed && visPrimCost == Collapsed &&
                visPublisher == Collapsed && visQuantity == Collapsed && visSellCost == Collapsed)
            {
                isEnable = true;
            } else
                isEnable = false;
        }

        public AddBookViewModel(IMessenger messenger, IBookShopRepository bookShopRepository)
        {
            this.messenger = messenger;
            this.bookShopRepository = bookShopRepository;
            UpdateData();
            Clear();

            messenger.Register<LoginUserMessage>(this, msg =>
            {
                User = msg.User;
            });
        }
        public User User { get; set; }
        public void UpdateData()
        {
            Authors = new ObservableCollection<Author>(bookShopRepository.GetAuthors());
            Genres = new ObservableCollection<Genre>(bookShopRepository.GetGenres());
            Publishers = new ObservableCollection<Publisher>(bookShopRepository.GetPublishers());
        }

        private readonly IMessenger messenger;
        private readonly IBookShopRepository bookShopRepository;
        private RelayCommand cancelCommand = null;

        public ObservableCollection<Author> Authors { get; set; }
        public ObservableCollection<Genre> Genres { get; set; }
        public ObservableCollection<Publisher> Publishers { get; set; }

        public RelayCommand CancelCommand => cancelCommand ??= new RelayCommand(
        () =>
        {
            messenger.Send(new NavigationMessage { ViewModelType = typeof(PersonalAreaViewModel) });
        });

        private RelayCommand saveCommand = null;

        public RelayCommand SaveCommand => saveCommand ??= new RelayCommand(
        () =>
        {

            Book book = new Book
            {
                AuthorId = bookShopRepository.GetAuthorIdByName(Author.Name),
                GenreId = bookShopRepository.GetGenreIdByName(Genre.Name),
                PublisherId = bookShopRepository.GetPublisherIdByName(Publisher.Name),
                Name = Name,
                Pages = Pages,
                PublishYear = date,
                PrimeCost = PrimCost,
                SellCost = SellCost,
                AdderUserId = App.Services.GetInstance<PersonalAreaViewModel>().User.Id
            };


            try
            {
                var id = bookShopRepository.AddBook(book);
                bookShopRepository.AddBookStorage(id, Quantity);
                messenger.Send(new UpdateData());
                Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        });

        private void Clear()
        {
            Genre = null;
            Author = null;
            Publisher = null;
            Name = string.Empty;
            Date = 0;
            PrimCost = 0;
            SellCost = 0;
            Quantity = 0;
            Pages = 0;
        }

        private RelayCommand<string> changeCommand = null;

        public RelayCommand<string> ChangeCommand => changeCommand ??= new RelayCommand<string>(
        (name) =>
        {
            if (User.IsModerator == 1 || User.Position.Name.Equals("Admin"))
            {
                AddItemViewModel viewModel = null;

                if (name.Equals("Authors"))
                {
                    viewModel = new AddItemViewModel(bookShopRepository, name, new ObservableCollection<MonoColumTable>(Authors));
                }
                else if (name.Equals("Genres"))
                {
                    viewModel = new AddItemViewModel(bookShopRepository, name, new ObservableCollection<MonoColumTable>(Genres));
                }
                else if (name.Equals("Publishers"))
                {
                    viewModel = new AddItemViewModel(bookShopRepository, name, new ObservableCollection<MonoColumTable>(Publishers));
                }

                AddItemView view = new AddItemView();
                view.DataContext = viewModel;
                view.ShowDialog();
                UpdateData();
            }
            else
            {
                MessageBox.Show("No permission for moderation", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        });
    }
}
