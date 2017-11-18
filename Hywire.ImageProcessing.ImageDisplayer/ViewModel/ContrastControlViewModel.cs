using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure.WPF.Framework;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows;
using System.Threading;
using System.Collections;

namespace Hywire.ImageProcessing.ImageDisplayer.ViewModel
{
    public class ContrastControlViewModel : ViewModelBase
    {
        #region Private Fields
        int _PixelHighLimit = 65535;
        int _WhiteValue = 65535;
        int _BlackValue = 0;
        double _GammaValue = 1.0;
        #endregion Private Fields

        #region Constructor
        public ContrastControlViewModel()
        {

        }
        #endregion Constructor

        #region Public Properties
        public int WhiteValue
        {
            get
            {
                return _WhiteValue;
            }
            set
            {
                if (_WhiteValue != value)
                {
                    if (value <= _BlackValue)
                    {
                        _WhiteValue = _BlackValue + 1;
                    }
                    else
                    {
                        _WhiteValue = value;
                    }
                    RaisePropertyChanged("WhiteValue");
                    RaisePropertyChanged("NormalizedWhiteValue");
                }
            }
        }

        public int BlackValue
        {
            get
            {
                return _BlackValue;
            }
            set
            {
                if (_BlackValue != value)
                {
                    if (value >= _WhiteValue)
                    {
                        _BlackValue = _WhiteValue - 1;
                    }
                    else
                    {
                        _BlackValue = value;
                    }
                    RaisePropertyChanged("BlackValue");
                    RaisePropertyChanged("NormalizedBlackValue");
                }
            }
        }

        public double GammaValue
        {
            get
            {
                return _GammaValue;
            }
            set
            {
                if (_GammaValue != value)
                {
                    _GammaValue = value;
                    RaisePropertyChanged("GammaValue");
                }
            }
        }

        public double NormalizedWhiteValue
        {
            get { return (double)WhiteValue / _PixelHighLimit; }
        }
        public double NormalizedBlackValue
        {
            get { return (double)BlackValue / _PixelHighLimit; }
        }
        #endregion Public Properties
    }
}
