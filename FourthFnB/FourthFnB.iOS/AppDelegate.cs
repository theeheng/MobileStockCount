using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DomainConstant;
using DomainInterface;
using Foundation;
using IoC;
using Microsoft.Practices.Unity;
using SQLite.Net.Interop;
using UIKit;
using ParameterOverride = Microsoft.Practices.Unity.ParameterOverride;
using ResolverOverride = Microsoft.Practices.Unity.ResolverOverride;

namespace FourthFnB.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //

       
       
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal); // Documents folder
            string libraryPath = Path.Combine(documentsPath, "../Library/"); // Library folder
            ISQLitePlatform sqlitePlatform = new SQLite.Net.Platform.XamarinIOS.SQLitePlatformIOS();

            if (IoCContainer.Container == null)
            {
                IoCContainer.InitialiseContainer();
            }

            IDatabase database = IoCContainer.Container.Resolve(typeof(IDatabase), null, new ResolverOverride[]
        {
            new ParameterOverride("sqlitePlatform", sqlitePlatform),
            new ParameterOverride("libraryPath", libraryPath)
        }) as IDatabase;
           // Database = new StockDatabase(Path.Combine(libraryPath, StockDatabase.DatabaseFileName));

            IFnBWebAPI fnbWebAPI = IoCContainer.Container.Resolve(typeof(IFnBWebAPI), null) as IFnBWebAPI;

            IoCContainer.Container.RegisterInstance(typeof(IDatabase), IoCInstanceConstant.StockDatabaseInstance, database, new ContainerControlledLifetimeManager());
            IoCContainer.Container.RegisterInstance(typeof(IFnBWebAPI), IoCInstanceConstant.WebAPIInstance, fnbWebAPI, new ContainerControlledLifetimeManager());

            global::Xamarin.Forms.Forms.Init();
            LoadApplication(new App());

            return base.FinishedLaunching(app, options);
        }
    }
}
