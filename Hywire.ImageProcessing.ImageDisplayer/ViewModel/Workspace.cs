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
using System.Drawing;
using System.Windows;
using System.Threading;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Collections.ObjectModel;

namespace Hywire.ImageProcessing.ImageDisplayer.ViewModel
{
    public class Workspace : ViewModelBase
    {
        [DllImport("psapi.dll")]
        static extern int EmptyWorkingSet(IntPtr hwProc);
        public static void ClearMemory(Process process)
        {  
            GC.Collect();  
            GC.WaitForPendingFinalizers();
            if ((process.ProcessName == "System") && (process.ProcessName == "Idle"))
            {
                return;
            }
            try
            {
                EmptyWorkingSet(process.Handle);
            }
            catch
            {
            }
        }

        #region Private Fields
        const string SoftwareName = "Hywire Image Displayer";
        string _Title = string.Empty;

        ImageGalleryViewModel _ImageGalleryVM = null;
        ContrastControlViewModel _ContrastControlVM = null;

        RelayCommand _OpenImageCommand = null;
        RelayCommand _CloseImageCommand = null;
        RelayCommand _ReleaseMemoryCommand = null;

        bool _IsImageLoaded = false;
        #endregion Private Fields

        static Workspace _this = new Workspace();

        public static Workspace This
        {
            get { return _this; }
            set { _this = value; }
        }
        public Workspace()
        {
            Title = SoftwareName;
            _ImageGalleryVM = new ImageGalleryViewModel();
            _ContrastControlVM = new ContrastControlViewModel();
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
            opDlg.Filter = "jpg|*.jpg|tiff|*.tif";
            opDlg.FilterIndex = 2;
            if (opDlg.ShowDialog() == true)
            {
                //ImageGalleryVM.DisplayImage = LoadImage(opDlg.FileName);
                //GC.Collect();
                //Uri fileUri = new Uri(opDlg.FileName);
                //BitmapDecoder decoder = BitmapDecoder.Create(fileUri, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
                //BitmapFrame image = decoder.Frames[0];
                //Workspace.This.Owner.mainWindow.imageGallery.image.Source = image;

                //BitmapImage image = new BitmapImage();
                //using (FileStream fs = File.OpenRead(opDlg.FileName))
                //{
                //    image.BeginInit();
                //    image.StreamSource = fs;
                //    image.CacheOption = BitmapCacheOption.OnLoad;
                //    image.CreateOptions = BitmapCreateOptions.None;
                //    //image.DecodePixelWidth = 1000;
                //    image.EndInit();
                //}
                ImageGalleryVM.WriteableBackImage = LoadWriteableImage(opDlg.FileName);
                ImageGalleryVM.DisplayImage = new WriteableBitmap(ImageGalleryVM.WriteableBackImage.PixelWidth,
                    ImageGalleryVM.WriteableBackImage.PixelHeight, 96, 96, PixelFormats.Gray8, null);
                //                ImageGalleryVM.DisplayImage = LoadWriteableImage(opDlg.FileName);
                Title = Path.GetFileName(opDlg.FileName) + "-" + SoftwareName;
                Workspace.This.IsImageLoaded = true;
                //ImageGalleryVM.DisplayImage = LoadImage(opDlg.FileName);
            }
            Task.Run(() =>
            {
            });
        }
        public bool CanExecuteOpenImageComand(object parameter)
        {
            return true;
        }
        private WriteableBitmap LoadWriteableImage(string @sFileName)
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
        public BitmapImage LoadImage(string @sFileName)
        {
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.StreamSource = new MemoryStream(File.ReadAllBytes(@sFileName));
            bitmap.EndInit();
            return bitmap;
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
            Task.Run(() =>
            {
                ImageGalleryVM.DisplayImage = null;
                Workspace.This.IsImageLoaded = false;
                Thread.Sleep(1000);     // wait for UI thread to release resources
                Title = SoftwareName;
                ClearMemory(Process.GetCurrentProcess());
            });
        }
        public bool CanExecuteCloseImageCommand(object parameter)
        {
            return ImageGalleryVM.DisplayImage != null;
        }
        #endregion CloseImageCommand

        #region ReleaseMemoryCommand
        public ICommand ReleaseMemoryCommand
        {
            get
            {
                if (_ReleaseMemoryCommand == null)
                {
                    _ReleaseMemoryCommand = new RelayCommand(ExecuteReleaseMemoryCommand, CanExecuteReleaseMemoryCommand);
                }
                return _ReleaseMemoryCommand;
            }
        }
        public void ExecuteReleaseMemoryCommand(object parameter)
        {
            string strPara = (string)parameter;
            if (strPara == "Self")
            {
                ClearMemory(Process.GetCurrentProcess());
            }
            else if (strPara == "All")
            {
                Process[] processes = Process.GetProcesses();
                foreach (Process process in processes)
                {
                    ClearMemory(process);
                }
            }
        }
        public bool CanExecuteReleaseMemoryCommand(object parameter)
        {
            return true;
        }
        #endregion ReleaseMemoryCommand

        #region Public Properties
        public string Title
        {
            get
            {
                return _Title;
            }
            set
            {
                if (_Title != value)
                {
                    _Title = value;
                    RaisePropertyChanged("Title");
                }
            }
        }
        public MainWindow Owner { get; set; }
        public ImageGalleryViewModel ImageGalleryVM
        {
            get
            {
                return _ImageGalleryVM;
            }
        }

        public bool IsImageLoaded
        {
            get
            {
                return _IsImageLoaded;
            }

            set
            {
                _IsImageLoaded = value;
                RaisePropertyChanged("IsImageLoaded");
            }
        }

        public ContrastControlViewModel ContrastControlVM
        {
            get
            {
                return _ContrastControlVM;
            }
        }
        #endregion Public Properties
    }
}
