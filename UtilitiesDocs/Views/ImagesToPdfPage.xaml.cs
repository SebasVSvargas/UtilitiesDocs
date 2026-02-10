using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using Windows.Storage.Pickers;
using UtilitiesDocs.ViewModels;
using WinRT.Interop;

namespace UtilitiesDocs.Views
{
    public sealed partial class ImagesToPdfPage : Page
    {
        public ImagesToPdfPage()
        {
            this.InitializeComponent();
        }

        private async void OnAddFilesClicked(object sender, RoutedEventArgs e)
        {
            var window = (Application.Current as App)?.MainWindow;
            if (window == null) return;

            var picker = new FileOpenPicker();
            picker.ViewMode = PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".png");
            picker.FileTypeFilter.Add(".bmp");

            var hwnd = WindowNative.GetWindowHandle(window);
            InitializeWithWindow.Initialize(picker, hwnd);

            var files = await picker.PickMultipleFilesAsync();
            if (files != null && files.Count > 0)
            {
                ViewModel.AddFiles(files);
            }
        }
        
        private async void OnConvertClicked(object sender, RoutedEventArgs e)
        {
            if (ViewModel.Files.Count == 0) return;

             var window = (Application.Current as App)?.MainWindow;
            if (window == null) return;

            var picker = new FileSavePicker();
            picker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            picker.FileTypeChoices.Add("PDF Document", new List<string>() { ".pdf" });
            picker.SuggestedFileName = "ImagesDocument";

            var hwnd = WindowNative.GetWindowHandle(window);
            InitializeWithWindow.Initialize(picker, hwnd);

            var file = await picker.PickSaveFileAsync();
            if (file != null)
            {
                await ViewModel.PerformConversion(file.Path);
            }
        }

        private void OnClearClicked(object sender, RoutedEventArgs e)
        {
            ViewModel.Files.Clear();
        }
    }
}
