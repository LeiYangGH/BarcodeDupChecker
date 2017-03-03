using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarcodeDupChecker
{
    public class BarcodeEventArgs : EventArgs
    {
        public BarcodeEventArgs(string barcode)
        {
            this.Barcode = barcode;
        }

        public string Barcode { get; set; }
    }
}
