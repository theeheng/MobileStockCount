using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace FourthFnB
{
    public partial class ViewStockCount : ContentPage
    {
        public ViewStockCount()
        {
            //InitializeComponent();

            Content = new StackLayout
            {
                Children = {
                new Label {
                  Text = "Hello, Custom Renderer !",
                }, 
                new StockCountGridView {
                  
                }
              },
                VerticalOptions = LayoutOptions.CenterAndExpand,
                HorizontalOptions = LayoutOptions.CenterAndExpand,
            };

            // Display stock count grid
            //IGridViewDisplay gridView = DependencyService.Get<IGridViewDisplay>();
            //gridView.DisplayStockCountGrid();

        }
    }
}
