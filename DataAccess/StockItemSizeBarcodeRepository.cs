using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainInterface;
using DomainObject;

namespace DataAccess
{
    public class StockItemSizeBarcodeRepository : IStockItemSizeBarcodeRepository
    {
        public IDatabase db { get; set; }
        static object locker = new object();

        public IEnumerable<IStockItemSizeBarcode> GetStockItemSizeBarcodes()
        {
            lock (locker)
            {
                return (from i in db.Table<StockItemSizeBarcode>() select i).ToList();
            }
        }

        public IStockItemSizeBarcode GetStockItemSizeBarcode(string barcodeContent, string barcodeFormat, int stockItemSizeId)
        {
            lock (locker)
            {
                return db.Table<StockItemSizeBarcode>().FirstOrDefault(x => x.BarcodeContent == barcodeContent && x.BarcodeFormat == barcodeFormat && x.StockItemSizeId == stockItemSizeId);
            }
        }

        public int InsertStockItemSizeBarcode(IStockItemSizeBarcode stockItemSizeBarcode)
        {
            lock (locker)
            {
                return db.Insert(stockItemSizeBarcode);
            }
        }

        public int InsertStockItemSizeBarcodes(IEnumerable<IStockItemSizeBarcode> stockItemSizeBarcodeList)
        {
            lock (locker)
            {
                return db.InsertAll(stockItemSizeBarcodeList);
            }
        }

        public int DeleteStockItemSizeBarcode(IStockItemSizeBarcode stockItemSizeBarcode)
        {
            lock (locker)
            {
                return db.Delete<StockCountItem>(stockItemSizeBarcode);
            }
        }

        public int ResetStockItemSizeBarcode()
        {
            lock (locker)
            {
                return db.Execute(@"DELETE FROM StockItemSizeBarcode");
            }
        }
    }
}
