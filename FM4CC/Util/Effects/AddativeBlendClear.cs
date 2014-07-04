using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Media3D;

namespace FM4CC.Util.Heatmap.Effects
{   
    public class AddativeBlendClear : System.Windows.Media.Effects.ShaderEffect
    {
        
        public static readonly DependencyProperty ClearColorProperty = DependencyProperty.Register("ClearColor", typeof(System.Windows.Media.Color), typeof(AddativeBlendClear), new UIPropertyMetadata(Color.FromArgb(255,0,0,0), PixelShaderConstantCallback(0)));
        public static readonly DependencyProperty InputProperty = ShaderEffect.RegisterPixelShaderSamplerProperty("Input", typeof(AddativeBlendClear), 0, SamplingMode.Auto);
        
        public AddativeBlendClear()
        {
            PixelShader pixelShader = new PixelShader();
            pixelShader.UriSource = new Uri("/FM4CC;component/Util/Effects/AddativeBlendClear.ps", UriKind.Relative);
            this.PixelShader = pixelShader;
            this.UpdateShaderValue(ClearColorProperty);
            this.UpdateShaderValue(InputProperty);
        }
        
        public virtual System.Windows.Media.Color ClearColor
        {
            get
            {
                return ((System.Windows.Media.Color)(this.GetValue(ClearColorProperty)));
            }
            set
            {
                this.SetValue(ClearColorProperty, value);
            }
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
    }
}
