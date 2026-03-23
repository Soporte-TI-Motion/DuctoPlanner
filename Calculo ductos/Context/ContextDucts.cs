using System.Text.Json;

namespace Calculo_ductos.Context
{
    using Config;
    using Params;
    using Utils;
    using static Calculo_ductos.Params.Component;
    using static Calculo_ductos.Params.Floor;

    internal class ContextDucts : IDisposable
    {
        private bool isDisposed;
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (isDisposed) return;
            if (disposing)
            {
                // free managed resources
            }

            isDisposed = true;
        }

        ~ContextDucts()
        {
            // Finalizer calls Dispose(false)
            Dispose(false);
        }

        public Duct CalculateDuctsByFloor(string paramsJson)
        {
            //public List<string> CalculateDucts(string paramsJson) {
            Duct duct = new Duct();
            List<Floor> calculatedFloors = new List<Floor>();
            List<string> result = new List<string>();
            Dictionary<DuctPiece.TypeDuct, int> counter = InitDuctsCounter();
            try
            {
                duct = GetDuct(paramsJson);


                foreach (Floor floor in duct.floors)
                {
                    floor.Ducts = PositionDucts(floor, out var heightAvaible);
                    //SumDucts(counter, positions);
                    var a2 = floor.Ducts.FirstOrDefault(duct => duct.Type == DuctPiece.TypeDuct.A2);
                    var s4 = floor.Ducts.FirstOrDefault(duct => duct.Type == DuctPiece.TypeDuct.S4);
                    var c4 = floor.Ducts.FirstOrDefault(duct => duct.Type == DuctPiece.TypeDuct.C4);

                    a2.Count = s4.Count + c4.Count;
                    floor.HeightAvailable = heightAvaible;
                }
                //duct.floors = calculatedFloors;
                CalculateComponents(duct);
            }
            catch (Exception ex)
            {
                return duct;
            }
            //return result;
            return duct;
        }
        public void CalculateComponents(Duct duct)
        {
            List<Component> result = InitComponentList();
            var sumPieces = duct.floors.SumDuctPieces();
            var par = duct.floors.Count % 2 == 0;
            //var division = duct.floors.Count / 2;
            var gates = sumPieces.Where(duct => duct.Type == DuctPiece.TypeDuct.C4).Sum(piece => piece.Count);
            var firstFloor = duct.floors.FirstOrDefault(f => f.Type == TypeFloor.discharge);
            var lastFloor = duct.floors.FirstOrDefault(f => f.Type == TypeFloor.last);

            for (int i = 0; i < result.Count; i++)
            {
                var temp = result[i];
                //CalculateComponent(duct.floors.Count, sumPieces, duct.NeedChimmey,duct.floors.FirstOrDefault(f=>f.Type==TypeFloor.discharge).Discharge, ref temp);
                switch (temp.Type)
                {
                    case TypeComponent.XN: temp.Count = duct.floors.Count - 2; break;
                    case TypeComponent.XNF: temp.Count = 1; break;
                    case TypeComponent.Sprinkler: temp.Count = duct.NeedSprinkler ? (gates / 2) + 1 : 0; break;
                    case TypeComponent.Gate: temp.Count = gates; break;
                    case TypeComponent.Guillotine: temp.Count = firstFloor.Discharge == TypeDischarge.guilloutine ? 1 : 0; break;
                    case TypeComponent.Discharge: temp.Count = firstFloor.Discharge == TypeDischarge.discharge ? 1 : 0; break;
                    case TypeComponent.TVA: temp.Count = lastFloor.NeedChimney ? 0 : 1; break;
                    case TypeComponent.DisinfectionSystem: temp.Count = duct.NeedDesinfectionSystem ? 1 : 0; break;
                    case TypeComponent.Chimney: temp.Count = lastFloor.NeedChimney ? 1 : 0; break;
                }
            }
            duct.Components = result;
        }
        private Duct GetDuct(string paramsJson)
        {
            try
            {
                return JsonSerializer.Deserialize<Duct>(paramsJson);
            }
            catch (Exception ex)
            {
                return new Duct();
            }
        }

        private List<DuctPiece> PositionDucts(Floor floor, out decimal heightAvaible)
        {
            //bool canPosition = true;
            heightAvaible = floor.Height;

            List<DuctPiece> results = Ducts.GetAllDucts();

                switch (floor.Type)
                {
                    case Floor.TypeFloor.discharge: 
                        CalculateDischargeLevel(ref results,ref heightAvaible, floor.Discharge); break;
                    case Floor.TypeFloor.common: { 
                        if (heightAvaible >= 4.5m) 
                            CalculateDoubleLevel(ref results, ref heightAvaible, floor.NeedGate);
                        else
                            CalculateCommonLevel(ref results, ref heightAvaible, floor.NeedGate);
                        };break;
                    case Floor.TypeFloor.last:
                    {
                        if (floor.NeedChimney)
                        {
                            heightAvaible += 0.8m;
                            if (heightAvaible > 4.8m)
                                CalculateDoubleLevel(ref results, ref heightAvaible, floor.NeedGate);
                            else
                                CalculateCommonLevel(ref results, ref heightAvaible, floor.NeedGate);
                        }
                        else
                        CalculateLastLevel(ref results, ref heightAvaible, floor.NeedGate); break;
                    }
                }

           
            
            return results;
        }

        private List<Component> InitComponentList()
        {
            List<Component> components = Components.GetAllComponents();

            return components;
        }

        private Dictionary<DuctPiece.TypeDuct, int> InitDuctsCounter()
        {
            Dictionary<DuctPiece.TypeDuct, int> counter = new Dictionary<DuctPiece.TypeDuct, int>();

            foreach (DuctPiece duct in Ducts.GetAllDucts())
            {
                counter.Add(duct.Type, 0);
            }

            return counter;
        }
  
        private void CalculateDischargeLevel(ref List<DuctPiece> results, ref decimal heightAvaible, TypeDischarge typeDischarge)
        {

            //Se resta la altura para el bote
            heightAvaible -= typeDischarge == TypeDischarge.guilloutine ? 1.5m : 2.3m;

            foreach (DuctPiece duct in Ducts.GetConfigDischargeLevel())
            {
                var ductPiece = results.FirstOrDefault(p => p.Type == duct.Type);
                int total = (int)(heightAvaible / duct.Height);
                ductPiece.Count += total;
                heightAvaible -= (total * duct.Height);
                //switch (duct.Type)
                //{
                //    case DuctPiece.TypeDuct.B4F:
                //        {
                            
                //            results[DuctPiece.TypeDuct.B4F] += total;
                            
                //        }; break;
                //    case DuctPiece.TypeDuct.B3F:
                //        {
                //            int total = (int)(heightAvaible / duct.Height);
                //            results[DuctPiece.TypeDuct.B3F] += total;
                //            heightAvaible -= (total * duct.Height);
                //        }; break;
                //    case DuctPiece.TypeDuct.B2F:
                //        {
                //            int total = (int)(heightAvaible / duct.Height);
                //            results[DuctPiece.TypeDuct.B2F] += total;
                //            heightAvaible -= (total * duct.Height);
                //        }; break;
                //}
            }
        }

        private void CalculateLastLevel(ref List<DuctPiece> results, ref decimal heightAvaible, bool needGate)
        {

            foreach (DuctPiece duct in Ducts.GetConfigLastLevel())
            {    
                var ductPiece = results.FirstOrDefault(piece => piece.Type == duct.Type);

                switch (duct.Type)
                {
                    case DuctPiece.TypeDuct.C4:
                        {
                            heightAvaible -= duct.Height;
                            if (needGate)
                            {
                                ductPiece.Count++;
                            }
                            else
                            {
                                var ductPieceS4 = results.FirstOrDefault(piece => piece.Type == DuctPiece.TypeDuct.S4);
                                ductPieceS4.Count++;
                            }
                        }; break;
                    case DuctPiece.TypeDuct.B3:
                    case DuctPiece.TypeDuct.B2:
                        {
                            ductPiece.Count++;
                            heightAvaible -= duct.Height;
                        }; break;
                } 
            }
        }

        private void CalculateDoubleLevel(ref List<DuctPiece> results, ref decimal heightAvaible, bool needGate)
        {
            foreach (DuctPiece duct in Ducts.GetConfigDoubleLevel())
            {
                var ductPiece = results.FirstOrDefault(piece => piece.Type == duct.Type);
                switch (duct.Type)
                {
                    case DuctPiece.TypeDuct.C4:
                        {
                            heightAvaible -= duct.Height;
                            if (needGate)
                            {
                                ductPiece.Count++;
                            }
                            else
                            {
                                var ductPieceS4 = results.FirstOrDefault(piece => piece.Type == DuctPiece.TypeDuct.S4);
                                ductPieceS4.Count++;
                            }
                        }; break;
                    case DuctPiece.TypeDuct.B4F:
                    case DuctPiece.TypeDuct.B3F:
                    case DuctPiece.TypeDuct.B2F:
                        {
                            int total = (int)(heightAvaible / duct.Height);
                            ductPiece.Count += total;
                            heightAvaible -= (total * duct.Height);
                        }; break;
                }
            }
        }

        private void CalculateCommonLevel(ref List<DuctPiece> results, ref decimal availableHeight, bool needGate)
        {
            if (availableHeight <= 0) return; // Validación de altura disponible

            foreach (DuctPiece duct in Ducts.GetConfigCommonLevel())
            {
                var ductPiece = results.FirstOrDefault(p => p.Type == duct.Type);
                switch (duct.Type)
                {
                    case DuctPiece.TypeDuct.C4:
                        if (needGate)
                        {
                            CalculateDucts(ref ductPiece, ref availableHeight, duct, DuctPiece.TypeDuct.C4);
                        }
                        else
                        {
                            var ductPieceS4 = results.FirstOrDefault(piece => piece.Type == DuctPiece.TypeDuct.S4);
                            CalculateDucts(ref ductPieceS4, ref availableHeight, new DuctPiece { Height = 1.20m, Type = DuctPiece.TypeDuct.S4 }, DuctPiece.TypeDuct.S4);
                        }
                        break;
                    case DuctPiece.TypeDuct.B4:
                    case DuctPiece.TypeDuct.B3:
                    case DuctPiece.TypeDuct.B2:
                        CalculateDucts(ref ductPiece, ref availableHeight, duct, duct.Type);
                        break;
                }
            }
        }

        private void CalculateDucts(ref DuctPiece piece, ref decimal availableHeight, DuctPiece duct, DuctPiece.TypeDuct type)
        {
            int total = type == DuctPiece.TypeDuct.C4 || type == DuctPiece.TypeDuct.S4 ? 1 : (int)(availableHeight / duct.Height);
            piece.Count += total;
            availableHeight -= (total * duct.Height);
        }
        
    }
}
