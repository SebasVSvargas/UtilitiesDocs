using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using UtilitiesDocs.Services.Pdf;
using Windows.Storage;

namespace UtilitiesDocs.ViewModels
{
    public partial class ImagesToPdfViewModel : ObservableObject
    {
        private readonly PdfService _pdfService;

        public ImagesToPdfViewModel()
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

        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set 
            {
                if (SetProperty(ref _isBusy, value))
                {
                    ConvertCommand.NotifyCanExecuteChanged();
                }
            }
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
                if (!Files.Any(f => f.Path == file.Path))
                {
                    Files.Add(file);
                }
            }
            ConvertCommand.NotifyCanExecuteChanged();
        }

        private bool CanConvert() => !IsBusy && Files != null && Files.Count > 0;

        [RelayCommand(CanExecute = nameof(CanConvert))]
        private async Task ConvertAsync()
        {
            await Task.CompletedTask;
        }

        public async Task PerformConversion(string outputPath)
        {
            try
            {
                IsBusy = true;
                StatusMessage = "Processing...";
                
                var paths = Files.Select(f => f.Path).ToList();
                
                await Task.Run(() => _pdfService.ImagesToPdf(paths, outputPath));

                StatusMessage = "Completed successfully!";
                
                try 
                {
                    var file = await StorageFile.GetFileFromPathAsync(outputPath);
                    await Windows.System.Launcher.LaunchFileAsync(file);
                }
                catch { /* Ignore */ }

                Files.Clear();
                ConvertCommand.NotifyCanExecuteChanged();
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
