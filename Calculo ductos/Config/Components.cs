using Calculo_ductos.Params;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Calculo_ductos.Params.Component;

namespace Calculo_ductos.Config
{
    static class Components
    {
        public static List<Component> GetAllComponents()
        {
            return new List<Component> {
                new Component { Name = "Soporte especial XE", Type = TypeComponent.XE},
                new Component { Name = "Soporte normal XN", Type = TypeComponent.XN},
                new Component { Name = "Soporte final XNF", Type = TypeComponent.XNF},
                new Component { Name = "Puerta", Type = TypeComponent.Gate},
                new Component { Name = "Aspersor", Type = TypeComponent.Sprinkler},
                new Component { Name = "Guillotina", Type = TypeComponent.Guillotine},
                new Component { Name = "Descargador", Type = TypeComponent.Discharge},
                new Component { Name = "Sistema de desinfección", Type = TypeComponent.DisinfectionSystem},
                new Component { Name = "Chimenea", Type = TypeComponent.Chimney},
                new Component { Name = "Cuello de ganso", Type = TypeComponent.TVA},
                new Component { Name = "Contenedor", Type = Component.TypeComponent.Container},
                new Component { Name = "Dispositivo anti-impactos", Type = Component.TypeComponent.AntiImpact}

            };
        }

    }
}
