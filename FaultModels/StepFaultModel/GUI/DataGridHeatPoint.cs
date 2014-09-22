using FM4CC.Util.Heatmap;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace FM4CC.FaultModels.Step
{
    internal class DataGridHeatPoint : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private double _baseUnit;
        private StepHeatPoint _containedHeatPoint;
        private bool _analyze;
        private double _desiredMax;
        private double _desiredMin;

        public bool PhysicalRangeExceeded 
        {
            get
            {
                return _containedHeatPoint.PhysicalRangeExceeded;
            }
        }

        public HeatPoint ContainedHeatPoint
        {
            get
            {
                return _containedHeatPoint;
            }
        }

        public bool Analyze
        {
            get
            {
                return _analyze;
            }
            set
            {
                _analyze = value;
                RaisePropertyChanged("Analyze");
            }
        }

        public double WorstIntensity
        {
            get
            {
                return _containedHeatPoint.WorstIntensity;
            }
        }


        public double Intensity 
        { 
            get 
            {
                return _containedHeatPoint.Intensity;
            }
        }

        public int InitialDesiredRegion
        {
            get
            {
                if (_containedHeatPoint.X == _desiredMin)
                {
                    return 0;
                }
                else if (_containedHeatPoint.X == _desiredMax)
                {
                    return (int)((_containedHeatPoint.X - _desiredMin) / _baseUnit) - 1;
                }
                else
                {
                    return (int)((_containedHeatPoint.X - _desiredMin) / _baseUnit);
                }
            }
        }

        public int FinalDesiredRegion 
        {
            get
            {
                if (_containedHeatPoint.Y == _desiredMin)
                {
                    return 0;
                }
                else if (_containedHeatPoint.Y == _desiredMax)
                {
                    return (int)((_containedHeatPoint.Y - _desiredMin) / _baseUnit) - 1;
                }
                else
                {
                    return (int)((_containedHeatPoint.Y - _desiredMin) / _baseUnit);
                }
            }
        }

        public double InitialDesired 
        { 
            get
            {
                return _containedHeatPoint.X;
            }
        }

        public double FinalDesired 
        { 
            get
            {
                return _containedHeatPoint.Y;
            }
        }

        public string Requirement { get; set; }

        public double BaseUnit
        {
            get
            {
                return _baseUnit;
            }
        }

        public DataGridHeatPoint(StepHeatPoint hp, double baseUnit, double desiredMin, double desiredMax, string requirement)
        {
            this.Requirement = requirement;
            _desiredMin = desiredMin;
            _desiredMax = desiredMax;
            _baseUnit = baseUnit;
            _containedHeatPoint = hp;
            _analyze = true;
        }

        #region Methods

        private void RaisePropertyChanged(string propertyName)
        {
            // take a copy to prevent thread issues
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion

    }
}
