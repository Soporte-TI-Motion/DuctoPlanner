using Calculo_ductos.Config;
using Calculo_ductos.Params;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculo_ductos.Utils
{
    public static class Dictionaries
    {
        /// <summary>
        /// Agrega el contenido de un diccionario a otro, validando que coincidan sus tipos.
        /// </summary>
        /// <typeparam name="TKey">Tipo de la clave.</typeparam>
        /// <typeparam name="TValue">Tipo del valor.</typeparam>
        /// <param name="target">Diccionario destino.</param>
        /// <param name="source">Diccionario fuente.</param>
        /// <param name="overwrite">Si es true, sobrescribe las claves existentes en el diccionario destino.</param>
        public static void AddRange<TKey, TValue>(
            this IDictionary<TKey, TValue> target,
            IDictionary<TKey, TValue> source,
            bool overwrite = true)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            foreach (var kvp in source)
            {
                if (target.ContainsKey(kvp.Key))
                {
                    if (overwrite)
                    {
                        if (target[kvp.Key] is int existingValue)
                        {
                            // Sumar el valor existente con el nuevo
                            target[kvp.Key] = (TValue)(object)(existingValue + Convert.ToInt32(kvp.Value)); ;
                        }
                        else
                            target[kvp.Key] = kvp.Value; // Sobrescribir valor existente
                    }
                }
                else
                {
                    target.Add(kvp.Key, kvp.Value);
                }
            }
        }

        public static Dictionary<DuctPiece.TypeDuct, int> SumDucts(
            this List<Floor> floors) {
            Dictionary<DuctPiece.TypeDuct, int> counter = InitDuctsCounter();
            //foreach (Floor floor in floors)
            //{
            //    foreach (var duct in floor.Ducts)
            //    {
            //        //counter[duct.Key] += duct.Value;
            //    }
            //}
            return counter;
            
        }
        public static Dictionary<DuctPiece.TypeDuct, int> InitDuctsCounter()
        {
            Dictionary<DuctPiece.TypeDuct, int> counter = new Dictionary<DuctPiece.TypeDuct, int>();

            foreach (DuctPiece duct in Ducts.GetAllDucts())
            {
                counter.Add(duct.Type, 0);
            }

            return counter;
        }
    }
}
