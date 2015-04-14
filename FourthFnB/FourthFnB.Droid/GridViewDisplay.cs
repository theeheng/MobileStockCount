using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using DSoft.Datatypes.Grid.Data;
using DSoft.UI.Grid;
using Xamarin.Forms;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

[assembly: Dependency(typeof(FourthFnB.Droid.GridViewDisplay))]

namespace FourthFnB.Droid
{
    public class GridViewDisplay : IGridViewDisplay
    {
        public void DisplayStockCountGrid()
        {
            //create the data table object and set a name
            var aDataSource = new DSDataTable("ADT");

            //add a column
            var dc1 = new DSDataColumn("Title");
            dc1.Caption = "Title";
            dc1.ReadOnly = true;
            dc1.DataType = typeof(String);
            dc1.AllowSort = true;
            dc1.Width = 20;

            aDataSource.Columns.Add(dc1);

            //add a row to the data table
            var dr = new DSDataRow();
            dr["ID"] = 123;
            dr["Title"] = @"Test";
            dr["Description"] = @"Some description would go here";
            dr["Date"] = DateTime.Now.ToShortDateString();
            dr["Value"] = "10000.00";       
            aDataSource.Rows.Add(dr); 


            var aGridView = new DSGridView(Forms.Context);
            aGridView.DataSource = aDataSource;
            aGridView.Visibility = Android.Views.ViewStates.Visible;
            aGridView.SetMinimumWidth(200);
            aGridView.SetMinimumHeight(200);

            Activity activity = Forms.Context as Activity;

            activity.AddContentView(aGridView, new ViewGroup.LayoutParams(
            ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent));

        }
    }
}