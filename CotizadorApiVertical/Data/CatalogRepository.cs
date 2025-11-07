using CotizadorApiVertical.Interfaces;
using CotizadorApiVertical.Models;
using System.Data;
using System.Data.SqlClient;
using Dapper;
using System.Collections.Generic;
using System.Configuration;

namespace CotizadorApiVertical.Data
{
    public class CatalogRepository : ICatalogRepository
    {
        private readonly string _connectionString;
        public CatalogRepository()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString; 
        }

        public IEnumerable<DoorTypeCatalog> GetDoorTypeCatalog()
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                return connection.Query<DoorTypeCatalog>("Obtener_Catalogo_Puertas", commandType: CommandType.StoredProcedure);
            }
        }

        public IEnumerable<FloorTypeCatalog> GetFloorTypeCatalog()
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                return connection.Query<FloorTypeCatalog>("Obtener_Catalogo_TipoNivel", commandType: CommandType.StoredProcedure);
            }
        }

        public IEnumerable<PurposeTypeCatalog> GetPurposeCatalog()
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {

                return connection.Query<PurposeTypeCatalog>("Obtener_Catalogo_Proposito", commandType: CommandType.StoredProcedure);
            }
        }

        public IEnumerable<SheetTypeCatalog> GetSheetTypeCatalog()
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                return connection.Query<SheetTypeCatalog>("Obtener_Catalogo_Lamina", commandType: CommandType.StoredProcedure);
            }
        }
        public IEnumerable<TruckTypeCatalog> GetTruckTypeCatalog()
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                return connection.Query<TruckTypeCatalog>("Obtener_Catalogo_Camiones", commandType: CommandType.StoredProcedure);
            }
        }
        public IEnumerable<EntityCatalog> GetEntityCatalog()
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                return connection.Query<EntityCatalog>("Obtener_Catalogo_Entidades", commandType: CommandType.StoredProcedure);
            }
        }
        public IEnumerable<MunicipalityCatalog> GetMunicipalityCatalog()
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                return connection.Query<MunicipalityCatalog>("Obtener_Catalogo_Municipios", commandType: CommandType.StoredProcedure);
            }
        }
        public IEnumerable<LocalityCatalog> GetLocalityCatalog()
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                return connection.Query<LocalityCatalog>("Obtener_Catalogo_Localidades", commandType: CommandType.StoredProcedure);
            }
        }
        public IEnumerable<ResourceCatalog> GetResourceCatalog() 
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                return connection.Query<ResourceCatalog>("Obtener_Catalogo_Recursos",commandType: CommandType.StoredProcedure);
            }
        }
        public IEnumerable<ResourceTypeCatalog> GetResourceTypeCatalog()
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                return connection.Query<ResourceTypeCatalog>("Obtener_Catalogo_Tipo_Recursos", commandType: CommandType.StoredProcedure);
            }
        }
        public IEnumerable<RentabilitiesCatalog> GetRentabilitiesCatalog()
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                return connection.Query<RentabilitiesCatalog>("Obtener_Catalogo_Rentabilidades", commandType: CommandType.StoredProcedure);
            }
        }
        public IEnumerable<IndirectsCatalog> GetIndirectsCatalog()
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                return connection.Query<IndirectsCatalog>("Obtener_Catalogo_Viaticos", commandType: CommandType.StoredProcedure);
            }
        }
        public IEnumerable<ZoneCatalog> GetZonesCatalog()
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                return connection.Query<ZoneCatalog>("Obtener_Catalogo_Zonas", commandType: CommandType.StoredProcedure);
            }
        }
        public IEnumerable<KitCatalog> GetKitsCatalog()
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                return connection.Query<KitCatalog>("Obtener_Catalogo_Kits", commandType: CommandType.StoredProcedure);
            }
        }
        public IEnumerable<ToolCatalog> GetToolCatalog()
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                return connection.Query<ToolCatalog>("Obtener_Catalogo_Herramientas", commandType: CommandType.StoredProcedure);
            }
        }
    }
}
