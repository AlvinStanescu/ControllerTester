using FM4CC.Util.Heatmap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FM4CC.FaultModels.Step
{
    internal class StepHeatPoint : HeatPoint
    {
        internal bool PhysicalRangeExceeded { get; set; }
        internal double WorstIntensity { get; set; }

        internal StepHeatPoint(double x, double y, double intensity, double worstIntensity, bool rangeExceeded) : base (x,y,intensity)
        {
            this.PhysicalRangeExceeded = rangeExceeded;
            this.WorstIntensity = worstIntensity;
        }
    }
}
