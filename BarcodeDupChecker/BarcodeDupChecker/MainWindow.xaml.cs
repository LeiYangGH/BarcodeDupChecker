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
using GalaSoft.MvvmLight;
using BarcodeDupChecker.ViewModel;

namespace BarcodeDupChecker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private SqliteRepository repository = SqliteRepository.Instance;
        private MainViewModel mainVM;
        public MainWindow()
        {
            InitializeComponent();
            this.mainVM = this.DataContext as MainViewModel;
            this.ShowVersion();
            //this.repository.CreateSqliteDb();
            Log.Instance.Logger.Info("\r\nUI started!");
        }

        private void ShowVersion()
        {
            string version = System.Reflection.Assembly.GetExecutingAssembly()
                                           .GetName()
                                           .Version
                                           .ToString();
            this.Title += " -" + version;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void lstIndexes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems != null && e.AddedItems.Count > 0)
            {
                int index = (int)e.AddedItems[0];
                this.lstAll.ScrollIntoView(this.lstAll.Items[index - 1]);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (this.mainVM.ObsAllBarcodes.Count > 0)
            {
                if (MessageBox.Show("真的要关闭吗?显示的数据会丢失", "关闭程序", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                    this.mainVM.CloseBarcodeReciever();
                else
                    e.Cancel = true;
            }
        }
    }
}
