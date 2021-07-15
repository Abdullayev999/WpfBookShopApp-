using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using SimpleInjector;
using System;
using System.Collections.Generic; 
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using WpfBookShopApp.Services;
using WpfBookShopApp.ViewModels;

namespace WpfBookShopApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static Container Services { get; set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            RegisterServices(); 
              Start<LoginViewModel>(); ;
        }

        private void Start<T>() where T : ViewModelBase
        {
            var windowViewModel = Services.GetInstance<MainViewModel>();
            windowViewModel.CurrentViewModel = Services.GetInstance<T>();

            var mainWindow = new MainWindow { DataContext = windowViewModel };
            mainWindow.ShowDialog();
        }
        public void RegisterServices()
        {
            Services = new Container(); 
            Services.RegisterSingleton<IBookShopRepository, BookShopRepository>();
            Services.RegisterSingleton<IMessenger, Messenger>();
            Services.RegisterSingleton<MainViewModel>();
            Services.RegisterSingleton<PersonalAreaViewModel>();
            Services.RegisterSingleton<RegistrationViewModel>();
            Services.RegisterSingleton<LoginViewModel>(); 
            Services.RegisterSingleton<ShopViewModel>();
            Services.RegisterSingleton<FavoritViewModel>();
            Services.RegisterSingleton<AddBookViewModel>();
            Services.RegisterSingleton<AdminViewModel>();

            Services.Verify();
        }
    }
}
