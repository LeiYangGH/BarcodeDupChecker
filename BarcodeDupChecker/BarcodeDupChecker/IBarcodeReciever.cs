using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarcodeDupChecker
{
    public interface IBarcodeReciever : IDisposable
    {
        event EventHandler<BarcodeEventArgs> BarcodeRecieved;
        string PortName { get; set; }
        bool Online { get; }
        void Start();
        void Close();
    }


}
