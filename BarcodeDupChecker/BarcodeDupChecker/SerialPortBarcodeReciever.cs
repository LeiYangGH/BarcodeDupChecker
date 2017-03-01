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
        SerialPort serialPort = new SerialPort();

        public SerialPortBarcodeReciever()
        {
            this.SetSerialPortParameters("");
            this.serialPort.DataReceived += Sp_DataReceived;
        }

        public bool IsSerailPortOpen
        {
            get
            {
                return this.serialPort.IsOpen;
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
            if (string.IsNullOrWhiteSpace(portName))
            {
                portName = this.GetFirstPortName();
            }
            Log.Instance.Logger.InfoFormat("portName={0}", portName);

            this.serialPort.PortName = portName;
            serialPort.BaudRate = 9600;
            serialPort.Parity = Parity.None;
            serialPort.StopBits = StopBits.One;
            serialPort.DataBits = 8;
            serialPort.Handshake = Handshake.None;
            serialPort.RtsEnable = true;
        }

        public void SetBarcodeLength(int length)
        {
            this.barcodeLength = length;
        }

        string full = "";
        private void Sp_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort sp = (SerialPort)sender;
            string barcode = sp.ReadExisting();
            full += barcode;
            if (full.Length >= barcodeLength)
            {
                if (this.BarcodeRecieved != null)
                {
                    this.BarcodeRecieved(this, full);
                    full = "";
                }
            }
            else
            {
                Log.Instance.Logger.InfoFormat("{0} < {1}", barcode, barcodeLength);
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
