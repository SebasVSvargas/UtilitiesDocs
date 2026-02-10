using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using Windows.Globalization;
using System.Linq;

namespace UtilitiesDocs.ViewModels
{
    public class LanguageOption
    {
        public string Name { get; }
        public string Tag { get; }

        public LanguageOption(string name, string tag)
        {
            Name = name;
            Tag = tag;
        }
    }

    public partial class SettingsViewModel : ObservableObject
    {
        public List<LanguageOption> Languages { get; } = new List<LanguageOption>
        {
            new LanguageOption("English", "en-US"),
            new LanguageOption("Español", "es-ES")
        };

        private LanguageOption _selectedLanguage;
        public LanguageOption SelectedLanguage
        {
            get => _selectedLanguage;
            set
            {
                if (SetProperty(ref _selectedLanguage, value))
                {
                    if (value != null)
                    {
                        ApplicationLanguages.PrimaryLanguageOverride = value.Tag;
                    }
                }
            }
        }

        public SettingsViewModel()
        {
            var current = ApplicationLanguages.PrimaryLanguageOverride;
            if (string.IsNullOrEmpty(current))
            {
                // Fallback to first available match or default
                 // Look at ApplicationLanguages.Languages (calculated list)
                 var topLang = ApplicationLanguages.Languages.Count > 0 ? ApplicationLanguages.Languages[0] : "en-US";
                 current = topLang;
            }

            _selectedLanguage = Languages.FirstOrDefault(l => current.StartsWith(l.Tag)) ?? Languages[0];
        }
    }
}
