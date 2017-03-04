using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace BarcodeDupChecker
{
    public class TimerBarcodeReciever : IBarcodeReciever, IDisposable
    {
        const int interval = 300;
        Timer t = new Timer(interval);
        int randUBound = (int)Math.Floor((2000d / (double)interval));
        Random r = new Random();
        private List<string> lstBarcodes = new List<string>();

        public string PortName
        {
            get
            {
                return "Timer";
            }

            set
            {

            }
        }

        public bool Online
        {
            get
            {
                return this.t.Enabled;
            }
        }

        public TimerBarcodeReciever()
        {
            t.Elapsed += T_Elapsed;
        }

        private void T_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (this.BarcodeRecieved != null)
            {
                string barcode = "BBBBBBB" + r.Next(0, randUBound).ToString().PadLeft(5, '0');
                this.BarcodeRecieved(this, new BarcodeEventArgs(barcode));
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

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.t.Dispose();
            }
        }
        public void Dispose()
        {

            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public event EventHandler<BarcodeEventArgs> BarcodeRecieved;
    }
}
