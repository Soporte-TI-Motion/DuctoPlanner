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
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Calculo_ductos_winUi_3.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CalculateIndirectsOthersSubview : Page
    {
        public StateViewModel stateApp { get; set; }
        public CalculateIndirectsOthersSubview()
        {
            this.InitializeComponent();
            stateApp = ((App)Application.Current).ViewModel;
            this.DataContext = stateApp;
        }
        private async void CalculateIndirects_Click(object sender, RoutedEventArgs e)
        {
            if (AppHasData())
            {
                await stateApp.CalculateIndirects(sender, e);
            }
            else
                await ShowEmptyDataDialog(sender, "Aun no se tiene un despiece.");
        }
        public async void AddIndirect_Click(object sender, RoutedEventArgs e)
        {
            var selected = stateApp.IndirectsVM.SelectedIndirect;
            var validations = new List<string>();
            if (stateApp.IndirectsVM.OtherIndirectsInstaller.Where(i => i.PoliticaViaticosId.Equals(selected.Id)).Count() > 0)
                validations.Add($"Ya se cuenta con un viático {selected.Concept}, por favor revísalo.");
            if (stateApp.IndirectsVM.selectedUnitCost.Equals("0"))
                validations.Add($"El viático {selected.Concept} no puede tener costo 0, por favor revísalo.");
            if (validations.Count == 0)
                stateApp.IndirectsVM.AddIndirect();
            else
                await stateApp.ShowEmptyDataDialog(string.Join(Environment.NewLine, validations));
        }
        private async Task ShowEmptyDataDialog(object sender, string message)
        {
            var frameworkElement = sender as FrameworkElement;

            var dialog = new ContentDialog
            {
                Title = "Sin datos",
                Content = message,
                CloseButtonText = "Aceptar",
                XamlRoot = frameworkElement.XamlRoot // ?? Esto sí es válido siempre
            };

            await dialog.ShowAsync();
        }
        private bool AppHasData()
        {
            return stateApp.ComponentsVM.ComponentList.Count > 0;
        }
    }
}
