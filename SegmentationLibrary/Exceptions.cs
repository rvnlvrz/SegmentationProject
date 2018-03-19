using System;

namespace SegmentationLibrary
{
    /// <inheritdoc />
    /// <summary>
    ///     Represents the error that occurs when the total size of all segments to be allocated exceed the available memory.
    /// </summary>
    public class NotEnoughMemoryException : Exception
    {
        /// <inheritdoc />
        /// <summary>
        ///     Initializes a new instance of the <see cref="NotEnoughMemoryException" /> class.
        /// </summary>
        /// <param name="message"></param>
        public NotEnoughMemoryException(string message) : base(message)
        {
        }
    }
}