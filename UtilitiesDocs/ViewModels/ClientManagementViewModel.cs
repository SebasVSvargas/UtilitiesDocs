using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Threading.Tasks;
using UtilitiesDocs.Models;
using System.Linq;
using System;

namespace UtilitiesDocs.ViewModels
{
    public partial class ClientManagementViewModel : ObservableObject
    {
        private List<Client> _allClients;

        public ClientManagementViewModel()
        {
            _allClients = new List<Client>();
            Clients = new ObservableCollection<Client>();
            UpdateUIState();
        }

        private ObservableCollection<Client> _clients;
        public ObservableCollection<Client> Clients
        {
            get => _clients;
            set => SetProperty(ref _clients, value);
        }

        private string _searchText;
        public string SearchText
        {
            get => _searchText;
            set
            {
                if (SetProperty(ref _searchText, value))
                {
                    PerformSearch();
                }
            }
        }

        private void PerformSearch()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                if (Clients.Count != _allClients.Count)
                {
                    Clients = new ObservableCollection<Client>(_allClients);
                }
            }
            else
            {
                var lowerQuery = SearchText.ToLower();
                var filtered = _allClients.Where(c => 
                    c.FullName.ToLower().Contains(lowerQuery) || 
                    c.IdNumber.Contains(lowerQuery) ||
                    c.PhoneNumber.Contains(lowerQuery)).ToList();
                
                Clients = new ObservableCollection<Client>(filtered);
            }
        }
        
        private Client? _selectedClient;
        public Client? SelectedClient
        {
            get => _selectedClient;
            set
            {
                if (SetProperty(ref _selectedClient, value))
                {
                    if (value != null)
                    {
                        // Populate fields for editing
                        FirstName = value.FirstName;
                        LastName = value.LastName;
                        IdNumber = value.IdNumber;
                        PhoneNumber = value.PhoneNumber;
                        Address = value.Address;
                        BirthYear = value.BirthYear;
                        StatusMessage = "Editing client.";
                    }
                    else
                    {
                        ClearFields();
                    }
                    UpdateUIState();
                }
            }
        }

        private bool _isEditing;
        public bool IsEditing
        {
            get => _isEditing;
            set => SetProperty(ref _isEditing, value);
        }

        private string _firstName;
        public string FirstName
        {
            get => _firstName;
            set => SetProperty(ref _firstName, value);
        }

        private string _lastName;
        public string LastName
        {
            get => _lastName;
            set => SetProperty(ref _lastName, value);
        }

        private string _idNumber;
        public string IdNumber
        {
            get => _idNumber;
            set => SetProperty(ref _idNumber, value);
        }

        private string _phoneNumber;
        public string PhoneNumber
        {
            get => _phoneNumber;
            set => SetProperty(ref _phoneNumber, value);
        }

        private string _address;
        public string Address
        {
            get => _address;
            set => SetProperty(ref _address, value);
        }

        private int _birthYear;
        public int BirthYear
        {
            get => _birthYear;
            set => SetProperty(ref _birthYear, value);
        }

        private string _statusMessage;
        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }

        [RelayCommand]
        private void SaveClient()
        {
            if (string.IsNullOrWhiteSpace(FirstName) || string.IsNullOrWhiteSpace(LastName) || string.IsNullOrWhiteSpace(IdNumber))
            {
                StatusMessage = "Please enter First Name, Last Name and ID.";
                return;
            }

            if (SelectedClient == null)
            {
                // Add new client
                var newClient = new Client
                {
                    Id = Guid.NewGuid(),
                    FirstName = FirstName,
                    LastName = LastName,
                    IdNumber = IdNumber,
                    PhoneNumber = PhoneNumber,
                    Address = Address,
                    BirthYear = BirthYear
                };

                _allClients.Add(newClient);
                PerformSearch();
                StatusMessage = "Client added successfully.";
                ClearFields();
            }
            else
            {
                // Update existing client
                var index = _allClients.FindIndex(c => c.Id == SelectedClient.Id);
                if (index >= 0)
                {
                    var updatedClient = new Client
                    {
                        Id = SelectedClient.Id,
                        FirstName = FirstName,
                        LastName = LastName,
                        IdNumber = IdNumber,
                        PhoneNumber = PhoneNumber,
                        Address = Address,
                        BirthYear = BirthYear
                    };
                    
                    _allClients[index] = updatedClient;
                    SelectedClient = null; // Exit edit mode
                    PerformSearch();
                    StatusMessage = "Client updated successfully.";
                }
            }
        }

        [RelayCommand]
        private void DeleteClient()
        {
            if (SelectedClient != null)
            {
                var clientToRemove = _allClients.FirstOrDefault(c => c.Id == SelectedClient.Id);
                if (clientToRemove != null)
                {
                    _allClients.Remove(clientToRemove);
                }
                
                SelectedClient = null;
                PerformSearch();
                StatusMessage = "Client deleted.";
            }
        }

        [RelayCommand]
        private void Cancel()
        {
            SelectedClient = null;
            StatusMessage = "Operation cancelled.";
        }

        private void ClearFields()
        {
            FirstName = string.Empty;
            LastName = string.Empty;
            IdNumber = string.Empty;
            PhoneNumber = string.Empty;
            Address = string.Empty;
            BirthYear = 0;
        }

        private void UpdateUIState()
        {
            IsEditing = SelectedClient != null;
        }
    }
}
