
namespace Calculo_ductos
{
    using Calculo_ductos.Config;
    using Calculo_ductos.Params;
    using Context;
    public static class Facade
    {
        //public static Dictionary<DuctPiece.TypeDuct, int> CalculateDucts(string paramsJson)
        //{
        //    using (ContextDucts context = new ContextDucts()) { 
        //        return context.CalculateDucts(paramsJson);
        //    }
        //}
        //public static List<Component> CalculateComponents(int floorCount, Dictionary<DuctPiece.TypeDuct, int> ducts, bool needChimney)
        //{
        //    using (ContextDucts context = new ContextDucts())
        //    {
        //        return context.CalculateComponents(floorCount, ducts, needChimney);
        //    }
        //}
        public static Duct CalculateDuctsByFloor(string paramsJson)
        {
            using (ContextDucts context = new ContextDucts())
            {
                return context.CalculateDuctsByFloor(paramsJson);
            }
        }

        //public static Dictionary<DuctPiece.TypeDuct, int> InitEmptyDucts()
        //{
        //    using (ContextDucts context = new ContextDucts())
        //    {
        //        return context.InitDucts();
        //    }
           
        //}
    }
}
