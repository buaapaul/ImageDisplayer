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
using Fluent;
using System.Windows.Threading;
using System.Threading;

namespace Hywire.ImageProcessing.ImageDisplayer
{
    /// <summary>
    /// MainWindowContent.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindowContent : UserControl
    {
        public MainWindowContent()
        {
            InitializeComponent();
        }

        private enum Theme
        {
            Office2010,
            Office2013
        }

        private Theme? currentTheme;

        private void DontUseDwm_Click(object sender, RoutedEventArgs e)
        {
            var control = sender as UIElement;

            if (control == null)
            {
                return;
            }

            var window = Window.GetWindow(control) as RibbonWindow;

            if (window == null)
            {
                return;
            }

            window.DontUseDwm = this.DontUseDwm.IsChecked.GetValueOrDefault();
        }

        private void silverThemeBtn_Click(object sender, RoutedEventArgs e)
        {
            this.ChangeTheme(Theme.Office2010, "pack://application:,,,/Fluent;component/Themes/Office2010/Silver.xaml");
        }
        private void ChangeTheme(Theme theme, string color)
        {
            this.Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, (ThreadStart)(() =>
            {
                var owner = Window.GetWindow(this);
                if (owner != null)
                {
                    owner.Resources.BeginInit();

                    if (owner.Resources.MergedDictionaries.Count > 0)
                    {
                        owner.Resources.MergedDictionaries.RemoveAt(0);
                    }

                    if (string.IsNullOrEmpty(color) == false)
                    {
                        owner.Resources.MergedDictionaries.Add(new ResourceDictionary { Source = new Uri(color) });
                    }

                    owner.Resources.EndInit();
                }

                if (this.currentTheme != theme)
                {
                    Application.Current.Resources.BeginInit();
                    switch (theme)
                    {
                        case Theme.Office2010:
                            Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary { Source = new Uri("pack://application:,,,/Fluent;component/Themes/Generic.xaml") });
                            Application.Current.Resources.MergedDictionaries.RemoveAt(0);
                            break;
                        case Theme.Office2013:
                            Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary { Source = new Uri("pack://application:,,,/Fluent;component/Themes/Office2013/Generic.xaml") });
                            Application.Current.Resources.MergedDictionaries.RemoveAt(0);
                            break;
                    }

                    this.currentTheme = theme;
                    Application.Current.Resources.EndInit();

                    if (owner != null)
                    {
                        owner.Style = null;
                        owner.Style = owner.FindResource("RibbonWindowStyle") as Style;
                        owner.Style = null;
                    }
                }
            }));
        }

        private void blackThemeBtn_Click(object sender, RoutedEventArgs e)
        {
            this.ChangeTheme(Theme.Office2010, "pack://application:,,,/Fluent;component/Themes/Office2010/Black.xaml");
        }

        private void blueThemeBtn_Click(object sender, RoutedEventArgs e)
        {
            this.ChangeTheme(Theme.Office2010, "pack://application:,,,/Fluent;component/Themes/Office2010/Blue.xaml");
        }
    }
}
