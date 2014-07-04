using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FM4CC.Util.Heatmap
{
    public class HeatMapDataSource
    {
        public HeatMapDataSource()
        {
            HeatPoints = new List<HeatPoint>();
        }

        public string Name { get; set; }

        public IList<HeatPoint> HeatPoints { get; set; }

        public int Rows { get; set; }

        public int Columns { get; set; }

        public decimal RowFromValue { get; set; }
        public decimal RowToValue { get; set; }

        public decimal ColumnFromValue { get; set; }
        public decimal ColumnToValue { get; set; }

        public double MaxIntensity { get; set; }
        public double MinIntensity { get; set; }
    }
}
