using System;
using System.Windows.Controls;
using LiveCharts;

namespace SegmentationApp
{
    /// <inheritdoc cref="UserControl" />
    /// <summary>
    /// Interaction logic for StackedRow.xaml
    /// </summary>
    public partial class StackedRow
    {
        /// <summary>
        /// Sets or Gets collection of series that are bound to the <see cref="StackedRow"/> control.
        /// </summary>
        public SeriesCollection SeriesCollection { get; set; }

        /// <summary>
        /// Contains the labels that are used for the Y-Axis of the graph
        /// </summary>
        public string[] Labels { get; set; }


        /// <summary>
        /// Sets or Gets the format of the string label for the graph separators.
        /// </summary>
        public Func<double, string> Formatter { get; set; }


        /// <inheritdoc />
        /// <summary>
        /// Creates a new instance of the <see cref="StackedRow"/> class.
        /// </summary>
        public StackedRow()
        {
            InitializeComponent();
            SeriesCollection = new SeriesCollection();
        }

        /// <summary>
        /// Sets the datacontext of the Stacked Row control to this class.
        /// </summary>
        public void BindContext()
        {
            DataContext = this;
        }
    }
}
