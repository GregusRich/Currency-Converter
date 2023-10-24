using Currency_Converter.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;

namespace Currency_Converter.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        // ObservableCollection to store the available currencies and display them to the View
        public ObservableCollection<string> AvailableCurrencies { get; set; } = new ObservableCollection<string>();

        // Backing fields 
        private string _selectedCurrency;
        public string SelectedCurrency
        {
            get { return _selectedCurrency; }
            set
            {
                if (_selectedCurrency != value)
                {
                    _selectedCurrency = value;
                    OnPropertyChanged(nameof(SelectedCurrency));
                }
            }
        }

        // INotifyPropertyChanged implementation
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        // Fetch the latest currency data
        public async void FetchDataAsync()
        {
            // For now, use the stored JSON data for development as to not ping the API 
            var jsonData = "CurrencyData.json";  // Note: You might need to read the content of this file first.

            CurrencyData currencyData = JsonConvert.DeserializeObject<CurrencyData>(jsonData);

            foreach (var rate in currencyData.Rates)
            {
                AvailableCurrencies.Add(rate.Key);
            }
        }




    }
}
