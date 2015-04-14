using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CrossGraphics;
using DomainInterface;
using Xamarin.Forms;
using Zumero.DataGrid.Core;
using Zumero.DataGrid.xGraphics;
using Zumero.DataGrid.XF;
using SQLitePCL;
using SQLitePCL.Ugly;
using Zumero.DataGrid.SQLite;

namespace FourthFnB
{
    public class SQLiteGrid : Zumero.DataGrid.XF.DataGridBase
    {
        private sqlite3 conn;
        private sqlite3_stmt statement;
        public void CloseConnection()
        {
            try
            { this.statement.Dispose();
                this.conn.Dispose();
               
                GC.Collect();

            }
            catch (Exception ex)
            {
                string err = ex.Message;
            }
        }

        public SQLiteGrid(IDatabase db, long siteId)
        {
            /*
             * sqlite3 conn = ugly.open(":memory:");
            conn.exec("BEGIN TRANSACTION");
            conn.exec("CREATE TABLE foo (a int, b int, c int);");
            for (int i = 0; i < 100; i++)
            {
                conn.exec("INSERT INTO foo (a,b,c) VALUES (?,?,?)", i, i * 5 - 3, i * i / 10);
            }
            conn.exec("COMMIT TRANSACTION");
             * 
             *  var stmt = conn.prepare("SELECT * FROM foo");
            */

            conn = ugly.open(db.DatabasePath);

            statement = conn.prepare("SELECT A.ItemName, B.Size, B.UnitOfMeasureCode,C.CurrentCount, C.PreviousCount, A.SiteItemId FROM StockCountItem A INNER JOIN StockItemSize B ON A.StockItemID = B.StockItemID LEFT OUTER JOIN StockCount C on A.SiteItemID = C.SiteItemID AND B.StockItemSizeID = C.StockItemSizeID  WHERE A.SiteId = " + siteId + " ORDER BY A.ItemName ASC");

            var colinfo = new Dimension_Columns_SQLite(statement);
            var rowinfo = new Dimension_Rows_SQLite();

            var mytextfmt = new MyTextFormat
            {
                TextFont = this.Font.ToCrossFont(),
                TextColor = CrossGraphics.Colors.Black,
                HorizontalTextAlignment = CrossGraphics.TextAlignment.Center,
                VerticalTextAlignment = CrossGraphics.TextAlignment.Center,
            };

            var fmt = new ValuePerCell_Steady<MyTextFormat>(
                mytextfmt
            );
            PropertyChanged += (object sender, System.ComponentModel.PropertyChangedEventArgs e) =>
            {
                if (e.PropertyName == A1Grid.FontProperty.PropertyName)
                {
                    mytextfmt.TextFont = Font.ToCrossFont();
                    fmt.notify_changed(-1, -1);
                }
            };
            var padding1 = new ValuePerCell_Steady<Padding?>(new Padding(1));
            var padding4 = new ValuePerCell_Steady<Padding?>(new Padding(4));
            var fill_white = new ValuePerCell_Steady<CrossGraphics.Color>(CrossGraphics.Colors.White);

            var rowlist = new RowList_SQLite_StringArray(statement);
            var rowlist_cached = new RowList_Cache<string[]>(rowlist);
            var vpc = new ValuePerCell_RowList_Indexed<string, string[]>(rowlist_cached);
            IDrawCell<IGraphics> dec = new DrawCell_Text(vpc, fmt);

            dec = new DrawCell_Chain_Padding(padding4, dec);
            dec = new DrawCell_Fill(fill_white, dec);
            dec = new DrawCell_Chain_Padding(padding1, dec);
            dec = new DrawCell_Chain_Cache(dec, colinfo, rowinfo);

            var sel = new Selection();

            var dec_selection = new DrawCell_FillRectIfSelected(sel, new CrossGraphics.Color(0, 255, 0, 120));

            var dh_layers = new DrawVisible_Layers(new IDrawVisible<IGraphics>[] {
				new DrawVisible_Adapter_DrawCell<IGraphics>(dec),
				new DrawVisible_Adapter_DrawCell<IGraphics>(dec_selection)
			});

            Main = new MainPanel(
                colinfo,
                rowinfo,
                dh_layers
            );
        }

        // --------------------------------
        // Font

        public static readonly BindableProperty FontProperty =
            BindableProperty.Create<A1Grid, Nullable<Xamarin.Forms.Font>>(
                p => p.Font, Xamarin.Forms.Font.SystemFontOfSize(18));

        public Xamarin.Forms.Font Font
        {
            get { return (Xamarin.Forms.Font)GetValue(FontProperty); }
            set { SetValue(FontProperty, value); }
        }
    }

    internal static class myutil
    {
        public static string GetExcelColumnName(int columnNumber)
        {
            int dividend = columnNumber;
            string columnName = String.Empty;
            int modulo;

            while (dividend > 0)
            {
                modulo = (dividend - 1) % 26;
                columnName = Convert.ToChar(65 + modulo).ToString() + columnName;
                dividend = (int)((dividend - modulo) / 26);
            }

            return columnName;
        }

        private static bool gv(int col, int row, out string val)
        {
            val = myutil.GetExcelColumnName(col + 1) + (row + 1).ToString();
            return true;
        }

        public static GetCellValueDelegate<string> get_delegate()
        {
            return new GetCellValueDelegate<string>(gv);
        }
    }

    public class A1Grid : Zumero.DataGrid.XF.DataGridBase
    {
        public A1Grid()
        {
            var colinfo = new RowColumnInfo_Steady_BindableProp(this, ColumnsProperty, ColumnWidthProperty);
            var rowinfo = new RowColumnInfo_Steady_BindableProp(this, RowsProperty, RowHeightProperty);

            var mytextfmt = new MyTextFormat
            {
                TextFont = this.Font.ToCrossFont(),
                TextColor = CrossGraphics.Colors.Black,
                HorizontalTextAlignment = CrossGraphics.TextAlignment.Center,
                VerticalTextAlignment = CrossGraphics.TextAlignment.Center,
            };

            var fmt = new ValuePerCell_Steady<MyTextFormat>(
                mytextfmt
                );
            PropertyChanged += (object sender, System.ComponentModel.PropertyChangedEventArgs e) =>
            {
                if (e.PropertyName == A1Grid.FontProperty.PropertyName)
                {
                    mytextfmt.TextFont = Font.ToCrossFont();
                    fmt.notify_changed(-1, -1);
                }
            };
            IDrawCell<IGraphics> dec = new DrawCell_Text(
                new ValuePerCell_FromDelegates<string>(myutil.get_delegate()),
                fmt
            );

            var padding1 = new ValuePerCell_Steady<Padding?>(new Padding(1));
            var padding4 = new ValuePerCell_Steady<Padding?>(new Padding(4));
            var fill_white = new ValuePerCell_Steady<CrossGraphics.Color>(CrossGraphics.Colors.White);

            dec = new DrawCell_Chain_Padding(padding4, dec);
            dec = new DrawCell_Fill(fill_white, dec);
            dec = new DrawCell_Chain_Padding(padding1, dec); // TODO probably useless
            //dec = new DrawCell_Chain_Cache (dec, colinfo, rowinfo);

#if not
			var sel = new Selection ();

			var dec_selection = new DrawCell_FillRectIfSelected (sel, Color.FromRgba (0, 255, 0, 120));

			var dh_layers = new DrawVisible_Layers (new IDrawVisible<IGraphics>[] {
				new DrawVisible_Cache(new DrawVisible_Adapter_DrawCell<IGraphics>(dec), colinfo, rowinfo),
				//new DrawVisible_Adapter_DrawCell<IGraphics>(dec_selection)
			});
#endif

            Main = new MainPanel(
                colinfo,
                rowinfo,
                new DrawVisible_Cache(
                new DrawVisible_Adapter_DrawCell<IGraphics>(dec)
                , colinfo, rowinfo)
            );
            //Notify_DemoToggleSelections.Listen (Main, sel);

#if not
			var fill_gray = new ValuePerCell_Steady<Color?> (Color.Gray);
			var frozen_textfmt = new ValuePerCell_Steady<MyTextFormat> (
				new MyTextFormat {
					TextFont = Font.SystemFontOfSize (18, FontAttributes.Bold),
					TextColor = Color.Black,
					HorizontalTextAlignment = TextAlignment.Center,
					VerticalTextAlignment = TextAlignment.Center,
				});

			Left = new FrozenColumnsPanel(
				new Dimension_Steady(1, 80, false),
				rowinfo,
				new DrawVisible_Adapter_DrawCell<IGraphics>(
					new DrawCell_Chain_Padding(
						padding1,
						new DrawCell_Fill(
							fill_gray,
							new DrawCell_Text(
								new ValuePerCell_RowNumber (), 
								frozen_textfmt
							)
						)
					)
				)
			);
			Left.SingleTap += (object sender, CellCoords e) => {
				// hack for testing purposes
				this.Font = Font.SystemFontOfSize(this.Font.FontSize + 1);
			};

			Top = new FrozenRowsPanel (
				colinfo,
				new Dimension_Steady(1, 40, false),
				new DrawVisible_Adapter_DrawCell<IGraphics>(
					new DrawCell_Chain_Padding(
						padding1,
						new DrawCell_Fill(
							fill_gray,
							new DrawCell_Text(
								new ValuePerCell_ColumnLetters (), 
								frozen_textfmt
							)
						)
					)
				)
			);
			Top.SingleTap += (object sender, CellCoords e) => {
				// hack for testing purposes
				this.RowHeight *= 1.1;
			};
#endif
        }

        // --------------------------------
        // Rows

        public static readonly BindableProperty RowsProperty =
            BindableProperty.Create<A1Grid, int?>(
                p => p.Rows, null);

        public int? Rows
        {
            get { return (int?)GetValue(RowsProperty); }
            set { SetValue(RowsProperty, value); } // TODO disallow invalid values
        }

        // --------------------------------
        // Columns

        public static readonly BindableProperty ColumnsProperty =
            BindableProperty.Create<A1Grid, int?>(
                p => p.Columns, null);

        public int? Columns
        {
            get { return (int?)GetValue(ColumnsProperty); }
            set { SetValue(ColumnsProperty, value); } // TODO disallow invalid values
        }

        // --------------------------------
        // ColumnWidth

        public static readonly BindableProperty ColumnWidthProperty =
            BindableProperty.Create<A1Grid, double>(
                p => p.ColumnWidth, 80);

        public double ColumnWidth
        {
            get { return (double)GetValue(ColumnWidthProperty); }
            set { SetValue(ColumnWidthProperty, value); } // TODO disallow invalid values
        }

        // --------------------------------
        // RowHeight

        public static readonly BindableProperty RowHeightProperty =
            BindableProperty.Create<A1Grid, double>(
                p => p.RowHeight, 40);

        public double RowHeight
        {
            get { return (double)GetValue(RowHeightProperty); }
            set { SetValue(RowHeightProperty, value); } // TODO disallow invalid values
        }

        // --------------------------------
        // Font

        public static readonly BindableProperty FontProperty =
            BindableProperty.Create<A1Grid, Nullable<Xamarin.Forms.Font>>(
                p => p.Font, Xamarin.Forms.Font.SystemFontOfSize(18));

        public Xamarin.Forms.Font Font
        {
            get { return (Xamarin.Forms.Font)GetValue(FontProperty); }
            set { SetValue(FontProperty, value); }
        }

    }
}
