using Calculo_ductos.Params;
using Calculo_ductos_winUi_3.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DocumentFormat.OpenXml.Bibliography;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.ApplicationModel.AppService;

namespace Calculo_ductos_winUi_3.ViewModels
{
    public class IndirectsViewModel : ObservableObject
    {
        #region Fields
        private decimal _SubTotalCostInstallers;
        private decimal _TotalCostInstallers;
        private decimal _SubTotalCostSupervisor;
        private decimal _TotalCostSupervisor;
        private decimal _SubTotalCostSecurity;
        private decimal _TotalCostSecurity;
        private decimal _SubTotalCostVisit;
        private decimal _TotalCostVisit;
        private decimal _TotalPriceWC;
        private decimal _TotalPriceStore;
        private decimal _SubTotalPriceWC;
        private decimal _SubTotalPriceStore;
        private decimal _SubTotalCostTool;
        private decimal _TotalCostTool;
        private ManPowerViewModel _ManPowerVm;
        private CatalogZoneModel _SelectedZone;
        private Opcion _SelectedIzaje;
        private Opcion _SelectedTransportType;
        private CatalogIndirectModel _selectedIndirect;
        private decimal _unitCost;
        private decimal _selectedTrasnportCost;
        private bool _HasDoubleHeightLevels;
        private int _TotalEvents;
        private int _TotalForeingResourceInstaller;
        private int _TotalResourceInstaller;
        private int _TotalEfectiveDaysInstaller;
        private int _TotalEfectiveDaysSupervisor;
        private int _TotalNoworkDaysInstaller;
        private int _TotalNoworkDaysSupervisor;
        private int _TotalWeeks;
        private int _TotalFloors;
        #endregion
        #region Constructor
        public IndirectsViewModel(ManPowerViewModel ManPowerVm) 
        {
            AllIndirects = new ObservableCollection<CatalogIndirectModel>();
            AllTools = new ObservableCollection<CatalogToolModel>();
            AvailableIndirects = new ObservableCollection<CatalogIndirectModel>();
            AvailableZones = new ObservableCollection<CatalogZoneModel>();
            IndirectsInstallers = new ObservableCollection<IndirectsModel>();
            IndirectsSupervisor = new ObservableCollection<IndirectsModel>();
            IndirectsSecurity = new ObservableCollection<IndirectsModel>();
            IndirectsVisit = new ObservableCollection<IndirectsModel>();
            OtherIndirectsInstaller = new ObservableCollection<IndirectsModel>();
            OtherIndirectsSupervisor = new ObservableCollection<IndirectsModel>();
            OtherIndirectsSecurity = new ObservableCollection<IndirectsModel>();
            OtherIndirectsVisit = new ObservableCollection<IndirectsModel>();
            MinorTool = new ObservableCollection<IndirectsModel>();
            MajorTool = new ObservableCollection<IndirectsModel>();
            AvailableIzaje = new ObservableCollection<Opcion> { new Opcion { Id = 1 , Description ="Si"}, new Opcion { Id = 2, Description = "No" } };
            AvailableTransportTypes = new ObservableCollection<Opcion> { new Opcion { Id = 1 , Description ="Aéreo"}, new Opcion { Id = 2, Description = "Autobus" } };
            _SubTotalCostInstallers = 0m;
            _TotalCostInstallers = 0m;
            _SubTotalCostSupervisor = 0m;
            _TotalCostSupervisor = 0m;
            _SubTotalCostSecurity = 0m;
            _TotalCostSecurity = 0m;
            _SubTotalCostVisit = 0m;
            _TotalCostVisit = 0m;
            _TotalPriceStore = 0m;
            _TotalPriceWC = 0m;
            _SubTotalPriceStore = 0m;
            _SubTotalPriceWC = 0m;
            _unitCost = 0m;
            _SubTotalCostTool=0m;
            _TotalCostTool = 0m;
            _TotalEvents = 0;
            _TotalForeingResourceInstaller = 0;
            _TotalResourceInstaller = 0;
            _TotalEfectiveDaysInstaller = 0;
            _TotalEfectiveDaysSupervisor=0;
            _TotalNoworkDaysInstaller=0;
            _TotalNoworkDaysSupervisor=0;
            _TotalWeeks = 0;
            _TotalFloors = 0;
            _HasDoubleHeightLevels = false;
            _ManPowerVm = ManPowerVm;
            _selectedTrasnportCost = 0m;
            SelectedTransportType = new();
            AddIndirectCommand = new RelayCommand(AddIndirect);
            AddTransportCommand = new RelayCommand(AddTransport);
            RemoveIndirectCommand = new RelayCommand<Guid>(RemoveIndirect);
        }
        public void New()
        {
            TotalPriceStore = 0m;
            TotalPriceWC = 0m;
            SubTotalPriceStore = 0m;
            SubTotalPriceWC = 0m;
            SubTotalCostInstallers = 0m;
            TotalCostInstallers = 0m;
            SubTotalCostSupervisor = 0m;
            TotalCostSupervisor = 0m;
            SubTotalCostSecurity = 0m;
            TotalCostSecurity = 0m;
            SubTotalCostVisit = 0m;
            TotalCostVisit = 0m;
            SubTotalCostTool = 0m;
            TotalCostTool=0m;
            selectedUnitCost = "0";
            TotalEvents = 0;
            TotalForeingResourceInstaller = 0;
            TotalResourceInstaller = 0;
            HasDoubleHeightLevels = false;
            selectedTrasnportCost = "0";
            SelectedTransportType = new();
            Clear();
        }
        #endregion
        #region Properties
        public ObservableCollection<CatalogToolModel> AllTools { get; set; }
        public ObservableCollection<CatalogIndirectModel> AllIndirects { get; set; }
        public ObservableCollection<CatalogIndirectModel> AvailableIndirects { get; set; }
        public ObservableCollection<Opcion> AvailableIzaje { get; set; }
        public ObservableCollection<Opcion> AvailableTransportTypes { get; set; }
        public ObservableCollection<CatalogZoneModel> AvailableZones { get; set; }
        public ObservableCollection<IndirectsModel> IndirectsInstallers { get; set; }
        public ObservableCollection<IndirectsModel> IndirectsSupervisor { get; set; }
        public ObservableCollection<IndirectsModel> IndirectsSecurity { get; set; }
        public ObservableCollection<IndirectsModel> IndirectsVisit { get; set; }
        public ObservableCollection<IndirectsModel> OtherIndirectsInstaller { get; set; }
        public ObservableCollection<IndirectsModel> OtherIndirectsSupervisor { get; set; }
        public ObservableCollection<IndirectsModel> OtherIndirectsSecurity { get; set; }
        public ObservableCollection<IndirectsModel> OtherIndirectsVisit { get; set; }
        public ObservableCollection<IndirectsModel> MinorTool { get; set; }
        public ObservableCollection<IndirectsModel> MajorTool { get; set; }
        public CatalogZoneModel SelectedZone { get => _SelectedZone; set { SetProperty(ref _SelectedZone, value); 
                FilterAvailableIndirects();
            } }
        public Opcion SelectedIzaje { get => _SelectedIzaje; set { SetProperty(ref _SelectedIzaje, value); } }
        public Opcion SelectedTransportType { get => _SelectedTransportType; set { SetProperty(ref _SelectedTransportType, value); } }
        public decimal TotalCostInstallers { get { return _TotalCostInstallers; } set { SetProperty(ref _TotalCostInstallers, value); } }
        public decimal TotalPriceWC { get { return _TotalPriceWC; } set { SetProperty(ref _TotalPriceWC, value); } }
        public decimal TotalPriceStore { get { return _TotalPriceStore; } set { SetProperty(ref _TotalPriceStore, value); } }
        public decimal SubTotalPriceWC { get { return _SubTotalPriceWC; } set { SetProperty(ref _SubTotalPriceWC, value); } }
        public decimal SubTotalPriceStore { get { return _SubTotalPriceStore; } set { SetProperty(ref _SubTotalPriceStore, value); } }
        public decimal SubTotalCostInstallers { get { return _SubTotalCostInstallers; } set { SetProperty(ref _SubTotalCostInstallers, value); } }
        public decimal TotalCostSupervisor { get { return _TotalCostSupervisor; } set { SetProperty(ref _TotalCostSupervisor, value); } }
        public decimal SubTotalCostSupervisor { get { return _SubTotalCostSupervisor; } set { SetProperty(ref _SubTotalCostSupervisor, value); } }
        public decimal TotalCostSecurity { get { return _TotalCostSecurity; } set { SetProperty(ref _TotalCostSecurity, value); } }
        public decimal SubTotalCostSecurity { get { return _SubTotalCostSecurity; } set { SetProperty(ref _SubTotalCostSecurity, value); } }
        public decimal TotalCostVisit { get { return _TotalCostVisit; } set { SetProperty(ref _TotalCostVisit, value); } }
        public decimal SubTotalCostVisit { get { return _SubTotalCostVisit; } set { SetProperty(ref _SubTotalCostVisit, value); } }
        public CatalogIndirectModel SelectedIndirect { get => _selectedIndirect; set {SetProperty(ref _selectedIndirect,value); } }
        public bool HasDoubleHeightLevels { get => _HasDoubleHeightLevels; set {SetProperty(ref _HasDoubleHeightLevels, value); } }
        public int TotalEvents { get => _TotalEvents; set {SetProperty(ref _TotalEvents,value); } }
        public int TotalForeingResourceInstaller { get => _TotalForeingResourceInstaller; set {SetProperty(ref _TotalForeingResourceInstaller, value); } }
        public int TotalResourceInstaller { get => _TotalResourceInstaller; set {SetProperty(ref _TotalResourceInstaller, value); } }
        public int TotalEfectiveDaysInstaller { get => _TotalEfectiveDaysInstaller; set {SetProperty(ref _TotalEfectiveDaysInstaller, value); } }
        public int TotalEfectiveDaysSupervisor { get => _TotalEfectiveDaysSupervisor; set {SetProperty(ref _TotalEfectiveDaysSupervisor, value); } }
        public int TotalNoWorkDaysInstaller { get => _TotalNoworkDaysInstaller; set {SetProperty(ref _TotalNoworkDaysInstaller, value); } }
        public int TotalNoWorkDaysSupervisor { get => _TotalNoworkDaysSupervisor; set {SetProperty(ref _TotalNoworkDaysSupervisor, value); } }
        public int TotalWeeks { get => _TotalWeeks; set {SetProperty(ref _TotalWeeks, value); } }
        public int TotalFloors { get => _TotalFloors; set {SetProperty(ref _TotalFloors, value); } }
        public decimal SubTotalCostTool { get { return _SubTotalCostTool; } set { SetProperty(ref _SubTotalCostTool, value); } }
        public decimal TotalCostTool { get { return _TotalCostTool; } set { SetProperty(ref _TotalCostTool, value); } }
        public decimal TotalPriceIndirects => TotalCostInstallers + TotalCostSupervisor + TotalCostSecurity + TotalCostVisit + TotalPriceWC + TotalPriceStore;
        public decimal SubTotalPriceIndirects => SubTotalCostInstallers + SubTotalCostSupervisor + SubTotalCostSecurity + SubTotalCostVisit + SubTotalPriceWC + SubTotalPriceStore;
        public string TotalPriceInstallersFormatted => $"Precio: $ {TotalCostInstallers:N2}";
        public string SubTotalPriceInstallersFormatted => $"Costo: $ {SubTotalCostInstallers:N2}";
        public string TotalPriceSupervisorFormatted => $"Precio: $ {TotalCostSupervisor:N2}";
        public string SubTotalPriceSupervisorFormatted => $"Costo: $ {SubTotalCostSupervisor:N2}";
        public string TotalPriceSecurityFormatted => $"Precio: $ {TotalCostSecurity:N2}";
        public string SubTotalPriceSecurityFormatted => $"Costo: $ {SubTotalCostSecurity:N2}";
        public string TotalPriceVisitFormatted => $"Precio: $ {TotalCostVisit:N2}";
        public string SubTotalPriceVisitFormatted => $"Costo: $ {SubTotalCostVisit:N2}";
        public string TotalPriceFormatted => $"Precio: $ {TotalCostInstallers+ TotalCostSupervisor+ TotalCostSecurity+ TotalCostVisit+TotalPriceWC+TotalPriceStore:N2}";
        public string SubTotalPriceFormatted => $"Costo: $ {SubTotalCostInstallers+ SubTotalCostSupervisor+ SubTotalCostSecurity+ SubTotalCostVisit+SubTotalPriceWC+SubTotalPriceStore:N2}";
        public string TotalPriceStoreFormatted => $"Precio: ${TotalPriceStore:N2}";
        public string TotalPriceWCFormatted => $"Precio: ${TotalPriceWC:N2}";
        public string SubTotalPriceStoreFormatted => $"Costo: ${SubTotalPriceStore:N2}";
        public string SubTotalPriceWCFormatted => $"Costo: ${SubTotalPriceWC:N2}";


        //public decimal selectedUnitCost { get => _unitCost;set {SetProperty(ref _unitCost,value); } }
        public string selectedUnitCost
        {
            get => _unitCost.ToString();
            set
            {
                if (decimal.TryParse(value, out decimal result))
                {
                    _unitCost = result;
                    OnPropertyChanged();
                }
            }
        }
        public string selectedTrasnportCost
        {
            get => _selectedTrasnportCost.ToString();
            set
            {
                if (decimal.TryParse(value, out decimal result))
                {
                    _selectedTrasnportCost = result;
                    OnPropertyChanged();
                }
            }
        }
        public void LoadTotals(Duct duct)
        {
            TotalEfectiveDaysInstaller = _ManPowerVm.EfectiveWorkDays.TotalWorkDays;
            TotalEfectiveDaysSupervisor = _ManPowerVm.EfectiveWorkDays.TotalWorkDays + 3;
            //TotalNoWorkDaysInstaller = _ManPowerVm.ManPower.Where(m => m.Recurso.Id == 1).Sum(m => m.DiasNoLaborales);
            TotalNoWorkDaysInstaller = _ManPowerVm.EfectiveWorkDays.NoWorkDays;
            TotalNoWorkDaysSupervisor = TotalEfectiveDaysSupervisor / 7;
            TotalWeeks = (int)Math.Ceiling(_ManPowerVm.EfectiveWorkDays.TotalWorkDays / 7.0);
            TotalFloors = duct.floors.Count;
            TotalEvents = GetTotalEvents(duct.floors);
            TotalForeingResourceInstaller = _ManPowerVm.ManPower.Where(r => r.TipoRecurso.Id == 1 && r.Recurso.Id == 1).Count();
            TotalResourceInstaller = _ManPowerVm.ManPower.Where(r => r.Recurso.Id == 1).Count();
            HasDoubleHeightLevels = duct.floors.Where(p => p.Height >= 4.5m).ToList().Count > 0;
        }
        //public decimal selectedTrasnportCost { get => _selectedTrasnportCost; set {SetProperty(ref _selectedTrasnportCost, value); } }
        #endregion
        #region Public Methods
        public async Task CalculateIndirects(CatalogRentabilityModel rentability, bool isLocalProject)
        {
            //int TotalEfectiveDays = _ManPowerVm.EfectiveWorkDays.TotalWorkDays;
            //int TotalWeeks = (int)Math.Ceiling(_ManPowerVm.EfectiveWorkDays.TotalWorkDays / 7.0);
            
            IndirectsInstallers.Clear();
            IndirectsSupervisor.Clear();
            IndirectsSecurity.Clear();
            IndirectsVisit.Clear();
            MinorTool.Clear();
            MajorTool.Clear();
            SubTotalCostInstallers = 0;
            SubTotalCostSupervisor = 0;
            SubTotalCostSecurity = 0;
            SubTotalCostVisit = 0;
            TotalCostInstallers = 0;
            TotalCostSupervisor = 0;
            TotalCostSecurity = 0;
            TotalCostVisit = 0;
            var mandatory = AvailableIndirects.Where(p => p.ZoneId == SelectedZone.Id && p.IsMandatory).ToList();
            foreach (var indirect in mandatory)
            {
                var newIndirectInstaller = new IndirectsModel
                {
                    Concepto = indirect.Concept,
                    Cantidad = ObtenerCantidad($"{indirect.Concept}i"),
                    PrecioUnitario = indirect.Cost
                };
                var newIndirectSupervisor = new IndirectsModel
                {
                    Concepto = indirect.Concept,
                    Cantidad = ObtenerCantidad($"{indirect.Concept}s"),
                    PrecioUnitario = indirect.Cost
                };
                var newIndirectSecurity = new IndirectsModel
                {
                    Concepto = indirect.Concept,
                    Cantidad = ObtenerCantidad($"{indirect.Concept}se"),
                    PrecioUnitario = indirect.Cost
                };
                var newIndirectVisit = new IndirectsModel
                {
                    Concepto = indirect.Concept,
                    Cantidad = ObtenerCantidad($"{indirect.Concept}v"),
                    PrecioUnitario = indirect.Cost
                };
                                   
                    IndirectsInstallers.Add(newIndirectInstaller);                    
                    IndirectsSupervisor.Add(newIndirectSupervisor);                    
                    IndirectsSecurity.Add(newIndirectSecurity);
                    if (!newIndirectVisit.Concepto.Contains("Hotel"))
                        IndirectsVisit.Add(newIndirectVisit);                     
                
                
            }
            if(!isLocalProject)
            if (SelectedTransportType.Id == 1)
            {
                foreach (var indirect in AvailableIndirects.Where(p => p.Concept.Contains("casa") && p.ZoneId == SelectedZone.Id).ToList())
                {
                    var newIndirectInstaller = new IndirectsModel
                    {
                        Concepto = indirect.Concept,
                        Cantidad = ObtenerCantidad($"{indirect.Concept}i"),
                        PrecioUnitario = indirect.Cost
                    };
                    var newIndirectSupervisor = new IndirectsModel
                    {
                        Concepto = indirect.Concept,
                        Cantidad = ObtenerCantidad($"{indirect.Concept}s"),
                        PrecioUnitario = indirect.Cost
                    };
                    var newIndirectSecurity = new IndirectsModel
                    {
                        Concepto = indirect.Concept,
                        Cantidad = ObtenerCantidad($"{indirect.Concept}se"),
                        PrecioUnitario = indirect.Cost
                    };
                    var newIndirectVisit = new IndirectsModel
                    {
                        Concepto = indirect.Concept,
                        Cantidad = ObtenerCantidad($"{indirect.Concept}v"),
                        PrecioUnitario = indirect.Cost
                    };
                    IndirectsInstallers.Add(newIndirectInstaller);
                    IndirectsSupervisor.Add(newIndirectSupervisor);
                    IndirectsSecurity.Add(newIndirectSecurity);
                    IndirectsVisit.Add(newIndirectVisit);
                }
            }

            CalculateTools();

            SumIndirect(1, IndirectsInstallers);
            SumIndirect(1, MinorTool);
            SumIndirect(1, MajorTool);
            SumIndirect(2, IndirectsSupervisor);
            SumIndirect(3, IndirectsSecurity);
            SumIndirect(4, IndirectsVisit);

            SumIndirect(1, OtherIndirectsInstaller);
            SumIndirect(2, OtherIndirectsSupervisor);
            SumIndirect(3, OtherIndirectsSecurity);
            SumIndirect(4, OtherIndirectsVisit);

            SubTotalCostTool = MinorTool.Sum(x => x.CostoToal) + MajorTool.Sum(x=>x.CostoToal);
           
            TotalCostInstallers = SubTotalCostInstallers * rentability.Rentability;
            TotalCostSupervisor = SubTotalCostSupervisor * rentability.Rentability;
            TotalCostSecurity = SubTotalCostSecurity * rentability.Rentability;
            TotalCostVisit = SubTotalCostVisit * rentability.Rentability;
            TotalPriceWC = SubTotalPriceWC * rentability.Rentability;
            TotalPriceStore = SubTotalPriceStore * rentability.Rentability;
            TotalCostTool = SubTotalCostTool * rentability.Rentability;
        }
        public void RecalculateRentability(CatalogRentabilityModel rentability)
        {
            TotalCostInstallers = SubTotalCostInstallers * rentability.Rentability;
            TotalCostSupervisor = SubTotalCostSupervisor * rentability.Rentability;
            TotalCostSecurity = SubTotalCostSecurity * rentability.Rentability;
            TotalCostVisit = SubTotalCostVisit * rentability.Rentability;
        }
        public void LoadCatalogs(List<CatalogIndirectModel> indirects,List<CatalogZoneModel> zones, List<CatalogToolModel> tools)
        {
            AllIndirects = new ObservableCollection<CatalogIndirectModel>(indirects);
            AvailableZones = new ObservableCollection<CatalogZoneModel>(zones);
            AllTools = new ObservableCollection<CatalogToolModel>(tools);
        }
        public void FilterAvailableIndirects()
        {
            AvailableIndirects = new ObservableCollection<CatalogIndirectModel>(AllIndirects.Where(i => i.ZoneId == SelectedZone.Id).ToList());
        }

        #endregion
        #region Commands
        public ICommand AddIndirectCommand { get; }
        public ICommand RemoveIndirectCommand { get; }
        public ICommand AddTransportCommand { get; }
        #endregion
        #region Private Methods
        public void AddIndirect()
        {
            var indirect = new IndirectsModel
            {
                Concepto = SelectedIndirect.Concept,
                PrecioUnitario = Convert.ToDecimal(selectedUnitCost),
                Cantidad = ObtenerCantidad($"{SelectedIndirect.Concept}i"),
                PoliticaViaticosId = SelectedIndirect.Id,
            };

            OtherIndirectsInstaller.Add(indirect);
            indirect.Cantidad = ObtenerCantidad($"{SelectedIndirect.Concept}s");
            OtherIndirectsSupervisor.Add(indirect);
            indirect.Cantidad = ObtenerCantidad($"{SelectedIndirect.Concept}se");
            OtherIndirectsSecurity.Add(indirect);
            indirect.Cantidad = ObtenerCantidad($"{SelectedIndirect.Concept}v");
            OtherIndirectsVisit.Add(indirect);
        }
        public void AddTransport()
        {
            var politicaViaticosId = SelectedTransportType.Id == 1 ? SelectedZone.Id == 1 ? 5 : 19 : SelectedZone.Id == 1 ? 6 : 20;
            var indirectInstaller = new IndirectsModel
            {
                Concepto = SelectedTransportType.Description,
                PrecioUnitario = Convert.ToDecimal(selectedTrasnportCost),
                Cantidad = TotalForeingResourceInstaller,
                PoliticaViaticosId = politicaViaticosId,
            };
            var indirectSupervisor = new IndirectsModel
            {
                Concepto = SelectedTransportType.Description,
                PrecioUnitario = Convert.ToDecimal(selectedTrasnportCost),
                Cantidad = 1,
                PoliticaViaticosId = politicaViaticosId,
            };
            var indirectSecurity = new IndirectsModel
            {
                Concepto = SelectedTransportType.Description,
                PrecioUnitario = Convert.ToDecimal(selectedTrasnportCost),
                Cantidad = 1,
                PoliticaViaticosId = politicaViaticosId,
            };
            var indirectVisit = new IndirectsModel
            {
                Concepto = SelectedTransportType.Description,
                PrecioUnitario = Convert.ToDecimal(selectedTrasnportCost),
                Cantidad = 1,
                PoliticaViaticosId = politicaViaticosId,
            };
            OtherIndirectsInstaller.Add(indirectInstaller);
            OtherIndirectsSupervisor.Add(indirectSupervisor);
            OtherIndirectsSecurity.Add(indirectSecurity);
            OtherIndirectsVisit.Add(indirectVisit);
        }
        private int ObtenerCantidad(string nameIndirect)
        { 
            int cantidad = 1;
            

            switch (nameIndirect)
            {
                case "Hoteli": 
                case "Hidratacióni": cantidad = (TotalEfectiveDaysInstaller + TotalNoWorkDaysInstaller) * TotalResourceInstaller; break;
                case "Alimentos c/comprobantei": 
                case "Alimentos s/comprobantei": cantidad = (TotalForeingResourceInstaller * 3) * (TotalEfectiveDaysInstaller + TotalNoWorkDaysInstaller); break;
                case "Pasajes localesi": 
                case "Renta de autosi": cantidad = TotalEfectiveDaysInstaller; break; 
                case "Taxis aereopuerto/terminali": 
                case "Transporte aereopuerto - casai": 
                case "Transporte casa - aereopuertoi": cantidad = TotalEvents * TotalForeingResourceInstaller; break;

                case "Taxis aereopuerto/terminals":
                case "Transporte aereopuerto - casas":
                case "Transporte casa - aereopuertos": cantidad = TotalEvents * _ManPowerVm.ManPower.Where(m=>m.Recurso.Id==2).Count(); break;

                case "Hotels":
                case "Hidratacións": 
                case "Hotelse":
                case "Hidrataciónse": cantidad = (TotalEfectiveDaysSupervisor + TotalNoWorkDaysSupervisor); break;
                case "Alimentos c/comprobantes":
                case "Alimentos s/comprobantes": 
                case "Alimentos c/comprobantese":
                case "Alimentos s/comprobantese": cantidad = (3 * (TotalEfectiveDaysSupervisor + TotalNoWorkDaysSupervisor)); break;

                case "Pasajes localesse":
                case "Pasajes localess":
                case "Renta de autoss": 
                case "Renta de autosse": cantidad = TotalEfectiveDaysSupervisor; break;

                case "Taxis aereopuerto/terminalse":
                case "Transporte aereopuerto - casase":
                case "Transporte casa - aereopuertose": cantidad = TotalEvents * _ManPowerVm.ManPower.Where(m => m.Recurso.Id == 3).Count(); break;

                case "Alimentos c/comprobantev":
                case "Alimentos s/comprobantev": cantidad = 3; break;
                case "Hidrataciónv": 
                case "Pasajes localesv":
                case "Renta de autosv":
                case "Taxis aereopuerto/terminalv":
                case "Transporte aereopuerto - casav":
                case "Transporte casa - aereopuertov": cantidad = 1; break;

                case "SemanaT": cantidad= TotalWeeks; break;
                case "DiaT": cantidad= TotalEfectiveDaysInstaller; break;
                case "MesT": cantidad= 1; break;
                case "NivelT": cantidad= TotalFloors/10; break;
            }
            return cantidad;
        }
        private void RemoveIndirect(Guid floorUuid)
        {
            OtherIndirectsInstaller.Remove(OtherIndirectsInstaller.FirstOrDefault(x => x.Uuid == floorUuid));
            OtherIndirectsSupervisor.Remove(OtherIndirectsSupervisor.FirstOrDefault(x => x.Uuid == floorUuid));
            OtherIndirectsSecurity.Remove(OtherIndirectsSecurity.FirstOrDefault(x => x.Uuid == floorUuid));
            OtherIndirectsVisit.Remove(OtherIndirectsVisit.FirstOrDefault(x => x.Uuid == floorUuid));
        }
        private void Clear()
        {
            IndirectsInstallers.Clear();
            IndirectsSupervisor.Clear();
            IndirectsSecurity.Clear();
            IndirectsVisit.Clear();
            MinorTool.Clear();
            MajorTool.Clear();
            OtherIndirectsInstaller.Clear();
            OtherIndirectsSupervisor.Clear();
            OtherIndirectsSecurity.Clear();
            OtherIndirectsVisit.Clear();
        }
        private int GetTotalEvents(List<Floor> floors)
        {
            int total = 0;
            int floorsCount = floors.Count;
            total = floorsCount <= 20 ? 1 : 1 + (int)Math.Ceiling((floorsCount - 20) / 10.0);
            return total;
        }
        private void SumIndirect(int type, ObservableCollection<IndirectsModel> lista) {
            foreach (var indirect in lista)
            {
                switch (type)
                {
                    case 1: SubTotalCostInstallers += indirect.CostoToal; break;
                    case 2: SubTotalCostSupervisor += indirect.CostoToal; break;
                    case 3: SubTotalCostSecurity += indirect.CostoToal; break;
                    case 4: SubTotalCostVisit += indirect.CostoToal; break;
                }
                
            }
        }
        private void CalculateTools()
        {
            MinorTool.Add(new IndirectsModel { Concepto = "2% de MO", Cantidad = 1, PrecioUnitario = _ManPowerVm.TotalPriceManPowerInstaller * 0.02m });

            foreach (var tool in AllTools)
            {
                switch (tool.Description)
                {
                    case "Escalera":
                        if (tool.IsMandatory)
                            MajorTool.Add(new IndirectsModel { Concepto = tool.Description, Cantidad = ObtenerCantidad($"{tool.Preiodicity}T"), PrecioUnitario = tool.Price }); break;
                    case "Andamio":
                        if (tool.IsMandatory && HasDoubleHeightLevels)
                            MajorTool.Add(new IndirectsModel { Concepto = tool.Description, Cantidad = ObtenerCantidad($"{tool.Preiodicity}T"), PrecioUnitario = tool.Price }); break;
                    case "Izaje":
                        if (tool.IsMandatory && SelectedIzaje.Id == 1)
                            MajorTool.Add(new IndirectsModel { Concepto = tool.Description, Cantidad = ObtenerCantidad($"{tool.Preiodicity}T"), PrecioUnitario = tool.Price }); break;
                    case "Sanitarios":
                        SubTotalPriceWC = ObtenerCantidad($"{tool.Preiodicity}T") * tool.Price;break;
                    case "Bodega":
                        SubTotalPriceStore = ObtenerCantidad($"{tool.Preiodicity}T") * tool.Price; break;
                }                
            }

            //MajorTool.Add(new IndirectsModel { Concepto = "Escalera", Cantidad = TotalWeeks, PrecioUnitario = 1350m });

            //if (HasDoubleHeightLevels)
            //    MajorTool.Add(new IndirectsModel { Concepto = "Andamio", Cantidad = TotalWeeks, PrecioUnitario = 2500m });
            //if (SelectedIzaje.Id == 1)
            //    MajorTool.Add(new IndirectsModel { Concepto = "Izaje", Cantidad = TotalWeeks, PrecioUnitario = TotalFloors <= 20 ? 2500m : 5000m });
        }
        #endregion
    }
}
