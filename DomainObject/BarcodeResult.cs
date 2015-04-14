using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainInterface;
using DomainConstant;

namespace DomainObject
{
    public class BarcodeResult : IBarcodeResult
    {
        public string Text { get; set; }
        public BarcodeFormat Format { get; set; }
    }

    
}
