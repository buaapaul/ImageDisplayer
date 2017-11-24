using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure.WPF.Framework;
using System.Windows.Media.Imaging;
using Azure.Image.Processing;
using System.IO;

namespace Hywire.ImageProcessing.ImageDisplayer.ViewModel
{
    public class ImageGalleryViewModel : ViewModelBase
    {
        private BitmapImage _DisplayImage;
        private int _PixelX;
        private int _PixelY;
        private string _PixelIntensity;
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

                    if (_DisplayImage == null)
                    {
                        PixelX = 0;
                        PixelY = 0;
                    }
                }
            }
        }

        public int PixelX
        {
            get
            {
                return _PixelX;
            }
            set
            {
                if (_PixelX != value)
                {
                    _PixelX = value;
                    RaisePropertyChanged("PixelX");
                }
            }
        }

        public int PixelY
        {
            get
            {
                return _PixelY;
            }
            set
            {
                if (_PixelY != value)
                {
                    _PixelY = value;
                    RaisePropertyChanged("PixelY");
                }
            }
        }

        public string PixelIntensity
        {
            get
            {
                return _PixelIntensity;
            }
            set
            {
                if (_PixelIntensity != value)
                {
                    _PixelIntensity = value;
                    RaisePropertyChanged("PixelIntensity");
                }
            }
        }
    }
}
