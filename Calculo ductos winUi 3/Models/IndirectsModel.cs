using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculo_ductos_winUi_3.Models
{
    public class IndirectsModel
    {
        public Guid Uuid { get; set; } = Guid.NewGuid();
        public string Concepto { get; set; }
        public int Cantidad { get; set; } = 0;
        public decimal PrecioUnitario { get; set; } = 0;
        public int PoliticaViaticosId { get; set; } = 0;
        public decimal CostoToal { get { return Cantidad * PrecioUnitario; } }
    }
    public class Opcion
    {
        public int Id { get; set; }
        public string Description { get; set; }

    }
   
}
