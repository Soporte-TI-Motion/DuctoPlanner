
using System;
using System.Collections.Generic;

namespace Calculo_ductos_winUi_3.Models
{
    public class QuoteModel
    {
        public int Id { get; set; } = 0;
        public DateTime Date { get; set; } = DateTime.MinValue;
        public string PT { get; set; } = string.Empty;
        public string ExecutiveName { get; set; } = string.Empty;
    }
    public class QuoteInsertionResultModel
    {
        public int Id { get; set; } = 0;
        public int Version { get; set; } = 0;
    }
    public class QuoteDetailModel
    {
        public int CotizacionId { get; set; } = -1;
        public string PT { get; set; } = string.Empty;
        public string NombreEjecutivo { get; set; } = "No asignado" ;
        public int NumeroVersion { get; set; } = 0;
        public string Diametro { get; set; } = string.Empty;
        public int TipoLaminaId { get; set; } = 0;
        public int PropositoId { get; set; } = 0;
        public string SiteRef { get; set; } = string.Empty;
        public bool NecesitaAspersor { get; set; } = false;
        public bool NecesitaSistemaDD { get; set; } = false;
        public int EntidadId { get; set; } = 0;
        public int MunicipioId { get; set; } = 0;
        public int LocalidadId { get; set; } = 0;
        public int RentabilidadMOId { get; set; } = 1;
        public bool NecesitaIzaje { get; set; } = true;
        public int ZonaId { get; set; } = 1;
        public List<FloorDetailModel> Niveles { get; set; }
        public List<HumanResource> ManoDeObra { get; set; }
        public List<Viatico> Viaticos { get; set; }

    }
    public class FloorDetailModel
    {
        public int TipoNivelId { get; set; } = 0;
        public int Cantidad {get;set;} = 0;
        public decimal Altura {get;set;} = 0;
        public bool NecesitaPuerta {get;set;} = false;
        public bool NecesitaChimenea {get;set;} = false;
        public bool NecesitaAntiImpactos { get; set; } = false;
        public int TipoPuertaId {get;set;} = 0;
        public int TipoDescargaId { get; set; } = 0;
    }
    public class HumanResource
    { 
        public int RecursoId { get; set; }
        public int TipoRecursoId { get; set; }
    }
    public class Viatico
    {
        public string Concepto { get; set; } = string.Empty;
        public int Cantidad { get; set; } = 0;
        public decimal PrecioUnitario { get; set; } = 0;
        public int PoliticaViaticosId {get;set;} = 0;

    }
    public class LogModel
    {
        public string message { get; set; }
        public string trace { get; set; }
        public QuoteDetailModel stateApp { get; set; }
        public string host { get; set; }
    }

}
