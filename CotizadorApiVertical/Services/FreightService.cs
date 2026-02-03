using CotizadorApiVertical.Data;
using CotizadorApiVertical.Facades;
using CotizadorApiVertical.Interfaces;
using CotizadorApiVertical.Params;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace CotizadorApiVertical.Services
{
    public class FreightService : IFreightFacade
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly IFreightRepository _freightRepository;
        public FreightService()
        {
            _freightRepository = new FreightRepository();
        }
        public async Task<Response> GetFreight(int localidadId, int tipoCamionId)
        {

            Response response = new Response();
            try
            {
                var detail = _freightRepository.GetFreight(localidadId, tipoCamionId);
                if (detail == null)
                {
                    response.StatusCode = 404;
                    response.Message = "No se encontró flete";
                    log.Debug($"No se encontró flete Localidad:{localidadId}, TipoCamion:{tipoCamionId}");
                    return response;
                }
                var Freight = new
                {
                    FreightId = detail.FleteId,
                    TruckTypeId = detail.TipoCamionId,
                    TruckDescription = detail.DescripcionCamion,
                    TruckName = detail.ClaseCamion,
                    LocalityId = detail.LocalidadId,
                    LocalityName = detail.NombreLocalidad,
                    EntityName = detail.NombreEntidad,
                    Price = detail.Precio
                };
                response.Data = Freight;
                response.StatusCode = 200;
                response.Message = "Éxito";
            }
            catch (Exception ex)
            {
                response.StatusCode = 500;
                response.Message = $"No se pudo obtener el flete";
                log.Error(
                            $"Error al obtener flete. Localidad:{localidadId}, TipoCamion:{tipoCamionId}",
                            ex
                        );
            }
            return response;
        }
    }
}