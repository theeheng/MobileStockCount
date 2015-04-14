using System;
using DataAccess;
using DomainInterface;
using DomainObject;
using FnBModelWebAPI;
using Microsoft.Practices.Unity;
using Newtonsoft.Json;

namespace IoC
{
    public static class IoCContainer
    {
        public static UnityContainer Container { get; set; }

        public static void InitialiseContainer()
        {
            if (Container == null)
            {
                Container = new UnityContainer();
                Container.RegisterType<ILogin, Login>();
                Container.RegisterType<IDatabase, SqlLiteDatabase>();
                Container.RegisterType<IStock, Stock>();
                Container.RegisterType<IBarcodeResult, BarcodeResult>();
                Container.RegisterType<IFnBWebAPI, FnBWebAPI>();
                Container.RegisterType<IOrganisation, Organisation>();
                Container.RegisterType<IUserProfile, UserProfile>();
                Container.RegisterType<ISite, Site>();
                Container.RegisterType<IStockPeriodHeader, StockPeriodHeader>();
                Container.RegisterType<IStockCount, StockCount>();
                Container.RegisterType<IStockItemSize, StockItemSize>();
                Container.RegisterType<IStockCountItem, StockCountItem>();
                Container.RegisterType<IStockRepository, StockRepository>();
                Container.RegisterType<IStockItemSizeBarcode, StockItemSizeBarcode>();
                Container.RegisterType<IStockCountItemRepository, StockCountItemRepository>();
                Container.RegisterType<IStockItemSizeRepository, StockItemSizeRepository>();
                Container.RegisterType<IStockCountRepository, StockCountRepositsory>();
                Container.RegisterType<IStockItemSizeBarcodeRepository, StockItemSizeBarcodeRepository>();
                Container.RegisterType<IAuthenticationProfile, AuthenticationProfile>();
                Container.RegisterType<IUploadStockCount, UploadStockCount>();

            }
        }

        public static JsonSerializerSettings  GetCustomSerializerSetting<T>() 
        {
            var converter = new JsonDataConverter<T>();
            var deseralizeSettings = new JsonSerializerSettings();
            
            deseralizeSettings.Converters.Add(converter);

            return deseralizeSettings;
        }
    }
}

