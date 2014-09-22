using FM4CC.Util;
using FM4CC.Util.Heatmap;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FM4CC.FaultModels.Step.Parsers
{
    internal static class HeatMapParser
    {
        internal static IList<HeatMapDataSource> Parse(string filePath, decimal rowFromValue, decimal rowToValue, int rows, decimal columFromValue, decimal columnToValue, int columns, IList<string> selectedRequirements)
        {
            IList<HeatMapDataSource> retrievedData = new List<HeatMapDataSource>();
            retrievedData.Add(new HeatMapDataSource() { Name = "Stability", Rows = rows, Columns = columns, RowFromValue = rowFromValue, RowToValue = rowToValue, ColumnFromValue = columFromValue, ColumnToValue = columnToValue });
            retrievedData.Add(new HeatMapDataSource() { Name = "Precision", Rows = rows, Columns = columns, RowFromValue = rowFromValue, RowToValue = rowToValue, ColumnFromValue = columFromValue, ColumnToValue = columnToValue });
            retrievedData.Add(new HeatMapDataSource() { Name = "Smoothness", Rows = rows, Columns = columns, RowFromValue = rowFromValue, RowToValue = rowToValue, ColumnFromValue = columFromValue, ColumnToValue = columnToValue });
            retrievedData.Add(new HeatMapDataSource() { Name = "Responsiveness", Rows = rows, Columns = columns, RowFromValue = rowFromValue, RowToValue = rowToValue, ColumnFromValue = columFromValue, ColumnToValue = columnToValue });
            retrievedData.Add(new HeatMapDataSource() { Name = "Steadiness", Rows = rows, Columns = columns, RowFromValue = rowFromValue, RowToValue = rowToValue, ColumnFromValue = columFromValue, ColumnToValue = columnToValue });
            retrievedData.Add(new HeatMapDataSource() { Name = "NormalizedSmoothness", Rows = rows, Columns = columns, RowFromValue = rowFromValue, RowToValue = rowToValue, ColumnFromValue = columFromValue, ColumnToValue = columnToValue });

            IEnumerable<string> lines = System.IO.File.ReadLines(filePath);

            int cnt = 0;
            double[] maximumValue = new double[] { Double.NegativeInfinity, Double.NegativeInfinity, Double.NegativeInfinity, Double.NegativeInfinity, Double.NegativeInfinity, Double.NegativeInfinity };
            double[] minimumValue = new double[] { Double.PositiveInfinity, Double.PositiveInfinity, Double.PositiveInfinity, Double.PositiveInfinity, Double.PositiveInfinity, Double.PositiveInfinity };

            foreach (string line in lines)
            {
                if (cnt++ == 0) continue;
                string[] values = line.Split(',');
                double x = Double.Parse(values[0], CultureInfo.InvariantCulture);
                double y = Double.Parse(values[1], CultureInfo.InvariantCulture);
                bool exceededRange = Convert.ToBoolean(Int32.Parse(values[8]));

                for (int i = 0; i < 6; i++)
                {
                    double intensity = 0.0;

                    if (i < 5)
                    {
                        intensity = ObjectiveFunctionValueParser.Parse(values[i + 2]);
                    }
                    else
                    {
                        // normalized smoothness
                        double value = ObjectiveFunctionValueParser.Parse(values[4]);
                        intensity = value/(value + Math.Abs(y));

                        if (Math.Abs(x - y) < 0.001 * (double)(rowFromValue - rowToValue))
                        {
                            intensity = 0;
                        }
                    }

                    StepHeatPoint hp = new StepHeatPoint(x, y, intensity, intensity, exceededRange);
                    retrievedData[i].HeatPoints.Add(hp);

                    if (intensity > maximumValue[i]) maximumValue[i] = intensity;
                    if (intensity < minimumValue[i]) minimumValue[i] = intensity;
                }
            }

            for (int i = 0; i < 6; i++)
            {
                retrievedData[i].MaxIntensity = maximumValue[i];
                retrievedData[i].MinIntensity = minimumValue[i];
            }
            
            IList<HeatMapDataSource> dataSources = new List<HeatMapDataSource>();

            foreach (string requirement in selectedRequirements)
            {
                foreach (HeatMapDataSource hmds in retrievedData)
                {
                    if (hmds.Name == requirement)
                    {
                        dataSources.Add(hmds);
                    }
                }
            }

            return dataSources;
        }
    }
}
