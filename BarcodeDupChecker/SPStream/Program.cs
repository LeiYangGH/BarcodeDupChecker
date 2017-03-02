using RJCP.IO.Ports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPStream
{
    class Program
    {
        static SerialPortStream spSteam;
        const string portName = "COM1";
        static void Main(string[] args)
        {
            spSteam = new SerialPortStream();
            spSteam.PortName = portName;
            //spSteam.NewLine = "\n";



            spSteam.DataReceived += SpSteam_DataReceived;
            try
            {
                Console.WriteLine("打开串口..." + spSteam.PortName);
                spSteam.Open();
                Console.WriteLine("打开串口成功，准备接收数据...");

            }
            catch (Exception ex)
            {
                Console.WriteLine("打开串口失败，原因" + ex.Message);
            }
            Console.ReadLine();
        }

        private static void SpSteam_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            string barcode = spSteam.ReadExisting();
            //string barcode = spSteam.ReadLine();
            Console.Write("<{0}>", barcode);
        }
    }
}
