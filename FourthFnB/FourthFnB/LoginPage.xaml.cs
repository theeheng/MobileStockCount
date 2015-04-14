using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainConstant;
using DomainInterface;
using IoC;
using Xamarin.Forms;

namespace FourthFnB
{
    public partial class LoginPage : ContentPage, INotifyPropertyChanged
    {
        private bool isBusy;
        private IFnBWebAPI fnbWebAPI;
        public event PropertyChangedEventHandler PropertyChanged;

        public bool IsBusy
        {
            set
            {
                if (isBusy != value)
                {
                    isBusy = value;

                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this,
                            new PropertyChangedEventArgs("IsBusy"));
                    }
                }
            }
            get { return isBusy; }
        }

        public LoginPage()
        {
            InitializeComponent();
            this.BindingContext = this;
            this.fnbWebAPI = IoCContainer.Container.Resolve(typeof(IFnBWebAPI), IoCInstanceConstant.WebAPIInstance) as IFnBWebAPI;
            IsBusy = false;
            NavigationPage.SetHasNavigationBar(this, false);
        }

        async void OnLoginButtonClicked(object sender, EventArgs args)
        {
            Button button = (Button)sender;

            IsBusy = true;

            string accessToken= string.Empty;

            accessToken = await this.fnbWebAPI.Login(this.entryUsername.Text, this.entryPassword.Text);

            Application.Current.Properties[ApplicationProperties.AccessToken] = accessToken;

            if (accessToken == "offline")
            {
                IsBusy = false;
                await Navigation.PushAsync(new HomePage(null));
            }
            else if (accessToken != string.Empty)
            {

                IEnumerable<IOrganisation> organisationList = await this.fnbWebAPI.GetOrganisation(accessToken);

                IsBusy = false;

                await Navigation.PushAsync(new OrganisationProfilePage(organisationList));
            }
            else
            {
                DisplayAlert("Login Failed!", "Please enter a correct username / password !", "OK");
            }
            
        }
    }
}
