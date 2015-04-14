using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DomainConstant;
using DomainInterface;
using IoC;
using Xamarin.Forms;
using Foundation;
using UIKit;

[assembly: Dependency(typeof(FourthFnB.iOS.BarcodeScanner))]
namespace FourthFnB.iOS
{
    class BarcodeScanner : IBarcodeScanner
    {
        public async void ScanBarcode()
        {
            /*var picker = new MediaPicker();

            var mediaFile = await picker.PickPhotoAsync();
            System.Diagnostics.Debug.WriteLine(mediaFile.Path);

            MessagingCenter.Send<IBarcodeScanner, string>(this, "Barcode", mediaFile.Path);
            */

            var scanner = new ZXing.Mobile.MobileBarcodeScanner();
            var result = await scanner.Scan();

            //var result = new { Text = "BETBB61229", BarcodeFormat = "CODE_39" };

            IBarcodeResult br = IoCContainer.Container.Resolve(typeof(IBarcodeResult), null) as IBarcodeResult;

            if (result != null)
            {
                Console.WriteLine("Scanned Barcode: " + result.Text);

                br.Text = result.Text;
                br.Format = (BarcodeFormat) Enum.Parse(typeof (BarcodeFormat), result.BarcodeFormat.ToString());
            }

            MessagingCenter.Send<IBarcodeScanner, IBarcodeResult>(this, "Barcode", br);
        }
    }
}