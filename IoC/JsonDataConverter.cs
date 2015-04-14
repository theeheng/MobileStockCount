using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DomainInterface;
using DomainObject;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace IoC
{
    public class JsonDataConverter<T> : CustomCreationConverter<T>
    {
        public JsonDataConverter()
        {
        }

        public override T Create(Type objectType)
        {
            // Converts the anonymouse IResultData to the strong type defined in the constructor
            switch (objectType.ToString())
            {
                case "DomainInterface.ILogin":
                    return (T) IoCContainer.Container.Resolve(typeof (ILogin), null);
                    break;
                case "DomainInterface.IOrganisation":
                    return (T) IoCContainer.Container.Resolve(typeof (IOrganisation), null);
                    break;
                case "DomainInterface.IUserProfile":
                    return (T)IoCContainer.Container.Resolve(typeof(IUserProfile), null);
                    break;
                case "DomainInterface.ISite":
                    return (T)IoCContainer.Container.Resolve(typeof(ISite), null);
                    break;
                case "DomainInterface.IStockPeriodHeader":
                    return (T)IoCContainer.Container.Resolve(typeof(IStockPeriodHeader), null);
                    break;
                case "DomainInterface.IUploadStockCount":
                    return (T)IoCContainer.Container.Resolve(typeof(IUploadStockCount), null);
                    break;
                case "DomainInterface.IAuthenticationProfile":
                    return (T)IoCContainer.Container.Resolve(typeof(IAuthenticationProfile), null);
                    break;
                default:

                    if (objectType.ToString().Contains("IEnumerable") && objectType.ToString().Contains("IOrganisation"))
                    {
                        return (T)(object)new List<IOrganisation>();
                        break;
                    }
                    else if (objectType.ToString().Contains("IEnumerable") && objectType.ToString().Contains("IUserProfile"))
                    {
                        return (T)(object)new List<IUserProfile>();
                        break;
                    }
                    else if (objectType.ToString().Contains("IEnumerable") && objectType.ToString().Contains("ISite"))
                    {
                        return (T)(object)new List<ISite>();
                        break;
                    }
                    else if (objectType.ToString().Contains("IEnumerable") && objectType.ToString().Contains("IStockCountItem"))
                    {
                        return (T)(object)new List<IStockCountItem>();
                        break;
                    }
                    else if (objectType.ToString().Contains("IEnumerable") && objectType.ToString().Contains("IUploadStockCount"))
                    {
                        return (T)(object)new List<IUploadStockCount>();
                        break;
                    }
                    else
                    {
                        return default(T);
                    }
            }


        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,  JsonSerializer serializer)
        {
            if (objectType.ToString().Contains("IEnumerable") && objectType.ToString().Contains("IOrganisation"))
            {
                /*JArray json = serializer.Deserialize<JArray>(reader);

                IList<IOrganisation> tempOrg = (from c in json
                                               select new Organisation
                                               {
                                                   OrganisationId = (int)c["OrganisationId"],
                                                   OrganisationCode = (string)c["OrganisationCode"],
                                                   OrganisationName = (string)c["OrganisationName"],
                                                   UniqueConfigId = (string) c["UniqueConfigId"],
                                                   UniqueOrganisationId = (string)c["UniqueOrganisationId"],
                                               }).ToList<IOrganisation>();

                return tempOrg;
                */
               
                    List<Organisation> res = new List<Organisation>();
                serializer.Populate(reader, res);
                return res;
            }
            else if (objectType.ToString().Contains("IEnumerable") && objectType.ToString().Contains("IUserProfile"))
            {
                List<UserProfile> res = new List<UserProfile>();
                serializer.Populate(reader, res);
                return res;
            }
            else if (objectType.ToString().Contains("IEnumerable") && objectType.ToString().Contains("ISite"))
            {
                List<Site> res = new List<Site>();
                serializer.Populate(reader, res);
                return res;
            }
            else if (objectType.ToString().Contains("IEnumerable") && objectType.ToString().Contains("IUploadStockCount"))
            {
                List<UploadStockCount> res = new List<UploadStockCount>();
                serializer.Populate(reader, res);
                return res;
            }
            else if (objectType.ToString().Contains("IEnumerable") && objectType.ToString().Contains("IStockCountItem"))
            {
                /*List<StockCountItem> res = new List<StockCountItem>();
                serializer.Populate(reader, res);
                return res;
                */

                JArray json = serializer.Deserialize<JArray>(reader);

                IEnumerable<IStockCountItem> tempStockCountItem = (from c in json
                                                                   select new StockCountItem
                                                                   {
                                                                       SiteItemId = (long)c["SiteItemID"],
                                                                       CategoryId = (long)c["CategoryID"],
                                                                       ItemName = (string)c["ItemName"],
                                                                       CategoryName = (string)c["CategoryName"],
                                                                       CategoryHierarchy = (string)c["CategoryHierarchy"],
                                                                       SupplierId = (long)c["SupplierID"],
                                                                       SiteId = (long)c["SiteID"],
                                                                       StockItemId = (long)c["StockItemID"],
                                                                       CostPrice = (double)c["CostPrice"],
                                                                       StockItemSizes = (from size in c["StockItemSizes"]
                                                                                         select new StockItemSize
                                                                                         {
                                                                                                StockItemSizeId = (long)size["StockItemSizeID"],
                                                                                                StockItemId = (long)size["StockItemID"],
                                                                                                Size = (double)size["Size"], 
                                                                                                UnitOfMeasureCode = (string)size["UnitOfMeasureCode"],
                                                                                                // sis.UnitOfMeasureId = Integer.parseInt(stockItemSizes.getJSONObject(j).getString("Size"));
                                                                                                ConversionRatio = (double)size["ConversionRatio"],
                                                                                                CaseSizeDescription = (string)size["CaseDescriptionID"],
                                                                                                IsDefault =(bool)size["IsDefault"],
                                                                                                StockCount = (size["StockCount"].ToString() == string.Empty) ? null : (double?)size["StockCount"],
                                                                                         }
                                                                                             ).ToList<IStockItemSize>(),
                                                                       Count = (from count in c["StockItemSizes"]
                                                                                where count["StockCount"].ToString() != string.Empty
                                                                                select new StockCount
                                                                                {
                                                                                    StockItemSizeId = (long)count["StockItemSizeID"],
                                                                                    SiteItemId = (long)c["SiteItemID"],
                                                                                    CurrentCount = (count["StockCount"].ToString() == string.Empty) ? null : (double?)count["StockCount"],
                                                                                    Updated = false,
                                                                                    PreviousCount = null
                                                                                }
                                                                                                ).ToList<IStockCount>() 
                                                                   }).ToList<IStockCountItem>();

                return tempStockCountItem;

                /*
                 *          for (int i = 0; i < response.length(); i++) {

                        JSONObject jsonUsrPro = (JSONObject) response.get(i);

                        StockCountItem s = new StockCountItem();
                        s.SiteItemId = Integer.parseInt(jsonUsrPro.getString("SiteItemID"));
                        s.CategoryId = Integer.parseInt(jsonUsrPro.getString("CategoryID"));
                        s.ItemName = jsonUsrPro.getString("ItemName");
                        s.CategoryName = jsonUsrPro.getString("CategoryName");
                        s.CategoryHierarchy = jsonUsrPro.getString("CategoryHierarchy");
                        s.SupplierId = Integer.parseInt(jsonUsrPro.getString("SupplierID"));
                        s.SiteId = Integer.parseInt(jsonUsrPro.getString("SiteID"));
                        s.StockItemId = Integer.parseInt(jsonUsrPro.getString("StockItemID"));
                        s.CostPrice = Double.parseDouble(jsonUsrPro.getString("CostPrice"));
                        s.Count = new LinkedList<StockCount>();

                        resultStockCountItemArray.add(s);

                        JSONArray stockItemSizes = (JSONArray) jsonUsrPro.getJSONArray("StockItemSizes");

                        for(int j=0; j < stockItemSizes.length() ; j++)
                        {
                            int stockItemSizeId = Integer.parseInt(stockItemSizes.getJSONObject(j).getString("StockItemSizeID"));
                            boolean exist = false;

                            for(StockItemSize stkSize : resultStockItemSizeArray)
                            {
                                if(stkSize.StockItemSizeId == stockItemSizeId)
                                {
                                    exist= true;
                                    break;
                                }
                            }

                            if(!exist) {
                                StockItemSize sis = new StockItemSize();
                                sis.StockItemSizeId = stockItemSizeId ;
                                sis.StockItemId = Integer.parseInt(stockItemSizes.getJSONObject(j).getString("StockItemID")) ;
                                sis.Size = Double.parseDouble(stockItemSizes.getJSONObject(j).getString("Size")) ;
                                sis.UnitOfMeasureCode = stockItemSizes.getJSONObject(j).getString("UnitOfMeasureCode");
                                // sis.UnitOfMeasureId = Integer.parseInt(stockItemSizes.getJSONObject(j).getString("Size"));
                                sis.ConversionRatio = Double.parseDouble(stockItemSizes.getJSONObject(j).getString("ConversionRatio"));
                                sis.CaseSizeDescription = stockItemSizes.getJSONObject(j).getString("CaseDescriptionID").equals("null") ? null : stockItemSizes.getJSONObject(j).getString("CaseDescriptionID") ;
                                sis.IsDefault = stockItemSizes.getJSONObject(j).getString("IsDefault").equals("true") ? true : false ;

                                String currentCount = stockItemSizes.getJSONObject(j).getString("StockCount");

                                if(!currentCount.equals("null"))
                                {
                                    StockCount cnt = new StockCount();
                                    cnt.StockItemSizeId = sis.StockItemSizeId;
                                    cnt.SiteItemId = s.SiteItemId;
                                    cnt.CurrentCount = Double.parseDouble(currentCount);
                                    cnt.Updated = false;
                                    s.Count.add(cnt);
                                }

                                resultStockItemSizeArray.add(sis);
                            }
                        }
                    }

                    resultStockCountItemArray.get(0).StockItemSizes = resultStockItemSizeArray;

                    CallResetStockCountItemDBForSite(resultStockCountItemArray, mNotificationHelper, startId);

                 */
            }
            else
            {
                /*
                // Load JObject from stream
                JObject jObject = JObject.Load(reader);

                // Create target object based on JObject
                T target = Create(objectType);

                // Populate the object properties
                serializer.Populate(jObject.CreateReader(), target);

                return target;
                */

                return base.ReadJson(reader, objectType, existingValue, serializer);
            }
        }
    }
}