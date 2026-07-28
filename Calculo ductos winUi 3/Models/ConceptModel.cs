using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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
                OnPropertyChanged(nameof(TotalPriceFormatted));
                OnPropertyChanged(nameof(CanIncrement));
                OnPropertyChanged(nameof(CanDecrement));
                IncrementCommand.NotifyCanExecuteChanged();
                DecrementCommand.NotifyCanExecuteChanged();
            } }
        public decimal TotalPrice => UnitPrice * Count;
        public string TotalPriceFormatted => $"$ {TotalPrice:N2}";
        public bool CanIncrement => Count < MaxCount;
        public bool CanDecrement => Count > 0;

        public TypeConcept Type { get; set; }
        public bool IsEditable { get; set; } = false;

        private bool _IsSelected = true;
        public bool IsSelected
        {
            get => _IsSelected;
            set
            {
                if (!IsEditable) return;
                SetProperty(ref _IsSelected, value);
            }
        }

        public IRelayCommand IncrementCommand { get; }
        public IRelayCommand DecrementCommand { get; }

        public ConceptModel()
        {
            IncrementCommand = new RelayCommand(() => Count++, () => CanIncrement);
            DecrementCommand = new RelayCommand(() => Count--, () => CanDecrement);
        }

        public void Initialize(int count)
        {
            MaxCount = count;
            _Count = count;
            OnPropertyChanged(nameof(Count));
            OnPropertyChanged(nameof(TotalPrice));
            OnPropertyChanged(nameof(TotalPriceFormatted));
            OnPropertyChanged(nameof(CanIncrement));
            OnPropertyChanged(nameof(CanDecrement));
            IncrementCommand.NotifyCanExecuteChanged();
            DecrementCommand.NotifyCanExecuteChanged();
        }
    }
}
