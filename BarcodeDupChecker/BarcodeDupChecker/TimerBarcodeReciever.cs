using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace BarcodeDupChecker
{
    public class TimerBarcodeReciever : IBarcodeReciever
    {
        const int interval = 300;
        Timer t = new Timer(interval);
        Random r = new Random();
        private List<string> lstBarcodes = new List<string>();
        public TimerBarcodeReciever()
        {
            t.Elapsed += T_Elapsed;
        }

        private void T_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (this.BarcodeRecieved != null)
            {
                string barcode = "BBBBBBB" + r.Next(0, 10000).ToString().PadLeft(5, '0');
                this.BarcodeRecieved(this, barcode);
            }
        }

        public void Close()
        {
            this.t.Close();
        }

        public void Start()
        {
            t.Start();
        }

        public event EventHandler<string> BarcodeRecieved;
    }
}
