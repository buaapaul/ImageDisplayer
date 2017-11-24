//------------------------------------------------------------------------------
// <auto-generated>
//     ?????????
//     ?????:4.0.30319.42000
//
//     ???????????????????????
//     ????????????????
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Media3D;


namespace Hywire.ShaderEffectLibrary {
	
	public class DisplayRangeEffect : ShaderEffect {
		public static readonly DependencyProperty InputProperty = ShaderEffect.RegisterPixelShaderSamplerProperty("Input", typeof(DisplayRangeEffect), 0);
		public static readonly DependencyProperty LowRangeProperty = DependencyProperty.Register("LowRange", typeof(double), typeof(DisplayRangeEffect), new UIPropertyMetadata(((double)(0D)), PixelShaderConstantCallback(0)));
		public static readonly DependencyProperty HighRangeProperty = DependencyProperty.Register("HighRange", typeof(double), typeof(DisplayRangeEffect), new UIPropertyMetadata(((double)(1D)), PixelShaderConstantCallback(1)));
        public static readonly DependencyProperty GammaProperty = DependencyProperty.Register("Gamma", typeof(double), typeof(DisplayRangeEffect), new UIPropertyMetadata(((double)(1D)), PixelShaderConstantCallback(2)));
        public DisplayRangeEffect() {
			PixelShader pixelShader = new PixelShader();
			pixelShader.UriSource = Global.MakePackUri("ShaderSource/DisplayRangeEffect.ps");
            this.PixelShader = pixelShader;

			this.UpdateShaderValue(InputProperty);
			this.UpdateShaderValue(LowRangeProperty);
			this.UpdateShaderValue(HighRangeProperty);
            this.UpdateShaderValue(GammaProperty);
        }
        public Brush Input {
            get
            {
                return ((Brush)(this.GetValue(InputProperty)));
            }
            set
            {
                this.SetValue(InputProperty, value);
            }
        }
        public double LowRange {
			get {
				return ((double)(this.GetValue(LowRangeProperty)));
			}
			set {
				this.SetValue(LowRangeProperty, value);
			}
		}
		public double HighRange {
            get
            {
                return ((double)(this.GetValue(HighRangeProperty)));
            }
            set
            {
                this.SetValue(HighRangeProperty, value);
            }
        }
        public double Gamma
        {
            get
            {
                return ((double)(this.GetValue(GammaProperty)));
            }
            set
            {
                this.SetValue(GammaProperty, value);
            }
        }
    }
}
