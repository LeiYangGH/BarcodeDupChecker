using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarcodeDupChecker.ViewModel
{
    public class DupBarcodeViewModel : ViewModelBase
    {
        public DupBarcodeViewModel(string barcode)
        {
            this.Barcode = barcode;
            this.ObsDupIndexes = new ObservableCollection<int>();
        }

        private string barcode;
        public string Barcode
        {
            get
            {
                return this.barcode;
            }
            set
            {
                if (this.barcode != value)
                {
                    this.barcode = value;
                    this.RaisePropertyChanged(nameof(Barcode));
                }
            }
        }

        private ObservableCollection<int> obsDupIndexes;
        public ObservableCollection<int> ObsDupIndexes
        {
            get
            {
                return this.obsDupIndexes;
            }
            set
            {
                if (this.obsDupIndexes != value)
                {
                    this.obsDupIndexes = value;
                    this.RaisePropertyChanged(nameof(ObsDupIndexes));
                }
            }
        }

        public string IndexesTooltip
        {
            get
            {
                return string.Join("\n", this.ObsDupIndexes);
            }
        }

        public void AddDupIndex(int index)
        {
            this.ObsDupIndexes.Add(index);
            this.RaisePropertyChanged(nameof(IndexesTooltip));
        }
    }
}
