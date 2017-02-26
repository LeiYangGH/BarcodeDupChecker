using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarcodeDupChecker
{
    public interface IBarcodeReciever
    {
        event EventHandler<string> BarcodeRecieved;


    }
}
