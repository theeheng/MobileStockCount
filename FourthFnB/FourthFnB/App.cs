using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using DomainInterface;
using FourthFnB.Translation;
using Xamarin.Forms;

namespace FourthFnB
{
    public class App : Application
    {
        public App()
        {
            System.Diagnostics.Debug.WriteLine("===============");
            var assembly = typeof(App).GetTypeInfo().Assembly;
            foreach (var res in assembly.GetManifestResourceNames())
                System.Diagnostics.Debug.WriteLine("found resource: " + res);


            if (Device.OS != TargetPlatform.WinPhone)
            {
                DependencyService.Get<ILocalize>().SetLocale();
                //Resx.AppResources.Culture = DependencyService.Get<ILocalize>().GetCurrentCultureInfo();
            }

            MainPage = new NavigationPage(new LoginPage());  //new NavigationPage();

        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
