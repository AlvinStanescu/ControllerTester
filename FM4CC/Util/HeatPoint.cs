using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FM4CC.Util.Heatmap
{
    public class HeatPoint
    {
        public double X;
        public double Y;
        public double Intensity;

        public HeatPoint(double x, double y, double intensity)
        {
            X = x;
            Y = y;
            Intensity = intensity;
        }
    }
}
