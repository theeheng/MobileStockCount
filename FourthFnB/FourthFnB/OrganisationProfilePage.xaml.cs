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
    public partial class OrganisationProfilePage : ContentPage, INotifyPropertyChanged
    {
        private bool isBusy;
        private IFnBWebAPI fnbWebAPI;
        private IEnumerable<IOrganisation> organisationList;
        private IEnumerable<IUserProfile> userProfileList;

        public event PropertyChangedEventHandler PropertyChanged;
        
        public string AccessToken
        {
            get { return Application.Current.Properties[ApplicationProperties.AccessToken].ToString(); }
        }

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

        public OrganisationProfilePage(IEnumerable<IOrganisation> orgList)
        {
            InitializeComponent();
            this.BindingContext = this;
            this.IsBusy = false;
            this.fnbWebAPI = IoCContainer.Container.Resolve(typeof(IFnBWebAPI), IoCInstanceConstant.WebAPIInstance) as IFnBWebAPI;
            this.organisationList = orgList;
            InitializeOrganisationPicker();
            UpdateLabel();
        }

        private void InitializeOrganisationPicker()
        {
            if (organisationList.Count() >= 1)
            {
                foreach (IOrganisation org in organisationList)
                {
                    organisationPkr.Items.Add(org.OrganisationName);
                }

                organisationPkr.SelectedIndex = 0;
            }
        }

        private void InitializeUserProfilePicker()
        {
            if (organisationList.Count() >= 1)
            {
                foreach (IUserProfile profile in userProfileList)
                {
                    userProfilePkr.Items.Add(profile.ProfileFullName);
                }

                userProfilePkr.SelectedIndex = 0;
            }
        }

        public void UpdateLabel()
        {
            this.accessTokenLbl.Text = AccessToken;
        }

        async void OnOrganisationSelectedIndexChanged(object sender, EventArgs args)
        {
            string uniqueOrgId = this.organisationList.ElementAt(organisationPkr.SelectedIndex).UniqueOrganisationId;

            Application.Current.Properties[ApplicationProperties.OrganisationGuid] = uniqueOrgId;

            //refresh user profile picker
            this.userProfileList = await this.fnbWebAPI.GetUserProfile(AccessToken, uniqueOrgId);
            InitializeUserProfilePicker();
        }

        async void OnUserProfileSelectedIndexChanged(object sender, EventArgs args)
        {
            Application.Current.Properties[ApplicationProperties.UserProfile] =
                this.userProfileList.ElementAt(userProfilePkr.SelectedIndex).UserProfileID;
        }

        async void OnNextButtonClicked(object sender, EventArgs args)
        {
            IsBusy = true;

            try
            {

                IAuthenticationProfile profile =
                    IoCContainer.Container.Resolve(typeof (IAuthenticationProfile), null) as IAuthenticationProfile;
                profile.AccessToken = AccessToken;
                profile.UserProfileId =
                    Convert.ToInt64(Application.Current.Properties[ApplicationProperties.UserProfile]);
                profile.UniqueOrganisationId =
                    Application.Current.Properties[ApplicationProperties.OrganisationGuid].ToString();

                var result = await this.fnbWebAPI.SetUserProfile(profile);

                IEnumerable<ISite> siteList = await this.fnbWebAPI.GetSite(AccessToken);

                await Navigation.PushAsync(new HomePage(siteList));
            }
            catch (Exception ex)
            {
                string err = ex.Message;
            }

            IsBusy = false;
        }
    }
}
