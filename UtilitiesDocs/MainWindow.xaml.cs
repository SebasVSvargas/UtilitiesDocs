using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using UtilitiesDocs.Views;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace UtilitiesDocs
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            ExtendsContentIntoTitleBar = true;
        }

        private void NavView_Loaded(object sender, RoutedEventArgs e)
        {
            // Navigate to the first item by default
            if (NavView.MenuItems.Count > 0)
            {
                NavView.SelectedItem = NavView.MenuItems[0];
            }
        }

        private void NavView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (args.IsSettingsSelected)
            {
                ContentFrame.Navigate(typeof(SettingsPage));
            }
            else
            {
                var item = args.SelectedItem as NavigationViewItem;
                if (item?.Tag is string tag)
                {
                    switch(tag)
                    {
                        case "Merge":
                            ContentFrame.Navigate(typeof(MergePdfPage));
                            break;
                        case "Unlock":
                            ContentFrame.Navigate(typeof(RemovePasswordPage));
                            break;
                        case "ImagesToPdf":
                            ContentFrame.Navigate(typeof(ImagesToPdfPage));
                            break;
                        case "Rotate":
                            ContentFrame.Navigate(typeof(RotatePagesPage));
                            break;
                    }
                }
            }
        }
    }
}
