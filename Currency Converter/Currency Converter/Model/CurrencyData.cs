using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Currency_Converter.Model
{
    public class CurrencyData
    {
        public int Timestamp {  get; set; }
        public string Base { get; set; }   
        public Dictionary<string, double> Rates { get; set; }
    }
}
