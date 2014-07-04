using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace FM4CC.FaultModels.Step
{
    internal class TestGenerationViewModel
    {
        public ICollectionView ExplorationResults { get; private set; }

        internal TestGenerationViewModel(List<DataGridHeatPoint> explorationResults)
        {
            ExplorationResults = CollectionViewSource.GetDefaultView(explorationResults);
        }
    }
}
