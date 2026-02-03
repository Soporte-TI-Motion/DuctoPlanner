using Calculo_ductos.Params;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Calculo_ductos.Params.Floor;

namespace Calculo_ductos_winUi_3.Models
{
    public class FloorDescription
    {
        public CatalogRowModel TypeDoor { get; set; }
        public Floor.TypeFloor Type { get; set; }
        public Floor.TypeDischarge Discharge { get; set; }
        public string DischargeDescription { get; set; }
        public CatalogRowModel ContainerType { get; set; }
        public Guid Uuid { get; set; }
        public int FloorCount { get; set; }
        public bool NeedGate { get; set; }
        public int DoorTypeId { get; set; }
        public bool NeedChimney { get; set; }
        public bool NeedAntiImpact { get; set; }
        public decimal FloorHeight { get; set; }
        public string Description => GetDescription();
        public string NeedGateString => GetNeedGate();
        public string NeedChimmeyString => GetNeedChimmey();
        public string DischargeString => GetDischargeString();
        public string DoorTypeString => GetDoorTypeString();
        public string NeedAntiImpactString => GetNeedAntiImpact();
        public void SetFloorType(int selection)
        {
            switch (selection)
            {
                case 0: Type = Floor.TypeFloor.discharge; 
                        NeedGate = false;
                        NeedChimney = false; break;
                case 1: Type = Floor.TypeFloor.common; 
                        NeedAntiImpact = false; 
                        NeedChimney = false; break;
                case 2: Type = Floor.TypeFloor.last; 
                        NeedAntiImpact = false; break;
            }
        }
        public void SetFloorTypeDischarge(int selection)
        {
            switch (selection)
            {
                case 0: Discharge = Floor.TypeDischarge.guilloutine;break;
                case 1: Discharge = Floor.TypeDischarge.discharge;break;
            }
        }
        private string GetDescription()
        {
            string description = "";
            switch (Type)
            { 
                case Floor.TypeFloor.discharge: description = "Descarga";break;
                case Floor.TypeFloor.common:description = "Normal";break;
                case Floor.TypeFloor.last:description = "Ventilación";break;

            }
            return description;
        }
        private string GetNeedGate()
        {
            return NeedGate ? "Si" : "No";
        }
        private string GetNeedAntiImpact()
        {
            return NeedAntiImpact ? "Si" : "No";
        }
        private string GetNeedChimmey()
        {
            return Type == Floor.TypeFloor.last ? NeedChimney ? "Chimenea" : "Cuello de ganso" : "-";
        }
        private string GetDischargeString (){
            return Type == Floor.TypeFloor.discharge ? Discharge == Floor.TypeDischarge.guilloutine ? DischargeDescription : "Descargador" : "-";
        }
        private string GetDoorTypeString() {
            return Type != Floor.TypeFloor.discharge ? NeedGate ? TypeDoor.Description: "-" : "-";
        }
        
    }
}
