using System;

namespace CotizadorApiVertical.Models
{
    public class QuoteModel
    {
        public int CotizacionId { get; set; } = 0;
        public DateTime Fecha { get; set; } = DateTime.MinValue;
        public string PT { get; set; } = string.Empty;
        public string NombreEjecutivo { get; set; } = string.Empty;
    }
}
