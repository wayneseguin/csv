using System;
using System.Collections.Generic;

namespace Csv
{
    /// <summary>
    /// Defines the options that can be passed to customize the reading or writing of csv files.
    /// </summary>
    /// <remarks>
    /// Do not reuse an instance of <see cref="CsvOptions"/> for multiple reads or writes.
    /// </remarks>
    public sealed class CsvOptions
    {
        /// <summary>
        /// Gets or sets the number of rows to skip before reading the header row, defaults to <c>0</c>.
        /// </summary>
        public int RowsToSkip { get; set; }

        /// <summary>
        /// Gets or sets a function to skip the current row based on its raw string value or 1-based index. Skips empty rows and rows starting with # by default.
        /// </summary>
#if NETCOREAPP3_1_OR_GREATER || NETSTANDARD2_1
        public Func<ReadOnlyMemory<char>, int, bool> SkipRow { get; set; } = (row, idx) => row.Span.IsEmpty || row.Span[0] == '#';
#else
        public Func<string, int, bool> SkipRow { get; set; } = (row, idx) => string.IsNullOrEmpty(row) || row[0] == '#';
#endif

        /// <summary>
        ///  Gets or sets the character to use for separating data, defaults to <c>'\0'</c> which will auto-detect from the header row.
        /// </summary>
        public char Separator { get; set; }

        /// <summary>  
        ///  Gets or sets the character to use for separating data, defaults to <c>'\0'</c> which will auto-detect from the header row.
        /// </summary>
        public char[] AutoSeparators { get; set; } = new char[] { ',', '\t', '|', ';' };

        /// <summary>
        ///  Gets or sets whether duplicate headers should be automatically fixed.
        /// </summary>
        public bool FixDuplicateHeaders { get; set; } = true;

        /// <summary>
        /// Gets or sets whether data should be trimmed when accessed, defaults to <c>false</c>.
        /// </summary>
        public bool TrimData { get; set; } = true;

        /// <summary>
        /// Gets or sets the comparer to use when looking up header names.
        /// </summary>
        public IEqualityComparer<string>? Comparer { get; set; } = StringComparer.OrdinalIgnoreCase;

        ///<summary>
        /// Gets or sets an indicator to the parser to expect a header row or not, defaults to <see cref="Csv.HeaderMode.HeaderPresent"/>.
        ///</summary>
        public HeaderMode HeaderMode { get; set; } = HeaderMode.HeaderPresent;

        /// <summary>
        /// Gets or sets whether a row should be validated immediately that the column count matches the header count, defaults to <c>false</c>.
        /// </summary>
        public bool ValidateColumnCount { get; set; }

        /// <summary>
        /// Gets or sets whether an empty string is returned for a missing column, defaults to <c>false</c>.
        /// </summary>
        public bool ReturnEmptyForMissingColumn { get; set; } = true;

        /// <summary>
        /// Can be used to use multiple names for a single column. (e.g. to allow "CategoryName", "Category Name", "Category-Name")
        /// </summary>
        /// <remarks>
        /// A group with no matches is ignored.
        /// </remarks>
        public ICollection<string[]>? Aliases { get; set; }

        /// <summary>
        /// Respects new line (either \r\n or \n) characters inside field values enclosed in double quotes, defaults to <c>false</c>.
        /// </summary>
        public bool AllowNewLineInEnclosedFieldValues { get; set; }

        /// <summary>
        /// Allows the sequence <c>"\""</c> to be a valid quoted value (in addition to the standard <c>""""</c>), defaults to <c>false</c>.
        /// </summary>
        public bool AllowBackSlashToEscapeQuote { get; set; } = true;

        /// <summary>
        /// Allows the sequence <c>"\""</c> to be a valid quoted value (in addition to the standard <c>""""</c>), defaults to <c>false</c>.
        /// </summary>
        public bool RemoveEmbeddedQuotes { get; set; } = true;

        /// <summary>
        /// Allows the single-quote character to be used to enclose field values, defaults to <c>false</c>.
        /// </summary>
        public bool AllowSingleQuoteToEncloseFieldValues { get; set; }

        /// <summary>
        /// The new line string to use when multiline field values are read, defaults to <see cref="Environment.NewLine"/>.
        /// </summary>
        /// <remarks>
        /// Requires <see cref="AllowNewLineInEnclosedFieldValues"/> to be set to <c>true</c> for this to have any effect.
        /// </remarks>
        public string NewLine { get; set; } = Environment.NewLine;

#if NETCOREAPP3_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER
        /// <summary>
        /// The new line string to use when multiline field values are read, defaults to <see cref="Environment.NewLine"/>.
        /// </summary>
        /// <remarks>
        /// Requires <see cref="AllowNewLineInEnclosedFieldValues"/> to be set to <c>true</c> for this to have any effect.
        /// </remarks>
        public bool UseStringPool { get; set; } = true;
        internal CommunityToolkit.HighPerformance.Buffers.StringPool stringPool = new CommunityToolkit.HighPerformance.Buffers.StringPool(8192);
#endif

#pragma warning disable 8618
        internal CsvLineSplitter Splitter { get; set; }
#pragma warning restore 8618

        /// <summary>
        /// Creates a new CsvOption object copying all options from parameter
        /// </summary>
        /// <param name="options"></param>
        /// <returns>CsvOption</returns>
        public static CsvOptions CreateFromExisting(CsvOptions options)
        {
            return new CsvOptions()
            {
                Aliases = options.Aliases,
                AllowBackSlashToEscapeQuote = options.AllowBackSlashToEscapeQuote,
                AllowNewLineInEnclosedFieldValues = options.AllowNewLineInEnclosedFieldValues,
                AllowSingleQuoteToEncloseFieldValues = options.AllowSingleQuoteToEncloseFieldValues,
                AutoSeparators = options.AutoSeparators,
                Comparer = options.Comparer,
                FixDuplicateHeaders = options.FixDuplicateHeaders,
                HeaderMode = options.HeaderMode,
                RowsToSkip = options.RowsToSkip,
                NewLine = options.NewLine,
                RemoveEmbeddedQuotes = options.RemoveEmbeddedQuotes,
                ReturnEmptyForMissingColumn = options.ReturnEmptyForMissingColumn,
                SkipRow = options.SkipRow,
                Separator = options.Separator,
                TrimData = options.TrimData,
#if NETCOREAPP3_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER || NET8_0_OR_GREATER
                UseStringPool = options.UseStringPool,
#endif
                ValidateColumnCount = options.ValidateColumnCount

            };
        }
    }
}