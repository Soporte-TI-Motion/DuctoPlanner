using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculo_ductos.Params
{
    public class DuctPiece
    {
        public enum TypeDuct { A2,B2,B3,B4,B2F,B3F,B4F,C4,S4 }
        public decimal Height { get; set; }
        public string Name { get; set; } = string.Empty;
        public TypeDuct Type { get; set; }
        public int Count { get; set; } = 0;

        
    }
}
