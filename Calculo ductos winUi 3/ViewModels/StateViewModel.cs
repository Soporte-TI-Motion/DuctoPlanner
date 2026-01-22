using Calculo_ductos.Params;
using Calculo_ductos_winUi_3.Models;
using Calculo_ductos_winUi_3.Services;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Services.Store;


namespace Calculo_ductos_winUi_3.ViewModels
{
    public class StateViewModel 
    {
        #region Fields
        private readonly string _baseUrl = "http://192.168.10.228:8092/CotizadorApiVertical/Api/";
        //private readonly string _baseUrl = "http://localhost:8081/CotizadorApiVertical/Api/";
        private List<CatalogRowModel> PurposeCatalog;
        private List<CatalogRowModel> DoorTypeCatalog;
        private List<CatalogRowModel> SheetTypeCatalog;
        private List<CatalogRowModel> FloorTypeCatalog;
        private List<CatalogRowEntityModel> StateCatalog;
        private List<CatalogRowEntityModel> MunicipalityCatalog;
        private List<CatalogRowEntityModel> LocalityCatalog;
        private List<CatalogRowTruckTypeModel> TruckTypeCatalog;
        private List<CatalogResourceModel> ResourceCatalog;
        private List<CatalogResourceTypeModel> ResourceTypeCatalog;
        private List<CatalogRentabilityModel> Rentabilities;
        private List<CatalogIndirectModel> Indirects;
        private List<CatalogZoneModel> Zones;
        private List<CatalogKitModel> KitsCatalog;
        private List<CatalogToolModel> ToolsCatalog;

        private WebApi Client;
        private readonly BusyService _Busy;
        #endregion
        #region Constructor
        public StateViewModel()
        {
            _Busy = new BusyService();
            Client = new WebApi(_baseUrl);
            _ = InitializeAsync();
            FloorVM = new FloorDescriptionViewModel(Busy);
            DuctsVM = new DuctsViewModel();
            ComponentsVM = new ComponentsViewModel();
            CompleteDuctVm = new CompleteDuctViewModel();
            CompleteDuctVm.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(CompleteDuctVm.PurposeId))
                {
                    FloorVM.FilterDoorsTypes(CompleteDuctVm.PurposeId == 0 ? "ropa" : "basura");
                }
            };
            FreightVM = new CalculateFreightViewModel(Client);
            ManPowerVM = new ManPowerViewModel();
            IndirectsVM = new IndirectsViewModel(ManPowerVM);
        }

        #endregion
        #region Properties
        public BusyService Busy
        {
            get => _Busy;
        }
        public FloorDescriptionViewModel FloorVM { get; }
        public DuctsViewModel DuctsVM { get; }
        public ComponentsViewModel ComponentsVM { get; }
        public CompleteDuctViewModel CompleteDuctVm { get; }
        public CalculateFreightViewModel FreightVM { get; }
        public ManPowerViewModel ManPowerVM { get; } 
        public IndirectsViewModel IndirectsVM { get; } 
        public string AppVersion => GetAppVersion();
        //public decimal TotalPrice { get { return CompleteDuctVm.TotalPrice + FreightVM.Freight.TotalPrice + ManPowerVM.TotalPriceManPower + IndirectsVM.TotalCostInstallers + IndirectsVM.TotalCostSupervisor + IndirectsVM.TotalCostSecurity + IndirectsVM.TotalCostVisit; } }
        public decimal TotalPrice { get 
            { 
                return 
                    CompleteDuctVm.SubTotalPrice + 
                    FreightVM.Freight.TotalPrice + 
                    ManPowerVM.TotalPriceManPower + 
                    IndirectsVM.TotalCostInstallers + 
                    IndirectsVM.TotalCostSupervisor + 
                    IndirectsVM.TotalCostSecurity + 
                    IndirectsVM.TotalCostVisit +
                    IndirectsVM.TotalPriceWC +
                    IndirectsVM.TotalPriceStore; } }
        public decimal SubTotalPrice { get { 
                return CompleteDuctVm.SubTotalPrice + 
                    FreightVM.Freight.SubTotalPrice + 
                    ManPowerVM.SubTotalPriceManPower + 
                    IndirectsVM.SubTotalCostInstallers + 
                    IndirectsVM.SubTotalCostSupervisor + 
                    IndirectsVM.SubTotalCostSecurity + 
                    IndirectsVM.SubTotalCostVisit+
                    IndirectsVM.SubTotalPriceWC+
                    IndirectsVM.SubTotalPriceStore; } }
        public string TotalPriceFormatted => $"Precio: $ {TotalPrice:N2}";
        public string SubTotalPriceFormatted => $"Costo: $ {SubTotalPrice:N2}";


        #endregion


        public async Task CalculateDucts(object sender, RoutedEventArgs e)
        {
            try
            {
                await ShowLoader("Calculando despiece...");

                DuctsVM.CalculateDuctsCommand.Execute(this.ToJsonString());
                ComponentsVM.CalculateComponentsCommand.Execute(DuctsVM.CompleteDuct);

                RemoveEmptyPieces();
                DiferenceFloors();
                CountDoubleLevels();
                CompleteDuctVm.CalculatePrice(DuctsVM, ComponentsVM, FloorVM);
                //IndirectsVM.LoadTotals(DuctsVM.CompleteDuct);
                await HideLoader("Calculo terminado.");

            }
            catch (Exception ex)
            {

                await HideLoader(ex.Message,18000);
            }
        }
        private void RemoveEmptyPieces() 
        {
            var floorList = DuctsVM.DuctDetailList.GroupBy(duct => duct.FloorName).Select(g => new { Name = g.Key, Count = g.Count() }).ToList();
            var itemsToRemove = DuctsVM.DuctDetailList.Where(duct => duct.Count == 0).ToList();
            foreach (var item in itemsToRemove)
            {
                DuctsVM.DuctDetailList.Remove(item);
            }

            foreach (var floorName in floorList)
            {
                if (floorName.Count == itemsToRemove.Where(d => d.FloorName.Equals(floorName.Name)).Count())
                    DuctsVM.DuctDetailList.Insert(0, new FloorDuctDetailModel { FloorName = floorName.Name, DuctType = DuctPiece.TypeDuct.SinDucto });
            }

            var ductsToRemove = DuctsVM.DucList.Where(duct => duct.Count == 0).ToList();
            foreach (var item in ductsToRemove)
            {
                DuctsVM.DucList.Remove(item);
            }

            var componentsToRemove = ComponentsVM.ComponentList.Where(component => component.Count == 0).ToList();
            foreach (var item in componentsToRemove)
            {
                ComponentsVM.ComponentList.Remove(item);
            }
        }
        private void DiferenceFloors()
        {
            //string? lastFloor = null;
            var count = DuctsVM.DuctDetailList.Count - 2;
            for ( int i = 0;  i <= count; i++)
            {
                var duct = DuctsVM.DuctDetailList[i];
                var ductNext = DuctsVM.DuctDetailList[i+1];
                duct.IsNewFloor = duct.FloorName != ductNext.FloorName;
                if (i == count)
                    ductNext.IsNewFloor = true;
                //lastFloor = duct.FloorName;
            }
        }
        private void CountDoubleLevels()
        { 
            CompleteDuctVm.TotalDoubleLevels = DuctsVM.CompleteDuct.floors.Where(floor => floor.Height >= 4.5m).Count();
        }
        public async Task CalculateFreigth(object sender, RoutedEventArgs e) 
        {
            await ShowLoader("Cargando información...");
            await FreightVM.CalculateFreight(DuctsVM.DucList.ToList(), CompleteDuctVm.SelectedRentability);
            ManPowerVM.CalculateWorkDays(DuctsVM.CompleteDuct, FreightVM.SelectedState);
            await HideLoader("Informacion cargada", 500);
            if (FreightVM.Freight.Price == 0)
               await ShowEmptyDataDialog("Por el momento no se tiene un costo para este destino, favor de contactar con almacén para cotizar.");
        }
        public async Task CalculateManPower(object sender, RoutedEventArgs e) {
            await ShowLoader("Calculando mano de obra...");
            await ManPowerVM.CalculateManPower(CompleteDuctVm.SelectedRentability);
            IndirectsVM.LoadTotals(DuctsVM.CompleteDuct);
            await HideLoader("Calculo terminado", 500);
        }
        public async Task CalculateIndirects(object sender, RoutedEventArgs e) {
            await ShowLoader("Calculando indirectos...");
            await IndirectsVM.CalculateIndirects(CompleteDuctVm.SelectedRentability,FreightVM.SelectedState.Name.Equals("CIUDAD DE MÉXICO"));
            await HideLoader("Calculo terminado", 500);
        }
        private async Task InitializeAsync()
        {
            await ShowLoader("Cargando datos...");
            await LoadCatalogsAsync();
            await LoadQuotesAsync();
            await HideLoader("Datos cargados.");
            
        }

        #region Api
        private async Task LoadCatalogsAsync() 
        {
            PurposeCatalog = new List<CatalogRowModel>();
            DoorTypeCatalog = new List<CatalogRowModel>();
            SheetTypeCatalog = new List<CatalogRowModel>();
            FloorTypeCatalog = new List<CatalogRowModel>();
            KitsCatalog = new List<CatalogKitModel>();
            ToolsCatalog = new List<CatalogToolModel>();
            
            try
            {
                var catalogListWrapper = await Client.GetAsync<CatalogModelList>("Catalog");

                if (catalogListWrapper != null)
                {
                    PurposeCatalog = catalogListWrapper.PurposeCatalog ?? new();
                    DoorTypeCatalog = catalogListWrapper.DoorTypeCatalog ?? new();
                    SheetTypeCatalog = catalogListWrapper.SheetTypeCatalog ?? new();
                    FloorTypeCatalog = catalogListWrapper.FloorTypeCatalog ?? new();
                    StateCatalog = catalogListWrapper.Entities ?? new();
                    MunicipalityCatalog = catalogListWrapper.Municipalities ?? new();
                    LocalityCatalog = catalogListWrapper.Localities ?? new();
                    TruckTypeCatalog = catalogListWrapper.TruckTypeCatalog ?? new();
                    ResourceCatalog = catalogListWrapper.Resources?? new();
                    ResourceTypeCatalog = catalogListWrapper.ResourceTypes ?? new();
                    Rentabilities = catalogListWrapper.Rentabilities ?? new();
                    Indirects = catalogListWrapper.Indirects ?? new();
                    Zones = catalogListWrapper.Zones ?? new();
                    KitsCatalog = catalogListWrapper.Kits ?? new();
                    ToolsCatalog = catalogListWrapper.Tools ?? new();

                    FloorVM.LoadCatalogs(DoorTypeCatalog);
                    FloorVM.FilterDoorsTypes("basura");//ya que esta seleccionado por automatico basura
                    CompleteDuctVm.LoadCatalogs(KitsCatalog, Rentabilities);
                    FreightVM.LoadCatalogs(StateCatalog, MunicipalityCatalog, LocalityCatalog, TruckTypeCatalog);
                    ManPowerVM.LoadCatalogs(ResourceCatalog, ResourceTypeCatalog);
                    IndirectsVM.LoadCatalogs(Indirects,Zones,ToolsCatalog);

                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine($"[Catalog Load Error]: {ex.Message}");
               
            }
        }
        private async Task LoadQuotesAsync() {
            try
            {
                var quotes = await Client.GetAsync<ObservableCollection<QuoteModel>>("Quoter");

                if (quotes != null)
                {
                    //CompleteDuctVm.Quotes.Clear();
                    CompleteDuctVm.Quotes = quotes;
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine($"[Quotes Load Error]: {ex.Message}");

            }
        }
        private async Task LoadQuoteDetail(int id)
        {
            try
            {
                var quote = await Client.GetAsync<QuoteDetailModel>($"Quoter/{id}");

                if (quote != null)
                {
                    var selectedState = FreightVM.AllStates.Where(state => state.Id == quote.EntidadId).FirstOrDefault();
                    var selectedMunicipality = FreightVM.AllMunicipalities.Where(municipality => municipality.Id == quote.MunicipioId).FirstOrDefault();
                    var selectedLocality = FreightVM.AllLocalities.Where(locality => locality.Id == quote.LocalidadId).FirstOrDefault();
                    var selectedRentability = CompleteDuctVm.AvailableRentabilities.Where(rentability => rentability.Id == quote.RentabilidadMOId).FirstOrDefault();
                    var izajeId = quote.NecesitaIzaje ? 1 : 2;
                    var selectedIzaje = IndirectsVM.AvailableIzaje.Where(q => q.Id == izajeId).FirstOrDefault();
                    var selectedZone = IndirectsVM.AvailableZones.Where(z => z.Id == quote.ZonaId).FirstOrDefault();
                    
                    //Ids de politica de viajes que se tienen en BD para transporte
                    var ids = new List<int> { 5, 6, 19, 20 };
                    var idsAereos = new List<int> { 5, 19 };
                    var trasnportIndirect = quote.Viaticos.Where(v => ids.Contains(v.PoliticaViaticosId)).FirstOrDefault();

                    this.New();
                    CompleteDuctVm.QuoteVersion = quote.NumeroVersion;
                    CompleteDuctVm.ExecutiveName = quote.NombreEjecutivo;
                    CompleteDuctVm.PT = quote.PT;
                    CompleteDuctVm.PurposeId = quote.PropositoId;
                    CompleteDuctVm.SheetTypeId = quote.TipoLaminaId;
                    CompleteDuctVm.NeedSprinkler = quote.NecesitaAspersor;
                    CompleteDuctVm.NeedDesinfectionSystem = quote.NecesitaSistemaDD;
                    CompleteDuctVm.SelectedRentability = selectedRentability;
                    FloorVM.FloorList = quote.MapQuoteDetailToFloorList(FloorVM);
                    FreightVM.SelectedState = selectedState;
                    FreightVM.SelectedMunicipality = selectedMunicipality;
                    FreightVM.SelectedLocality = selectedLocality;
                    IndirectsVM.SelectedIzaje = selectedIzaje;
                    IndirectsVM.SelectedZone = selectedZone;
                    IndirectsVM.OtherIndirectsInstaller = quote.MapQuoteDetailToIndirects();
                    IndirectsVM.OtherIndirectsSupervisor = quote.MapQuoteDetailToIndirects();
                    IndirectsVM.OtherIndirectsSecurity = quote.MapQuoteDetailToIndirects();
                    IndirectsVM.OtherIndirectsVisit = quote.MapQuoteDetailToIndirects();
                    


                    DuctsVM.CalculateDuctsCommand.Execute(this.ToJsonString());
                    ComponentsVM.CalculateComponentsCommand.Execute(DuctsVM.CompleteDuct);
                    RemoveEmptyPieces();
                    DiferenceFloors();
                    CountDoubleLevels();
                    CompleteDuctVm.CalculatePrice(DuctsVM, ComponentsVM, FloorVM);

                    await FreightVM.CalculateFreight(DuctsVM.DucList.ToList(), CompleteDuctVm.SelectedRentability);
                    ManPowerVM.CalculateWorkDays(DuctsVM.CompleteDuct,selectedState);
                    CompleteDuctVm.SelectedRentability = CompleteDuctVm.AvailableRentabilities.Where(r => r.Id == quote.RentabilidadMOId).FirstOrDefault();
                    ManPowerVM.ManPower = quote.MapQuoteDetailManPower(ManPowerVM);

                    await ManPowerVM.CalculateManPower(CompleteDuctVm.SelectedRentability);
                    IndirectsVM.LoadTotals(DuctsVM.CompleteDuct);

                    if (trasnportIndirect != null) 
                    {
                        var typeTransport = idsAereos.Contains( trasnportIndirect.PoliticaViaticosId) ? 1 : 2;
                        var selectdTransport = IndirectsVM.AvailableTransportTypes.Where(t => t.Id == typeTransport).FirstOrDefault();
                        IndirectsVM.SelectedTransportType = selectdTransport;
                    }
                        await IndirectsVM.CalculateIndirects(CompleteDuctVm.SelectedRentability, FreightVM.SelectedState.Name.Equals("CIUDAD DE MÉXICO"));
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine($"[Quotes Load Error]: {ex.Message}");

            }
        }
       
        private async Task SaveState() 
        {
            await ShowLoader("Guardando datos...");
            try
            {
                var quote = this.MapStateAppToQuoteDetail();
                var response = await Client.PostAsync<QuoteDetailModel, QuoteInsertionResultModel>("Quoter", quote);
                if (response != null)
                {
                    CompleteDuctVm.QuoteVersion = response.Version;
                    await LoadQuotesAsync();
                    await HideLoader("Datos guardados con exito.");
                }
                else await HideLoader("No se pudo guardar los datos.");
            }
            catch (Exception ex)
            {

                await HideLoader("Ocurrio un erro al guardar los datos: "+ex.Message,18000);
            }
            

        }
        #endregion


        #region UI
        private string GetAppVersion()
        {
            PackageVersion v = Package.Current.Id.Version;
            return $"Version: {v.Major}.{v.Minor}.{v.Build}.{v.Revision}";
        }
        public void New() {
            FloorVM.New();
            DuctsVM.New();
            ComponentsVM.New();
            CompleteDuctVm.New();
            FreightVM.New();
            ManPowerVM.New();
            IndirectsVM.New();
        }
        public async Task LoadQuote(int id)
        {
            await ShowLoader("Cargando información...");
            await LoadQuoteDetail(id);
            await HideLoader("Informacion cargada",500);
        }
        public async Task Save() {
            await SaveState();
        }
        public async Task Export(string filePath)
        {
            await ShowLoader("Exportando...");
            try
            {
                await this.ExportToExcel(filePath);
                Trace.WriteLine("Se terminó de crear con Aspose");
                await this.FinishExport(filePath);
                Trace.WriteLine("Se terminó de crear con ClosedXML");
                await HideLoader("Se termino de exportar.");
            }
            catch (Exception ex)
            {
                Trace.WriteLine("ERROR AL GUARDAR: " + ex.ToString());
                await HideLoader("Ocurrio un error, contacte al administrador.");
            }
        }
        public async Task ShowLoader(string message, int seconds = 1000) {
            Busy.IsBusy = true;
            Busy.StatusMessage = message;
            await Task.Delay(seconds);
        }
        public async Task HideLoader(string message, int seconds = 1000)
        {
            Busy.StatusMessage = message;
            await Task.Delay(seconds);
            Busy.IsBusy = false;
        }
        public async Task ShowEmptyDataDialog(string message, int seconds = 5000)
        {
            Busy.IsBusy = true;
            Busy.StatusMessage = message;
            await Task.Delay(seconds);
            Busy.IsBusy = false;
        }
        #endregion

    }
}
