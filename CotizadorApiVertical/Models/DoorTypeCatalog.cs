namespace CotizadorApiVertical.Models
{
    public class PurposeTypeCatalog
    {
        public int PropositoId { get; set; }
        public string Descripcion {get;set;}
    }
    public class DoorTypeCatalog
    {
        public int TipoPuertaId { get; set; }
        public string Descripcion { get; set; }
        public string Clase { get; set; }
        public string ItemIdSyteLine { get; set; }
    }
    public class FloorTypeCatalog
    {
        public int TipoNivelId { get; set; }
        public string Descripcion { get; set; }
    }
    public class SheetTypeCatalog
    {
        public int TipoLaminaId { get; set; }
        public string Descripcion { get; set; }
    }
    public class TruckTypeCatalog
    {
        public int TipoCamionId { get; set; }
        public string Descripcion { get; set; }
        public string Clase { get; set; }
        public int CapacidadMinima { get; set; }
        public int CapacidadMaxima { get; set; }
        public decimal CostoManiobra { get; set; } 
    }
    public class EntityCatalog 
    {
        public int EntidadId { get; set; }
        public string Nombre { get; set; }
    }
    public class MunicipalityCatalog
    {
        public int MunicipioId { get; set; }
        public int EntidadId { get; set; }
        public string Nombre { get; set; }
    }
    public class LocalityCatalog
    {
        public int LocalidadId { get; set; }
        public int MunicipioId { get; set; }
        public string Nombre { get; set; }
    }
    public class ResourceCatalog 
    {
        public int RecursoId { get; set; }
        public string Descripcion { get; set; }
        public decimal SalarioPorJornada { get; set; }
    }
    public class ResourceTypeCatalog
    {
        public int TipoRecursoId { get; set; }
        public string Descripcion { get;set; }
    }
    public class RentabilitiesCatalog
    {
        public int RentabilidadMOId { get; set; }
        public decimal Rentabilidad { get; set; }
        public string Descripcion { get; set; }
    }
    public class IndirectsCatalog
    {
        public int PoliticaViaticosId { get; set; }
        public string Concepto { get; set; }
        public int ZonaId { get; set; }
        public string Zona { get; set; }
        public int UnidadMedidaId { get; set; }
        public string Medida { get; set; }
        public decimal Monto { get; set; }
        public bool EsObligatorio { get; set; }
        public bool EsObligatorioOpcional { get; set; }
        public int UbicacionId { get; set; }
        public string Ubicacion { get; set; }
        public int RedondeoId { get; set; }
        public string Redondeo { get; set; }
        public string Base { get; set; }
        public int Multiplicador { get; set; }
        public int Divisor { get; set; }
    }
    public class ZoneCatalog
    {
        public int ZonaId { get; set; }
        public string Descripcion { get; set; }
    }
    public class KitCatalog
    { 
        public int KitId { get; set; }
        public string Descripcion { get;set;}
        public string Item { get; set; }
        public int TipoKitId { get; set; }
        public string TipoKit { get; set; }
        public int PropositoId { get; set; }
        public string Proposito { get; set; }
        public decimal Precio { get; set; }
        public string Moneda { get; set; }
        public decimal TipoCambio { get; set; }

    }
    public class ToolCatalog
    {
        public int HerramientaId { get; set; }
        public string Descripcion { get; set; }
        public decimal PrecioUnitario { get; set; }
        public bool EsObligatorio { get; set; }
        public int Grupo { get; set; }
        public string Periodicidad { get; set; }
    }

}
