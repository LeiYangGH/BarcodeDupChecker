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
        Timer t = new Timer(500);
        Random r = new Random();
        int lstCount;
        private List<string> lstBarcodes = new List<string>
        {
            "B111111111111",
            "B222222222222",
            "B333333333333",
            "B444444444444",
            "B555555555555",
            "B666666666666",
            "B777777777777",
            "B888888888888"
        };
        public TimerBarcodeReciever()
        {
            this.lstCount = this.lstBarcodes.Count;
            t.Elapsed += T_Elapsed;
        }

        private void T_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (this.BarcodeRecieved != null)
            {
                string barcode = this.lstBarcodes[r.Next(0, lstCount - 1)];
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
