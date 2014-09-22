using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FM4CC.Simulation
{
    public class ModelRegressionSettings
    {
        public int TrainingSetSizeEqualDistance { get; set; }
        public int TrainingSetSizeRandom { get; set; }
        public int ValidationSetSize { get; set; }
        public int RefinedCandidatePoints { get; set; }
        public int RefinementPoints { get; set; }
        public double ModelQuality { get; set; }

        public ModelRegressionSettings()
        {
            ModelQuality = 1.0;
            TrainingSetSizeEqualDistance = 256;
            TrainingSetSizeRandom = 64;
            ValidationSetSize = 64;
            RefinedCandidatePoints = 3;
            RefinementPoints = 16;
        }
    }
}
