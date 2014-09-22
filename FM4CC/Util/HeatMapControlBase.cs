using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace FM4CC.Util.Heatmap
{
    public abstract class HeatMapControlBase : UserControl
    {
        #region Properties

        public HeatMapDataSource HeatMapSource
        {
            get
            {
                return _source;
            }
            set
            {
                _source = value;
                CreateMap(GetHeatMapGrid());
            }
        }

        private HeatMapDataSource _source;

        #endregion

        #region HeatMap
        
        protected HeatMapControlBase()
        {

        }

        private void CreateMap(Grid heatmapGrid)
        {
            if (HeatMapSource.Columns > 0 && HeatMapSource.Rows > 0)
            {
                heatmapGrid.ColumnDefinitions.Clear();
                heatmapGrid.RowDefinitions.Clear();

                CreateGrid(heatmapGrid);
                CreateColumnHeaders(heatmapGrid);
                CreateRowHeaders(heatmapGrid);
                CreateHeatMap();
                RenderIntensityMap();
            }
        }

        protected virtual void CreateGrid(Grid heatmapGrid)
        {
            ColumnDefinition headerColumn = new ColumnDefinition();
            headerColumn.Width = new GridLength(60);
            heatmapGrid.ColumnDefinitions.Add(headerColumn);

            for (int i = 0; i < HeatMapSource.Columns; ++i)
            {
                ColumnDefinition dc = new ColumnDefinition();
                dc.Width = new GridLength(30);
                heatmapGrid.ColumnDefinitions.Add(dc);

                Border columnBorder = new Border();
                columnBorder.BorderThickness = new Thickness(0.5);
                columnBorder.BorderBrush = Brushes.White;
                columnBorder.SetValue(Grid.ColumnProperty, i + 1);
                columnBorder.SetValue(Grid.RowProperty, 1);
                columnBorder.SetValue(Grid.RowSpanProperty, HeatMapSource.Rows);
                columnBorder.SetValue(Panel.ZIndexProperty, 255);
                heatmapGrid.Children.Add(columnBorder);
            }

            ColumnDefinition rightColumn = new ColumnDefinition();
            rightColumn.Width = new GridLength(30);
            heatmapGrid.ColumnDefinitions.Add(rightColumn);

            RowDefinition topRow = new RowDefinition();
            topRow.Height = new GridLength(30);
            heatmapGrid.RowDefinitions.Add(topRow);

            for (int i = 0; i < HeatMapSource.Rows; ++i)
            {
                RowDefinition dr = new RowDefinition();
                dr.Height = new GridLength(30);
                heatmapGrid.RowDefinitions.Add(dr);

                Border rowBorder = new Border();
                rowBorder.BorderThickness = new Thickness(0.5);
                rowBorder.BorderBrush = Brushes.White;
                rowBorder.SetValue(Grid.RowProperty, i+1);
                rowBorder.SetValue(Grid.ColumnProperty, 1);
                rowBorder.SetValue(Grid.ColumnSpanProperty, HeatMapSource.Columns);
                rowBorder.SetValue(Panel.ZIndexProperty, 255);

                heatmapGrid.Children.Add(rowBorder);
            }

            RowDefinition headerRow = new RowDefinition();
            headerRow.Height = new GridLength(60);
            heatmapGrid.RowDefinitions.Add(headerRow);

        }

        /// <summary>
        /// Create column header
        /// </summary>
        private void CreateColumnHeaders(Grid heatmapGrid)
        {
            for (int i = 0; i <= HeatMapSource.Columns; ++i)
            {
                Label lbl = new Label();
                lbl.FontSize = 12;
                lbl.FontFamily = new FontFamily("Segoe UI");

                lbl.Content = (HeatMapSource.ColumnFromValue + (HeatMapSource.ColumnToValue - HeatMapSource.ColumnFromValue) / HeatMapSource.Columns * i).ToString();
                
                lbl.Background = Brushes.Transparent;

                lbl.SetValue(Grid.RowProperty, HeatMapSource.Rows+1);
                lbl.SetValue(Grid.ColumnProperty, i);
                lbl.SetValue(Grid.ColumnSpanProperty, 2);

                lbl.VerticalAlignment = System.Windows.VerticalAlignment.Top;

                if (i != 0)
                {
                    lbl.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                }
                else
                {
                    lbl.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
                    lbl.Margin = new Thickness(0, 0, 22, 0);
                }

                heatmapGrid.Children.Add(lbl);
            }
        }

        /// <summary>
        /// Create row header
        /// </summary>
        private void CreateRowHeaders(Grid heatmapGrid)
        {
            for (int i = 0; i <= HeatMapSource.Rows; ++i)
            {
                Label lbl = new Label();
                lbl.FontSize = 12;
                lbl.FontFamily = new FontFamily("Segoe UI");

                lbl.Content = (HeatMapSource.RowFromValue + (HeatMapSource.RowToValue - HeatMapSource.RowFromValue) / HeatMapSource.Rows * (HeatMapSource.Rows - i)).ToString();

                lbl.Background = Brushes.Transparent;

                lbl.SetValue(Grid.ColumnProperty, 0);
                lbl.SetValue(Grid.RowProperty, i);
                lbl.SetValue(Grid.RowSpanProperty, 2);
                lbl.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;

                if (i != HeatMapSource.Rows)
                {
                    lbl.VerticalAlignment = System.Windows.VerticalAlignment.Center; 
                }
                else
                {
                    lbl.VerticalAlignment = System.Windows.VerticalAlignment.Top; 
                    lbl.Margin = new Thickness(0, 15, 0, 0);
                }

                heatmapGrid.Children.Add(lbl);
            }
        }
        #endregion

        public abstract void RenderIntensityMap();
        public abstract void ClearIntensityMap();
        protected abstract void CreateHeatMap();
        protected abstract Grid GetHeatMapGrid();

    }
}
