using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using Windows.Storage.Pickers;
using UtilitiesDocs.ViewModels;
using WinRT.Interop;

namespace UtilitiesDocs.Views
{
    public sealed partial class MergePdfPage : Page
    {
        public MergePdfPage()
        {
            this.InitializeComponent();
        }

        private async void OnAddFilesClicked(object sender, RoutedEventArgs e)
        {
            var window = (Application.Current as App)?.MainWindow;
            if (window == null) return;

            var picker = new FileOpenPicker();
            picker.ViewMode = PickerViewMode.List;
            picker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            picker.FileTypeFilter.Add(".pdf");

            var hwnd = WindowNative.GetWindowHandle(window);
            InitializeWithWindow.Initialize(picker, hwnd);

            var files = await picker.PickMultipleFilesAsync();
            if (files != null && files.Count > 0)
            {
                ViewModel.AddFiles(files);
            }
        }
        
        private async void OnMergeClicked(object sender, RoutedEventArgs e)
        {
            if (ViewModel.Files.Count == 0) return;

             var window = (Application.Current as App)?.MainWindow;
            if (window == null) return;

            var picker = new FileSavePicker();
            picker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            picker.FileTypeChoices.Add("PDF Document", new List<string>() { ".pdf" });
            picker.SuggestedFileName = "MergedDocument";

            var hwnd = WindowNative.GetWindowHandle(window);
            InitializeWithWindow.Initialize(picker, hwnd);

            var file = await picker.PickSaveFileAsync();
            if (file != null)
            {
                await ViewModel.PerformMerge(file.Path);
            }
        }

        private void OnClearClicked(object sender, RoutedEventArgs e)
        {
            ViewModel.Files.Clear();
        }
    }
}
