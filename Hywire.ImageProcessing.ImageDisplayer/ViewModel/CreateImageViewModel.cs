using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure.WPF.Framework;
using System.Windows.Input;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Drawing;
using Microsoft.Win32;
using System.IO;

namespace Hywire.ImageProcessing.ImageDisplayer.ViewModel
{
    public class CreateImageViewModel : ViewModelBase
    {
        #region Private Fields
        int _PixelWidth = 100;
        int _PixelHeight = 100;
        RelayCommand _SaveCloseCommand = null;
        #endregion Private Fields

        #region SaveCloseCommand
        public ICommand SaveCloseCommand
        {
            get
            {
                if (_SaveCloseCommand == null)
                {
                    _SaveCloseCommand = new RelayCommand(ExecuteSaveCloseCommand, CanExecuteSaveCloseCommand);
                }
                return _SaveCloseCommand;
            }
        }

        public void ExecuteSaveCloseCommand(object parameter)
        {
            if (PixelWidth <= 0 || PixelHeight <= 0)
            {
                MessageBox.Show("Invalid parameters!");
                return;
            }

            try
            {
                SaveFileDialog saveDlg = new SaveFileDialog();
                saveDlg.Filter = "Tiff|*.tif";
                if (saveDlg.ShowDialog() == true)
                {
                    ushort[] imgDataBytes = new ushort[PixelWidth * PixelHeight * 2];
                    for (int row = 0; row < PixelHeight; row++)
                    {
                        for (int col = 0; col < PixelWidth; col++)
                        {
                            if ((row + col) % 2 == 0)
                            {
                                imgDataBytes[row * PixelHeight + col] = (ushort)(65535 / PixelHeight * row);
                            }
                            else
                            {
                                imgDataBytes[row * PixelHeight + col] = 0;
                            }
                        }
                    }

                    WriteableBitmap image = Azure.Image.Processing.ImageProcessing.FrameToBitmap(imgDataBytes, PixelWidth, PixelHeight);
                    using (var stream = File.Create(saveDlg.FileName))
                    {
                        Azure.Image.Processing.ImageProcessing.Save(stream, Azure.Image.Processing.ImageProcessing.TIFF_FILE, image);
                    }
                    Workspace.This.CreateImageWind.Close();
                    imgDataBytes = null;
                }
            }
            catch
            {
                MessageBox.Show("Failed to save the image!");
            }
        }
        public bool CanExecuteSaveCloseCommand(object parameter)
        {
            return true;
        }
        #endregion SaveCloseCommand

        #region Public Properties
        public int PixelWidth
        {
            get
            {
                return _PixelWidth;
            }
            set
            {
                if (_PixelWidth != value)
                {
                    _PixelWidth = value;
                    RaisePropertyChanged("PixelWidth");
                }
            }
        }

        public int PixelHeight
        {
            get
            {
                return _PixelHeight;
            }
            set
            {
                if (_PixelHeight != value)
                {
                    _PixelHeight = value;
                    RaisePropertyChanged("PixelHeight");
                }
            }
        }
        #endregion Public Properties
    }
}
