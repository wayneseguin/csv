using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;

namespace Csv
{
    /// <summary>
    /// Represents a single data line inside a csv file.
    /// </summary>
    public interface ICsvLine : IEquatable<ICsvLine>, IEqualityComparer<ICsvLine>
    {
        /// <summary>
        /// Gets the headers from the csv file.
        /// </summary>
        string[] Headers { get; }

        /// <summary>
        /// Gets length of list of headers without having to instanciate the array.
        /// </summary>
        int HeaderLength { get; }

        /// <summary>
        /// Gets a list of values in string format for the current row.
        /// </summary>
        string[] Values { get; }

        /// <summary>
        /// Gets length of list of values without having to instanciate the array.
        /// </summary>
        int ValueLength { get; }

        /// <summary>
        /// Gets the original raw content of the line.
        /// </summary>
        string Raw { get; }

        /// <summary>
        /// Gets the 1-based index for the line inside the file.
        /// </summary>
        int Index { get; }

        /// <summary>
        /// Gets the number of columns of the line.
        /// </summary>
        int ColumnCount { get; }

        /// <summary>
        /// Indicates whether the specified <paramref name="name"/> exists.
        /// </summary>
        bool HasColumn(string name);

        /// <summary>
        /// Gets the data for the specified named header.
        /// </summary>
        /// <param name="name">The name of the header.</param>
        string this[string name] { get; }

        void Wipe();

#if NETCOREAPP3_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        string this[Index index] { get; }
        string[] this[Range range] { get; }
#else
        /// <summary>
        /// Gets the data for the specified indexed header.
        /// </summary>
        /// <param name="index">The index of the header.</param>
        string this[int index] { get; }

#endif

    }
}