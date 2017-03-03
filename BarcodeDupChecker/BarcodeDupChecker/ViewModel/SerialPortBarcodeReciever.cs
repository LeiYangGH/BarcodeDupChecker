using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarcodeDupChecker.ViewModel
{
    public class SerialPortBarcodeReciever : ViewModelBase, IBarcodeReciever, IDisposable
    {
        PInvokeSerialPort.SerialPort serialPort;

        public SerialPortBarcodeReciever()
        {
            this.serialPort = new PInvokeSerialPort.SerialPort("COM?",9600);
            this.serialPort.DataReceived += SerialPort_DataReceived;
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
                return this.serialPort.Online;
            }
        }

        static StringBuilder sb = new StringBuilder();
        private void SerialPort_DataReceived(byte b)
        {
            if (b == 13 || b == 10 || b == 122)
            {
                if (this.BarcodeRecieved != null)
                {
                    this.BarcodeRecieved(this, new BarcodeEventArgs(sb.ToString()));
                    sb.Clear();
                }
            }
            else
            {
                sb.Append((char)b);
                MessengerInstance.Send<string>(sb.ToString());
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

        public event EventHandler<BarcodeEventArgs> BarcodeRecieved;

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.serialPort.Dispose();
            }
        }
        public void Dispose()
        {

            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}

