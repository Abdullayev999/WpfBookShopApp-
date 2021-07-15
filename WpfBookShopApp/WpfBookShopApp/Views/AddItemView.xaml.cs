﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WpfBookShopApp.ViewModels;

namespace WpfBookShopApp.Views
{
    /// <summary>
    /// Логика взаимодействия для AddItemView.xaml
    /// </summary>
    public partial class AddItemView : Window
    {
        public AddItemView()
        {
            InitializeComponent(); 
        }
         

        private void Cancel(object sender, RoutedEventArgs e)
        { 
            Close();
        }
    }
}
