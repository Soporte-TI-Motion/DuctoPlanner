using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculo_ductos_winUi_3.Models
{
    public class EfectiveWorkDayModel : ObservableObject
    {
        #region Fields
        private int _WorkDawysBase;
        private int _WorkDayForeign;
        private int _WorkDaysExtraFloors;
        private double _WorkDaysDobleFloors;
        private int _WorkDayDueDate;
        private int _NoWorkDays;

        #endregion
        #region Constructor
        public EfectiveWorkDayModel() { 
            _WorkDawysBase = 0;
            _WorkDayForeign = 0;
            _WorkDaysExtraFloors = 0;
            _WorkDaysDobleFloors = 0;
            _NoWorkDays = 0;
            _WorkDayDueDate = 1;
        }
        #endregion
        #region Properties
        public int WorkDaysBase {
            get => _WorkDawysBase;
            set { SetProperty(ref _WorkDawysBase, value); }
        }
        public int WorkDayForeign
        {
            get => _WorkDayForeign;
            set { SetProperty(ref _WorkDayForeign, value); }
        }
        public int WorkDaysExtraFloors
        {
            get => _WorkDaysExtraFloors;
            set { SetProperty(ref _WorkDaysExtraFloors, value); }
        }
        public double WorkDaysDobleFloors
        {
            get => _WorkDaysDobleFloors;
            set { SetProperty(ref _WorkDaysDobleFloors, value); }
        }
        public int WorkDayDueDate
        {
            get => _WorkDayDueDate;
            set { SetProperty(ref _WorkDayDueDate, value); }
        }
        public int NoWorkDays
        {
            get=> _NoWorkDays;
            set { 
                SetProperty(ref _NoWorkDays, value);
            }
        }
        public int TotalWorkDays {
            get {
                return Convert.ToInt32(Math.Ceiling(_WorkDawysBase + _WorkDayForeign + _WorkDaysExtraFloors + _WorkDaysDobleFloors + _WorkDayDueDate));

            }
        }
        #endregion
    }
}
