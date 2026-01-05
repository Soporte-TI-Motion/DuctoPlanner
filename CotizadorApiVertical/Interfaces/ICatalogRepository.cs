using CotizadorApiVertical.Models;
using System.Collections.Generic;

namespace CotizadorApiVertical.Interfaces
{
    public interface ICatalogRepository
    {
        IEnumerable<PurposeTypeCatalog> GetPurposeCatalog();
        IEnumerable<SheetTypeCatalog> GetSheetTypeCatalog();
        IEnumerable<FloorTypeCatalog> GetFloorTypeCatalog();
        IEnumerable<DoorTypeCatalog> GetDoorTypeCatalog();
        IEnumerable<TruckTypeCatalog> GetTruckTypeCatalog();
        IEnumerable<EntityCatalog> GetEntityCatalog();
        IEnumerable<MunicipalityCatalog> GetMunicipalityCatalog();
        IEnumerable<LocalityCatalog> GetLocalityCatalog();
        IEnumerable<ResourceCatalog> GetResourceCatalog();
        IEnumerable<ResourceTypeCatalog> GetResourceTypeCatalog();
        IEnumerable<RentabilitiesCatalog> GetRentabilitiesCatalog();
        IEnumerable<IndirectsCatalog> GetIndirectsCatalog();
        IEnumerable<ZoneCatalog> GetZonesCatalog();
        IEnumerable<KitCatalog> GetKitsCatalog();
        IEnumerable<ToolCatalog> GetToolCatalog();
    }
}
