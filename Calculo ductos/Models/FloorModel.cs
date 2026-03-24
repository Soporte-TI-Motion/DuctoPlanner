using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculo_ductos.Models
{
    public class FloorModel
    {
        public List<DuctPieceModel> Ducts { get; set; }
        public List<ComponentModel> Components { get; set; }
    }
}
