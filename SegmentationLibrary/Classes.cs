using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using SegmentationLibrary.Annotations;

namespace SegmentationLibrary
{
    /// <summary>
    /// Provides datatype definition for a logical address.
    /// </summary>
    public class LogicalAddress
    {
        /// <summary>
        /// Contains the limit and base address of a segment in memory.
        /// </summary>
        public Tuple<int, int> Value;
    }

    /// <summary>
    /// Object used to store and arrange all the segments.
    /// </summary>
    public class SegmentTable
    {
        /// <summary>
        /// Collection of entries in the Segment Table.
        /// </summary>
        public List<LogicalAddress> Entries;
        private int _memsize, _pointer;

        /// <summary>
        /// Intitializes a new instance of the <see cref="SegmentTable"/> class.
        /// </summary>
        /// <param name="memsize">The size of allocated/available memory (in Bytes).</param>
        public SegmentTable(int memsize)
        {
            _pointer = 0;
            _memsize = memsize;
        }

        /// <summary>
        /// Arranges the segments as to how they should be placed in the memory.
        /// </summary>
        /// <param name="segments">The collection of segments to be allocated.</param>
        /// <exception cref="NotImplementedException"></exception>
        public void Allocate(ObservableCollection<Segment> segments)
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
             *          subtract the result from _memsize and move pointer after the allocated space
             *          (we do not need to create an entry for free space here)
             *      else:
             *          create entry for the segment
             *          move the pointer after allocated space
             *          
             * end while
             */

            _pointer = 0;

            throw new NotImplementedException();
        }
    }

    /// <inheritdoc />
    /// <summary>
    /// Provides dataype definition for a memory segment.
    /// </summary>
    public sealed class Segment : INotifyPropertyChanged
    {
        private string _name;
        private int? _size, _number;

        /// <summary>
        /// Gets or Sets the name of the memory segment.
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
        /// Gets or Sets the size (in Bytes) of the memory segment.
        /// </summary>
        public int? Size
        {
            get => _size;
            set
            {
                if (value != _size)
                {
                    _size = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or Sets the segment number.
        /// </summary>
        public int? Number
        {
            get => _number;
            set
            {
                if (value != _number)
                {
                    _number = value;
                    OnPropertyChanged();
                }
            }
        }

        // Implementation of the INotifyPropertyChanged Interface
        // Used by the WPF ListBox to suto-update its elements without writing code/triggers
        /// <summary>
        /// Automatically notifies the binding control about changes to an item's property.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
