using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure.WPF.Framework;
using System.Windows.Input;
using Microsoft.Win32;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using System.IO;
using System.Windows.Media;

namespace Hywire.ImageProcessing.ImageDisplayer.ViewModel
{
    public class Workspace : ViewModelBase
    {
        #region Private Fields
        ImageGalleryViewModel _ImageGalleryVM = null;

        RelayCommand _OpenImageCommand = null;
        RelayCommand _CloseImageCommand = null;
        #endregion Private Fields

        static Workspace _this = new Workspace();
        public static Workspace This
        {
            get { return _this; }
            set { _this = value; }
        }
        public Workspace()
        {
            Title = "Hywire Image Displayer";
            _ImageGalleryVM = new ImageGalleryViewModel();
        }

        #region OpenImageCommand
        public ICommand OpenImageCommand
        {
            get
            {
                if (_OpenImageCommand == null)
                {
                    _OpenImageCommand = new RelayCommand(ExecuteOpenImageComand, CanExecuteOpenImageComand);
                }
                return _OpenImageCommand;
            }
        }
        public void ExecuteOpenImageComand(object parameter)
        {
            OpenFileDialog opDlg = new OpenFileDialog();
            if (opDlg.ShowDialog() == true)
            {
                BitmapImage image = new BitmapImage();
                using (FileStream fs = File.OpenRead(opDlg.FileName))
                {
                    image.BeginInit();
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.StreamSource = fs;
                    image.EndInit();
                }
                ImageGalleryVM.DisplayImage = image;
                //ImageGalleryVM.DisplayImage = LoadImage(opDlg.FileName);
            }
        }
        public bool CanExecuteOpenImageComand(object parameter)
        {
            return true;
        }
        public static WriteableBitmap LoadImage(string @sFileName)
        {
            BitmapImage bitmap = null;
            WriteableBitmap wbBitmap = null;
            try
            {
                bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(@sFileName, UriKind.RelativeOrAbsolute);
                bitmap.CacheOption = BitmapCacheOption.None;
                bitmap.CreateOptions = BitmapCreateOptions.None | BitmapCreateOptions.PreservePixelFormat | BitmapCreateOptions.IgnoreImageCache;
                bitmap.EndInit();
                bitmap.Freeze();

                wbBitmap = new WriteableBitmap((BitmapSource)bitmap);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                bitmap = null;
            }

            return wbBitmap;
        }
        #endregion OpenImageCommand

        #region CloseImageCommand
        public ICommand CloseImageCommand
        {
            get
            {
                if (_CloseImageCommand == null)
                {
                    _CloseImageCommand = new RelayCommand(ExecuteCloseImageCommand, CanExecuteCloseImageCommand);
                }
                return _CloseImageCommand;
            }
        }
        public void ExecuteCloseImageCommand(object parameter)
        {
            ImageGalleryVM.DisplayImage = null;
        }
        public bool CanExecuteCloseImageCommand(object parameter)
        {
            return ImageGalleryVM.DisplayImage != null;
        }
        #endregion CloseImageCommand

        #region Public Properties
        public string Title { get; set; }
        public MainWindow Owner { get; set; }
        public ImageGalleryViewModel ImageGalleryVM
        {
            get
            {
                return _ImageGalleryVM;
            }
        }
        #endregion Public Properties
    }
}
