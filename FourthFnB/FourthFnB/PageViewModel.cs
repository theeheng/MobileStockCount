using System;
using System.ComponentModel;
using Xamarin.Forms;

namespace FourthFnB
{
    public class PageViewModel : INotifyPropertyChanged
    {
        private bool isBusy;

        public event PropertyChangedEventHandler PropertyChanged;

        public PageViewModel()
        {
            this.IsBusy = false;
        }

        public bool IsBusy
        {
            set
            {
                if (isBusy != value)
                {
                    isBusy = value;

                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this,
                            new PropertyChangedEventArgs("IsBusy"));
                    }
                }
            }
            get { return isBusy; }
        }
    }
}