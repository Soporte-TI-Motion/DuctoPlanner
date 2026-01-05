using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CotizadorApiVertical.Params
{
    public class QuoteParam
    {
        public int CotizacionId { get; set; } = 0;
        public string PT { get; set; } = string.Empty;
        public string NombreEjecutivo { get; set; } = "NO ASIGNADO";
        public string Diametro { get; set; } = "24\"";
        public int PropositoId { get; set; } = 0;
        public int TipoLaminaId { get; set; } = 0;
        public int TipoPuertaId { get; set; } = 0;
        public string SiteRef { get; set; } = "VERS";
        public bool NecesitaAspersor { get; set; } = false;
        public bool NecesitaSistemaDD { get; set; } = false;
        public int LocalidadId { get; set; } = 0;
        public int RentabilidadMOId { get; set; } = 0;
        public bool NecesitaIzaje { get; set; } = true;
        public int ZonaId { get; set; } = 1;
        public List<LevelParam> Niveles { get; set; }
        public List<HumanResource> ManoDeObra { get; set; }
        public List<Viatico> Viaticos { get; set; }
    }
    public class LevelParam 
    {
        public int TipoNivelId { get; set; } = 0;
        public int Cantidad { get; set; } = 0;
        public decimal Altura { get; set; } = 0;
        public bool NecesitaPuerta { get; set; } = false;
        public bool NecesitaChimenea { get; set; }= false;
        public bool NecesitaAntiImpactos { get; set; } = false;
        public int TipoPuertaId { get; set; } = 0;
        public int TipoDescargaId { get; set; } = 0;

    }
    public class HumanResource 
    {
        public int RecursoId { get; set; } = 0;
        public int TipoRecursoId { get; set; } = 0;

    }
    public class Viatico 
    {
        public string Concepto { get; set; } = string.Empty;
        public int Cantidad { get; set; } = 0;
        public decimal PrecioUnitario { get; set; } = 0;
        public int PoliticaViaticosId { get; set; } = 0;

    }
}