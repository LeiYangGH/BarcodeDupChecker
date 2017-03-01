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

namespace BarcodeDupChecker.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
#if SerialPort
        private SerialPortBarcodeReciever barReciever = new SerialPortBarcodeReciever();
#else
        private IBarcodeReciever barReciever = new TimerBarcodeReciever();
#endif


        public MainViewModel()
        {
            this.ObsAllBarcodes = new ObservableCollection<AllBarcodeViewModel>();
            this.ObsDupBarcodes = new ObservableCollection<DupBarcodeViewModel>();
            this.IsOpened = false;

#if SerialPort
            this.PortName = this.barReciever.GetFirstPortName();
#else
            this.PortName = "Timer";
#endif
            //MessengerInstance.Register<MainWindow>(this, (p) =>
            //{
            //    this.barReciever.Close();
            //});
            if (IsInDesignMode)
            {
                //this.obsAllBarcodes = new ObservableCollection<string>() { "a", "bb", "a", "bb", "ccc" };
                //this.ObsDupBarcodes = new ObservableCollection<string>() { "a", "bb" };
            }
            else
            {
                barReciever.BarcodeRecieved += BarReciever_BarcodeRecieved;

            }
        }

        private string portName;
        public string PortName
        {
            get
            {
                return this.portName;
            }
            set
            {
                if (this.portName != value)
                {
                    this.portName = value;
                    this.RaisePropertyChanged(() => this.PortName);
                }
            }
        }

        private bool isOpened;
        public bool IsOpened
        {
            get
            {
                return this.isOpened;
            }
            set
            {
                if (this.isOpened != value)
                {
                    this.isOpened = value;
                    this.RaisePropertyChanged(() => this.IsOpened);
                }

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
#if SerialPort
            this.IsOpened = this.barReciever.IsSerailPortOpen;
#else
            this.IsOpened = true;
#endif
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
        private async Task Close()
        {
            this.barReciever.Close();
#if SerialPort
            this.IsOpened = this.barReciever.IsSerailPortOpen;
#else
            this.IsOpened = false;
#endif
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
            this.barReciever.Close();
            this.IsOpened = false;

            SettingWindow setWin = new SettingWindow();
            SettingsViewModel setVM = (setWin.DataContext) as SettingsViewModel;
            setVM.SelectedPortName = this.PortName;
            if (setWin.ShowDialog() ?? false)
            {
#if SerialPort
                this.PortName = setVM.SelectedPortName;
                this.barReciever.SetSerialPortParameters(this.PortName);
#else

#endif
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
                return true;
            }
            catch (Exception ex)
            {
                Log.Instance.Logger.Error(ex.Message);
                return false;
            }
        }

        private async Task Export()
        {
            this.barReciever.Close();
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

                        await Add10K();

                        isAdd10King = false;
                        Add10KCommand.RaiseCanExecuteChanged();
                    },
                    () => !isAdd10King));
            }
        }


        private async Task Add10K()
        {
            this.barReciever.Close();
            Stopwatch sw = new Stopwatch();
            sw.Start();
            Random r = new Random();
            for (int i = 1; i <= 10000; i++)
            {
                string barcode = "BBBBBBB" + r.Next(0, 10000).ToString().PadLeft(5, '0');
                this.obsAllBarcodes.Add(new AllBarcodeViewModel(barcode, i));

                //this.GotBarcode(barcode);
            }
            var dupBarcodes = this.obsAllBarcodes.GroupBy(x => x.Barcode).Where(g => g.Count() > 1)
    .Select(g => g.Key).Distinct();
            foreach (var bar in dupBarcodes)
            {
                this.obsDupBarcodes.Add(new DupBarcodeViewModel(bar));
            }
            for (int a = 0; a < this.obsAllBarcodes.Count; a++)
            {
                var Abar = this.obsAllBarcodes[a];
                string barcode = Abar.Barcode;
                if (dupBarcodes.Any(x => x == barcode))
                {
                    Abar.HasDup = true;
                    var dupBar = this.obsDupBarcodes.First(x => x.Barcode == barcode);
                    dupBar.AddDupIndex(a + 1);
                }
            }
            this.RaisePropertyChanged(() => this.ObsAllBarcodes);
            this.RaisePropertyChanged(() => this.ObsDupBarcodes);
            sw.Stop();
            Console.WriteLine(sw.ElapsedMilliseconds);
        }

        private void BarReciever_BarcodeRecieved(object sender, string e)
        {
            this.GotBarcode(e);
        }

        private void GotBarcode(string barcode)
        {
            if (App.Current != null)//walkaround
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

        public override void Cleanup()
        {
            this.barReciever.Close();
            base.Cleanup();
        }
    }
}