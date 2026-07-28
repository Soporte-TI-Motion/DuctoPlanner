using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculo_ductos_winUi_3.Models
{
    public class ConceptModel : ObservableObject
    {
        private int _Count = 0;
        public enum TypeConcept {duct, freight, manpower, indirect }
        public decimal UnitPrice { get; set; } = 0.0m;
        public string Description { get; set; } = string.Empty;
        public int MaxCount { get; private set; } = 0;
        public int Count { 
            get => _Count; 
            set {
                var clamped = Math.Clamp(value, 0, MaxCount);
                SetProperty(ref _Count, clamped);
                OnPropertyChanged(nameof(TotalPrice));
                OnPropertyChanged(nameof(CanIncrement));
                OnPropertyChanged(nameof(CanDecrement));
            } }
        public decimal TotalPrice => UnitPrice * Count;
        // helpers para habilitar/deshabilitar botones en la vista
        public bool CanIncrement => Count < MaxCount;
        public bool CanDecrement => Count > 0;

        public TypeConcept Type { get; set; }
        public bool IsEditable { get; set; } = false;

        public void Initialize(int count)
        {
            MaxCount = count;
            _Count = count; // sin pasar por el setter para no disparar notificaciones antes de tiempo
            OnPropertyChanged(nameof(Count));
            OnPropertyChanged(nameof(TotalPrice));
            OnPropertyChanged(nameof(CanIncrement));
            OnPropertyChanged(nameof(CanDecrement));
        }
    }
}
