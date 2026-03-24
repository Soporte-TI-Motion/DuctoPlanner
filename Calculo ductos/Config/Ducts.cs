using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculo_ductos.Config
{
    using Params;
    using static Calculo_ductos.Params.DuctPiece;

    static class Ducts
    {
        public static List<DuctPiece> GetConfigCommonLevel()
        {
            return new List<DuctPiece> {
                // El tipo C4 siempre irá primero y solo será uno puesto que tiene la adaptacion para la compuerta
                new DuctPiece { Height = 1.20m, Name = "ducto C4", Type = TypeDuct.C4},
                new DuctPiece { Height = 1.20m, Name = "ducto B4", Type = TypeDuct.B4},
                new DuctPiece { Height = 0.90m, Name = "ducto B3", Type = TypeDuct.B3},
                new DuctPiece { Height = 0.60m, Name = "ducto B2", Type = TypeDuct.B2}

            };
        }

        public static List<DuctPiece> GetConfigLastLevel() {
            return new List<DuctPiece>
            {
                new DuctPiece { Height = 1.20m, Name = "ducto C4", Type = TypeDuct.C4},
                new DuctPiece { Height = 0.90m, Name = "ducto B3", Type = TypeDuct.B3},
                new DuctPiece { Height = 0.60m, Name = "ducto B2", Type = TypeDuct.B2}
            };
        }
        public static List<DuctPiece> GetConfigDoubleLevel()
        {
            return new List<DuctPiece>
            {
                new DuctPiece { Height = 1.20m, Name = "ducto C4", Type = TypeDuct.C4},
                new DuctPiece { Height = 1.20m, Name = "ducto B4-F", Type = TypeDuct.B4F },
                new DuctPiece { Height = 0.90m, Name = "ducto B3-F", Type = TypeDuct.B3F },
                new DuctPiece { Height = 0.60m, Name = "ducto B2-F", Type = TypeDuct.B2F }
            };
        }
        public static List<DuctPiece> GetConfigDischargeLevel()
        {
            return new List<DuctPiece>
            {
                new DuctPiece { Height = 1.20m, Name = "ducto B4-F", Type = TypeDuct.B4F },
                new DuctPiece { Height = 0.90m, Name = "ducto B3-F", Type = TypeDuct.B3F },
                new DuctPiece { Height = 0.60m, Name = "ducto B2-F", Type = TypeDuct.B2F }
            };
        }

        public static List<DuctPiece> GetAllDucts()
        {
            return new List<DuctPiece> {
                // El tipo C4 siempre irá primero y solo será uno puesto que tiene la adaptacion para la compuerta
                new DuctPiece { Height = 1.20m, Name = "ducto S4", Type = TypeDuct.S4},
                new DuctPiece { Height = 1.20m, Name = "ducto C4", Type = TypeDuct.C4},
                new DuctPiece { Height = 1.20m, Name = "ducto B4", Type = TypeDuct.B4},
                new DuctPiece { Height = 1.20m, Name = "ducto B4-F", Type = TypeDuct.B4F },
                new DuctPiece { Height = 0.90m, Name = "ducto B3", Type = TypeDuct.B3},
                new DuctPiece { Height = 0.90m, Name = "ducto B3-F", Type = TypeDuct.B3F },
                new DuctPiece { Height = 0.60m, Name = "ducto B2", Type = TypeDuct.B2},
                new DuctPiece { Height = 0.60m, Name = "ducto B2-F", Type = TypeDuct.B2F },
                new DuctPiece { Height = 0.60m, Name = "ducto A2", Type = TypeDuct.A2}

            };
        }
    }
}
