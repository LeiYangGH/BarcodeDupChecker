using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Win32;
using System.IO;
using BarcodeDupChecker.Properties;
using System.Runtime.ExceptionServices;
using System.Text;

namespace BarcodeDupChecker.ViewModel
{
    public class MainViewModel : ViewModelBase, IDisposable
    {
        private SerialPort serialPort = new SerialPort("COM?", 9600);

        public MainViewModel()
        {
            this.serialPort.DataReceived += SerialPort_DataReceived;

#if Test
            this.PortName = "COM10";
#else
            this.serialPort.PortName = this.GetFirstPortName();
#endif
            this.obsAllBarcodes = new ObservableCollection<AllBarcodeViewModel>();
            this.obsDupBarcodes = new ObservableCollection<DupBarcodeViewModel>();

            if (IsInDesignMode)
            {

            }
            else
            {
                //MessengerInstance.Register<string>(this, (msg) =>
                //{
                //    this.Message = msg;
                //});
            }
        }


        public string PortName
        {
            get
            {
                return this.serialPort.PortName;
            }
            set
            {
                if (this.serialPort.PortName != value)
                {
                    this.serialPort.PortName = value;
                    this.RaisePropertyChanged(nameof(PortName));
                }
            }
        }

        public bool IsOpened
        {
            get
            {
                return this.serialPort.IsOpen;
            }

        }


        private ObservableCollection<AllBarcodeViewModel> obsAllBarcodes;
        public ObservableCollection<AllBarcodeViewModel> ObsAllBarcodes
        {
            get
            {
                return this.obsAllBarcodes;
            }
            set
            {
                if (this.obsAllBarcodes != value)
                {
                    this.obsAllBarcodes = value;
                    this.RaisePropertyChanged(nameof(ObsAllBarcodes));
                }
            }
        }

        private ObservableCollection<DupBarcodeViewModel> obsDupBarcodes;
        public ObservableCollection<DupBarcodeViewModel> ObsDupBarcodes
        {
            get
            {
                return this.obsDupBarcodes;
            }
            set
            {
                if (this.obsDupBarcodes != value)
                {
                    this.obsDupBarcodes = value;
                    this.RaisePropertyChanged(nameof(ObsDupBarcodes));
                }
            }
        }


        private DupBarcodeViewModel selectedObsDupBarcode;
        public DupBarcodeViewModel SelectedObsDupBarcode
        {
            get
            {
                return this.selectedObsDupBarcode;
            }
            set
            {
                if (this.selectedObsDupBarcode != value)
                {
                    this.selectedObsDupBarcode = value;
                    this.RaisePropertyChanged(nameof(SelectedObsDupBarcode));
                }
            }
        }


        private string message;
        public string Message
        {
            get
            {
                return this.message;
            }
            set
            {
                if (this.message != value)
                {
                    this.message = value;
                    this.RaisePropertyChanged(nameof(Message));
                }
            }
        }


        private bool isOpening;
        private RelayCommand openCommand;

        public RelayCommand OpenCommand
        {
            get
            {
                return openCommand
                  ?? (openCommand = new RelayCommand(
                    async () =>
                    {
                        if (isOpening)
                        {
                            return;
                        }

                        isOpening = true;
                        OpenCommand.RaiseCanExecuteChanged();

                        await Open();

                        isOpening = false;
                        OpenCommand.RaiseCanExecuteChanged();
                    },
                    () => !isOpening));
            }
        }
        private async Task Open()
        {
            this.OpenSerialPort();
            this.RaisePropertyChanged(nameof(IsOpened));
            if (this.IsOpened)
            {
                Settings.Default.PortName = this.PortName;
                Settings.Default.Save();
            }

        }

        public void OpenSerialPort()
        {
            try
            {
                this.serialPort.Open();
                Log.Instance.Logger.InfoFormat("open {0} success", this.PortName);
                this.Message = string.Format("成功打开串口{0}！", this.PortName);
            }
            catch (Exception ex)
            {
                Log.Instance.Logger.FatalFormat(ex.Message);
                MessengerInstance.Send<string>(ex.Message);
            }
        }

        private bool isCloseing;
        private RelayCommand closeCommand;

        public RelayCommand CloseCommand
        {
            get
            {
                return closeCommand
                  ?? (closeCommand = new RelayCommand(
                    async () =>
                    {
                        if (isCloseing)
                        {
                            return;
                        }

                        isCloseing = true;
                        CloseCommand.RaiseCanExecuteChanged();

                        await Close();

                        isCloseing = false;
                        CloseCommand.RaiseCanExecuteChanged();
                    },
                    () => !isCloseing));
            }
        }

        [HandleProcessCorruptedStateExceptions]
        private async Task Close()
        {
            //App.Current.Dispatcher.BeginInvoke((Action)(delegate
            //{
            this.CloseSerialPort();
            this.RaisePropertyChanged(nameof(IsOpened));
            //}));
        }

        public void CloseSerialPort()
        {
            try
            {
                this.serialPort.Close();
                Log.Instance.Logger.InfoFormat("close {0} success", this.PortName);
                this.Message = string.Format("成功关闭串口{0}！", this.PortName);

            }
            catch (Exception ex)
            {
                Log.Instance.Logger.FatalFormat(ex.Message);
                MessengerInstance.Send<string>(ex.Message);
            }
        }

        private bool isSeting;
        private RelayCommand setCommand;

        public RelayCommand SetCommand
        {
            get
            {
                return setCommand
                  ?? (setCommand = new RelayCommand(
                    async () =>
                    {
                        if (isSeting)
                        {
                            return;
                        }

                        isSeting = true;
                        SetCommand.RaiseCanExecuteChanged();

                        await Set();

                        isSeting = false;
                        SetCommand.RaiseCanExecuteChanged();
                    },
                    () => !isSeting));
            }
        }
        private async Task Set()
        {
            Log.Instance.Logger.Info("settings");

            SettingWindow setWin = new SettingWindow();
            setWin.Owner = MainWindow.Instance;
            SettingsViewModel setVM = (setWin.DataContext) as SettingsViewModel;
            setVM.SelectedPortName = this.PortName;

            if (setWin.ShowDialog() ?? false)
            {
                this.PortName = setVM.SelectedPortName;
                this.RaisePropertyChanged(nameof(IsOpened));
            }

        }

        private bool isExporting;
        private RelayCommand exportCommand;

        public RelayCommand ExportCommand
        {
            get
            {
                return exportCommand
                  ?? (exportCommand = new RelayCommand(
                    async () =>
                    {
                        if (isExporting)
                        {
                            return;
                        }

                        isExporting = true;
                        ExportCommand.RaiseCanExecuteChanged();

                        await Export();

                        isExporting = false;
                        ExportCommand.RaiseCanExecuteChanged();
                    },
                    () => !isExporting));
            }
        }

        private bool ExportTabTxt(string fileName)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(fileName))
                    foreach (var dupVM in this.ObsDupBarcodes)
                    {
                        sw.Write(dupVM.Barcode);
                        foreach (int index in dupVM.ObsDupIndexes)
                        {
                            sw.Write("\t");
                            sw.Write(index);
                        }
                        sw.Write("\r\n");
                    }
                Log.Instance.Logger.Info(fileName);
                this.Message = fileName;
                return true;
            }
            catch (Exception ex)
            {
                Log.Instance.Logger.Error(ex.Message);
                this.Message = ex.Message;
                return false;
            }
        }

        private async Task Export()
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "Text (*.txt)|*.txt";
            if (dlg.ShowDialog() ?? false)
            {
                this.ExportTabTxt(dlg.FileName);
            }

        }

        private bool isAdd10King;
        private RelayCommand add10KCommand;

        public RelayCommand Add10KCommand
        {
            get
            {
                return add10KCommand
                  ?? (add10KCommand = new RelayCommand(
                    async () =>
                    {
                        if (isAdd10King)
                        {
                            return;
                        }

                        isAdd10King = true;
                        Add10KCommand.RaiseCanExecuteChanged();

                        await ImportBarcodesFromFile();

                        isAdd10King = false;
                        Add10KCommand.RaiseCanExecuteChanged();
                    },
                    () => !isAdd10King));
            }
        }

        private async Task ImportBarcodesFromFile()
        {
            this.ObsDupBarcodes.Clear();
            this.ObsAllBarcodes.Clear();
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Text (*.txt)|*.txt";
            if (dlg.ShowDialog() ?? false)
            {
                this.Message = "开始导入" + dlg.FileName;
                string fileName = dlg.FileName;
                foreach (string line in File.ReadLines(fileName))
                {
                    this.GotBarcode(line.Replace("\r", "").Replace("\n", ""));
                }
                this.Message = "结束导入" + dlg.FileName;
            }
        }


        private string full = string.Empty;
        private const char CR = (char)13;
        private const char LF = (char)10;
        private char[] splitChars = new char[] { CR, LF };
        private StringBuilder sb = new StringBuilder("");

        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            string recieved = this.serialPort.ReadExisting();
            this.sb.Append(recieved);
            if (recieved.Contains(CR) || recieved.Contains(LF))
            {
                string[] ss = sb.ToString().Split(splitChars);
                this.GotBarcode(ss[0]);
                this.sb.Clear();
                this.sb.Append(ss[1]);
            }

        }

        private void GotBarcode(string barcode)
        {
            //if (App.Current != null)//walkaround
            App.Current.Dispatcher.BeginInvoke((Action)(delegate
         {
             int oldCount = this.ObsAllBarcodes.Count;
             bool hasDup = false;
             DupBarcodeViewModel dupVM = null;
             for (int a = 0; a < oldCount; a++)
             {
                 AllBarcodeViewModel aVM = this.ObsAllBarcodes[a];
                 if (aVM.Barcode == barcode)
                 {
                     hasDup = true;
                     aVM.HasDup = true;
                     Log.Instance.Logger.InfoFormat("Dup={0}", barcode);
                     dupVM = this.ObsDupBarcodes.FirstOrDefault(x => x.Barcode == barcode);
                     if (dupVM == null)
                     {
                         dupVM = new DupBarcodeViewModel(barcode);
                         dupVM.AddDupIndex(a + 1);
                         this.ObsDupBarcodes.Add(dupVM);
                     }
                     break;
                 }
             }
             AllBarcodeViewModel newAllVM = new AllBarcodeViewModel(barcode, oldCount + 1);
             this.ObsAllBarcodes.Add(newAllVM);
             if (hasDup)
             {
                 newAllVM.HasDup = true;
                 dupVM.AddDupIndex(oldCount + 1);
             }
         }));
        }

        private Random rnd = new Random();
        public void AddRandomBarcode()
        {
            string barcode= rnd.Next(10000, 10010).ToString().PadLeft(5, '0');
            this.GotBarcode(barcode);
        }

        private string GetFirstPortName()
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


        public override void Cleanup()
        {
            this.serialPort.Close();
            base.Cleanup();
        }

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