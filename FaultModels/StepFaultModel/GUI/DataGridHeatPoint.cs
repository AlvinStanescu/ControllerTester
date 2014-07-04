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
        private double _baseUnit;
        private HeatPoint _containedHeatPoint;
        private Button _button;
        private bool _analyze;
        public event PropertyChangedEventHandler PropertyChanged;

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
                return (int)(_containedHeatPoint.X / _baseUnit);
            }
        }

        public int FinalDesiredRegion 
        {
            get
            {
                return (int)(_containedHeatPoint.Y / _baseUnit);
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

        public Button RunButton 
        {
            get
            {
                return _button;
            }

            private set 
            {
                _button = value;
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

        public DataGridHeatPoint(HeatPoint hp, double baseUnit, string requirement)
        {
            this.Requirement = requirement;
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
