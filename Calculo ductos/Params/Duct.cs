using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculo_ductos.Params
{
    public class Duct
    {
        public enum TypePurpose { Ropa, Basura}
        public TypePurpose Purpose { get; set; }
        public bool NeedChimmey { get; set; } = false;
        public bool NeedSprinkler { get; set; } = false;
        public bool NeedDesinfectionSystem { get; set; } = false;
        public int TipoLamina { get; set; }
        public List<Floor> floors { get; set; }
        public List<Component> Components { get; set; }

    }
}
