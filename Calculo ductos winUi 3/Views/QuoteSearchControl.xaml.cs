using Calculo_ductos_winUi_3.Models;
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
    public sealed partial class QuoteSearchControl : UserControl
    {
        public QuoteSearchControl()
        {
            InitializeComponent();
        }
        public event Action<QuoteModel> QuoteSelected;

        private void Quote_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem is QuoteModel q)
            {
                QuoteSelected?.Invoke(q);
            }
        }
    }
}
