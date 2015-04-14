using System;
using System.Globalization;

namespace FourthFnB.Translation
{
    public interface ILocalize
    {
        CultureInfo GetCurrentCultureInfo();

        void SetLocale();
    }
}

