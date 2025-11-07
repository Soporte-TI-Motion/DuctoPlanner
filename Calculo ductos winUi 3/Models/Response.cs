using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculo_ductos_winUi_3.Models
{
    public class Response<T>
    {
        public T Result { get; set; }
    }

    public class ResultData<T>
    {
        public string Message { get; set; }
        public T Data { get; set; }
        public int StatusCode { get; set; }
    }
    public class CatalogModelList
    {
        public List<CatalogRowModel> PurposeCatalog { get; set; }
        public List<CatalogRowModel> DoorTypeCatalog { get; set; }
        public List<CatalogRowModel> SheetTypeCatalog { get; set; }
        public List<CatalogRowModel> FloorTypeCatalog { get; set; }
        public List<CatalogRowTruckTypeModel> TruckTypeCatalog { get; set; }
        public List<CatalogRowEntityModel> Entities { get; set; }
        public List<CatalogRowEntityModel> Municipalities { get; set; }
        public List<CatalogRowEntityModel> Localities { get; set; }
        public List<CatalogResourceModel> Resources{ get; set; }
        public List<CatalogResourceTypeModel> ResourceTypes { get; set; }
        public List<CatalogRentabilityModel> Rentabilities { get; set; }
        public List<CatalogIndirectModel> Indirects { get; set; }
        public List<CatalogZoneModel> Zones { get; set; }
        public List<CatalogKitModel> Kits { get; set; }
        public List<CatalogToolModel> Tools { get; set; }
    }
    public class CatalogModel
    {
        public string Name { get; set; }
        public List<CatalogRowModel> Data { get; set; }
    }

    public class CatalogRowModel
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public string Class { get; set; }
        public string IdSyteLine { get; set; }  
    }
    public class CatalogRowEntityModel
    {
        public int Id { get; set; } = 0;
        public int ParentId { get; set; } = -1;
        public string Name { get; set; } = string.Empty;
    }
    public class CatalogRowFregihtModel
    {
        public int Id { get; set; } = 0;
        public int LocalityId { get; set; } = 0;
        public int TruckTypeId { get; set; } = 0;
        public decimal Price { get; set; } = decimal.Zero;
    }
    public class CatalogRowTruckTypeModel
    {
        public int Id { get; set; } = 0;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int MinCapacity { get; set; } = 0;
        public int MaxCapacity { get; set; } = 0;
        public decimal HandlingCost { get; set; } = 0;

    }
    public class CatalogResourceModel
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public decimal SalaryPerWorkday { get; set; }
    }
    public class CatalogResourceTypeModel
    {
        public int Id { get; set; }
        public string Description { get; set; }
    }
    public class CatalogRentabilityModel
    {
        public int Id { get; set; }
        public decimal Rentability { get; set; }
        public string Description { get; set; }
    }
    public class CatalogIndirectModel
    {
        public int Id { get; set; }
        public string Concept { get; set; }
        public int ZoneId { get; set; }
        public string Zone { get; set; }
        public int UnitId { get; set; }
        public string Unit { get; set; }
        public decimal Cost { get; set; }
        public bool IsMandatory { get; set; }
    }
    public class CatalogZoneModel
    {
        public int Id { get; set; }
        public string Description { get; set; }
    }
    public class CatalogKitModel
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public string Item { get; set; }
        public int TypeKitId { get; set; }
        public string TypeKit { get; set; }
        public int PurposeId { get; set; }
        public string Purpose { get; set; }
        public decimal Price { get; set; }
        public string Currency { get; set; }
        public decimal ExchangeRate { get; set; }
    }
    public class CatalogToolModel
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public bool IsMandatory { get; set; }
        public int Group { get; set; }
        public string Preiodicity { get; set; }
    }

}
