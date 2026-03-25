using System.Text.Json;

namespace Calculo_ductos.Context
{
    using Config;
    using Params;
    using System.Drawing;
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
            Duct duct = new Duct();
            
            try
            {
                duct = GetDuct(paramsJson);
                foreach (Floor floor in duct.floors)
                {
                    floor.Ducts = PositionDucts(floor, out var heightAvaible);
                    
                    var a2 = floor.Ducts.FirstOrDefault(duct => duct.Type == DuctPiece.TypeDuct.A2);
                    var s4 = floor.Ducts.FirstOrDefault(duct => duct.Type == DuctPiece.TypeDuct.S4);
                    var c4 = floor.Ducts.FirstOrDefault(duct => duct.Type == DuctPiece.TypeDuct.C4);

                    a2.Count = s4.Count + c4.Count;
                    if (floor.Type.Equals(Floor.TypeFloor.last))
                    {
                        a2.Count = 0;
                    }
                    if (floor.Type.Equals(Floor.TypeFloor.discharge))
                    {
                        a2.Count = 1;
                    }
                    floor.HeightAvailable = heightAvaible;

                }
                
                CalculateComponents(duct);
            }
            catch (Exception ex)
            {
                return duct;
            }
            return duct;
        }
        public void CalculateComponents(Duct duct)
        {
            List<Component> result = InitComponentList();
            var sumPieces = duct.floors.SumDuctPieces();
            //var par = duct.floors.Count % 2 == 0;

            var gates = sumPieces.Where(duct => duct.Type == DuctPiece.TypeDuct.C4).Sum(piece => piece.Count);
            var firstFloor = duct.floors.FirstOrDefault(f => f.Type == TypeFloor.discharge);
            var lastFloor = duct.floors.FirstOrDefault(f => f.Type == TypeFloor.last);

            for (int i = 0; i < result.Count; i++)
            {
                var temp = result[i];
                
                switch (temp.Type)
                {
                    // Se modifica regla para tener un soporte normal mas
                    case TypeComponent.XN: temp.Count = duct.floors.Count - 1; break;
                    case TypeComponent.XNF: temp.Count = 1; break;
                    case TypeComponent.Sprinkler: temp.Count = duct.NeedSprinkler ? (gates / 2) + 1 : 0; break;
                    case TypeComponent.Gate: temp.Count = gates; break;
                    case TypeComponent.Guillotine: temp.Count = firstFloor.Discharge == TypeDischarge.guilloutine ? 1 : 0; break;
                    case TypeComponent.Discharge: temp.Count = firstFloor.Discharge == TypeDischarge.discharge ? 1 : 0; break;
                    case TypeComponent.TVA: temp.Count = lastFloor.NeedChimney ? 0 : 1; break;
                    case TypeComponent.DisinfectionSystem: temp.Count = duct.NeedDesinfectionSystem ? 1 : 0; break;
                    case TypeComponent.Chimney: temp.Count = lastFloor.NeedChimney ? 1 : 0; break;
                    case TypeComponent.Container: temp.Count = 2;break;
                    case TypeComponent. AntiImpact: temp.Count = (firstFloor.NeedAntiImpact ? 1 : 0);break;
                }
            }

            var soportesFinales = result.Where((Component r) => r.Type == Component.TypeComponent.XNF).FirstOrDefault();
            var soportesNormales = result.Where((Component r) => r.Type == Component.TypeComponent.XN).FirstOrDefault();
            Floor pisoAnterior = new Floor();

            int normalesPorRestar = 0;
            int finalesPorSumar = 0;
            foreach (Floor pisoActual in duct.floors)
            {
                if (pisoActual.Type != Floor.TypeFloor.discharge)
                {
                    normalesPorRestar = ((pisoAnterior.IsExceededDoubleLevel || pisoAnterior.IsNormalDoubleLevel || pisoAnterior.Type == Floor.TypeFloor.discharge) ? 1 : 2);
                    if (pisoActual.IsExceededDoubleLevel)
                    {
                        finalesPorSumar = ((pisoAnterior.IsExceededDoubleLevel || pisoAnterior.IsNormalDoubleLevel || pisoAnterior.Type == Floor.TypeFloor.discharge) ? 2 : 3);
                    }
                    else if (pisoActual.IsNormalDoubleLevel)
                    {
                        finalesPorSumar = ((pisoAnterior.IsExceededDoubleLevel || pisoAnterior.IsNormalDoubleLevel || pisoAnterior.Type == Floor.TypeFloor.discharge) ? 1 : 2);
                    }
                    else
                    {
                        finalesPorSumar = 0;
                        normalesPorRestar = 0;
                    }
                }

                soportesFinales.Count += finalesPorSumar;
                soportesNormales.Count -= normalesPorRestar;
                pisoAnterior = pisoActual;
            }

            duct.Components = result;
        }
        public Dictionary<DuctPiece.TypeDuct, int> InitDucts()
        {
            return InitDuctsCounter();
        }

        private List<Floor> GetFloors(string paramsJson)
        {
            try
            {
                return JsonSerializer.Deserialize<List<Floor>>(paramsJson);
            }
            catch (Exception)
            {
                return new List<Floor>();
            }
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
                        CalculateDischargeLevel(ref results, ref heightAvaible, floor.Discharge, floor.NeedAntiImpact); break;
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
                            if (heightAvaible > 4.5m)
                            {
                                heightAvaible += 0.9m;
                                CalculateDoubleLevel(ref results, ref heightAvaible, floor.NeedGate, true);
                            }
                            else
                            {
                                heightAvaible += 0.9m;
                                CalculateCommonLevel(ref results, ref heightAvaible, floor.NeedGate,  true);
                            }
                        }
                        else
                        {
                            CalculateLastLevelWithoutChimney(ref results, ref heightAvaible, floor.NeedGate);
                        }
                        break;
                    }
                }

           
            
            return results;
        }

        private List<Component> InitComponentList()
        {
            return Components.GetAllComponents();
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

        private void CalculateDischargeLevel(ref List<DuctPiece> results, ref decimal heightAvaible, Floor.TypeDischarge typeDischarge, bool needAntiImpact)
        {
            Stack<DuctPiece> stack = new Stack<DuctPiece>();
            int num = 0;
            List<DuctPiece> configDischargeLevel = Ducts.GetConfigDischargeLevel();
            heightAvaible -= ((typeDischarge != Floor.TypeDischarge.guilloutine) ? 2.3m : (needAntiImpact ? 2.42m : 1.5m));
            DuctPiece oldBiggerDuct = configDischargeLevel.OrderByDescending((DuctPiece d) => d.Height).FirstOrDefault();
            while (true)
            {
                foreach (DuctPiece item in configDischargeLevel)
                {
                    DuctPiece duct = new DuctPiece(item);
                    CalculateDucts(ref stack, ref heightAvaible, duct);
                }
                if ((heightAvaible <= 0.3m || num >= 3) && IsConsecutive(stack, isLastLevel: false, isNormalLevel: false))
                {
                    break;
                }
                int num2 = 0;
                do
                {
                    if ((from p in stack
                         group p by p.Type into p
                         where p.Key == oldBiggerDuct.Type
                         select p).Count() > 0)
                    {
                        DuctPiece ductPiece = new DuctPiece();
                        do
                        {
                            ductPiece = stack.Pop();
                            heightAvaible += ductPiece.Height;
                        }
                        while (ductPiece.Type != oldBiggerDuct.Type);
                        configDischargeLevel.Remove(oldBiggerDuct);
                        break;
                    }
                    num2++;
                    oldBiggerDuct = (from d in configDischargeLevel
                                     where d.Type != oldBiggerDuct.Type && d.Type != DuctPiece.TypeDuct.C4
                                     orderby d.Height descending
                                     select d).FirstOrDefault();
                }
                while (num2 < 2);
                num++;
            }
            foreach (DuctPiece duct2 in stack)
            {
                results.FirstOrDefault((DuctPiece p) => p.Type == duct2.Type).Count++;
            }
        }

        private void CalculateLastLevelWithoutChimney(ref List<DuctPiece> results, ref decimal heightAvaible, bool needGate)
        {
            foreach (DuctPiece duct in Ducts.GetConfigLastLevel())
            {
                DuctPiece ductPiece = results.FirstOrDefault((DuctPiece piece) => piece.Type == duct.Type);
                switch (duct.Type)
                {
                    case DuctPiece.TypeDuct.C4:
                        heightAvaible -= duct.Height;
                        if (needGate)
                        {
                            ductPiece.Count++;
                            break;
                        }
                        results.FirstOrDefault((DuctPiece piece) => piece.Type == DuctPiece.TypeDuct.S4).Count++;
                        break;
                    case DuctPiece.TypeDuct.B2:
                    case DuctPiece.TypeDuct.B3:
                        ductPiece.Count++;
                        heightAvaible -= duct.Height;
                        break;
                }
            }
        }

        private void CalculateDoubleLevel(ref List<DuctPiece> results, ref decimal heightAvaible, bool needGate, bool isLastLevel = false)
        {
            Stack<DuctPiece> stack = new Stack<DuctPiece>();
            int num = 0;
            List<DuctPiece> configDoubleLevel = Ducts.GetConfigDoubleLevel();
            DuctPiece oldBiggerDuct = (from d in configDoubleLevel
                                       where d.Type != DuctPiece.TypeDuct.C4
                                       orderby d.Height descending
                                       select d).FirstOrDefault();
            DuctPiece item = configDoubleLevel.Where((DuctPiece d) => d.Type == DuctPiece.TypeDuct.C4).FirstOrDefault();
            while (true)
            {
                foreach (DuctPiece item2 in configDoubleLevel)
                {
                    DuctPiece duct = new DuctPiece(item2);
                    if (item2.Type == DuctPiece.TypeDuct.C4)
                    {
                        if (needGate)
                        {
                            CalculateDucts(ref stack, ref heightAvaible, duct);
                            continue;
                        }
                        CalculateDucts(ref stack, ref heightAvaible, new DuctPiece
                        {
                            Name = item2.Name,
                            Height = 1.20m,
                            Type = DuctPiece.TypeDuct.S4
                        });
                    }
                    else
                    {
                        CalculateDucts(ref stack, ref heightAvaible, duct);
                    }
                }
                if ((heightAvaible <= 0.3m || num >= 3) && IsConsecutive(stack, isLastLevel))
                {
                    break;
                }
                while ((from p in stack
                        group p by p.Type into p
                        where p.Key == oldBiggerDuct.Type
                        select p).Count() <= 0)
                {
                    oldBiggerDuct = (from d in configDoubleLevel
                                     where d.Type != oldBiggerDuct.Type && d.Type != DuctPiece.TypeDuct.C4
                                     orderby d.Height descending
                                     select d).FirstOrDefault();
                }
                DuctPiece ductPiece = new DuctPiece();
                do
                {
                    ductPiece = stack.Pop();
                    heightAvaible += ductPiece.Height;
                }
                while (ductPiece.Type != oldBiggerDuct.Type);
                configDoubleLevel.Remove(oldBiggerDuct);
                configDoubleLevel.Remove(item);
                num++;
            }
            foreach (DuctPiece duct2 in stack)
            {
                results.FirstOrDefault((DuctPiece p) => p.Type == duct2.Type).Count++;
            }
        }

        private void CalculateCommonLevel(ref List<DuctPiece> results, ref decimal heightAvaible, bool needGate, bool isLastLevel = false)
        {
            if (heightAvaible <= 0m)
            {
                return;
            }
            Stack<DuctPiece> stack = new Stack<DuctPiece>();
            int num = 0;
            List<DuctPiece> configCommonLevel = Ducts.GetConfigCommonLevel();
            DuctPiece oldBiggerDuct = (from d in configCommonLevel
                                       where d.Type != DuctPiece.TypeDuct.C4
                                       orderby d.Height descending
                                       select d).FirstOrDefault();
            DuctPiece item = configCommonLevel.Where((DuctPiece d) => d.Type == DuctPiece.TypeDuct.C4).FirstOrDefault();
            while (true)
            {
                foreach (DuctPiece item2 in configCommonLevel)
                {
                    new DuctPiece(item2);
                    if (item2.Type == DuctPiece.TypeDuct.C4)
                    {
                        if (needGate)
                        {
                            CalculateDucts(ref stack, ref heightAvaible, item2);
                            continue;
                        }
                        CalculateDucts(ref stack, ref heightAvaible, new DuctPiece
                        {
                            Name = item2.Name,
                            Height = 1.20m,
                            Type = DuctPiece.TypeDuct.S4
                        });
                    }
                    else
                    {
                        CalculateDucts(ref stack, ref heightAvaible, item2);
                    }
                }
                if ((heightAvaible <= 0.3m || num >= 3) && IsConsecutive(stack, isLastLevel))
                {
                    break;
                }
                while ((from p in stack
                        group p by p.Type into p
                        where p.Key == oldBiggerDuct.Type
                        select p).Count() <= 0)
                {
                    oldBiggerDuct = (from d in configCommonLevel
                                     where d.Type != oldBiggerDuct.Type && d.Type != DuctPiece.TypeDuct.C4
                                     orderby d.Height descending
                                     select d).FirstOrDefault();
                }
                DuctPiece ductPiece = new DuctPiece();
                do
                {
                    ductPiece = stack.Pop();
                    heightAvaible += ductPiece.Height;
                }
                while (ductPiece.Type != oldBiggerDuct.Type);
                configCommonLevel.Remove(oldBiggerDuct);
                configCommonLevel.Remove(item);
                num++;
            }
            foreach (DuctPiece duct in stack)
            {
                results.FirstOrDefault((DuctPiece p) => p.Type == duct.Type).Count++;
            }
        }

        private void CalculateDucts(ref Stack<DuctPiece> stack, ref decimal availableHeight, DuctPiece duct)
        {
            if (duct.Type == DuctPiece.TypeDuct.C4 || duct.Type == DuctPiece.TypeDuct.S4)
            {
                stack.Push(duct);
                availableHeight -= duct.Height;
                return;
            }
            while (availableHeight >= duct.Height)
            {
                stack.Push(duct);
                availableHeight -= duct.Height;
            }
        }

        private bool IsConsecutive(Stack<DuctPiece> stack, bool isLastLevel = false, bool isNormalLevel = true)
        {
            bool result = true;
            if (stack.Count == 0)
            {
                return result;
            }
            List<DuctPiece> list = stack.Reverse().ToList();
            if (isNormalLevel)
            {
                list.RemoveAt(0);
            }
            for (int i = 0; i < list.Count - 1; i++)
            {
                if (Math.Abs(list[i].GetWeight() - list[i + 1].GetWeight()) > 1)
                {
                    result = false;
                }
            }
            if (list.Last().GetWeight() == 4 && isLastLevel)
            {
                result = false;
            }
            return result;
        }

    }
}
