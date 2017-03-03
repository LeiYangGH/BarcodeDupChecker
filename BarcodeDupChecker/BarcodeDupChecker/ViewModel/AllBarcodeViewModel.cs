﻿using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarcodeDupChecker.ViewModel
{
    public class AllBarcodeViewModel : ViewModelBase
    {
        public AllBarcodeViewModel(string barcode, int index)
        {
            this.Barcode = barcode;
            this.Index = index;
            this.HasDup = false;
        }

        private int index;
        public int Index
        {
            get
            {
                return this.index;
            }
            set
            {
                if (this.index != value)
                {
                    this.index = value;
                    this.RaisePropertyChanged(() => this.Index);
                }
            }
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
                    this.RaisePropertyChanged(() => this.Barcode);
                }
            }
        }

        private bool hasDup;
        public bool HasDup
        {
            get
            {
                return this.hasDup;
            }
            set
            {
                if (this.hasDup != value)
                {
                    this.hasDup = value;
                    this.RaisePropertyChanged(() => this.HasDup);
                }
            }
        }
    }
}
