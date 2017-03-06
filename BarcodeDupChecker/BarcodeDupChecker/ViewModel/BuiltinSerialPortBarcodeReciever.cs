using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BarcodeDupChecker.ViewModel
{
    public class BuiltinSerialPortBarcodeReciever : ViewModelBase, IBarcodeReciever, IDisposable
    {
        SerialPort serialPort;

        public BuiltinSerialPortBarcodeReciever()
        {
            this.serialPort = new SerialPort("COM?", 9600);
            //this.serialPort.NewLine = "\r\n";
            this.serialPort.ReceivedBytesThreshold = 13;

            this.serialPort.DataReceived += SerialPort_DataReceived;
        }

        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            string barcode = this.serialPort.ReadExisting().Replace("\r","").Replace("\n", "");
            //string barcode = this.serialPort.();
            if (this.BarcodeRecieved != null)
            {
                this.BarcodeRecieved(this, new BarcodeEventArgs(barcode));
            }
        }

        public string PortName
        {
            get
            {
                return this.serialPort.PortName;
            }
            set
            {
                this.serialPort.PortName = value;
            }
        }

        public bool Online
        {
            get
            {
                return this.serialPort.IsOpen;
            }
        }

        public int BarcodeLength
        {
            get
            {
                return this.serialPort.ReceivedBytesThreshold;
            }

            set
            {
                this.serialPort.ReceivedBytesThreshold = value;
            }
        }

        public void Start()
        {
            try
            {
                this.serialPort.Open();
                Log.Instance.Logger.InfoFormat("open {0} success", this.PortName);
                MessengerInstance.Send<string>(string.Format("成功打开串口{0}！", this.PortName));
            }
            catch (Exception ex)
            {
                Log.Instance.Logger.FatalFormat(ex.Message);
                MessengerInstance.Send<string>(ex.Message);
            }
        }
        public void Close()
        {
            try
            {
                this.serialPort.Close();
                Log.Instance.Logger.InfoFormat("close {0} success", this.PortName);
                MessengerInstance.Send<string>(string.Format("成功关闭串口{0}！", this.PortName));
            }
            catch (Exception ex)
            {
                Log.Instance.Logger.FatalFormat(ex.Message);
                MessengerInstance.Send<string>(ex.Message);
            }
        }



        public event EventHandler<BarcodeEventArgs> BarcodeRecieved;

        bool disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;
            if (disposing)
            {
                this.serialPort.DataReceived -= SerialPort_DataReceived;
                this.serialPort.Dispose();
            }
            disposed = true;
        }
        public void Dispose()
        {

            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}

