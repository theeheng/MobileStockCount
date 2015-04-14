using System;
using System.IO;
using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using DomainConstant;
using DomainInterface;
using IoC;
using Microsoft.Practices.Unity;
using SQLite.Net.Interop;

namespace FourthFnB.Droid
{
    [Activity(Label = "FourthFnB", Icon = "@drawable/icon", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation, Theme = "@android:style/Theme.Holo.Light")]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsApplicationActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            if (IoCContainer.Container == null)
            {
                IoCContainer.InitialiseContainer();
            }

            // Just use whatever directory SpecialFolder.Personal returns
            string libraryPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            ISQLitePlatform sqlitePlatform = new SQLite.Net.Platform.XamarinAndroid.SQLitePlatformAndroid();

            IDatabase database = IoCContainer.Container.Resolve(typeof(IDatabase), null, new ResolverOverride[]
                        {
                            new ParameterOverride("sqlitePlatform", sqlitePlatform),
                            new ParameterOverride("libraryPath", libraryPath)
                        }) as IDatabase;

           // Database = new StockDatabase(Path.Combine(libraryPath, StockDatabase.DatabaseFileName));

            IFnBWebAPI fnbWebAPI = IoCContainer.Container.Resolve(typeof(IFnBWebAPI), null) as IFnBWebAPI;

            IoCContainer.Container.RegisterInstance(typeof(IDatabase), IoCInstanceConstant.StockDatabaseInstance, database, new ContainerControlledLifetimeManager());
            IoCContainer.Container.RegisterInstance(typeof(IFnBWebAPI), IoCInstanceConstant.WebAPIInstance, fnbWebAPI, new ContainerControlledLifetimeManager());


            base.OnCreate(bundle);

            global::Xamarin.Forms.Forms.Init(this, bundle);
            LoadApplication(new App());
        }
    }
}

