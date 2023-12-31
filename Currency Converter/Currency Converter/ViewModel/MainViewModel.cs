﻿using Currency_Converter.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Currency_Converter.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        // ObservableCollection to store the available currencies and display them to the View
        public ObservableCollection<string> AvailableCurrencies { get; set; } = new ObservableCollection<string>();

        // Field to store the fetched currency data
        private CurrencyData _currencyData;
        
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
                    if (_currencyData != null && _currencyData.Rates.ContainsKey(_selectedCurrency))
                    // Update SelectedCurrencyRate when SelectedCurrency changes.
                    {
                        SelectedCurrencyRate = _currencyData.Rates[_selectedCurrency].ToString();
                    }
                    OnPropertyChanged(nameof(SelectedCurrency));
                }
            }
        }

        private string _selectedCurrencyRate;
        public string SelectedCurrencyRate
        {
            get => _selectedCurrencyRate;
            set
            {
                if (_selectedCurrencyRate != value)
                {
                    _selectedCurrencyRate = value;
                    OnPropertyChanged(nameof(SelectedCurrencyRate));
                }
            }
        }

        // Currency only becomes visible when the ConvertCurrencyCommand is executed
        private bool _isConvertFromCurrencyVisible = false;
        public bool IsConvertFromCurrencyVisible
        {
            get { return _isConvertFromCurrencyVisible; }
            set
            {
                if (_isConvertFromCurrencyVisible != value)
                {
                    _isConvertFromCurrencyVisible = value;
                    OnPropertyChanged(nameof(IsConvertFromCurrencyVisible));
                }
            }
        }

        // Currency that we are converting to only becomes visible when ConvertCurrencyCommand is executed
        private bool _isConvertToCurrencyVisible = false;
        public bool IsConvertToCurrencyVisible
        {
            get { return _isConvertToCurrencyVisible; }
            set
            {
                if (_isConvertToCurrencyVisible != value)
                {
                    _isConvertToCurrencyVisible = value;
                    OnPropertyChanged(nameof(IsConvertToCurrencyVisible));
                }
            }
        }


        private string _entryValue;
        public string EntryValue
        {
            get { return _entryValue; }
            set
            {
                IsConvertToCurrencyVisible = false; // Hides convert "to" currency text when value changes

                if (string.IsNullOrEmpty(value))
                {
                    IsConvertFromCurrencyVisible = false;
                }

                if (_entryValue != value)
                {
                    _entryValue = value;
                    OnPropertyChanged(nameof(EntryValue));
                }
            }
        }


        // Backing field for converting currency
        private string _convertedValue;
        public string ConvertedValue
        {
            get { return _convertedValue; }
            set
            {
                if (_convertedValue != value)
                {
                    _convertedValue = value;
                    OnPropertyChanged(nameof(ConvertedValue));
                }
            }
        }


        // Initialise MainViewModel with the iCommands
        public MainViewModel()
        {
            NumericButtonCommand = new Command<string>(OnNumericButtonPressed); // Keypad button clicks command

            ConvertCurrencyCommand = new Command(() =>
            {
                if (double.TryParse(EntryValue, out double inputValue) &&
                    double.TryParse(SelectedCurrencyRate, out double rate))
                {
                    var result = inputValue * rate;
                    ConvertedValue = $"{result:0.##}";
                    Debug.WriteLine($"${EntryValue} USD has been converted to {ConvertedValue}");
                    IsConvertFromCurrencyVisible = true;
                    IsConvertToCurrencyVisible = true;
                }
                else
                {
                    // Handle the error:
                    ConvertedValue = "Please select a currency to convert to.";
                }
            });
        }

        public ICommand NumericButtonCommand { get; private set; } // ICommand for button keypad clicks
        public ICommand ConvertCurrencyCommand { get; private set; } // ICommand for button to convert currency



        /* COMMENTED OUT FOR DEVELOPMENT
         *
         * This method is used to read a save JSON file instead of making an API call.
         *
         *
        // Fetch the latest currency data from JSON file. First need to load and then deserialise the JSON file objects. 
        public async Task FetchDataAsync()
        {
            string jsonData = string.Empty;

            // Using Stream to read embedded resource
            var assembly = IntrospectionExtensions.GetTypeInfo(typeof(MainViewModel)).Assembly;
            Stream stream = assembly.GetManifestResourceStream("Currency_Converter.CurrencyData.json");

            using (var reader = new StreamReader(stream))
            {
                jsonData = await reader.ReadToEndAsync();
            }

            _currencyData = JsonConvert.DeserializeObject<CurrencyData>(jsonData);

            foreach (var rate in _currencyData.Rates)
            {
                AvailableCurrencies.Add(rate.Key);
            }
        }
        */


        // Fetch the latest currency data from the API. This has been tested and returns up to date currencies. First need to load and then deserialise the JSON file objects. 
        public async Task FetchDataAsync()
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    var response = await client.GetStringAsync("https://openexchangerates.org/api/latest.json?app_id=f3ef28819b2f4f9db32689bfd7869ca6");
                    _currencyData = JsonConvert.DeserializeObject<CurrencyData>(response);

                    // Clear previous data
                    AvailableCurrencies.Clear();

                    foreach (var rate in _currencyData.Rates) // Adds rates to the AvailableCurrencies observable collection.
                    {
                        AvailableCurrencies.Add(rate.Key);
                    }
                }
                catch (HttpRequestException httpEx)
                {
                    // Handle connectivity or network related issues
                    Debug.WriteLine("Network error: " + httpEx.Message);
                    await Application.Current.MainPage.DisplayAlert("Error", "Network error occurred. Please check your internet connection.", "OK");
                }
                catch (JsonException jsonEx)
                {
                    // Handle issues related to JSON deserialisation
                    Debug.WriteLine("JSON error: " + jsonEx.Message);
                    await Application.Current.MainPage.DisplayAlert("Error", "Error processing data. Please try again later.", "OK");
                }
                catch (Exception ex)
                {
                    // General exception errors
                    Debug.WriteLine("General error: " + ex.Message);
                    await Application.Current.MainPage.DisplayAlert("Error", "An error occurred. Please try again.", "OK");
                }
            }
        }


        // Handles the button pressed command (Keypad)
        private void OnNumericButtonPressed(string value)
        {
            if (value == "C")
            {
                EntryValue = string.Empty;
                IsConvertFromCurrencyVisible = false;
            }
            else
            {
                EntryValue += value;
                if (string.IsNullOrEmpty(EntryValue))
                {
                    IsConvertFromCurrencyVisible = false;
                }
            }
        }


        // INotifyPropertyChanged implementation
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
