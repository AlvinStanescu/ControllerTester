using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Media3D;

namespace FM4CC.Util.Heatmap.Effects
{   
    public class HeatColorizer : System.Windows.Media.Effects.ShaderEffect
    {
        public static readonly DependencyProperty InputProperty = ShaderEffect.RegisterPixelShaderSamplerProperty("Input", typeof(HeatColorizer), 0, SamplingMode.Auto);
        public static readonly DependencyProperty PaletteProperty = ShaderEffect.RegisterPixelShaderSamplerProperty("Palette", typeof(HeatColorizer), 1, SamplingMode.Auto);
        
        public HeatColorizer()
        {
            PixelShader pixelShader = new PixelShader();
            pixelShader.UriSource = new Uri("/FM4CC;component/Util/Effects/HeatColorizer.ps", UriKind.Relative);
            this.PixelShader = pixelShader;
            this.UpdateShaderValue(InputProperty);
            this.UpdateShaderValue(PaletteProperty);
        }
        
        public virtual System.Windows.Media.Brush Input
        {
            get
            {
                return ((System.Windows.Media.Brush)(this.GetValue(InputProperty)));
            }
            set
            {
                this.SetValue(InputProperty, value);
            }
        }
        
        public virtual System.Windows.Media.Brush Palette
        {
            get
            {
                return ((System.Windows.Media.Brush)(this.GetValue(PaletteProperty)));
            }
            set
            {
                this.SetValue(PaletteProperty, value);
            }
        }
    }
}
