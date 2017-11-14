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
        private WriteableBitmap _DisplayImage;
        private WriteableBitmap _WriteableBackImage;
        private BitmapImage _BackImage;
        public WriteableBitmap DisplayImage
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
                    //if(_DisplayImage != null)
                    //{
                    //    using(MemoryStream stream=new MemoryStream())
                    //    {
                    //        TiffBitmapEncoder encoder = new TiffBitmapEncoder();
                    //        encoder.Frames.Add(BitmapFrame.Create(_DisplayImage));
                    //        encoder.Save(stream);
                    //        _BackImage = new BitmapImage();
                    //        _BackImage.BeginInit();
                    //        _BackImage.CacheOption = BitmapCacheOption.OnLoad;
                    //        _BackImage.StreamSource = stream;
                    //        _BackImage.EndInit();
                    //        _BackImage.Freeze();
                    //    }
                    //}
                }
            }
        }

        public WriteableBitmap WriteableBackImage
        {
            get
            {
                return _WriteableBackImage;
            }

            set
            {
                _WriteableBackImage = value;
            }
        }
    }
}
