using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Hywire.ImageProcessing.ImageDisplayer.ViewModel;

namespace Hywire.ImageProcessing.ImageDisplayer.View
{
    /// <summary>
    /// ImageGallery.xaml 的交互逻辑
    /// </summary>
    public partial class ImageGallery : UserControl
    {
        #region Private data...

        private Point origin;  // Original Offset of image
        private Point start;   // Original Position of the mouse

        private double _ZoomRate = 1;
        //private double _LastZoomRate = 1;
        private double _ImageZoomRate = 1;
        //private MatrixTransform _MatrixTransform;

        private const double _RateStep = 1.1;
        private double dShiftX = 0;
        private double dShiftY = 0;

        #endregion

        public ImageGallery()
        {
            InitializeComponent();
        }

        private void _Image_ManipulationStarting(object sender, ManipulationStartingEventArgs e)
        {
            e.ManipulationContainer = _DisplayCanvas;

            e.Mode = ManipulationModes.Scale | ManipulationModes.Translate;
        }

        private void _Image_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            FrameworkElement element = (FrameworkElement)e.Source;
            element.Opacity = 1;
        }

        private void _Image_ManipulationDelta(object sender, ManipulationDeltaEventArgs e)
        {
            //promised the miniscale is 1
            double dZoomStep = 1;
            if (_ZoomRate * e.DeltaManipulation.Scale.X < 1)
            {
                dZoomStep = 1 / _ZoomRate;
                _ZoomRate = 1;
            }
            else
            {
                dZoomStep = e.DeltaManipulation.Scale.X;
                _ZoomRate *= e.DeltaManipulation.Scale.X;
            }

            double dTanslationX = e.DeltaManipulation.Translation.X;
            double dTanslationY = e.DeltaManipulation.Translation.Y;

            //manipulate the image is in canvas when image is shifted
            Point ImageInCanvasPtLT = _DisplayImage.TranslatePoint(new Point(0, 0), _DisplayCanvas);
            Point ImageInCanvasPtRB = _DisplayImage.TranslatePoint(new Point(_DisplayImage.ActualWidth, _DisplayImage.ActualHeight), _DisplayCanvas);
            if (_DisplayImage.ActualWidth * _ZoomRate < _DisplayCanvas.ActualWidth)
            {
                //dTanslationX = 0;
                if (dTanslationX < 0)
                {
                    if (ImageInCanvasPtLT.X + dTanslationX < 0)
                    {
                        dTanslationX = -ImageInCanvasPtLT.X;
                    }
                }
                else
                {
                    if (ImageInCanvasPtRB.X + dTanslationX > _DisplayCanvas.ActualWidth)
                    {
                        dTanslationX = _DisplayCanvas.ActualWidth - ImageInCanvasPtRB.X;
                    }
                }
            }
            else
            {
                if (dTanslationX < 0)
                {
                    if (ImageInCanvasPtRB.X + dTanslationX < _DisplayCanvas.ActualWidth)
                    {
                        dTanslationX = _DisplayCanvas.ActualWidth - ImageInCanvasPtRB.X;
                    }
                }
                else
                {
                    if (ImageInCanvasPtLT.X + dTanslationX > 0)
                    {
                        dTanslationX = -ImageInCanvasPtLT.X;
                    }
                }
            }

            if (_DisplayImage.ActualHeight * _ZoomRate < _DisplayCanvas.ActualHeight)
            {
                //dTanslationY = 0;
                if (dTanslationY < 0)
                {
                    if (ImageInCanvasPtLT.Y + dTanslationY < 0)
                    {
                        dTanslationY = -ImageInCanvasPtLT.Y;
                    }
                }
                else
                {
                    if (ImageInCanvasPtRB.Y + dTanslationY > _DisplayCanvas.ActualHeight)
                    {
                        dTanslationY = _DisplayCanvas.ActualHeight - ImageInCanvasPtRB.Y;
                    }
                }
            }
            else
            {
                if (dTanslationY < 0)
                {
                    if (ImageInCanvasPtRB.Y + dTanslationY < _DisplayCanvas.ActualHeight)
                    {
                        dTanslationY = _DisplayCanvas.ActualHeight - ImageInCanvasPtRB.Y;
                    }
                }
                else
                {
                    if (ImageInCanvasPtLT.Y + dTanslationY > 0)
                    {
                        dTanslationY = -ImageInCanvasPtLT.Y;
                    }
                }
            }

            //shift
            dShiftX += dTanslationX;
            dShiftY += dTanslationY;

            FrameworkElement element = (FrameworkElement)e.Source;
            //element.Opacity = 0.5;

            Matrix matrix = ((MatrixTransform)element.RenderTransform).Matrix;

            var deltaManipulation = e.DeltaManipulation;

            Point center = new Point(_ImageBorder.ActualWidth / 2, _ImageBorder.ActualHeight / 2);
            center = _ImageBorder.TranslatePoint(center, _DisplayImage);

            matrix.ScaleAt(dZoomStep, dZoomStep, center.X, center.Y);
            matrix.Translate(dTanslationX, dTanslationY);

            ((MatrixTransform)element.RenderTransform).Matrix = matrix;
            //_LastZoomRate = _ZoomRate;
        }

        private void _DisplayImage_MouseMove(object sender, MouseEventArgs e)
        {
            ImageGalleryViewModel viewModel = (ImageGalleryViewModel)DataContext;
            if (viewModel == null) { return; }

            if (_DisplayImage.Source == null)
            {
                return;
            }

            Point point = new Point((e.GetPosition(_DisplayImage).X * _ImageZoomRate), (e.GetPosition(_DisplayImage).Y * _ImageZoomRate));

            viewModel.PixelX = ((int)point.X);
            viewModel.PixelY = ((int)point.Y);

            byte[] pixels = new byte[viewModel.DisplayImage.Format.BitsPerPixel / 8];
            var stride = viewModel.DisplayImage.Format.BitsPerPixel / 8 * viewModel.DisplayImage.PixelWidth;
            var offset = viewModel.PixelY * stride + viewModel.PixelX * viewModel.DisplayImage.Format.BitsPerPixel / 8;
            viewModel.DisplayImage.CopyPixels(new Int32Rect(viewModel.PixelX, viewModel.PixelY, 1, 1), pixels, stride, 0);

            if (viewModel.DisplayImage.Format == PixelFormats.Rgb24)
            {
                viewModel.PixelIntensity = string.Format("R: {0} G: {1} B: {2}", pixels[0], pixels[1], pixels[2]);
            }
            else if (viewModel.DisplayImage.Format == PixelFormats.Gray16)
            {
                viewModel.PixelIntensity = string.Format("{0}", pixels[1] * 255 + pixels[0]);
            }

            if (_DisplayImage.IsMouseCaptured)
            {
                Point p = e.MouseDevice.GetPosition(_DisplayCanvas);

                Matrix m = _DisplayImage.RenderTransform.Value;
                //m.OffsetX = origin.X + (p.X - start.X);
                //m.OffsetY = origin.Y + (p.Y - start.Y);
                double deltaX = origin.X + (p.X - start.X);
                double deltaY = origin.Y + (p.Y - start.Y);

                if (p.X > 0 && p.Y > 0 && p.X < _DisplayCanvas.ActualWidth && p.Y < _DisplayCanvas.ActualHeight)
                {
                    //m.OffsetX = deltaX;
                    //Matrix m = _DisplayImage.RenderTransform.Value;
                    //m.Translate(deltaX, deltaY);
                    m.OffsetX = deltaX;
                    m.OffsetY = deltaY;

                    _DisplayImage.RenderTransform = new MatrixTransform(m);
                    //_DisplayImage.ReleaseMouseCapture();
                    //viewModel.Matrix = _DisplayImage.RenderTransform.Value;
                }
            }
        }

        private void _DisplayImage_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (_DisplayImage.Source == null) { return; }

            Point p = e.MouseDevice.GetPosition(_DisplayImage);

            Matrix m = _DisplayImage.RenderTransform.Value;
            if (e.Delta > 0)
            {
                _ZoomRate *= _RateStep;
                m.ScaleAtPrepend(_RateStep, _RateStep, p.X, p.Y);
                _DisplayImage.RenderTransform = new MatrixTransform(m);
            }
            else
            {
                if (_ZoomRate / _RateStep < 1.0)
                {
                    _ZoomRate = 1.0;
                }
                else
                {
                    _ZoomRate /= _RateStep;
                }

                if (_ZoomRate == 1)
                {
                    RecoverTransform();
                }
                else
                {
                    m.ScaleAtPrepend(1 / _RateStep, 1 / _RateStep, p.X, p.Y);
                    _DisplayImage.RenderTransform = new MatrixTransform(m);
                }
            }
        }

        /// <summary>
        /// Fit display image to window
        /// </summary>
        public void RecoverTransform()
        {
            _DisplayImage.RenderTransform = new MatrixTransform(_ZoomRate, 0, 0, _ZoomRate, -dShiftX, -dShiftY);
            dShiftX = 0;
            dShiftY = 0;
            _DisplayImage.RenderTransform = new MatrixTransform(1, 0, 0, 1, -dShiftX, -dShiftY);
        }

        private void _DisplayImage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (_DisplayImage.Source == null)
            {
                _ImageZoomRate = 1.0;
                return;
            }
            if (_DisplayImage.ActualWidth < _DisplayImage.Width)
            {
                _ImageZoomRate = ((BitmapImage)_DisplayImage.Source).PixelHeight / _DisplayImage.ActualHeight;
            }
            else if (_DisplayImage.ActualHeight < _DisplayImage.Height)
            {
                _ImageZoomRate = ((BitmapImage)_DisplayImage.Source).PixelWidth / _DisplayImage.ActualWidth;
            }
        }

        private void _DisplayImage_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (_DisplayImage.IsMouseCaptured) return;

            _DisplayImage.CaptureMouse();

            start = e.GetPosition(_DisplayCanvas);
            origin.X = _DisplayImage.RenderTransform.Value.OffsetX;
            origin.Y = _DisplayImage.RenderTransform.Value.OffsetY;
        }

        private void _DisplayImage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _DisplayImage.ReleaseMouseCapture();
        }
    }
}
