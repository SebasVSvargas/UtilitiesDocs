using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using UtilitiesDocs.Services.Pdf;
using Windows.Storage;
using System.Linq;
using System.Collections.Generic;

namespace UtilitiesDocs.ViewModels
{
    public partial class RemovePasswordViewModel : ObservableObject
    {
        private readonly PdfService _pdfService;

        public RemovePasswordViewModel()
        {
            _pdfService = new PdfService();
            Files = new ObservableCollection<StorageFile>();
        }

        private ObservableCollection<StorageFile> _files;
        public ObservableCollection<StorageFile> Files
        {
            get => _files;
            set => SetProperty(ref _files, value);
        }

        private string _password;
        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }

        private string _statusMessage;
        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }

        public void AddFiles(IEnumerable<StorageFile> newFiles)
        {
            foreach (var file in newFiles)
            {
                Files.Add(file);
            }
        }

        [RelayCommand]
        private async Task RemovePasswordsAsync()
        {
            if (string.IsNullOrWhiteSpace(Password))
            {
                StatusMessage = "Please enter a password.";
                return;
            }

            if (Files.Count == 0)
            {
                StatusMessage = "No files selected.";
                return;
            }

            IsBusy = true;
            StatusMessage = "Processing...";

            int successCount = 0;
            int failCount = 0;

            await Task.Run(() =>
            {
                var currentFiles = Files.ToList();
                
                foreach (var file in currentFiles)
                {
                    string inputPath = file.Path;
                    string directory = System.IO.Path.GetDirectoryName(inputPath);
                    string filename = System.IO.Path.GetFileNameWithoutExtension(inputPath);
                    string extension = System.IO.Path.GetExtension(inputPath);
                    string outputPath = System.IO.Path.Combine(directory, $"{filename}_unlocked{extension}");

                    bool result = _pdfService.RemovePassword(inputPath, outputPath, Password);
                    if (result) successCount++;
                    else failCount++;
                }
            });

            StatusMessage = $"Completed. Success: {successCount}, Failed: {failCount}";
            IsBusy = false;
        }
    }
}
