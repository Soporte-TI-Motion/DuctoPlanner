using Calculo_ductos_winUi_3.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Calculo_ductos.Params;
using DocumentFormat.OpenXml.Bibliography;
using Calculo_ductos_winUi_3.ViewModels;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Security.AccessControl;
using System.Security.Cryptography;

namespace Calculo_ductos_winUi_3.Services
{
    public static class Mapper
    {
        public static ObservableCollection<DuctModel> MapDuctsFromList(this List<DuctPiece> ducts)
        {
            return new ObservableCollection<DuctModel>(
                ducts.Select(kvp => new DuctModel
                {
                    Type = kvp.Type,
                    Count = kvp.Count
                }));
        }
        public static ObservableCollection<FloorDuctDetailModel> MapToFloorDuctDetails(this List<Floor> floors)
        {
            var result = new ObservableCollection<FloorDuctDetailModel>();

            foreach (var floor in floors)
            {
                foreach (var duct in floor.Ducts)
                {
                    result.Add(new FloorDuctDetailModel
                    {
                        FloorName = floor.Name,
                        DuctType = duct.Type,
                        Count = duct.Count
                    });
                }
            }

            return result;
        }
        public static ObservableCollection<ComponentModel> MapComponents(this List<Component> components) 
        {
            return new ObservableCollection<ComponentModel>(
                components.Select(component => new ComponentModel
                {
                    Name = component.Name,
                    Count = component.Count,
                    Type = component.Type
                }));
        }
        public static Dictionary<DuctPiece.TypeDuct, int> MapDuctsToDictionary(this ObservableCollection<DuctModel> ducts)
        {
            return ducts
                .GroupBy(duct => duct.Type)
                .ToDictionary(group => group.Key, group => group.Sum(d => d.Count));
        }
        public static QuoteDetailModel MapStateAppToQuoteDetail(this StateViewModel stateApp)
        {
            QuoteDetailModel quote = new QuoteDetailModel();
            try
            {
                //quote =
                //    new QuoteDetailModel
                //    {
                //        CotizacionId = 0,
                //        PT = stateApp.CompleteDuctVm.PT,
                //        NombreEjecutivo = stateApp.CompleteDuctVm.ExecutiveName,
                //        TipoLaminaId = stateApp.CompleteDuctVm.SheetTypeId,
                //        PropositoId = stateApp.CompleteDuctVm.PurposeId,
                //        Diametro = "24\"",
                //        SiteRef ="VERS",
                //        NecesitaAspersor = stateApp.CompleteDuctVm.NeedSprinkler,
                //        NecesitaSistemaDD = stateApp.CompleteDuctVm.NeedDesinfectionSystem,
                //        LocalidadId = stateApp.FreightVM.SelectedLocality.Id,
                //        RentabilidadMOId = stateApp.CompleteDuctVm.SelectedRentability.Id,
                //        NecesitaIzaje = stateApp.IndirectsVM.SelectedIzaje.Id == 1,
                //        ZonaId = stateApp.IndirectsVM.SelectedZone.Id,
                //        Niveles = stateApp.FloorVM.FloorList.Select(
                //            floor => new FloorDetailModel
                //            {
                //                TipoNivelId = Convert.ToInt32(floor.Type),
                //                Cantidad = floor.FloorCount,
                //                Altura = floor.FloorHeight,
                //                NecesitaPuerta = floor.NeedGate,
                //                NecesitaChimenea = floor.NeedChimney,
                //                NecesitaAntiImpactos = floor.NeedAntiImpact,
                //                TipoPuertaId = floor.TypeDoor.Id,
                //                TipoDescargaId = Convert.ToInt32(floor.Discharge)
                //            }
                //            ).ToList(),
                //        ManoDeObra = stateApp.ManPowerVM.ManPower.Select(
                //            resource => new HumanResource{
                //                RecursoId = resource.Recurso.Id,
                //                TipoRecursoId = resource.TipoRecurso.Id
                //            }).ToList(),
                //        //por el momento se toma solo de uno ya que actualmente la logica de negocio agrega lo mismo a todos
                //        Viaticos = stateApp.IndirectsVM.OtherIndirectsInstaller.Select(
                //            viatico=> new Viatico { 
                //                Concepto = viatico.Concepto, 
                //                Cantidad = viatico.Cantidad, 
                //                PrecioUnitario = viatico.PrecioUnitario, 
                //                PoliticaViaticosId = viatico.PoliticaViaticosId
                //            }).ToList(),
                //    };

                //version para contemplar nulos
                quote = new QuoteDetailModel
                {
                    CotizacionId = 0,
                    PT = stateApp?.CompleteDuctVm?.PT,
                    NombreEjecutivo = stateApp?.CompleteDuctVm?.ExecutiveName,
                    TipoLaminaId = stateApp?.CompleteDuctVm?.SheetTypeId ?? 0,
                    PropositoId = stateApp?.CompleteDuctVm?.PurposeId ?? 0,
                    Diametro = "24\"",
                    SiteRef = "VERS",
                    NecesitaAspersor = stateApp?.CompleteDuctVm?.NeedSprinkler ?? false,
                    NecesitaSistemaDD = stateApp?.CompleteDuctVm?.NeedDesinfectionSystem ?? false,

                    LocalidadId = stateApp?.FreightVM?.SelectedLocality?.Id ?? 0,
                    RentabilidadMOId = stateApp?.CompleteDuctVm?.SelectedRentability?.Id ?? 0,

                    NecesitaIzaje = (stateApp?.IndirectsVM?.SelectedIzaje?.Id ?? 0) == 1,
                    ZonaId = stateApp?.IndirectsVM?.SelectedZone?.Id ?? 0,

                    Niveles = stateApp?.FloorVM?.FloorList?
                    .Select(floor => new FloorDetailModel
                    {
                        TipoNivelId = Convert.ToInt32(floor?.Type ?? 0),
                        Cantidad = floor?.FloorCount ?? 0,
                        Altura = floor?.FloorHeight ?? 0,
                        NecesitaPuerta = floor?.NeedGate ?? false,
                        NecesitaChimenea = floor?.NeedChimney ?? false,
                        NecesitaAntiImpactos = floor?.NeedAntiImpact ?? false,
                        TipoPuertaId = floor?.TypeDoor?.Id ?? 0,
                        TipoDescargaId = Convert.ToInt32(floor?.Discharge ?? 0)
                    })
                    .ToList() ?? new List<FloorDetailModel>(),

                                ManoDeObra = stateApp?.ManPowerVM?.ManPower?
                    .Select(resource => new HumanResource
                    {
                        RecursoId = resource?.Recurso?.Id ?? 0,
                        TipoRecursoId = resource?.TipoRecurso?.Id ?? 0
                    })
                    .ToList() ?? new List<HumanResource>(),

                                Viaticos = stateApp?.IndirectsVM?.OtherIndirectsInstaller?
                    .Select(viatico => new Viatico
                    {
                        Concepto = viatico?.Concepto,
                        Cantidad = viatico?.Cantidad ?? 0,
                        PrecioUnitario = viatico?.PrecioUnitario ?? 0,
                        PoliticaViaticosId = viatico?.PoliticaViaticosId ?? 0
                    })
                    .ToList() ?? new List<Viatico>()
                };
            }
            catch (Exception ex)
            {
                Trace.Write(ex.ToString());
            }
            return quote;
        }
        public static ObservableCollection<FloorDescription> MapQuoteDetailToFloorList(this QuoteDetailModel quote, FloorDescriptionViewModel floorVm)
        {
            ObservableCollection<FloorDescription> map = new ObservableCollection<FloorDescription>();
            var floorList = quote.Niveles.Select(floor =>
                new FloorDescription {
                    Uuid = Guid.NewGuid(),
                    Type = (Floor.TypeFloor)floor.TipoNivelId,
                    FloorCount = floor.Cantidad,
                    FloorHeight = floor.Altura,
                    NeedGate = floor.NecesitaPuerta,
                    NeedChimney = floor.NecesitaChimenea,
                    NeedAntiImpact = floor.NecesitaAntiImpactos,
                    TypeDoor = floorVm.AllDoorType.Where(type => type.Id == floor.TipoPuertaId).FirstOrDefault(),
                    Discharge = (Floor.TypeDischarge)floor.TipoDescargaId
                }
            ).ToList();

            foreach (var floor in floorList)
                map.Add(floor);

            return map;
        }
        public static ObservableCollection<HumanResourceModel> MapQuoteDetailManPower(this QuoteDetailModel quote,ManPowerViewModel manPowerVm)
        {
            ObservableCollection<HumanResourceModel> map = new ObservableCollection<HumanResourceModel>();
            
            var humaResourceList = quote.ManoDeObra.Select(resource =>
                new HumanResourceModel { 
                    Uuid = new Guid(),
                    Recurso = manPowerVm.AvailableResources.Where(x => x.Id == resource.RecursoId).FirstOrDefault(),
                    TipoRecurso = manPowerVm.AvailableResourceTypes.Where(x => x.Id ==resource.TipoRecursoId).FirstOrDefault(),
                    JornadasEfectivas = resource.RecursoId == 1 ? manPowerVm.EfectiveWorkDays.TotalWorkDays : manPowerVm.EfectiveWorkDays.TotalWorkDays + 3,
                    DiasNoLaborales = manPowerVm.AvailableResourceTypes.Where(x => x.Id == resource.TipoRecursoId).FirstOrDefault().Id == 1 ? manPowerVm.EfectiveWorkDays.TotalWorkDays / 7 : 0
                }
                ).ToList();
            foreach(var resource in humaResourceList)
                map.Add(resource);
            return map;
        }

        public static ObservableCollection<IndirectsModel> MapQuoteDetailToIndirects(this QuoteDetailModel quote)
        { 
            ObservableCollection<IndirectsModel> map = new ObservableCollection<IndirectsModel>();
            var indirectsList = quote.Viaticos.Select(viatico =>
                    new IndirectsModel
                    { 
                        Uuid = Guid.NewGuid(),
                        Concepto = viatico.Concepto,
                        Cantidad = viatico.Cantidad,
                        PrecioUnitario = viatico.PrecioUnitario,
                    }
                ).ToList();
            foreach(var indirect in indirectsList)
                map.Add(indirect);
            return map;
        }
    }
}
