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
    public partial class MergePdfViewModel : ObservableObject
    {
        private readonly PdfService _pdfService;

        public MergePdfViewModel()
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
                    MergeCommand.NotifyCanExecuteChanged();
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
            MergeCommand.NotifyCanExecuteChanged();
        }

        private bool CanMerge() => !IsBusy && Files != null && Files.Count > 0;

        [RelayCommand(CanExecute = nameof(CanMerge))]
        private async Task MergeAsync()
        {
            await Task.CompletedTask;
        }

        public async Task PerformMerge(string outputPath)
        {
             try
            {
                IsBusy = true;
                StatusMessage = "Processing...";
                
                var paths = Files.Select(f => f.Path).ToList();
                
                await Task.Run(() => _pdfService.MergePdfs(paths, outputPath));

                StatusMessage = "Completed successfully!";
                Files.Clear();
                MergeCommand.NotifyCanExecuteChanged();
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
