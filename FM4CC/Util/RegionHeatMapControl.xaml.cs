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

namespace FM4CC.Util.Heatmap
{
    /// <summary>
    /// Interaction logic for RegionHeatMapControl.xaml
    /// </summary>
    public partial class RegionHeatMapControl : HeatMapControlBase
    {
        byte[,] worstCaseHeatPoints;
        public RegionHeatMapControl()
        {
            InitializeComponent();
        }

        protected override void CreateHeatMap()
        {
            worstCaseHeatPoints = new byte[HeatMapSource.Columns+1, HeatMapSource.Rows+1];

            decimal rowLength = (HeatMapSource.RowToValue - HeatMapSource.RowFromValue) / HeatMapSource.Rows;
            decimal columnLength = (HeatMapSource.ColumnToValue - HeatMapSource.ColumnFromValue) / HeatMapSource.Columns;

            foreach (HeatPoint hp in HeatMapSource.HeatPoints)
            {
                int column = (hp.X.CompareTo((double)HeatMapSource.ColumnToValue) == 0) ? HeatMapSource.Columns - 1 : (int)(((decimal)hp.X - HeatMapSource.ColumnFromValue) / columnLength);
                int row = (hp.Y.CompareTo((double)HeatMapSource.RowToValue) == 0) ? HeatMapSource.Rows - 1 : (int)(((decimal)hp.Y - HeatMapSource.RowFromValue) / rowLength);

                worstCaseHeatPoints[column, row] = (byte)((hp.Intensity - HeatMapSource.MinIntensity) / (HeatMapSource.MaxIntensity - HeatMapSource.MinIntensity) * 255);
            }
        }

        public override void RenderIntensityMap()
        {
            for (int i = 0; i < HeatMapSource.Columns; i++)
            {
                for (int j = 0; j < HeatMapSource.Rows; j++)
                {
                    byte intensity = worstCaseHeatPoints[i, j];
                    Rectangle r = new Rectangle();
                    r.SetValue(Grid.ColumnProperty, i + 1);
                    r.SetValue(Grid.RowProperty, HeatMapSource.Rows - j);
                    r.Fill = new SolidColorBrush(Color.FromRgb((byte)Math.Abs(intensity), (byte)Math.Abs(intensity), (byte)Math.Abs(intensity)));
                    this.HeatMapGrid.Children.Add(r);
                }
            }
        }

        public override void ClearIntensityMap()
        {
            this.HeatMapGrid.Children.Clear();
        }

        protected override Grid GetHeatMapGrid()
        {
            return this.HeatMapGrid;
        }
    }
}
