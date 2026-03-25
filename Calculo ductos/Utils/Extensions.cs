using Calculo_ductos.Config;
using Calculo_ductos.Models;
using Calculo_ductos.Params;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculo_ductos.Utils
{
    public static class Extensions
    {
        public static List<DuctPiece> SumDuctPieces(this List<Floor> floors) 
        {
            List<DuctPiece> duct = new List<DuctPiece>();
            duct = Ducts.GetAllDucts();
            try
            {
                duct = floors
                    .SelectMany(floor => floor.Ducts)
                    .GroupBy(piece => new { piece.Type , piece.Name})
                    .Select(group => new DuctPiece 
                    {
                        Type = group.Key.Type,
                        Name = group.Key.Name,
                        Count = group.Sum(piece=>piece.Count)
                    }).ToList();
                //Se agrega regla de tener un B2 extra para cualquier eventualidad
                var B2 = duct.Where(piece=>piece.Type.Equals(DuctPiece.TypeDuct.B2)).FirstOrDefault();
                if (B2!=null)B2.Count++;
            }
            catch (Exception ex)
            {

            }
            return duct;
        }
        //public static List<Component> SumComponents(this List<Floor> floors)
        //{
        //    List<Component> components = new List<Component>();
        //    components = Components.GetAllComponents();
        //    try
        //    {
        //        components = floors
        //            .SelectMany(floor => floor.Components)
        //            .GroupBy(component => new { component.Type, component.Name })
        //            .Select(group => new Component
        //            {
        //                Type = group.Key.Type,
        //                Name = group.Key.Name,
        //                Count = group.Count()
        //            }).ToList();
        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //    return components;
        //}


    }
}
