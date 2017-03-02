using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PInvokeSerialPort;
namespace SPInvoke
{
    class Program
    {
        const string portName = "COM1";

        static SerialPort serialPort;
        static void Main(string[] args)
        {

            serialPort = new SerialPort(portName, 9600);
            serialPort.DataReceived += Sp_DataReceived;
            try
            {
                Console.WriteLine("打开串口..." + serialPort.PortName);
                serialPort.Open();
                Console.WriteLine("打开串口成功，准备接收数据...");

            }
            catch (Exception ex)
            {
                Console.WriteLine("打开串口失败，原因" + ex.Message);
            }
            Console.ReadLine();
            Console.ReadLine();
        }

        static List<byte> lstBytes = new List<byte>();
        static StringBuilder sb = new StringBuilder();

        private static void Sp_DataReceived(byte b)
        {
            if (b == 13)
            {
                //foreach(byte b in lstBytes)
                //{
                //    sb.Append((char)b);
                //}
                Console.WriteLine(sb.ToString());
                sb.Clear();

            }
            else
            {
                sb.Append((char)b);
            }

        }
    }
}
