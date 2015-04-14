using System;
using DomainConstant;
using DomainInterface;
using Xamarin.Forms;

namespace FourthFnB
{
    public class ViewGridPage : ContentPage
    {

        private SQLiteGrid grid;
        private bool isConnectionClosed;

        public ViewGridPage(IDatabase database)
        {
            grid = new SQLiteGrid(database,
                Convert.ToInt64(Application.Current.Properties[ApplicationProperties.Site]))
            {
                BackgroundColor = Color.White,
                VerticalOptions = LayoutOptions.FillAndExpand,
            };

            Content = grid;

            this.isConnectionClosed = false;
        }

        protected override bool OnBackButtonPressed()
        {
            CloseConnection();

            return base.OnBackButtonPressed();
        }

        protected override void OnDisappearing()
        {
            CloseConnection();
            base.OnDisappearing();
        }

        public async void CloseConnection()
        {
            if (!this.isConnectionClosed)
            {
                grid.CloseConnection();
                this.isConnectionClosed = true;
                //await Navigation.PopAsync(true);
            }
        }
    }
}