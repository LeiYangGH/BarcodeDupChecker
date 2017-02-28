using GalaSoft.MvvmLight;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace BarcodeDupChecker.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        //private IBarcodeReciever barReciever = new TimerBarcodeReciever();
        private IBarcodeReciever barReciever = new SerialPortBarcodeReciever();

        public MainViewModel()
        {
            this.ObsAllBarcodes = new ObservableCollection<AllBarcodeViewModel>();
            this.ObsDupBarcodes = new ObservableCollection<DupBarcodeViewModel>();
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