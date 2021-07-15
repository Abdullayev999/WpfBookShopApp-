using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WpfBookShopApp.Models;
using WpfBookShopApp.Services;

namespace WpfBookShopApp.ViewModels
{
    public class AddItemViewModel : ViewModelBase
    {
        private readonly IBookShopRepository bookShopRepository;
        public MonoColumTable CurrentItem { get; set; }
        public ObservableCollection<MonoColumTable> Items { get; set; }
        public Visibility VisName { get; set; }
        public bool IsEnable { get; set; } = false;

        public string TableName { get; set; }

        private string name;

        public string Name
        {
            get { return name; }
            set { name = value; 
                if (string.IsNullOrWhiteSpace(name)) { VisName = Visibility.Visible; IsEnable = false; }
                else { VisName = Visibility.Collapsed; IsEnable = true; } 
            }
        } 

        private RelayCommand addCommand = null;

        public RelayCommand AddCommand => addCommand ??= new RelayCommand(
        () =>
        {
            try
            {
                if (TableName.Equals("Authors"))
                {
                    bookShopRepository.AddAuthor(Name);
                    Items = new ObservableCollection<MonoColumTable>(bookShopRepository.GetAuthors());
                }
                else if (TableName.Equals("Genres"))
                {
                    bookShopRepository.AddGenre(Name);
                    Items = new ObservableCollection<MonoColumTable>(bookShopRepository.GetGenres());
                }
                else if (TableName.Equals("Publishers"))
                {
                    bookShopRepository.AddPublisher(Name);
                    Items = new ObservableCollection<MonoColumTable>(bookShopRepository.GetPublishers());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message,"Error",MessageBoxButton.OK,MessageBoxImage.Error);
            }

            Name = string.Empty;
        });


        private RelayCommand removeCommand = null;

        public RelayCommand RemoveCommand => removeCommand ??= new RelayCommand(
        () =>
        {

            if (CurrentItem!= null)
            {
                if (TableName.Equals("Authors"))
                {
                    var item = CurrentItem as Author;
                    bookShopRepository.DeleteAuthor(item.Name);
                    Items = new ObservableCollection<MonoColumTable>(bookShopRepository.GetAuthors());
                }
                else if (TableName.Equals("Genres"))
                {
                    var item = CurrentItem as Genre;
                    bookShopRepository.DeleteGenre(item.Name);
                    Items = new ObservableCollection<MonoColumTable>(bookShopRepository.GetGenres());
                }
                else if (TableName.Equals("Publishers"))
                {
                    var item = CurrentItem as Publisher;
                    bookShopRepository.DeletePublisher(item.Name);
                    Items = new ObservableCollection<MonoColumTable>(bookShopRepository.GetPublishers());
                }
            }

            Name = string.Empty;
        });

        public AddItemViewModel(IBookShopRepository bookRepository,string tableName, ObservableCollection<MonoColumTable> Items)
        {
            this.Items = Items;
            this.bookShopRepository = bookRepository;
            this.TableName = tableName;

            if (TableName.Equals("Authors"))
            {
                Items = new ObservableCollection<MonoColumTable>(bookRepository.GetAuthors());
            }
            else if (TableName.Equals("Genres"))
            {
                Items = new ObservableCollection<MonoColumTable>(bookRepository.GetGenres());
            }
            else if (TableName.Equals("Publishers"))
            {
                Items = new ObservableCollection<MonoColumTable>(bookRepository.GetPublishers());
            }
        } 
    }
}
