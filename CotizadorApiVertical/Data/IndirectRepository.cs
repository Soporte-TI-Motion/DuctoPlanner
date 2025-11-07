using CotizadorApiVertical.Interfaces;
using CotizadorApiVertical.Models;
using CotizadorApiVertical.Params;
using Dapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace CotizadorApiVertical.Data
{
    public class IndirectRepository : IIndirectsRepository
    {
        private readonly string _connectionString;
        public IndirectRepository()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        }
        public ResultOperationModel GetIndirect(int cotizacionId)
        {
            ResultOperationModel result = new ResultOperationModel();
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@CotizacionId", cotizacionId);
                    result.Success = true;
                    result.Message = "Se consulto correctamente";
                    result.Data = connection.Query<Viatico>("Obtener_Viaticos_Cotizacion", parameters, commandType: CommandType.StoredProcedure).ToList();

                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = $"Ocurrio un error en GetIndirect: {ex.Message}";

            }
            return result;
        }

        public ResultOperationModel InsertIndirect(SqlConnection connection, SqlTransaction transaction, List<Viatico> viaticos, int cotizacionId)
        {
            ResultOperationModel result = new ResultOperationModel();
            try
            {
                foreach (var viatico in viaticos)
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@Concepto", viatico.Concepto);
                    parameters.Add("@Cantidad", viatico.Cantidad);
                    parameters.Add("@PrecioUnitario", viatico.PrecioUnitario);
                    parameters.Add("@PoliticaViaticosId", viatico.PoliticaViaticosId);
                    parameters.Add("@CotizacionId", cotizacionId);

                    connection.Execute(
                        "Insertar_Viatico_Cotizacion",
                        parameters,
                        transaction: transaction,
                        commandType: CommandType.StoredProcedure
                    );
                }
                result.Success = true;
                result.Message = "Se agregaron correctamente los viaticos";
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = $"Ocurrio un error en InsertIndirect: {ex.Message}";

            }
            return result;
        }
    }
}