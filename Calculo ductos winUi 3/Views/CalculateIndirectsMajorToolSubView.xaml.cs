using Calculo_ductos_winUi_3.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Calculo_ductos_winUi_3.Views
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CalculateIndirectsMajorToolSubView : Page
    {
        public StateViewModel stateApp { get; set; }
        public CalculateIndirectsMajorToolSubView()
        {
            this.InitializeComponent();
            stateApp = ((App)Application.Current).ViewModel;
            this.DataContext = stateApp;
        }
        
        public async void AddMajorTool_Click(object sender, RoutedEventArgs e)
        {
            var selected = stateApp.IndirectsVM.SelectedTransportType;
            var validations = new List<string>();
            if (stateApp.IndirectsVM.OtherIndirectsInstaller.Where(i => i.Concepto.Equals(selected.Description)).Count() > 0)
                validations.Add($"Ya se cuenta con un viático de transporte {selected.Description}, por favor revísalo.");
            if (stateApp.IndirectsVM.selectedTrasnportCost.Equals("0"))
                validations.Add($"El viático de transporte {selected.Description} no puede tener costo 0, por favor revísalo.");
            if (validations.Count == 0)
                stateApp.IndirectsVM.AddTransport();
            else
                await stateApp.ShowEmptyDataDialog(string.Join(Environment.NewLine, validations));
        }
    }
}
