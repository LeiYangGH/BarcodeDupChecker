﻿using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestSerialPortSD
{
    class Program
    {
        static SerialPort serialPort = new SerialPort();

        const char CR = (char)13;
        const char LF = (char)10;
        static char[] splitChars = new char[] { CR, LF };
        static void SerialPort_DataReceived(string str)
        {
            //string barcode = this.serialPort.ReadExisting().Replace("\r","").Replace("\n", "");
            string recieved = str;
            full += recieved;
            if (recieved.Contains(CR) || recieved.Contains(LF))//this.full should not contain
            {
                string[] ss = full.Split(splitChars);

                string barcode = ss[0];
                Console.WriteLine(barcode);
                full = ss[1];

            }

        }

        static void Main(string[] args)
        {
            SerialPort_DataReceived("abcde\n");
            SerialPort_DataReceived("abc");
            SerialPort_DataReceived("de\n");
            SerialPort_DataReceived("abcd");
            SerialPort_DataReceived("e\n");
            SerialPort_DataReceived("a");
            SerialPort_DataReceived("bcde\n");
            Console.WriteLine("done");
            Console.ReadLine();
            return;


            ShowAllPortNames();

            //-------------------下面的参数就是有可能导致条码截断的原因-------------
            serialPort.ReceivedBytesThreshold = 4;
            //serialPort.ReadTimeout = 1000;
            serialPort.PortName = "COM10";//修改要读的串口名字

            serialPort.BaudRate = 9600;//波特率

            //奇偶校验
            //serialPort.Parity = Parity.None;//默认值
            //serialPort.Parity = Parity.Even;
            //serialPort.Parity = Parity.Mark;
            //serialPort.Parity = Parity.Odd;
            //serialPort.Parity = Parity.Space;

            //终止位数
            //serialPort.StopBits = StopBits.None;
            //serialPort.StopBits = StopBits.One;//默认值 1 
            //serialPort.StopBits = StopBits.OnePointFive;//1.5
            //serialPort.StopBits = StopBits.Two;//2

            //每字节数，默认8应该不能改
            //serialPort.DataBits = 8;

            //serialPort.Handshake = Handshake.None;//默认值
            //serialPort.Handshake = Handshake.RequestToSend;
            //serialPort.Handshake = Handshake.RequestToSendXOnXOff;
            //serialPort.Handshake = Handshake.XOnXOff;

            //serialPort.RtsEnable = true;//默认值
            //serialPort.RtsEnable = false;

            //--------------------------------

            serialPort.DataReceived += SerialPort_DataReceived;
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
        }

        static string full = "";
        private static void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            string barcode = serialPort.ReadExisting();
            Console.WriteLine("<" + barcode + ">");
        }

        static void ShowAllPortNames()
        {
            Console.WriteLine("所有串口：");
            Console.WriteLine("--------------");
            foreach (string name in SerialPort.GetPortNames())
            {
                Console.WriteLine(name);
            }
            Console.WriteLine("--------------");
        }
    }
}
