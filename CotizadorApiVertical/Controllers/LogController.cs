using CotizadorApiVertical.Facades;
using CotizadorApiVertical.Params;
using CotizadorApiVertical.Services;
using CotizadorVerticalApi.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CotizadorApiVertical.Controllers
{
    public class LogController : ApiController
    {
        private LogService _service;
        public LogController()
        {
            _service = new LogService();
        }
        // GET api/<controller>
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<controller>/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<controller>
        public IHttpActionResult Post([FromBody] LogParam logParam)
        {
            return Ok(_service.WriteLog(logParam));
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }
    }
}