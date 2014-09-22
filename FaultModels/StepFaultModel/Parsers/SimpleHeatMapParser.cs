using FM4CC.Util;
using FM4CC.Util.Heatmap;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FM4CC.FaultModels.Step.Parsers
{
    internal static class SimpleHeatMapParser
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
                bool exceededRange = Convert.ToBoolean(Int32.Parse(values[2]));

                for (int i = 0; i < 6; i++)
                {
                    double x = 0.0;
                    double y = 0.0;

                    double intensity = 0.0;
                    double worstIntensity = 0.0;

                    if (i < 5)
                    {
                        x = Double.Parse(values[4 * i + 5], CultureInfo.InvariantCulture);
                        y = Double.Parse(values[4 * i + 6], CultureInfo.InvariantCulture);

                        intensity = ObjectiveFunctionValueParser.Parse(values[4 * i + 3]);
                        worstIntensity = ObjectiveFunctionValueParser.Parse(values[4 * i + 4]);
                    }
                    else
                    {
                        x = Double.Parse(values[13], CultureInfo.InvariantCulture);
                        y = Double.Parse(values[14], CultureInfo.InvariantCulture);

                        double baseUnit = ((double)(rowToValue - rowFromValue))/rows;
                        double regionStartY = 0.0;

                        if (y == (double)rowFromValue)
                        {
                            regionStartY = (double)rowFromValue;
                        }
                        else if (y == (double)rowToValue)
                        {
                            regionStartY = ((int)(y / baseUnit) - 1)*baseUnit;
                        }
                        else
                        {
                            regionStartY = ((int)(y / baseUnit))*baseUnit;
                        }

                        intensity = Math.Abs(ObjectiveFunctionValueParser.Parse(values[11]) / (ObjectiveFunctionValueParser.Parse(values[11])+regionStartY+baseUnit/2));
                        
                        worstIntensity = Math.Abs(ObjectiveFunctionValueParser.Parse(values[12])/(ObjectiveFunctionValueParser.Parse(values[12])+y));

                        if (Math.Abs(y) < 0.001 * (double)(rowFromValue - rowToValue))
                        {
                            worstIntensity = 0;
                        }
                        
                        if (intensity > worstIntensity)
                        {
                            intensity = worstIntensity;
                        }
                    
                    }

 
                    StepHeatPoint hp = new StepHeatPoint(x, y, intensity, worstIntensity, exceededRange);
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
