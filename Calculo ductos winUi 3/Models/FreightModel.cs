using CommunityToolkit.Mvvm.ComponentModel;
using DocumentFormat.OpenXml.Office.CoverPageProps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculo_ductos_winUi_3.Models
{
    public class FreightModel : ObservableObject
    {
        private int _FreightId;
        private int _TruckTypeId;
        private string _TruckDescription;
        private string _TruckName;
        private string _EntityName;
        private string _MunicipalityName;
        private int _LocalityId;
        private string _LocalityName;
        private decimal _Price;
        private decimal _HandlingCost;
        private bool _IsSelected;
        private string _ImagePath;
        private int _EstimatedTimeArrival;
        private decimal _TotalPrice;

        public FreightModel()
        {
            _FreightId = 0;
            _TruckTypeId = 0;
            _TruckTypeId = 0;
            _TruckDescription = string.Empty;
            _TruckName= string.Empty;
            _EntityName=string.Empty;
            _MunicipalityName = string.Empty;
            _LocalityId=0;
            _LocalityName=string.Empty;
            _Price=0;
            _HandlingCost=0;
            _IsSelected=false;
            _ImagePath=string.Empty;
            _EstimatedTimeArrival=0;
            _TotalPrice=0;

        }
        public int FreightId { get=> _FreightId; set=>SetProperty(ref _FreightId,value); }
        public int TruckTypeId { get=> _TruckTypeId; set=>SetProperty(ref _TruckTypeId,value); }
        public string TruckDescription { get=>_TruckDescription; set=>SetProperty(ref _TruckDescription,value); } 
        public string TruckName { get=>_TruckName; set=>SetProperty(ref _TruckName,value); } 
        public string EntityName { get=>_EntityName; set=>SetProperty(ref _EntityName,value); } 
        public int LocalityId { get=>_LocalityId; set=>SetProperty(ref _LocalityId,value); }
        public string LocalityName { get=>_LocalityName; set=>SetProperty(ref _LocalityName,value); }
        public string MunicipalityName { get=> _MunicipalityName; set=>SetProperty(ref _MunicipalityName, value); }
        public decimal Price { get=>_Price; set=>SetProperty(ref _Price,value); }
        public decimal HandlingCost { get=>_HandlingCost; set=>SetProperty(ref _HandlingCost,value); }
        public bool IsSelected { get=>_IsSelected; set=>SetProperty(ref _IsSelected,value); }
        public string ImagePath { get=>_ImagePath; set=>SetProperty(ref _ImagePath,value); } 
        public int EstimatedTimeArrival { get=>_EstimatedTimeArrival; set=>SetProperty(ref _EstimatedTimeArrival,value); }
        public decimal FirstSubTotalPrice
        {
            get => Price + HandlingCost;
        }
        public decimal SubTotalPrice { 
            get => (Price + HandlingCost) * 1.1m;
        }
        public decimal TotalPrice { get => _TotalPrice; set => SetProperty(ref _TotalPrice, value); }
    }
}
