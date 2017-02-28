using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Linq;
using System.Threading.Tasks;

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

        private void BarReciever_BarcodeRecieved(object sender, string e)
        {
            string barcode = e;
            if (App.Current != null)//walkaround
                App.Current.Dispatcher.Invoke(() =>
                {
                    int lastIndex = this.ObsAllBarcodes.Count + 1;
                    bool hasDup = this.CheckDup(barcode, lastIndex);
                    AllBarcodeViewModel newAllVM = new AllBarcodeViewModel(barcode, lastIndex);
                    this.ObsAllBarcodes.Add(newAllVM);
                    if (hasDup)
                    {
                        Log.Instance.Logger.InfoFormat("Dup={0}", barcode);
                        foreach (AllBarcodeViewModel allVM in this.ObsAllBarcodes.Where(x => x.Barcode == barcode))
                        {
                            allVM.HasDup = true;
                        }
                    }
                });
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
            this.IsOpened = true;
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
            this.IsOpened = false;
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
            if (setWin.ShowDialog() ?? true)
            {
#if SerialPort
                SettingsViewModel setVM = (setWin.DataContext) as SettingsViewModel;
                this.PortName = setVM.SelectedPortName;
                this.barReciever.SetSerialPortParameters(this.PortName);
#else
        
#endif
            }

        }



        private bool CheckDup(string barcode, int lastIndex)
        {
            AllBarcodeViewModel allVM = this.ObsAllBarcodes.FirstOrDefault(x => x.Barcode == barcode);
            if (allVM != null)
            {
                int firstIndex = this.ObsAllBarcodes.IndexOf(allVM) + 1;
                DupBarcodeViewModel dupVM = this.ObsDupBarcodes.FirstOrDefault(x => x.Barcode == barcode);
                if (dupVM == null)
                {
                    dupVM = new DupBarcodeViewModel(barcode);
                    this.ObsDupBarcodes.Add(dupVM);
                    dupVM.AddDupIndex(firstIndex);
                }
                dupVM.AddDupIndex(lastIndex);
                return true;
            }
            else
                return false;
        }


        public override void Cleanup()
        {
            this.barReciever.Close();
            base.Cleanup();
        }
    }
}