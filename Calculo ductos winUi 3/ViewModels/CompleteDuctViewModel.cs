using Calculo_ductos.Params;
using Calculo_ductos_winUi_3.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Calculo_ductos_winUi_3.ViewModels
{
    public class CompleteDuctViewModel : INotifyPropertyChanged
    {
        private int _PurposeId;
        private string _ExecutiveName;
        private string _PT;
        private int _SheetTypeId;
        private int _QuoteVersion;
        private bool _NeedSprinkler;
        private bool _NeedDesinfectionSystem;
        private decimal _SubtotalPrice;
        private decimal _TotalPrice;
        private int _totalDoubleLevels;
        private decimal _exchangeRate;
        private CatalogRentabilityModel _SelectedRentability;
        private ObservableCollection<QuoteModel> _Quotes { get; set; }
        //private int _DischargeTypeId;
        private ObservableCollection<QuoteModel> _filteredQuotes;
        private string _searchText;
        private string _selectedFilter = "PT";


        public CompleteDuctViewModel() {
            PurposeId = 1;
            ExecutiveName = string.Empty;
            SheetTypeId = 0;
            _exchangeRate = 0;
            NeedSprinklerValue = 1;
            NeedDesinfetionSystemValue = 1;
            _totalDoubleLevels = 0;
            _SelectedRentability = new CatalogRentabilityModel();
            _filteredQuotes = new ObservableCollection<QuoteModel>();
            _Quotes = new ObservableCollection<QuoteModel>();
            _searchText = string.Empty;
        }
        public void New() {
            PurposeId = 1;
            ExecutiveName = string.Empty;
            PT = string.Empty;
            SheetTypeId = 0;
            QuoteVersion = 0;
            TotalDoubleLevels = 0;
            TotalPrice = 0;
            SubTotalPrice = 0;
            _SelectedRentability = new CatalogRentabilityModel();
        }


        public ObservableCollection<CatalogKitModel> AllKits { get; set; } = new();
        public ObservableCollection<CatalogKitModel> AvailableKits { get; set; } = new();
        public CatalogRentabilityModel SelectedRentability
        {
            get => _SelectedRentability;
            set
            {
                if (_SelectedRentability != value)
                {
                    _SelectedRentability = value;
                    OnPropertyChanged();
                }
            }
        }
        public ObservableCollection<CatalogRentabilityModel> AvailableRentabilities { get; set; }

        public int PurposeId 
        {
            get => _PurposeId;
            set {
                _PurposeId = value;
                OnPropertyChanged();
                FilterKits();
            }
        }
        public string ExecutiveName 
        {
            get => _ExecutiveName;
            set {
                _ExecutiveName = value;
                OnPropertyChanged();
            }
        }
        public string PT
        {
            get => _PT;
            set
            {
                _PT = value;
                OnPropertyChanged();
            }
        }
        public int SheetTypeId
        {
            get => _SheetTypeId;
            set {
                _SheetTypeId = value;
                OnPropertyChanged();
            }
        }
        public int QuoteVersion 
        {
            get => _QuoteVersion;
            set {
                _QuoteVersion = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(QuoteVersionDescription));
            }
        }
        public string QuoteVersionDescription
        {
            get => QuoteVersion > 0 ? $"Versión: {QuoteVersion.ToString()}" : string.Empty;
        }
        public int NeedSprinklerValue
        {
            get => _NeedSprinkler ? 0 : 1;
            set {
                _NeedSprinkler = value == 0 ? true : false;
                OnPropertyChanged(nameof(NeedSprinklerValue));
            }
        }
        public string NeedSprinklerString
        {
            get => _NeedSprinkler ? "Si" : "No";
        }
        public bool NeedSprinkler
        {
            get => _NeedSprinkler;
            set
            {
                if (_NeedSprinkler != value)
                {
                    _NeedSprinkler = value;
                    OnPropertyChanged(nameof(NeedSprinkler));
                    OnPropertyChanged(nameof(NeedSprinklerString));
                    OnPropertyChanged(nameof(NeedSprinklerValue));
                }
            }
        }
        public int NeedDesinfetionSystemValue
        {
            get => _NeedDesinfectionSystem ? 0 : 1;
            set
            {
                _NeedDesinfectionSystem = value == 0 ? true : false;
                OnPropertyChanged(nameof(NeedDesinfetionSystemValue));
            }
        }
        public string NeedDesinfetionSystemString
        {
            get => _NeedDesinfectionSystem ? "Si" : "No";
        }
        public bool NeedDesinfectionSystem
        {
            get => _NeedDesinfectionSystem;
            set 
            {
                if (_NeedDesinfectionSystem != value)
                {
                    _NeedDesinfectionSystem = value;
                    OnPropertyChanged(nameof(NeedDesinfectionSystem));
                    OnPropertyChanged(nameof(NeedDesinfetionSystemString));
                    OnPropertyChanged(nameof(NeedDesinfetionSystemValue));
                }
            }
        }
        public decimal SubTotalPrice 
        {
            get => _SubtotalPrice;
            set {
                if (_SubtotalPrice != value)
                {
                    _SubtotalPrice = value;
                    OnPropertyChanged();
                }
            }
        }
        public decimal TotalPrice
        {
            get => _TotalPrice;
            set
            {
                if (_TotalPrice != value)
                {
                    _TotalPrice = value;
                    OnPropertyChanged();
                }
            }
        }
        public string TotalPriceFormatted => $"Precio: $ {TotalPrice:N2}";
        public string SubTotalPriceFormatted => $"Precio: $ {SubTotalPrice:N2}";
        public int TotalDoubleLevels
        {
            get => _totalDoubleLevels; set
            {
                if (_totalDoubleLevels != value)
                {
                    _totalDoubleLevels = value;
                    OnPropertyChanged();
                }
            }
        }
        public decimal ExchangeRate { get => _exchangeRate; set { if (_exchangeRate != value) { _exchangeRate = value; OnPropertyChanged(); } } }
        public string ExchangeRateFormatted => $"1 USD = ${ExchangeRate:N2} MXN";

        //public string SubTotalPriceFormatted => string.Format("{0:N2}", SubTotalPrice);
        public ObservableCollection<QuoteModel> Quotes
        {
            get => _Quotes;
            set 
            {
                _Quotes = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<QuoteModel> FilteredQuotes
        {
            get => _filteredQuotes;
            set  {
                if (value != _filteredQuotes)
                {
                    _filteredQuotes = value;
                    OnPropertyChanged();
                }
            }
        }
        public string SearchText
        {
            get => _searchText;
            set
            {
                if (value != _searchText)
                { 
                    _searchText = value;
                    Filtrar();
                    OnPropertyChanged();
                }
            }
        }
        public List<string> FilterOptions { get; } = new()
        {
            "PT",
            "Ejecutivo"
        };

        
        public string SelectedFilter
        {
            get => _selectedFilter;
            set
            {
                if (value != _selectedFilter)
                { 
                    _selectedFilter = value;
                    OnPropertyChanged();
                    Filtrar();
                }
            }
        }

        private void Filtrar()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                FilteredQuotes = new ObservableCollection<QuoteModel>(Quotes);
                return;
            }

            var result = Quotes.Where(q =>
            {
                return SelectedFilter switch
                {
                    "PT" => q.PT?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) == true,
                    "Ejecutivo" => q.ExecutiveName?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) == true,
                    _ => false
                };
            });

            FilteredQuotes = new ObservableCollection<QuoteModel>(result);
        }

        public void LoadCatalogs(List<CatalogKitModel> kits, List<CatalogRentabilityModel> rentabilities)
        {
            AllKits = new ObservableCollection<CatalogKitModel>(kits);
            AvailableRentabilities = new ObservableCollection<CatalogRentabilityModel>(rentabilities);
            FilterKits();
        }
        public void CalculatePrice(DuctsViewModel ductVm, ComponentsViewModel componentVm, FloorDescriptionViewModel floorVm) {
            SetExchangeRate();
            CalculateSubTotalPrice(ductVm,componentVm,floorVm);
            RecalculateRentability();
        }
        public void RecalculateRentability() {
            TotalPrice = SubTotalPrice * SelectedRentability.Rentability;
        }
        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        private void FilterKits()
        {
            AvailableKits.Clear();
            var filtrados = AllKits.Where(p => p.PurposeId == PurposeId).ToList();

            foreach (var item in filtrados)
                AvailableKits.Add(item);
        }
        private void CalculateSubTotalPrice(DuctsViewModel ductVm, ComponentsViewModel componentVm, FloorDescriptionViewModel floorVm) {
            SubTotalPrice = 0;
            //Ductos
            foreach (var duct in ductVm.DucList)
            {
                var kit = AvailableKits.Where(k => k.TypeKit.Equals(duct.Type.ToString())).FirstOrDefault();
                if (kit != null)
                    SubTotalPrice += (kit.Price*kit.ExchangeRate*duct.Count);
            }
            //Componentes
            foreach (var component in componentVm.ComponentList)
            {
                
                if (component.Type == Calculo_ductos.Params.Component.TypeComponent.Gate)
                {
                    var kits = AvailableKits.Where(k => k.TypeKit.Equals(component.Type.ToString())).ToList();
                    foreach(var kit in kits)
                    if (kit != null)
                        SubTotalPrice += (kit.Price * kit.ExchangeRate * GetDoorCount(kit.Item, floorVm));
                }
                else 
                {
                    var kit = AvailableKits.Where(k => k.TypeKit.Equals(component.Type.ToString())).FirstOrDefault();
                    if (kit != null)
                        SubTotalPrice += (kit.Price * kit.ExchangeRate * component.Count);
                }
            }
        }
        private void SetExchangeRate() {
            ExchangeRate = AvailableKits.FirstOrDefault().ExchangeRate;
        }
        private int GetDoorCount(string kit, FloorDescriptionViewModel floorVm)
        {
            var count = 0;
            try
            {
                count = floorVm.FloorList
                    .Where(floor => floor.TypeDoor.IdSyteLine == kit && floor.NeedGate && floor.Type != Floor.TypeFloor.discharge)
                    .Sum(floor => floor.FloorCount);
            }
            catch (Exception ex)
            {
                count = 0;
            }
            return count;
        }

    }
}
