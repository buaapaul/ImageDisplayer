using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure.WPF.Framework;
using System.Windows.Media.Imaging;
using System.Windows.Controls;

namespace Hywire.ImageProcessing.ImageDisplayer.ViewModel
{
    public class ImageGalleryViewModel : ViewModelBase
    {
        private BitmapImage _DisplayImage;
        private WriteableBitmap _WriteableDisplayImage;
        public BitmapImage DisplayImage
        {
            get
            {
                return _DisplayImage;
            }
            set
            {
                if (_DisplayImage != value)
                {
                    _DisplayImage = value;
                    RaisePropertyChanged("DisplayImage");
                }
            }
        }

        //public WriteableBitmap DisplayImage
        //{
        //    get
        //    {
        //        return _WriteableDisplayImage;
        //    }
        //    set
        //    {
        //        if (_WriteableDisplayImage != value)
        //        {
        //            _WriteableDisplayImage = value;
        //            RaisePropertyChanged("DisplayImage");
        //        }
        //    }
        //}
    }
}
