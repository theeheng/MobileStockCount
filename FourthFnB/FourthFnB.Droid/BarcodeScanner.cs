using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DomainConstant;
using DomainInterface;
using IoC;
using Xamarin.Forms;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

[assembly: Dependency(typeof(FourthFnB.Droid.BarcodeScanner))]

namespace FourthFnB.Droid
{
    class BarcodeScanner : IBarcodeScanner
    {
        public async void ScanBarcode()
        {
            Context c = Forms.Context;

            var scanner = new ZXing.Mobile.MobileBarcodeScanner(c);

            var result = await scanner.Scan();

            IBarcodeResult br = IoCContainer.Container.Resolve(typeof(IBarcodeResult), null) as IBarcodeResult;

            if (result != null)
            {
                Console.WriteLine("Scanned Barcode: " + result.Text);
            
                br.Text = result.Text;
                br.Format = (BarcodeFormat)Enum.Parse(typeof(BarcodeFormat), result.BarcodeFormat.ToString());
            }

            MessagingCenter.Send<IBarcodeScanner,IBarcodeResult>(this, "Barcode",  br);

            /*var activity = Forms.Context as Activity;

            var picker = new MediaPicker(activity);
            var intent = picker.GetTakePhotoUI(new StoreCameraMediaOptions
            {
                Name = "test.jpg",
                Directory = "Pluralsight"
            });
            activity.StartActivityForResult(intent, 1);
            */
        }


        /*
         * protected override async void OnActivityResult(int requestCode, Result resultCode, 
            Android.Content.Intent data)
        {
            if (resultCode == Result.Canceled)
                return;

            var mediaFile = await data.GetMediaFileExtraAsync(Forms.Context);
            System.Diagnostics.Debug.WriteLine(mediaFile.Path);

            MessagingCenter.Send<IBarcodeScanner, string>(this, "Barcode", mediaFile.Path);

        }
         */
    }
}