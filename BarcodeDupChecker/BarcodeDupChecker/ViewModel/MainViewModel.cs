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
using System.Diagnostics;
using System.Runtime.ExceptionServices;

namespace BarcodeDupChecker.ViewModel
{
    public class MainViewModel : ViewModelBase, IDisposable
    {
#if SerialPort
        private IBarcodeReciever barReciever;
#else
        private IBarcodeReciever barReciever = new TimerBarcodeReciever();
#endif


        public MainViewModel()
        {
            this.UsePInvokeReader = true;
            this.CreateBarcodeReciever();
            this.PortName = this.GetFirstPortName();
            this.ObsAllBarcodes = new ObservableCollection<AllBarcodeViewModel>();
            this.ObsDupBarcodes = new ObservableCollection<DupBarcodeViewModel>();

            if (IsInDesignMode)
            {

            }
            else
            {
                MessengerInstance.Register<string>(this, (msg) =>
                {
                    this.Message = msg;
                });
            }
        }

        private bool usePInvokeReader;
        public bool UsePInvokeReader
        {
            get
            {
                return this.usePInvokeReader;
            }
            set
            {
                if (this.usePInvokeReader != value)
                {
                    this.usePInvokeReader = value;
                    this.RaisePropertyChanged(() => this.UsePInvokeReader);
                }
            }
        }

        public string PortName
        {
            get
            {
                return this.barReciever.PortName;
            }
            set
            {
                if (this.barReciever.PortName != value)
                {
                    this.barReciever.PortName = value;
                    this.RaisePropertyChanged(() => this.PortName);
                }
            }
        }

        public bool IsOpened
        {
            get
            {
                return this.barReciever.Online;
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
                    this.RaisePropertyChanged(() => this.ObsAllBarcodes);
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
                    this.RaisePropertyChanged(() => this.ObsDupBarcodes);
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
                    this.RaisePropertyChanged(() => this.SelectedObsDupBarcode);
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
                    this.RaisePropertyChanged(() => this.Message);
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
            this.barReciever.Start();
            this.RaisePropertyChanged(() => this.IsOpened);
            if (this.IsOpened)
            {
                Settings.Default.PortName = this.PortName;
                Settings.Default.Save();
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
            this.barReciever.Close();
            this.RaisePropertyChanged(() => this.IsOpened);
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
            SettingsViewModel setVM = (setWin.DataContext) as SettingsViewModel;
            setVM.SelectedPortName = this.PortName;
            setVM.UsePInvokeReader = this.UsePInvokeReader;

            if (setWin.ShowDialog() ?? false)
            {

                if (this.UsePInvokeReader != setVM.UsePInvokeReader)
                {
                    this.UsePInvokeReader = setVM.UsePInvokeReader;
                    this.CreateBarcodeReciever();
                    Log.Instance.Logger.InfoFormat("切换为读{0}串口", this.UsePInvokeReader ? "物理" : "虚拟");
                }
                this.PortName = setVM.SelectedPortName;
                this.RaisePropertyChanged(() => this.IsOpened);
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

        private async Task ImportBarcodesFromFile()//open
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

        private void BarReciever_BarcodeRecieved(object sender, BarcodeEventArgs e)
        {
            this.GotBarcode(e.Barcode);
        }

        private void GotBarcode(string barcode)
        {
            //if (App.Current != null)//walkaround
            App.Current.Dispatcher.Invoke(() =>
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
            });
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

        private void CreateBarcodeReciever()
        {
            if (this.barReciever != null)
            {
                this.barReciever.BarcodeRecieved -= BarReciever_BarcodeRecieved;
                this.barReciever.Dispose();
            }
            if (this.UsePInvokeReader)
                this.barReciever = new PInvokeSerialPortBarcodeReciever();
            else
                this.barReciever = new BuiltinSerialPortBarcodeReciever();
            this.barReciever.BarcodeRecieved += BarReciever_BarcodeRecieved;
        }
        public override void Cleanup()
        {
            this.barReciever.Close();
            base.Cleanup();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.barReciever.Dispose();
            }
        }
        public void Dispose()
        {

            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}