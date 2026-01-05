using CotizadorApiVertical.Models;
using Dapper;
using System.Data.SqlClient;
using System.Data;
using CotizadorApiVertical.Interfaces;
using CotizadorApiVertical.Params;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;
using System.Web.UI.WebControls;

namespace CotizadorVerticalApi.Data
{
    public class QuoteRepository : IQuoteRepository
    {
        private readonly string _connectionString;

        public QuoteRepository()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        }
        
        public QuoteInsertionResultModel InsertQuote(SqlConnection connection, SqlTransaction transaction, QuoteParam quote)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@PT", quote.PT);
            parameters.Add("@NombreEjecutivo", quote.NombreEjecutivo);
            parameters.Add("@Diametro", quote.Diametro);
            parameters.Add("@PropositoId", quote.PropositoId);
            parameters.Add("@TipoLaminaId", quote.TipoLaminaId);
            parameters.Add("@SiteRef", quote.SiteRef);
            parameters.Add("@NecesitaAspersor", quote.NecesitaAspersor);
            parameters.Add("@NecesitaSistemaDD", quote.NecesitaSistemaDD);
            parameters.Add("@RentabilidadMOId", quote.RentabilidadMOId);
            parameters.Add("@NecesitaIzaje", quote.NecesitaIzaje);
            parameters.Add("@ZonaId", quote.ZonaId);

            var quoteResult = connection.Query<QuoteInsertionResultModel>(
                "Insertar_Cotizacion",
                parameters,
                transaction: transaction,
                commandType: CommandType.StoredProcedure
            ).FirstOrDefault();

            return quoteResult;
        }

        public void InsertLevels(SqlConnection connection, SqlTransaction transaction, QuoteParam quote)
        {
            foreach (var level in quote.Niveles)
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Altura", level.Altura);
                parameters.Add("@Cantidad", level.Cantidad);
                parameters.Add("@NecesitaPuerta", level.NecesitaPuerta);
                parameters.Add("@CotizacionId", quote.CotizacionId);
                parameters.Add("@TipoNivelId", level.TipoNivelId);
                parameters.Add("@NecesitaChimenea", level.NecesitaChimenea);
                parameters.Add("@TipoPuertaId", level.TipoPuertaId);
                parameters.Add("@TipoDescargaId", level.TipoDescargaId);
                parameters.Add("@NecesitaAntiImpacto", level.NecesitaAntiImpactos);

                connection.Execute(
                    "Insertar_Nivel",
                    parameters,
                    transaction: transaction,
                    commandType: CommandType.StoredProcedure
                );
            }
        }

        public IEnumerable<QuoteModel> GetLastQuotes()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                
                return connection.Query<QuoteModel>("Obtener_Ultimas_Cotizaciones", commandType: CommandType.StoredProcedure);

            };
            
        }

        public IEnumerable<QuoteDetailModel> GetQuoteById(int id)
        {
            using (var connection = new SqlConnection(_connectionString)) 
            {
                var parameters = new DynamicParameters();
                parameters.Add("@CotizacionId", id);

                return connection.Query<QuoteDetailModel>("Obtener_Cotizacion", parameters, commandType: CommandType.StoredProcedure).ToList();

            } ;
            
        }

    }
}
