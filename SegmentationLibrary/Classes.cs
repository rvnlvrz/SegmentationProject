using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using SegmentationLibrary.Annotations;

namespace SegmentationLibrary
{
    /// <inheritdoc />
    /// <summary>
    ///     Provides datatype definition for a logical address.
    /// </summary>
    public class LogicalAddress : IComparable<LogicalAddress>
    {
        /// <summary>
        ///     Contains the limit and base address of a segment in memory.
        /// </summary>
        public Tuple<int, int> Value;

        /// <summary>
        ///     Creates a new instance of the <see cref="LogicalAddress" /> class.
        /// </summary>
        /// <param name="value"></param>
        public LogicalAddress(Tuple<int, int> value)
        {
            Value = value;
        }

        /// <inheritdoc />
        /// <summary>
        ///     Implements sorting of the <see cref="T:SegmentationLibrary.LogicalAddress" /> by their base address.
        /// </summary>
        /// <param name="other">The <see cref="T:SegmentationLibrary.LogicalAddress" /> to compare with.</param>
        /// <returns></returns>
        public int CompareTo(LogicalAddress other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (ReferenceEquals(null, other)) return 1;
            if (Value.Item2 > other.Value.Item2) return 1;
            if (Value.Item2 < other.Value.Item2) return -1;
            return 0;
        }
    }

    /// <summary>
    ///     Object used to store and arrange all the segments.
    /// </summary>
    public class SegmentTable
    {
        private readonly int _memsize;
        private int _pointer;

        ///// <summary>
        /////     Collection of entries in the Segment Table.
        ///// </summary>
        //public readonly List<Tuple<Segment, LogicalAddress>> Entries = new List<Tuple<Segment, LogicalAddress>>();

        /// <summary>
        ///     Intitializes a new instance of the <see cref="SegmentTable" /> class.
        /// </summary>
        /// <param name="memsize">The size of allocated/available memory (in Bytes).</param>
        public SegmentTable(int memsize)
        {
            _pointer = 0;
            _memsize = memsize;
        }

        /// <summary>
        ///     Arranges the segments as to how they should be placed in the memory.
        /// </summary>
        /// <param name="segments">The collection of segments to be allocated.</param>
        /// <param name="required">The size of memory required to allocate all the segments.</param>
        /// <exception cref="NotImplementedException"></exception>
        public void Allocate(ObservableCollection<Segment> segments, int required)
        {
            /*
             * NOTE: This part is only concerned about taking note of the allocated segment as seen
             *       in the book's `segment table` (see Figure 8.9, p. 367). The actual physical memory
             *       graph will be handled by a different method.
             *       
             * Algorithm:
             * set memory pointer to zero (starting point)
             * do while not all segments are allocated:
             * 
             *      if free space = 0, then allocate a segment
             *      else, Decide randomly whether to allocate a segment or a free space (toss coin kind of thing)
             *      
             *      if a free space is to be allocated:
             *          get the current free memory space (using _memsize)
             *          decide randomly how much free space should be allocated (must not be 0)
             *          subtract the result from _memsize and move pointer at the end of the allocated space
             *          (we do not need to create an entry for free space here)
             *      else:
             *          create entry for the segment
             *          move the pointer after allocated space
             *          
             * end while
             */

            var random = new Random();
            var free = _memsize - required;
            var allocatedIndex = new List<int>();
            _pointer = 0;

            while (allocatedIndex.Count < segments.Count) // checks whether all segments have already been allocated
            {
                var coin = random.Next(0, 2); // toss coin 0 or 1
                var picker = random.Next(0, segments.Count);

                while (allocatedIndex.Contains(picker)) picker = random.Next(0, segments.Count);

                if (coin == 0 && free != 0) // allocate a random amount of free space
                {
                    var space = random.Next(0, free);
                    space = (int) (Math.Round(space / 100d, MidpointRounding.AwayFromZero) * 100);
                    _pointer += space;
                    free -= space;
                }
                else // allocate memory segment
                {
                    var size = segments[picker].Size;
                    segments[picker].Base = _pointer;
                    segments[picker].Limit = size + _pointer;
                    _pointer += size;
                    allocatedIndex.Add(picker);
                }
            }
        }
    }

    /// <inheritdoc cref="INotifyPropertyChanged" />
    /// <summary>
    ///     Provides dataype definition for a memory segment.
    /// </summary>
    public sealed class Segment : INotifyPropertyChanged, IComparable<Segment>
    {
        private readonly LogicalAddress _address = new LogicalAddress(new Tuple<int, int>(0, 0));
        private string _name;
        private int? _size, _number;

        /// <summary>
        ///     Gets or Sets the segment number.
        /// </summary>
        public int Number
        {
            get
            {
                Debug.Assert(_number != null, nameof(_number) + " != null");
                return (int) _number;
            }
            set
            {
                if (value != _number)
                {
                    _number = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        ///     Gets or Sets the name of the memory segment.
        /// </summary>
        public string Name
        {
            get => _name;
            set
            {
                if (value != _name)
                {
                    _name = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        ///     Gets or Sets the base address of the segment.
        /// </summary>
        public int Base
        {
            get => _address.Value.Item2;
            set
            {
                _address.Value = new Tuple<int, int>(_address.Value.Item1, value);
                OnPropertyChanged();
            }
        }

        /// <summary>
        ///     Gets or Sets the limit/range address of the segment.
        /// </summary>
        public int Limit
        {
            get => _address.Value.Item1;
            set
            {
                _address.Value = new Tuple<int, int>(value, _address.Value.Item2);
                OnPropertyChanged();
            }
        }

        /// <summary>
        ///     Gets or Sets the size (in Bytes) of the memory segment.
        /// </summary>
        public int Size
        {
            get
            {
                Debug.Assert(_size != null, nameof(_size) + " != null");
                return (int) _size;
            }
            set
            {
                if (value != _size)
                {
                    _size = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <inheritdoc />
        /// <summary>
        ///     Implements sorting a collection of <see cref="Segment" /> by their <see cref="LogicalAddress" />.
        /// </summary>
        /// <param name="other">The <see cref="Segment" /> to compare with.</param>
        /// <returns></returns>
        public int CompareTo(Segment other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (ReferenceEquals(null, other)) return 1;
            return Comparer<LogicalAddress>.Default.Compare(_address, other._address);
        }

        // Implementation of the INotifyPropertyChanged Interface
        // Used by the WPF ListBox to suto-update its elements without writing code/triggers
        /// <summary>
        ///     Automatically notifies the binding control about changes to an item's property.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}