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
    public partial class RotatePagesViewModel : ObservableObject
    {
        private readonly PdfService _pdfService;

        public RotatePagesViewModel()
        {
            _pdfService = new PdfService();
            Files = new ObservableCollection<StorageFile>();
            // Default settings
            SelectedRotation = 90; 
            SelectedScope = RotationScope.All;
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
                    RotateCommand.NotifyCanExecuteChanged();
                }
            }
        }

        private string _statusMessage;
        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }

        // 90, 180, 270
        private int _selectedRotation;
        public int SelectedRotation
        {
            get => _selectedRotation;
            set => SetProperty(ref _selectedRotation, value);
        }

        private RotationScope _selectedScope;
        public RotationScope SelectedScope
        {
            get => _selectedScope;
            set 
            {
                if (SetProperty(ref _selectedScope, value))
                {
                    OnPropertyChanged(nameof(IsSpecificRangeVisible));
                }
            }
        }

        private string _specificRange;
        public string SpecificRange
        {
            get => _specificRange;
            set => SetProperty(ref _specificRange, value);
        }

        public bool IsSpecificRangeVisible => SelectedScope == RotationScope.Specific;

        public void AddFiles(IEnumerable<StorageFile> newFiles)
        {
            foreach (var file in newFiles)
            {
                if (!Files.Any(f => f.Path == file.Path))
                {
                    Files.Add(file);
                }
            }
            RotateCommand.NotifyCanExecuteChanged();
        }

        [RelayCommand(CanExecute = nameof(CanRotate))]
        private async Task RotateAsync()
        {
            if (CanRotate())
            {
                await PerformRotation();
            }
        }

        private bool CanRotate()
        {
            return !IsBusy && Files != null && Files.Count > 0;
        }

        private async Task PerformRotation()
        {
            try
            {
                IsBusy = true;
                StatusMessage = "Processing...";
                
                int successCount = 0;
                int failCount = 0;

                await Task.Run(() =>
                {
                    var currentFiles = Files.ToList();
                    foreach (var file in currentFiles)
                    {
                        try
                        {
                            string inputPath = file.Path;
                            string directory = System.IO.Path.GetDirectoryName(inputPath);
                            string filename = System.IO.Path.GetFileNameWithoutExtension(inputPath);
                            string extension = System.IO.Path.GetExtension(inputPath);
                            
                            // e.g. "Doc_rotated90.pdf"
                            string suffix = $"_rotated{SelectedRotation}";
                            string outputPath = System.IO.Path.Combine(directory, $"{filename}{suffix}{extension}");

                            _pdfService.RotatePages(inputPath, outputPath, SelectedRotation, SelectedScope, SpecificRange);
                            successCount++;
                        }
                        catch
                        {
                            failCount++;
                        }
                    }
                });

                StatusMessage = $"Done. Processed: {successCount}, Failed: {failCount}";
                Files.Clear();
                RotateCommand.NotifyCanExecuteChanged();
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
