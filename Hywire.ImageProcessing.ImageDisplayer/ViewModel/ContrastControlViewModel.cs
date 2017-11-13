using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure.WPF.Framework;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using ImageMagick;

namespace Hywire.ImageProcessing.ImageDisplayer.ViewModel
{
    public class ContrastControlViewModel : ViewModelBase
    {
        #region Private Fields
        int _WhiteValue = 1;
        int _BlackValue = 0;
        double _GammaValue;
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
        #endregion Public Properties

        public BitmapSource GetImageData(BitmapSource bitmap)
        {
            FormatConvertedBitmap newBitmap;
            newBitmap = new FormatConvertedBitmap();
            newBitmap.BeginInit();
            newBitmap.Source = bitmap;
            newBitmap.DestinationFormat = PixelFormats.Gray8;
            newBitmap.EndInit();
            bitmap = null;
            return newBitmap;
        }
    }
}
