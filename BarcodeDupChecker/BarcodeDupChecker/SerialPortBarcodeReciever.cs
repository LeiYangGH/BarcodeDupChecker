using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BarcodeDupChecker.Properties;
namespace BarcodeDupChecker
{
    public class SerialPortBarcodeReciever : IBarcodeReciever
    {
        int barcodeLength = 12;
        PInvokeSerialPort.SerialPort serialPort;// = new PInvokeSerialPort.SerialPort();

        public SerialPortBarcodeReciever()
        {
            this.serialPort = new PInvokeSerialPort.SerialPort(this.GetFirstPortName());
            this.SetSerialPortParameters("");
            this.serialPort.DataReceived += SerialPort_DataReceived;
        }



        public bool IsSerailPortOpen
        {
            get
            {
                return this.serialPort.Online;//??
            }
        }

        public string GetFirstPortName()
        {
            string[] names = SerialPort.GetPortNames();
            if (names.Length > 0)
            {
                string setPort = Settings.Default.PortName;

                if (!string.IsNullOrWhiteSpace(setPort) && names.Contains(setPort))
                    return setPort;
                else
                    return names[0];
            }
            else
                return "COM?";
        }

        public void SetSerialPortParameters(string portName)
        {
            //if (string.IsNullOrWhiteSpace(portName))
            //{
            //    portName = this.GetFirstPortName();
            //}
            Log.Instance.Logger.InfoFormat("portName={0}", portName);

            this.serialPort.PortName = portName;
            serialPort.BaudRate = 9600;
            //serialPort.Parity = PInvokeSerialPort.Parity.None;
            //serialPort.StopBits = PInvokeSerialPort.StopBits.One;
            //serialPort.DataBits = 8;
            //serialPort.Handshake = PInvokeSerialPort.Handshake.None;
            //serialPort.RtsEnable = true;
        }

        public void SetBarcodeLength(int length)
        {
            this.barcodeLength = length;
        }

        static StringBuilder sb = new StringBuilder();
        private void SerialPort_DataReceived(byte b)
        {
            if (b == 13)
            {
                if (this.BarcodeRecieved != null)
                {
                    this.BarcodeRecieved(this, sb.ToString());
                    sb.Clear();
                }
                Console.WriteLine();

            }
            else
            {
                sb.Append((char)b);
            }
        }


        public void Close()
        {
            this.serialPort.Close();
        }

        public void Start()
        {
            try
            {
                serialPort.Open();
                Log.Instance.Logger.InfoFormat("open success");
            }
            catch (Exception ex)
            {
                Log.Instance.Logger.FatalFormat(ex.Message);
            }
        }

        public event EventHandler<string> BarcodeRecieved;
    }
}
