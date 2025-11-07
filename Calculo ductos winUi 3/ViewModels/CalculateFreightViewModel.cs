using Calculo_ductos.Params;
using Calculo_ductos_winUi_3.Models;
using Calculo_ductos_winUi_3.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculo_ductos_winUi_3.ViewModels
{
    public class CalculateFreightViewModel : ObservableObject
    {

        #region Fields
        //private ObservableCollection<FreightService> _FreightServices;
        //private CatalogRowModel _City;
        private CatalogRowEntityModel _SelectedState;
        private CatalogRowEntityModel _SelectedMunicipality;
        private CatalogRowEntityModel _SelectedLocality;
        private FreightModel _Freight;
        private readonly WebApi Client;
        #endregion
        #region Constructor
        public CalculateFreightViewModel(WebApi client) 
        {
            //_FreightServices = new ObservableCollection<FreightService>();
            _SelectedState = new CatalogRowEntityModel();
            _SelectedMunicipality = new CatalogRowEntityModel();
            _SelectedLocality = new CatalogRowEntityModel();
            _Freight = new FreightModel();
            //Freight.TruckTypeId = 2;
            //Freight.LocalityName = "HEROICA PUEBLA DE ZARAGOZA";
            //Freight.LocalityId = 3351;
            //Freight.EntityName = "PUEBLA";
            //Freight.FreightId = 1;
            //Freight.TruckDescription = "Trailer";
            //Freight.Price = 20000;
            //Freight.TruckName = "Trailer";
            Freight.ImagePath = "ms-appx:///Assets/Trailer.png";
            Client = client;

        }

        public void New()
        {
            var newFreight = new FreightModel();
            newFreight.ImagePath = "ms-appx:///Assets/Trailer.png";
            Freight = newFreight;
            
        }
        #endregion
        #region Properties
        public ObservableCollection<CatalogRowEntityModel> AllStates { get; set; } = new();
        public ObservableCollection<CatalogRowEntityModel> AvailableStates { get; set; } = new();
        public ObservableCollection<CatalogRowEntityModel> AllMunicipalities { get; set; } = new();
        public ObservableCollection<CatalogRowEntityModel> AvailableMunicipalities { get; set; } = new();
        public ObservableCollection<CatalogRowEntityModel> AllLocalities { get; set; } = new();
        public ObservableCollection<CatalogRowEntityModel> AvailableLocalities { get; set; } = new();
        public ObservableCollection<CatalogRowTruckTypeModel> AllTrucks { get; set; } = new();
        public ObservableCollection<FreightModel> FreightServices { get; set; } = new();
        public CatalogRowEntityModel SelectedLocality
        {
            get => _SelectedLocality;
            set 
            { 
                SetProperty(ref _SelectedLocality, value);
                
            }
        }
        public CatalogRowEntityModel SelectedMunicipality
        {
            get => _SelectedMunicipality;
            set 
            {
                if (value !=null)
                {
                    if (_SelectedMunicipality.Id != value.Id)
                    { 
                        SetProperty(ref _SelectedMunicipality, value);
                        FilterLocalities(value.Id);
                    }
                }
            }
        }
        public CatalogRowEntityModel SelectedState
        {
            get => _SelectedState;
            set
            {
                if (value!=null)
                {
                    if (_SelectedState.Id != value.Id)
                    { 
                        SetProperty(ref _SelectedState, value);
                        FilterMunicipalities(value.Id);
                    }
                }
            }
        }
        public FreightModel Freight
        {
            get => _Freight;
            set
            {
                //value.ImagePath = GetImagePath(value.TruckTypeId);
                SetProperty(ref _Freight, value);
                //OnPropertyChanged(nameof(Freight));
            }
        }
        public string TotalPriceFormatted => $"Precio: $ {Freight.TotalPrice:N2}";
        public string SubTotalPriceFormatted => $"Costo: $ {Freight.SubTotalPrice:N2}";

        #endregion


        #region Public Methods      
        public void LoadCatalogs(List<CatalogRowEntityModel> entitiesList, List<CatalogRowEntityModel> municipalitiesList, List<CatalogRowEntityModel> localitiesList,List<CatalogRowTruckTypeModel> truckList)
        {
            AllStates = new ObservableCollection<CatalogRowEntityModel>(entitiesList);
            AllMunicipalities = new ObservableCollection<CatalogRowEntityModel>(municipalitiesList);
            AllLocalities = new ObservableCollection<CatalogRowEntityModel>(localitiesList);
            AvailableStates = new ObservableCollection<CatalogRowEntityModel>(entitiesList);
            AllTrucks = new ObservableCollection<CatalogRowTruckTypeModel>(truckList);
        } 
        public void FilterMunicipalities(int stateId)
        {
            AvailableMunicipalities.Clear();
            var filtrados = AllMunicipalities
                .Where(p => p.ParentId.Equals(stateId))
                .ToList();

            foreach (var item in filtrados)
                AvailableMunicipalities.Add(item);

            // Reset selección si no es válida
            if (!AvailableMunicipalities.Contains(SelectedMunicipality))
                SelectedMunicipality = AvailableMunicipalities.FirstOrDefault();
        }
        public async Task CalculateFreight(List<DuctModel> ductList, CatalogRentabilityModel rentability )
        {
            var truck = GetTruckNeeded(ductList);
            await LoadFreight(SelectedLocality.Id,truck);
            RecalculateRentability(rentability);
        }
        public void RecalculateRentability(CatalogRentabilityModel rentability) {
            Freight.TotalPrice = Freight.SubTotalPrice * rentability.Rentability;
        }
        public void FilterLocalities(int stateId)
        {
            AvailableLocalities.Clear();
            var filtrados = AllLocalities
                .Where(p => p.ParentId.Equals(stateId))
                .ToList();

            foreach (var item in filtrados)
                AvailableLocalities.Add(item);

            // Reset selección si no es válida
            if (!AvailableLocalities.Contains(SelectedLocality))
                SelectedLocality = AvailableLocalities.FirstOrDefault();
        }
        #endregion

        #region Private Methods
        private CatalogRowTruckTypeModel GetTruckNeeded(List<DuctModel> ductList)
        {
            CatalogRowTruckTypeModel result = new();
            var locations = GetLocations(ductList);
            result = AllTrucks.Where(truck => truck.MaxCapacity >= locations && truck.MinCapacity<= locations)
                .OrderBy(truck => truck.MinCapacity).FirstOrDefault();
            return result;
        }
        private string GetImagePath(int truckTypeId)
        {
            string path = string.Empty;
            switch (truckTypeId)
            {
                case 0: path = "ms-appx:///Assets/NissanM.png"; break;
                case 1: path = "ms-appx:///Assets/3MTonM.png"; break;
                case 2: path = "ms-appx:///Assets/5TonM.png"; break;
                case 3: path = "ms-appx:///Assets/RabonM.png"; break;
                case 4: path = "ms-appx:///Assets/TortonM.png"; break;
                case 5: path = "ms-appx:///Assets/MudanceroM.png";break;
                case 6: path = "ms-appx:///Assets/Trailer48M.png";break;
                case 7: path = "ms-appx:///Assets/Trailer53M.png";break;
                default: path = "ms-appx:///Assets/Trailer.png"; break;

            }
            return path;
        }
        private async Task LoadFreight(int localityId, CatalogRowTruckTypeModel truckType)
        {
            try
            {
                var freight = await Client.GetAsync<FreightModel>($"Freight?idLocalidad={localityId}&idTipoCamion={truckType.Id}");

                if (freight != null)
                {
                    freight.ImagePath = GetImagePath(freight.TruckTypeId);
                    freight.HandlingCost = truckType.HandlingCost;
                    Freight = freight;
                }
                else {
                    freight = new FreightModel();
                    freight.ImagePath = GetImagePath(truckType.Id);
                    freight.HandlingCost = truckType.HandlingCost;
                    freight.TruckDescription = truckType.Description;
                    freight.EntityName = SelectedState.Name;
                    freight.LocalityName = SelectedLocality.Name;
                    Freight = freight;
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine($"[Freight Load Error]: {ex.Message}");

            }
        }
        private int GetLocations(List<DuctModel> ductList)
        {
            var locations = 0;
            var totalB2 = ductList.Where(duct=> duct.Type == DuctPiece.TypeDuct.B2 || duct.Type == DuctPiece.TypeDuct.B2F).Sum(duct => duct.Count);
            var totalB3 = ductList.Where(duct=> duct.Type == DuctPiece.TypeDuct.B3 || duct.Type == DuctPiece.TypeDuct.B3F).Sum(duct => duct.Count);
            var totalB4 = ductList.Where(duct=> duct.Type == DuctPiece.TypeDuct.B4 || duct.Type == DuctPiece.TypeDuct.B4F).Sum(duct => duct.Count);
            var totalCS = ductList.Where(duct => duct.Type == DuctPiece.TypeDuct.C4 || duct.Type == DuctPiece.TypeDuct.S4).Sum(duct => duct.Count);
            
            if (totalB4 > totalCS)
            {
                locations += totalCS;
                var diff = totalB4 - totalCS;
                locations += (diff + 1) / 2;
                locations += (totalB3 + 1) / 2;
            }
            else {
                locations += totalCS;
                var diff = totalCS - totalB4;
                var diffB3 = totalB3 - diff;
                locations += (diffB3 + 1) / 2;
            }

            locations += (totalB2 + 3) / 4;

            return locations;
        }
        #endregion
    }
}
