using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using LiveCharts;
using LiveCharts.Wpf;
using SegmentationLibrary;

namespace SegmentationApp
{
    /// <inheritdoc cref="Window" />
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private ObservableCollection<Segment> _segments;

        /// <inheritdoc />
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            Segments = new ObservableCollection<Segment>();
        }

        // ReSharper disable once ConvertToAutoProperty
        // ReSharper disable once MemberCanBePrivate.Global
        /// <summary>
        ///     Gets or Sets the collection of segments that the program is currently using.
        /// </summary>
        public ObservableCollection<Segment> Segments
        {
            get => _segments;
            set => _segments = value;
        }

        private void BtnPlaceInMemory_Click(object sender, RoutedEventArgs e)
        {
            if (Segments.Count == 0) return;

            var memsize = IntMemSize.Value;
            int? required = 0;

            foreach (var t in Segments)
                required += t.Size;

            // throws an error if the total of all segment sizes exceeds the alloted memory
            try
            {
                if (memsize < required)
                    throw new NotEnoughMemoryException(
                        "Required memory for segment allocation exceeds Main Memory size!");
            }
            catch (NotEnoughMemoryException exception)
            {
                MessageBox.Show(exception.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Pass generated segments into segment table
            Debug.Assert(IntMemSize.Value != null, "IntMemSize.Value != null");
            var table = new SegmentTable((int) IntMemSize.Value);

            Debug.Assert(required != null, nameof(required) + " != null");
            table.Allocate(Segments, (int) required);

            // Create bar graph out of the table
            var pointer = 0;
            var sortedSegment = Segments.ToList();
            sortedSegment.Sort();

            BarMemory.SeriesCollection.Clear();
            foreach (var segment in sortedSegment)
                if (segment.Base == pointer) // indicates this is a memory segment
                {
                    BarMemory.SeriesCollection.Add(new StackedRowSeries
                    {
                        Values = new ChartValues<int> {segment.Size},
                        DataLabels = true,
                        Title = segment.Name,
                        LabelPoint = point => point.From + " - " + point.To
                    });
                    pointer += segment.Size;
                }
                else
                {
                    BarMemory.SeriesCollection.Add(new StackedRowSeries
                    {
                        Values = new ChartValues<int> {segment.Base - pointer},
                        DataLabels = true,
                        Title = "Free Space"
                    });
                    pointer = segment.Base;

                    BarMemory.SeriesCollection.Add(new StackedRowSeries
                    {
                        Values = new ChartValues<int> {segment.Size},
                        DataLabels = true,
                        Title = segment.Name,
                        LabelPoint = point => point.From + " - " + point.To
                    });
                    pointer += segment.Size;
                }

            if (pointer != memsize)
            {
                Debug.Assert(memsize != null, nameof(memsize) + " != null");
                BarMemory.SeriesCollection.Add(new StackedRowSeries
                {
                    Values = new ChartValues<int> {(int) (memsize - pointer)},
                    DataLabels = true,
                    Title = "Free Space"
                });
            }
            BarMemory.ResetZoom();
        }

        private void BtnApply_Click(object sender, RoutedEventArgs e)
        {
            // Creates n number of segments based on the user input
            Segments.Clear();
            var count = IntSegmentCount.Value;
            for (var i = 0; i < count; i++)
            {
                Debug.Assert(IntSegmentSize.DefaultValue != null, "IntSegmentSize.DefaultValue != null");
                Segments.Add(
                    new Segment
                    {
                        Name = $"Segment {i}",
                        Number = i,
                        Size = (int) IntSegmentSize.DefaultValue
                    });
            }
            ListSegments.SelectedIndex = 0;

            // Update bar graph to show memory size
            BarMemory.SeriesCollection.Clear();
            Debug.Assert(IntMemSize.Value != null, "IntMemSize.Value != null");
            BarMemory.SeriesCollection.Add(
                new StackedRowSeries
                {
                    Values = new ChartValues<int> {(int) IntMemSize.Value},
                    //DataLabels = true,
                    Title = "Free Space"
                    //LabelPoint = point => point.Sum + ""
                });
            BarMemory.Formatter = value => value + " Bytes";
            BarMemory.Labels = new[] {"Main Memory"};
            BarMemory.BindContext();
            BarMemory.ResetZoom();
        }

        private void IntMemSize_LostFocus(object sender, RoutedEventArgs e)
        {
            if (IntMemSize.Value != null)
            {
                var value = (double) IntMemSize.Value;
                value = Math.Round(value / 100d, 0) * 100;
                IntMemSize.Value = (int?) value;
            }
        }

        private void IntSegmentSize_LostFocus(object sender, RoutedEventArgs e)
        {
            if (IntSegmentSize.Value != null)
            {
                var value = (double) IntSegmentSize.Value;
                value = Math.Round(value / 100d, 0) * 100;
                IntSegmentSize.Value = (int?) value;
            }
        }
    }
}