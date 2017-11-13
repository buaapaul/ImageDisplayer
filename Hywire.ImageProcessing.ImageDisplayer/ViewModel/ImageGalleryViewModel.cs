using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure.WPF.Framework;
using System.Windows.Media.Imaging;

namespace Hywire.ImageProcessing.ImageDisplayer.ViewModel
{
    public class ImageGalleryViewModel : ViewModelBase
    {
        private BitmapSource _DisplayImage;
        public BitmapSource DisplayImage
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
