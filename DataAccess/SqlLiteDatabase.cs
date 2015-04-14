using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DomainInterface;
using DomainObject;
using SQLite.Extensions;
using SQLite.Net;
using SQLite.Net.Interop;

namespace DataAccess
{
    public class SqlLiteDatabase : SQLiteConnection, IDatabase
    {
        private readonly static string sqliteFilename = "StockDB.db3";
        
        /*public string DatabaseFileName
        {
            get
            {

                return sqliteFilename;

                
#if NETFX_CORE
				var path = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, sqliteFilename);
#else

#if SILVERLIGHT
				// Windows Phone expects a local path, not absolute
				var path = sqliteFilename;
#else

#if __ANDROID__
				// Just use whatever directory SpecialFolder.Personal returns
				string libraryPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal); ;
#else
                // we need to put in /Library/ on iOS5.1 to meet Apple's iCloud terms
                // (they don't want non-user-generated data in Documents)
                string documentsPath = ""; //Environment.GetFolderPath(Environment.SpecialFolder.Personal); // Documents folder
                string libraryPath = Path.Combine(documentsPath, "../Library/"); // Library folder
#endif
                var path = Path.Combine(libraryPath, sqliteFilename);
#endif

#endif
                return path;
                
            }
        } * */

        public SqlLiteDatabase(ISQLitePlatform sqlitePlatform, string libraryPath)
            : base(sqlitePlatform, Path.Combine(libraryPath, sqliteFilename))
        {
            // create the tables
            CreateTable<Stock>();
            CreateTable<StockCountItem>();            
            CreateTable<StockItemSize>();
            CreateTable<StockItemSizeBarcode>();
            CreateTable<StockCount>();
            CreateIndex("StockCount_UNIQUE", "StockCount", new string[] { "SiteItemId", "StockItemSizeId" }, true);
        }
    }
}