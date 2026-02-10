using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using Windows.Storage.Pickers;
using UtilitiesDocs.ViewModels;
using UtilitiesDocs.Services.Pdf;
using WinRT.Interop;

namespace UtilitiesDocs.Views
{
    public sealed partial class RotatePagesPage : Page
    {
        public RotatePagesPage()
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

        private void OnClearClicked(object sender, RoutedEventArgs e)
        {
            ViewModel.Files.Clear();
        }

        private void RotationCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ViewModel == null) return;
            if (RotationCombo.SelectedItem is ComboBoxItem item && item.Tag != null)
            {
                if (int.TryParse(item.Tag.ToString(), out int angle))
                {
                    ViewModel.SelectedRotation = angle;
                }
            }
        }

        private void ScopeCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ViewModel == null) return;
            if (ScopeCombo.SelectedItem is ComboBoxItem item && item.Tag != null)
            {
               if (Enum.TryParse<RotationScope>(item.Tag.ToString(), out var scope))
               {
                   ViewModel.SelectedScope = scope;
               }
            }
        }
    }
}
