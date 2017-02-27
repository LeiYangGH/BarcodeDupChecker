using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BarcodeDupChecker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //private IBarcodeReciever barReciever = new TimerBarcodeReciever();
        private IBarcodeReciever barReciever = new SerialPortBarcodeReciever();
        private List<string> lstAllBarcodes = new List<string>();
        private List<string> lstDupBarcodes = new List<string>();
        public MainWindow()
        {
            InitializeComponent();
            barReciever.BarcodeRecieved += BarReciever_BarcodeRecieved;
            Log.Instance.Logger.Info("UI started!");
        }

        private void BarReciever_BarcodeRecieved(object sender, string e)
        {
            string barcode = e;
            this.lstAll.Dispatcher.Invoke(() =>
            {
                this.CheckDup(barcode);
                this.lstAllBarcodes.Add(barcode);
                this.lstAll.Items.Add(barcode);
                this.txtAllCount.Text = this.lstAllBarcodes.Count.ToString();
            });
        }

        private void CheckDup(string dupBarcode)
        {
            if (this.lstAllBarcodes.Contains(dupBarcode))
            {
                this.lstDupBarcodes.Add(dupBarcode);
                this.lstDup.Items.Add(dupBarcode);
                this.txtDupCount.Text = this.lstDupBarcodes.Count.ToString();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        //private delegate void NoArgDelegate();

        //public static void Refresh(DependencyObject obj)

        //{

        //    obj.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.ApplicationIdle,

        //        (NoArgDelegate)delegate { });

        //}
    }
}
