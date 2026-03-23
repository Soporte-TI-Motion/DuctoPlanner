using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculo_ductos.Params
{
    public class Floor
    {
        public enum TypeFloor { discharge,common,last}
        public enum TypeDischarge { guilloutine, discharge}
        public decimal Height { get; set; }
        public decimal HeightAvailable { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool NeedGate { get; set; } = true;
        public bool NeedChimney {  get; set; } = false;
        public TypeDischarge Discharge { get; set; } = TypeDischarge.guilloutine;
        public TypeFloor Type { get; set; } = TypeFloor.common;
        //public Dictionary<DuctPiece.TypeDuct, int> Ducts { get; set; } = new Dictionary<DuctPiece.TypeDuct, int>();
        public List<DuctPiece> Ducts { get; set; }

    }
}
