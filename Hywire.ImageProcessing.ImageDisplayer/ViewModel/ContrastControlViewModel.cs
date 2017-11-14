using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure.WPF.Framework;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using ImageMagick;
using System.Windows;
using System.Threading;

namespace Hywire.ImageProcessing.ImageDisplayer.ViewModel
{
    public class ContrastControlViewModel : ViewModelBase
    {
        #region Private Fields
        int _WhiteValue = 65535;
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

                    if (Workspace.This.ImageGalleryVM.DisplayImage != null)
                    {
                        Workspace.This.Owner.Dispatcher.BeginInvoke((Action)delegate
                        {
                            Scale_16u8u_C1_No_Copy(Workspace.This.ImageGalleryVM.WriteableBackImage,
                                Workspace.This.ImageGalleryVM.DisplayImage,
                                _BlackValue, _WhiteValue, 1, false, 0, false);
                            //    unsafe
                            //    {
                            //        WriteableBitmap image = Workspace.This.ImageGalleryVM.DisplayImage;
                            //        image.Lock();
                            //        var imageBackPointer = (ushort*)image.BackBuffer.ToPointer();
                            //        int imageRowStartAddress = 0;
                            //        for (int row = 0; row < image.PixelHeight; row++)
                            //        {
                            //            for (int column = 0; column < image.PixelWidth; column++)
                            //            {
                            //                ushort temp = imageBackPointer[imageRowStartAddress + column];
                            //                if (temp >= _WhiteValue)
                            //                {
                            //                    imageBackPointer[imageRowStartAddress + column] = 65535;
                            //                }
                            //            }
                            //            imageRowStartAddress = (row + 1) * image.BackBufferStride / 2;
                            //        }
                            //        image.AddDirtyRect(new Int32Rect(0, 0, image.PixelWidth, image.PixelHeight));
                            //        image.Unlock();
                            //        Workspace.This.ImageGalleryVM.DisplayImage = image;
                            //    }
                        });
                    }
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

                    if (Workspace.This.ImageGalleryVM.DisplayImage != null)
                    {
                        Workspace.This.Owner.Dispatcher.BeginInvoke((Action)delegate
                        {
                            Scale_16u8u_C1_No_Copy(Workspace.This.ImageGalleryVM.WriteableBackImage,
                                Workspace.This.ImageGalleryVM.DisplayImage,
                                _BlackValue, _WhiteValue, 1, false, 0, false);
                            //unsafe
                            //{
                            //    WriteableBitmap image = Workspace.This.ImageGalleryVM.DisplayImage;
                            //    image.Lock();
                            //    var imageBackPointer = (ushort*)image.BackBuffer.ToPointer();
                            //    int imageRowStartAddress = 0;
                            //    for (int row = 0; row < image.PixelHeight; row++)
                            //    {
                            //        for (int column = 0; column < image.PixelWidth; column++)
                            //        {
                            //            ushort temp = imageBackPointer[imageRowStartAddress + column];
                            //            if (temp <= _BlackValue)
                            //            {
                            //                imageBackPointer[imageRowStartAddress + column] = (ushort)_BlackValue;
                            //            }
                            //        }
                            //        imageRowStartAddress = (row + 1) * image.BackBufferStride / 2;
                            //    }
                            //    image.AddDirtyRect(new Int32Rect(0, 0, image.PixelWidth, image.PixelHeight));
                            //    image.Unlock();
                            //}
                        });
                    }
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

        internal static unsafe void Scale_16u8u_C1_No_Copy(WriteableBitmap srcImage, WriteableBitmap displayImage, int iMin, int iMax, double dGamma,
                                                              bool bIsSaturation, int iSaturatonValue, bool bIsInverted)
        {

            if (srcImage.Format != PixelFormats.Gray16 && srcImage.Format != PixelFormats.Gray8)
            {
                return;
            }

            srcImage.Lock();
            displayImage.Lock();

            int bitsPerPixel = srcImage.Format.BitsPerPixel;

            byte[] lut = CreateLUT(bitsPerPixel, iMin, iMax, dGamma, bIsInverted);

            Thread[] contrastThreads = new Thread[4];
            contrastThreads[0] = new Thread(() => contrastCalArea(srcImage, displayImage, iMin, iMax, dGamma, lut, 0));
            contrastThreads[1] = new Thread(() => contrastCalArea(srcImage, displayImage, iMin, iMax, dGamma, lut, 1));
            contrastThreads[2] = new Thread(() => contrastCalArea(srcImage, displayImage, iMin, iMax, dGamma, lut, 2));
            contrastThreads[3] = new Thread(() => contrastCalArea(srcImage, displayImage, iMin, iMax, dGamma, lut, 3));
            for(int i = 0; i < contrastThreads.Length; i++)
            {
                contrastThreads[i].Start();
            }
            for(int i = 0; i < contrastThreads.Length; i++)
            {
                contrastThreads[i].Join();
            }

            displayImage.AddDirtyRect(new Int32Rect(0, 0, displayImage.PixelWidth, displayImage.PixelHeight));
            srcImage.Unlock();
            displayImage.Unlock();
            return;
        }

        private unsafe static void contrastCalArea(WriteableBitmap srcImage, WriteableBitmap displayImage, int iMin, int iMax, double dGamma, byte[] lut, int areaIndex)
        {
            Workspace.This.Owner.Dispatcher.BeginInvoke((Action)delegate
            {
                byte* pSrcData = null;
                byte* pDstData = null;
                //byte* pSatData = null;
                ushort tmpValue;
                int iWidth = srcImage.PixelWidth;
                int iHeight = srcImage.PixelHeight;
                int iSrcStep = srcImage.BackBufferStride;
                int bytesPerPixel = srcImage.Format.BitsPerPixel / 8;

                int iDstStep = displayImage.BackBufferStride;

                byte* srcData = (byte*)srcImage.BackBuffer.ToPointer();
                byte* dstData = (byte*)displayImage.BackBuffer.ToPointer();

                int startRaw = 0, stopRaw = 0;
                int startCol = 0, stopCol = 0;
                switch (areaIndex)
                {
                    case 0:
                        startRaw = 0; stopRaw = iHeight / 2;
                        startCol = 0; stopCol = iWidth / 2;
                        break;
                    case 1:
                        startRaw = 0; stopRaw = iHeight / 2;
                        startCol = iWidth / 2 + 1; stopCol = iWidth;
                        break;
                    case 2:
                        startRaw = iHeight / 2 + 1; stopRaw = iHeight;
                        startCol = 0; stopCol = iWidth / 2;
                        break;
                    case 3:
                        startRaw = iHeight / 2 + 1; stopRaw = iHeight;
                        startCol = iWidth / 2 + 1; stopCol = iWidth;
                        break;
                }
                for (int i = startRaw; i < stopRaw; i++)
                {
                    pSrcData = srcData + i * iSrcStep;
                    pDstData = dstData + i * iDstStep;

                    for (int j = startCol; j < stopCol; j++)
                    {
                        if (bytesPerPixel == 1)
                        {
                            tmpValue = *(pSrcData + j);
                        }
                        else
                        {
                            tmpValue = *(((ushort*)pSrcData) + j);
                        }

                        if (lut[tmpValue] > 255)
                        {
                            *(pDstData + j) = 255;
                        }
                        else if (lut[tmpValue] < 0)
                        {
                            *(pDstData + j) = 0;
                        }
                        else
                        {
                            *(pDstData + j) = lut[tmpValue];
                        }
                    } // for width
                } //for height
            });
        }

        private static byte[] CreateLUT(int bpp, double dMin, double dMax, double dGamma, bool bIsInvert)
        {
            int maxInput = 1 << bpp;

            byte[] lut = new byte[maxInput];

            for (int i = 0; i < maxInput; ++i)
            {
                if (i <= dMin)
                {
                    lut[i] = (byte)((bIsInvert) ? 255 : 0);
                }
                else if (i >= dMax)
                {
                    lut[i] = (byte)((bIsInvert) ? 0 : 255);
                }
                else
                {
                    if (bIsInvert)
                    {
                        lut[i] = (byte)(255 - (255 * Math.Pow(((double)i - dMin) / (dMax - dMin), (double)1.0 / dGamma)));
                    }
                    else
                    {
                        lut[i] = (byte)(255 * Math.Pow(((double)i - dMin) / (dMax - dMin), (double)1.0 / dGamma));
                    }
                    if (lut[i] > 255)
                    {
                        lut[i] = 255;
                    }
                    else if (lut[i] < 0)
                    {
                        lut[i] = 0;
                    }
                }
            }
            return lut;
        }
    }
}
