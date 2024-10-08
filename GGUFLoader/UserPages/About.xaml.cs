﻿using GGUFLoader.UserPages.Tips;
using System;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using MessageBox = iNKORE.UI.WPF.Modern.Controls.MessageBox;

namespace GGUFLoader.UserPages
{
    /// <summary>
    /// About.xaml 的交互逻辑
    /// </summary>
    public partial class About : Page
    {
        public About()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow.NavigateToPage(new Tip_ViewDevelopers());
        }
    }
}
