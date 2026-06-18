using CotizadorApiVertical.Params;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace CotizadorApiVertical.Services
{
    public class LogService
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public async Task<Response> WriteLog(LogParam logParam)
        {
            Response response = new Response();

            response.StatusCode = 200;
            response.Message = "Se guardo log con éxito";
            log.Debug("========== Notificación de log ==========");
            log.Debug($"host: {logParam.host} - Mensaje: {logParam.message}");
            log.Debug($"StateApp: {JsonConvert.SerializeObject(logParam.stateApp)}");
            if(!string.IsNullOrEmpty(logParam.trace)) log.Debug($"Error : Ocurrio un error {logParam.trace}");
                        
            return response;

        }
    }
}