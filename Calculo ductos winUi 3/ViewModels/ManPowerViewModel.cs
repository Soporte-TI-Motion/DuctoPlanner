using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Calculo_ductos_winUi_3.ViewModels
{
    using Calculo_ductos.Params;
    using CommunityToolkit.Mvvm.Input;
    using DocumentFormat.OpenXml.Drawing.Charts;
    using Models;
    using System.Collections.ObjectModel;
    using System.Windows.Input;

    public class ManPowerViewModel : ObservableObject
    {
        #region Fields
        private CatalogResourceModel _resource;
        private CatalogResourceTypeModel _resourceType;
        private EfectiveWorkDayModel _EfectiveWorkDays;
        private decimal _TotalPriceManPower;
        private decimal _SubTotalPriceManPower;
        private decimal _TotalPriceManPowerInstaller;
        private decimal _SubTotalPriceManPowerInstaller;
        private decimal _TotalPriceManPowerSupervisor;
        private decimal _SubTotalPriceManPowerSupervisor;
        private decimal _TotalPriceManPowerSecurity;
        private decimal _SubTotalPriceManPowerSecurity;
        private decimal _TotalPriceManPowerVisit;
        private decimal _SubTotalPriceManPowerVisit;
        #endregion
        #region Constructor
        public ManPowerViewModel()
        {
            _resource = new CatalogResourceModel();
            _resourceType = new CatalogResourceTypeModel();
            _EfectiveWorkDays = new EfectiveWorkDayModel();
            _TotalPriceManPower = 0m;
            _SubTotalPriceManPower = 0m;
            _TotalPriceManPowerInstaller = 0m;
            _SubTotalPriceManPowerInstaller = 0m;
            _TotalPriceManPowerSupervisor = 0m;
            _SubTotalPriceManPowerSupervisor = 0m;
            _TotalPriceManPowerSecurity = 0m;
            _SubTotalPriceManPowerSecurity = 0m;
            _TotalPriceManPowerVisit = 0m;
            _SubTotalPriceManPowerVisit = 0m;
            AddResourceCommand = new RelayCommand(AddResource);
            RemoveResourceCommand = new RelayCommand<Guid>(RemoveResource);
            ManPower = new ObservableCollection<HumanResourceModel>();
            Subtotals = new ObservableCollection<SubtotalHumaResource>();
        }
        public void New()
        {
            _resource = new CatalogResourceModel();
            _resourceType = new CatalogResourceTypeModel();
            TotalPriceManPower = 0;
            SubTotalPriceManPower = 0;
            ManPower.Clear();
            EfectiveWorkDays = new EfectiveWorkDayModel();
            Subtotals.Clear();
            TotalPriceManPowerInstaller = 0;
            TotalPriceManPowerSupervisor = 0;
            TotalPriceManPowerSecurity = 0;
            TotalPriceManPowerVisit = 0;
            SubTotalPriceManPowerInstaller = 0;
            SubTotalPriceManPowerSupervisor = 0;
            SubTotalPriceManPowerSecurity = 0;
            SubTotalPriceManPowerVisit = 0;

        }
        #endregion
        #region Properties
        public CatalogResourceModel Resource
        {
            get => _resource;
            set {
                SetProperty(ref _resource, value);
            }
        }
        public CatalogResourceTypeModel ResourceType
        {
            get => _resourceType;
            set
            {
                SetProperty(ref _resourceType, value);
            }
        }
        public EfectiveWorkDayModel EfectiveWorkDays
        {
            get => _EfectiveWorkDays;
            set { SetProperty(ref _EfectiveWorkDays, value); }
        }
        public ObservableCollection<HumanResourceModel> ManPower { get; set; }
        public ObservableCollection<CatalogResourceModel> AvailableResources { get; set; }
        public ObservableCollection<CatalogResourceTypeModel> AvailableResourceTypes { get; set; }
        public ObservableCollection<SubtotalHumaResource> Subtotals { get; set; }
        
        public decimal TotalPriceManPower{ get => _TotalPriceManPower; set {SetProperty(ref _TotalPriceManPower, value);}}
        public decimal SubTotalPriceManPower{ get => _SubTotalPriceManPower; set {SetProperty(ref _SubTotalPriceManPower, value);}}
        public string TotalPriceFormatted => $"Precio: $ {TotalPriceManPower:N2}";
        public string SubTotalPriceFormatted => $"Costo: $ {SubTotalPriceManPower:N2}";

        public decimal TotalPriceManPowerInstaller { get => _TotalPriceManPowerInstaller; set { SetProperty(ref _TotalPriceManPowerInstaller, value); } }
        public decimal SubTotalPriceManPowerInstaller { get => _SubTotalPriceManPowerInstaller; set { SetProperty(ref _SubTotalPriceManPowerInstaller, value); } }
        public string TotalPriceInstallerFormatted => $"Precio: $ {TotalPriceManPowerInstaller:N2}";
        public string SubTotalPriceInstallerFormatted => $"Costo: $ {SubTotalPriceManPowerInstaller:N2}";

        public decimal TotalPriceManPowerSupervisor { get => _TotalPriceManPowerSupervisor; set { SetProperty(ref _TotalPriceManPowerSupervisor, value); } }
        public decimal SubTotalPriceManPowerSupervisor { get => _SubTotalPriceManPowerSupervisor; set { SetProperty(ref _SubTotalPriceManPowerSupervisor, value); } }
        public string TotalPriceSupervisorFormatted => $"Precio: $ {TotalPriceManPowerSupervisor:N2}";
        public string SubTotalPriceSupervisorFormatted => $"Costo: $ {SubTotalPriceManPowerSupervisor:N2}";

        public decimal TotalPriceManPowerSecurity { get => _TotalPriceManPowerSecurity; set { SetProperty(ref _TotalPriceManPowerSecurity, value); } }
        public decimal SubTotalPriceManPowerSecurity { get => _SubTotalPriceManPowerSecurity; set { SetProperty(ref _SubTotalPriceManPowerSecurity, value); } }
        public string TotalPriceSecurityFormatted => $"Precio: $ {TotalPriceManPowerSecurity:N2}";
        public string SubTotalPriceSecurityFormatted => $"Costo: $ {SubTotalPriceManPowerSecurity:N2}";

        public decimal TotalPriceManPowerVisit { get => _TotalPriceManPowerVisit; set { SetProperty(ref _TotalPriceManPowerVisit, value); } }
        public decimal SubTotalPriceManPowerVisit { get => _SubTotalPriceManPowerVisit; set { SetProperty(ref _SubTotalPriceManPowerVisit, value); } }
        public string TotalPriceVisitFormatted => $"Precio: $ {TotalPriceManPowerVisit:N2}";
        public string SubTotalPriceVisitFormatted => $"Costo: $ {SubTotalPriceManPowerVisit:N2}";

        #endregion
        #region Public Methods

        public async Task CalculateManPower(CatalogRentabilityModel rentability)
        {
            Subtotals.Clear ();
            UpdateEffectiveDaysToMO();
            foreach (var model in ManPower) {
                Subtotals.Add(new SubtotalHumaResource {Descripcion = model.Recurso.Description, Subtotal = model.PrecioTotal });
            }
            Subtotals.Add(new SubtotalHumaResource { Descripcion = "Visita técnica", Subtotal = AvailableResources.Where(p => p.Id == 2).FirstOrDefault().SalaryPerWorkday});

            SubTotalPriceManPower = Subtotals.Select(p => p.Subtotal).Sum();
            TotalPriceManPower = SubTotalPriceManPower * rentability.Rentability;

            SubTotalPriceManPowerInstaller = Subtotals.Where(p => p.Descripcion.Contains("instalador")).Select(p => p.Subtotal).Sum();
            SubTotalPriceManPowerSupervisor = Subtotals.Where(p => p.Descripcion.Contains("obra")).Select(p => p.Subtotal).Sum();
            SubTotalPriceManPowerSecurity = Subtotals.Where(p => p.Descripcion.Contains("seguridad")).Select(p => p.Subtotal).Sum();
            SubTotalPriceManPowerVisit = Subtotals.Where(p => p.Descripcion.Contains("Visita")).Select(p => p.Subtotal).Sum();

            TotalPriceManPowerInstaller = SubTotalPriceManPowerInstaller * rentability.Rentability;
            TotalPriceManPowerSupervisor = SubTotalPriceManPowerSupervisor * rentability.Rentability;
            TotalPriceManPowerSecurity = SubTotalPriceManPowerSecurity * rentability.Rentability;
            TotalPriceManPowerVisit = SubTotalPriceManPowerVisit * rentability.Rentability;
        }
        public void RecalculateRentability(CatalogRentabilityModel rentability)
        {
            TotalPriceManPower = SubTotalPriceManPower * rentability.Rentability;
            TotalPriceManPowerInstaller = SubTotalPriceManPowerInstaller * rentability.Rentability;
            TotalPriceManPowerSupervisor = SubTotalPriceManPowerSupervisor * rentability.Rentability;
            TotalPriceManPowerSecurity = SubTotalPriceManPowerSecurity * rentability.Rentability;
            TotalPriceManPowerVisit = SubTotalPriceManPowerVisit * rentability.Rentability;
        }
        public void LoadCatalogs(List<CatalogResourceModel> resources, List<CatalogResourceTypeModel> resourceTypes)
        {
            AvailableResources = new ObservableCollection<CatalogResourceModel>(resources);
            AvailableResourceTypes = new ObservableCollection<CatalogResourceTypeModel>(resourceTypes);
        }
        public void CalculateWorkDays(Duct duct,CatalogRowEntityModel entidad)
        {
            EfectiveWorkDays.WorkDaysBase = Convert.ToInt32(Math.Ceiling((duct.floors.Count ) / 2.5));
            EfectiveWorkDays.WorkDaysDobleFloors = duct.floors.Where(p => p.Height >= 4.5m ).ToList().Count * 0.5;
            EfectiveWorkDays.WorkDaysExtraFloors = duct.floors.Count > 10 ? 1 : 0;
            EfectiveWorkDays.WorkDayForeign = entidad.Name.Equals("CIUDAD DE MÉXICO") ? 0 : 1;
            EfectiveWorkDays.NoWorkDays = EfectiveWorkDays.WorkDaysBase / 7;
        }
        #endregion
        #region Commands
        public ICommand AddResourceCommand { get; }
        public ICommand RemoveResourceCommand { get; }
        #endregion
        #region Private Methods
        private void AddResource()
        {
            var human = new HumanResourceModel();
            human.Recurso = Resource;
            human.TipoRecurso = ResourceType;
            human.JornadasEfectivas = Resource.Id != 1 ? EfectiveWorkDays.TotalWorkDays + 3 : EfectiveWorkDays.TotalWorkDays;
            human.DiasNoLaborales = ResourceType.Id == 1 ? human.JornadasEfectivas / 7 : 0;
            ManPower.Add(human);
        }
        private void RemoveResource(Guid uuid)
        {
            ManPower.Remove(ManPower.FirstOrDefault(x => x.Uuid == uuid));
        }
        private void UpdateEffectiveDaysToMO()
        {
            foreach (var human in ManPower)
            {
                human.JornadasEfectivas = human.Recurso.Id != 1 ? EfectiveWorkDays.TotalWorkDays + 3 : EfectiveWorkDays.TotalWorkDays;
                human.DiasNoLaborales = human.TipoRecurso.Id == 1 ? human.JornadasEfectivas / 7 : 0;
            }
        }
        #endregion
    }
}
