using FM4CC;
using FM4CC.Util.Heatmap.Effects;
using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace FM4CC.Util.Heatmap
{
    public partial class RadialHeatMapControl : HeatMapControlBase
    {
        private const int heatRadius = 8;
        private RenderTargetBitmap intensityMap;
        private Rectangle clearRectangle;

        public RadialHeatMapControl()
        {
            InitializeComponent();
        }

        #region Private Method

        protected override void CreateHeatMap()
        {
            intensityMap = new RenderTargetBitmap(500, 500, 96, 96, PixelFormats.Pbgra32);
            AddativeBlendClear cleareffect = new AddativeBlendClear();
            cleareffect.ClearColor = Color.FromArgb(0x01, 0xFF, 0xFF, 0xFF);

            ClearIntensityMap();
            Size sz = new Size(intensityMap.PixelWidth, intensityMap.PixelHeight);

            // Create the clear rectangle, we need this to render a fade pass.
            clearRectangle = new Rectangle();
            clearRectangle.Fill = new ImageBrush(intensityMap);
            clearRectangle.Effect = cleareffect;
            clearRectangle.Measure(sz);
            clearRectangle.Arrange(new Rect(sz));

            // Connect the intensity map containing our heat to our image.
            HeatMapImage.Source = intensityMap;
        }

        protected override void CreateGrid(Grid heatmapGrid)
        {
            base.CreateGrid(heatmapGrid);

            this.HeatMapImage.SetValue(Grid.RowProperty, 1);
            this.HeatMapImage.SetValue(Grid.RowSpanProperty, HeatMapSource.Rows);
            this.HeatMapImage.SetValue(Grid.ColumnProperty, 1);
            this.HeatMapImage.SetValue(Grid.ColumnSpanProperty, HeatMapSource.Columns);
        }

        public override void RenderIntensityMap()
        {
            if (HeatMapSource.HeatPoints != null)
            {
                RadialGradientBrush radialBrush = new RadialGradientBrush();

                foreach (HeatPoint point in HeatMapSource.HeatPoints)
                {
                    DrawingVisual dv = new DrawingVisual();
                    using (DrawingContext ctx = dv.RenderOpen())
                    {
                        radialBrush.GradientStops.Clear();
                        byte intensity = (byte)((point.Intensity-HeatMapSource.MinIntensity)/(HeatMapSource.MaxIntensity - HeatMapSource.MinIntensity)*255);
                        radialBrush.GradientStops.Add(new GradientStop(Color.FromArgb(intensity, 0, 0, 0), 0.0));
                        radialBrush.GradientStops.Add(new GradientStop(Color.FromArgb(0, 0, 0, 0), 1));
                        
                        ctx.DrawRectangle(radialBrush, null, new Rect((point.X - (double)HeatMapSource.ColumnFromValue) / (double)(HeatMapSource.ColumnToValue - HeatMapSource.ColumnFromValue) * 500.0, 500.0 - (point.Y - (double)HeatMapSource.RowFromValue) / (double)(HeatMapSource.RowToValue - HeatMapSource.RowFromValue) * 500.0, heatRadius * 2, heatRadius * 2));
                    }
                    intensityMap.Render(dv);
                }
            }
        }

        public override void ClearIntensityMap()
        {
            DrawingVisual dv = new DrawingVisual();
            using (DrawingContext ctx = dv.RenderOpen())
            {
                ctx.DrawRectangle(Brushes.White, null, new Rect(0, 0, intensityMap.PixelWidth, intensityMap.PixelHeight));
            }
            intensityMap.Render(dv);
        }

        internal void Refresh()
        {
            if (HeatMapSource.Columns > 0 && HeatMapSource.Rows > 0)
            {
                RenderIntensityMap();
            }
        }

        #endregion
        
        protected override Grid GetHeatMapGrid()
        {
            return this.HeatMapGrid;
        }
    }    

}
