using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculo_ductos.Params
{
    public class DuctPiece
    {
        public enum TypeDuct { A2,B2,B3,B4,B2F,B3F,B4F,C4,S4,SinDucto }
        public decimal Height { get; set; }
        public string Name { get; set; } = string.Empty;
        public TypeDuct Type { get; set; }
        public int Count { get; set; } = 0;
        public int GetWeight()
        {
            switch (Type)
            {
                case TypeDuct.A2:
                case TypeDuct.B2:
                case TypeDuct.B2F:
                    return 2;
                case TypeDuct.B3:
                case TypeDuct.B3F:
                    return 3;
                case TypeDuct.B4:
                case TypeDuct.B4F:
                case TypeDuct.C4:
                case TypeDuct.S4:
                    return 4;
                default:
                    return 0;
            }
        }
        public DuctPiece(DuctPiece other)
        {
            Height = other.Height;
            Name = other.Name;
            Type = other.Type;
            Count = other.Count;
        }

        public DuctPiece()
        {
        }

        public override bool Equals(object obj)
        {
            if (obj is DuctPiece ductPiece)
            {
                if (Height == ductPiece.Height && Name == ductPiece.Name && Type == ductPiece.Type)
                {
                    return Count == ductPiece.Count;
                }
                return false;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Height, Name, Type, Count);
        }

    }
}
