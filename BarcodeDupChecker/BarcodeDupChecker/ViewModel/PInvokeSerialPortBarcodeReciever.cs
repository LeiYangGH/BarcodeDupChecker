using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;

namespace BarcodeDupChecker.ViewModel
{
    public class PInvokeSerialPortBarcodeReciever : ViewModelBase, IBarcodeReciever, IDisposable
    {
        PInvokeSerialPort.SerialPort serialPort;

        public PInvokeSerialPortBarcodeReciever()
        {
            this.serialPort = new PInvokeSerialPort.SerialPort("COM?", 9600);
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

        public int BarcodeLength
        {
            get; set;
        }

        private StringBuilder sb = new StringBuilder();
        private void SerialPort_DataReceived(byte b)
        {
            //if (b == 13 || b == 10 || b == 122)
            if (b == 13)
            {
                if (this.BarcodeRecieved != null)
                {
                    this.BarcodeRecieved(this, new BarcodeEventArgs(this.sb.ToString()));
                    this.sb.Clear();
                }
            }
            else
            {
                this.sb.Append((char)b);
                MessengerInstance.Send<string>(this.sb.ToString());
            }
        }


        public void Start()
        {
            try
            {
                if (this.serialPort.Open())
                {
                    Log.Instance.Logger.InfoFormat("open {0} success", this.PortName);
                    MessengerInstance.Send<string>(string.Format("成功打开串口{0}！", this.PortName));
                }
                else
                {
                    MessengerInstance.Send<string>(string.Format("打开串口{0}失败！", this.PortName));
                }
            }
            catch (Exception ex)
            {
                Log.Instance.Logger.FatalFormat(ex.Message);
                MessengerInstance.Send<string>(ex.Message);
            }
        }

        [HandleProcessCorruptedStateExceptions]
        public void Close()
        {
            try
            {
                this.serialPort.DataReceived -= SerialPort_DataReceived;
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

