using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CrossGraphics;
using Xamarin.Forms;
using Zumero.DataGrid.Core;
using Zumero.DataGrid.XF;
using Zumero.DataGrid.xGraphics;

namespace FourthFnB
{
    public class ColumnishGrid<T> : Zumero.DataGrid.XF.DataGridBase where T : class
    {
        public class ColumnInfo // TODO INotify...
        {
            public string Title = "Untitled";
            public double Width = 100;
            public string PropertyName = null;
            public Xamarin.Forms.Font TextFont = Xamarin.Forms.Font.SystemFontOfSize(16);
            public Xamarin.Forms.Color TextColor = Xamarin.Forms.Color.Black;
            public Xamarin.Forms.Color FillColor = Xamarin.Forms.Color.White;
            public Xamarin.Forms.TextAlignment HorizontalAlignment = Xamarin.Forms.TextAlignment.Center;
        }

        private class myRowInfo : IDimension
        {
            private ColumnishGrid<T> _top;

            public myRowInfo(ColumnishGrid<T> top)
            {
                _top = top;
                // TODO listen for change number of rows
                _top.PropertyChanged += (object sender, System.ComponentModel.PropertyChangedEventArgs e) =>
                {
                    if (e.PropertyName == ColumnishGrid<T>.RowHeightProperty.PropertyName)
                    {
                        if (changed != null)
                        {
                            changed(this, -1);
                        }
                    }
                };
            }

            public bool variable_sizes
            {
                get
                {
                    return false;
                }
            }

            public bool wraparound
            {
                get
                {
                    return false;
                }
            }

            public double func_size(int n)
            {
                return _top.RowHeight;
            }

            public int? number
            {
                get
                {
                    if (_top.Rows == null)
                    {
                        return 0;
                    }

                    return _top.Rows.Count;
                }
            }

            public event EventHandler<int> changed;
        }

        private class myColumnInfo : IDimension
        {
            private ColumnishGrid<T> _top;

            public myColumnInfo(ColumnishGrid<T> top)
            {
                _top = top;
                // TODO hookup both
            }

            public bool variable_sizes
            {
                get
                {
                    return true;
                }
            }

            public bool wraparound
            {
                get
                {
                    return false;
                }
            }

            public double func_size(int n)
            {
                return _top.Columns[n].Width;
            }

            public int? number
            {
                get
                {
                    if (_top.Columns == null)
                    {
                        return 0;
                    }
                    return _top.Columns.Count;
                }
            }

            public event EventHandler<int> changed;
        }

        private class myFrozenRowInfo : IDimension
        {
            private ColumnishGrid<T> _top;

            public myFrozenRowInfo(ColumnishGrid<T> top)
            {
                _top = top;
                // TODO hookup
            }

            public bool variable_sizes
            {
                get
                {
                    return false;
                }
            }

            public bool wraparound
            {
                get
                {
                    return false;
                }
            }

            public int? number
            {
                get
                {
                    return 1;
                }
            }

            public double func_size(int n)
            {
                // TODO might want a separate HeaderHeight
                return _top.RowHeight;
            }

            public event EventHandler<int> changed;
        }

        private class myFrozenGetCellTextValue : IValuePerCell<string>
        {
            private ColumnishGrid<T> _top;

            public myFrozenGetCellTextValue(ColumnishGrid<T> top)
            {
                _top = top;
                // TODO hookup
            }

            public void func_begin_update(CellRange viz)
            {

            }

            public void func_end_update()
            {

            }

            public bool get_value(int col, int row, out string val)
            {
                val = _top.Columns[col].Title;
                return true;
            }

            public event EventHandler<CellCoords> changed;
        }

        private bool gv_fmt(int col, int row, out MyTextFormat v)
        {
            v = new MyTextFormat
            {
                TextFont = Columns[col].TextFont.ToCrossFont(),
                TextColor = Columns[col].TextColor.ToCrossColor(),
                HorizontalTextAlignment = Columns[col].HorizontalAlignment.ToCrossTextAlignment(),
                VerticalTextAlignment = CrossGraphics.TextAlignment.Center,
            };
            return true;
        }

        private bool gv_clr(int col, int row, out CrossGraphics.Color v)
        {
            v = Columns[col].FillColor.ToCrossColor();
            return true;
        }

        public ColumnishGrid()
        {
            var colinfo = new myColumnInfo(this);
            var rowinfo = new myRowInfo(this);

            var fmt = new OneValueForEachColumn<MyTextFormat>(new ValuePerCell_FromDelegates<MyTextFormat>(gv_fmt));

            IRowList<T> rowlist = new RowList_Bindable_IList<T>(this, RowsProperty);

            // TODO it would be better if these propnames were stored separately
            // from the formatting info.
            var propnames = new Dictionary<int, string>();
            this.PropertyChanged += (object sender, System.ComponentModel.PropertyChangedEventArgs e) =>
            {
                if (e.PropertyName == ColumnsProperty.PropertyName)
                {
                    propnames.Clear();
                    for (int i = 0; i < Columns.Count; i++)
                    {
                        propnames[i] = Columns[i].PropertyName;
                    }
                }
            };

            IValuePerCell<string> vals = new ValuePerCell_RowList_Properties<string, T>(rowlist, propnames);

            IDrawCell<IGraphics> dec = new DrawCell_Text(vals, fmt);

            var padding1 = new ValuePerCell_Steady<Padding?>(new Padding(1, 1, 1, 1));
            var padding8 = new ValuePerCell_Steady<Padding?>(new Padding(8, 8, 8, 8));
            IValuePerCell<CrossGraphics.Color> bginfo = new ValuePerCell_FromDelegates<CrossGraphics.Color>(gv_clr);
            bginfo = new OneValueForEachColumn<CrossGraphics.Color>(bginfo);

            dec = new DrawCell_Chain_Padding(padding8, dec);
            dec = new DrawCell_Fill(bginfo, dec);
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
            Notify_DemoToggleSelections.Listen(Main, sel);
#if not
			// TODO the mod happens, but the notification does not
			_main.SingleTap += (object sender, CellCoords e) => {
			T r = Rows [e.Row];
			ColumnInfo ci = Columns[e.Column];
			var typ = typeof(T);
			var ti = typ.GetTypeInfo();
			var p = ti.GetDeclaredProperty(ci.PropertyName);
			if (p != null)
			{
			var val = p.GetValue(r);
			p.SetValue(r, val.ToString() + "*");
			}
			};
#endif

            var bginfo_gray = new ValuePerCell_Steady<CrossGraphics.Color>(CrossGraphics.Colors.Gray);
            Top = new FrozenRowsPanel(
                colinfo,
                new Dimension_Steady(1, 40, false),
                new DrawVisible_Adapter_DrawCell<IGraphics>(
                    new DrawCell_Chain_Padding(
                        padding1,
                        new DrawCell_Fill(
                            bginfo_gray,
                            new DrawCell_Text(
                                new myFrozenGetCellTextValue(this),
                                new ValuePerCell_Steady<MyTextFormat>(
                                    new MyTextFormat
                                    {
                                        TextFont = CrossGraphics.Font.BoldSystemFontOfSize(20),
                                        TextColor = CrossGraphics.Colors.Red,
                                        HorizontalTextAlignment = CrossGraphics.TextAlignment.Center,
                                        VerticalTextAlignment = CrossGraphics.TextAlignment.Center
                                    }
                                )
                            )
                        )
                    )
                )
            );

        }

        // --------------------------------
        // Rows

        public static readonly BindableProperty RowsProperty =
            BindableProperty.Create<ColumnishGrid<T>, IList<T>>(
                p => p.Rows, null);

        public IList<T> Rows
        {
            get { return (IList<T>)GetValue(RowsProperty); }
            set { SetValue(RowsProperty, value); } // TODO disallow invalid values
        }

        // --------------------------------
        // Columns

        public static readonly BindableProperty ColumnsProperty =
            BindableProperty.Create<ColumnishGrid<T>, IList<ColumnInfo>>(
                p => p.Columns, null);

        public IList<ColumnInfo> Columns
        {
            get { return (IList<ColumnInfo>)GetValue(ColumnsProperty); }
            set { SetValue(ColumnsProperty, value); } // TODO disallow invalid values
        }

        // --------------------------------
        // RowHeight

        public static readonly BindableProperty RowHeightProperty =
            BindableProperty.Create<ColumnishGrid<T>, double>(
                p => p.RowHeight, 40);

        public double RowHeight
        {
            get { return (double)GetValue(RowHeightProperty); }
            set { SetValue(RowHeightProperty, value); } // TODO disallow invalid values
        }

    }
}
