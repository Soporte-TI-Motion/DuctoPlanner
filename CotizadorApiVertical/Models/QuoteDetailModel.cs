using CotizadorApiVertical.Params;
using System;
using System.Collections.Generic;

namespace CotizadorApiVertical.Models
{
    public class QuoteDetailModel
    {
        public string PT { get; set; } = string.Empty;
        public string NombreEjecutivo { get; set; } = "No asignado";
        public int CotizacionId { get; set; } = -1;
        public string Diametro { get; set; } = string.Empty;
        public DateTime Fecha { get; set; } = DateTime.MinValue;
        public int PropositoId { get; set; } = -1;
        public string Proposito { get; set; } = string.Empty;
        public int TipoLaminaId { get; set; } = -1;
        public string TipoLamina { get; set; } = string.Empty;
        public int NumeroVersion { get; set; } = -1;
        public decimal Altura { get; set; } = -1;
        public int Cantidad { get; set; } = 0; 
        public bool NecesitaPuerta { get; set; } = false;
        public int TipoPuertaId { get; set; } = -1;
        public string TipoPuerta { get; set; } = string.Empty;
        public int TipoNivelId { get; set; } = -1;
        public string TipoNivel { get; set; } = string.Empty;
        public int TipoDescargaId { get; set; } = -1;
        public bool NecesitaChimenea { get; set; } = false;
        public bool NecesitaAspersor { get; set; } = false;
        public bool NecesitaSistemaDD { get; set; } = false;
        public bool NecesitaAntiImpacto { get; set; } = false;
        public int LocalidadId { get; set; } = 0;
        public int MunicipioId { get; set; } = 0;
        public int EntidadId { get; set; } = 0;
        public int RentabilidadMOId { get; set; } = 0;
        public bool NecesitaIzaje { get; set; } = false;
        public int ZonaId { get; set; } = 0;
    }
}
