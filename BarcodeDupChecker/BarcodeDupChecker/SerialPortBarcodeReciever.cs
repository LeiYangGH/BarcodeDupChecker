using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarcodeDupChecker
{
    public class SerialPortBarcodeReciever : IBarcodeReciever
    {
        //Timer t = new Timer(500);
        SerialPort serialPort = new SerialPort();


        public void SetSerialPortParameters(string portName)
        {
            if (string.IsNullOrWhiteSpace(portName))
            {
                string[] names = SerialPort.GetPortNames();
                if (names.Length > 0)

                    portName = names[0];

                else
                    portName = "COM1";
            }
            Log.Instance.Logger.InfoFormat("portName={0}", portName);

            this.serialPort.PortName = portName;

            //this.serialPort.PortName = "COM3";
            serialPort.BaudRate = 9600;
            serialPort.Parity = Parity.None;
            serialPort.StopBits = StopBits.One;
            serialPort.DataBits = 8;
            serialPort.Handshake = Handshake.None;
            serialPort.RtsEnable = true;
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

        public SerialPortBarcodeReciever()
        {
            this.SetSerialPortParameters("");
            this.serialPort.DataReceived += Sp_DataReceived;

        }

        private void Sp_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort sp = (SerialPort)sender;
            string barcode = sp.ReadExisting();
            if (this.BarcodeRecieved != null)
            {
                this.BarcodeRecieved(this, barcode);
            }
        }

        public event EventHandler<string> BarcodeRecieved;
    }
}
