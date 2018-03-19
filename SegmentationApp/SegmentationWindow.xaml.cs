using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using LiveCharts;
using LiveCharts.Wpf;
using SegmentationLibrary;

namespace SegmentationApp
{
    /// <inheritdoc cref="Window" />
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private ObservableCollection<Segment> _segments;

        // ReSharper disable once ConvertToAutoProperty
        // ReSharper disable once MemberCanBePrivate.Global
        /// <summary>
        /// Gets or Sets the collection of segments that the program is currently using.
        /// </summary>
        public ObservableCollection<Segment> Segments
        {
            get => _segments;
            set => _segments = value;
        }

        /// <inheritdoc />
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            Segments = new ObservableCollection<Segment>();
        }

        private void BtnReset_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void BtnPlaceInMemory_Click(object sender, RoutedEventArgs e)
        {
            int? memsize = IntMemSize.Value;
            int? required = 0;

            foreach (Segment t in Segments)
                required += t.Size;

            // throws an error if the total of all segment sizes exceeds the alloted memory
            try
            {
                if (memsize < required)
                    throw new NotEnoughMemoryException("Required memory for segment allocation exceeds Main Memory size!");
            }
            catch (NotEnoughMemoryException exception)
            {
                MessageBox.Show(exception.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            // Pass generated segments into segment table
            Debug.Assert(IntMemSize.Value != null, "IntMemSize.Value != null");
            SegmentTable table = new SegmentTable((int)IntMemSize.Value);

            Debug.Assert(required != null, nameof(required) + " != null");
            table.Allocate(Segments, (int)required);
        }

        private void BtnApply_Click(object sender, RoutedEventArgs e)
        {
            // Creates n number of segments based on the user input
            Segments.Clear();
            int? count = IntSegmentCount.Value;
            for (int i = 0; i < count; i++)
            {
                Segments.Add(
                    new Segment
                    {
                        Name = $"Segment {i}",
                        Number = i,
                        Size = IntSegmentSize.DefaultValue
                    });
            }
            ListSegments.SelectedIndex = 0;

            // Update bar graph to show memory size
            Debug.Assert(IntMemSize.Value != null, "IntMemSize.Value != null");
            BarMemory.SeriesCollection.Add(
                new StackedRowSeries
                {
                    Values = new ChartValues<int> { (int)IntMemSize.Value },
                    DataLabels = true,
                    Title = "Free Space",
                    LabelPoint = point => point.From + " - " + point.To
                });
            BarMemory.Formatter = value => value + " Bytes";
            BarMemory.Labels = new[] { "Main Memory" };
            BarMemory.BindContext();
        }

        private void IntMemSize_LostFocus(object sender, RoutedEventArgs e)
        {
            if (IntMemSize.Value != null)
            {
                double value = (double)IntMemSize.Value;
                value = Math.Round(value / 100d, 0) * 100;
                IntMemSize.Value = (int?)value;
            }
        }

        private void IntSegmentSize_LostFocus(object sender, RoutedEventArgs e)
        {
            if (IntSegmentSize.Value != null)
            {
                double value = (double)IntSegmentSize.Value;
                value = Math.Round(value / 100d, 0) * 100;
                IntSegmentSize.Value = (int?)value;
            }
        }
    }
}

