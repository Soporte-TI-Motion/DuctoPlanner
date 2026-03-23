using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculo_ductos.Params
{
    public class Component
    {
        public enum TypeComponent { XE, XN, XNF, Gate, Sprinkler, Guillotine, Discharge, DisinfectionSystem, Chimney, TVA, Container, AntiImpact}
        public string Name { get; set; } = string.Empty;
        public int Count { get; set; } = 0;
        public TypeComponent Type { get; set; }


    }
}
