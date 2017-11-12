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
    }
}
