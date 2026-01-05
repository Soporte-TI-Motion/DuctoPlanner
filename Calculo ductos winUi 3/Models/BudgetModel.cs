using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculo_ductos_winUi_3.Models
{
    public class IndirectPpto {
        public ElementModel Installer { get; set; }
        public ElementModel Visit { get; set; }
        public ElementModel Security { get; set; }
        public ElementModel Supervisor { get; set; }
        public ElementModel WC { get; set; }
        public ElementModel Store { get; set; }

    }
    public class ElementModel 
    {
        public BudgetModel budget { get; set; }
        public BudgetModel price { get; set; }
    }
    public class BudgetModel
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Amount { get; set; } = 0m;
        public int EfectiveWorkDays { get; set; } = 0;
        public int NoWorkDays { get; set; } = 0;
        public int TotalDays => EfectiveWorkDays + NoWorkDays;
        public decimal MOAmount { get; set; } = 0m;
        public decimal TravelExpensesAmount { get; set; } = 0m;
        public decimal ToolsAmount { get; set; } = 0m;
        public decimal FreightAmount { get; set; } = 0m;
        public decimal TotalAmount => MOAmount + TravelExpensesAmount + ToolsAmount + FreightAmount;
    }                                 
}                                     
                                      