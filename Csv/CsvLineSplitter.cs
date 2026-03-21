using System.Collections.Generic;
using System.Text.RegularExpressions;

using MemoryText = System.ReadOnlyMemory<char>;
using SpanText = System.ReadOnlySpan<char>;

namespace Csv
{
    /// <summary>
    /// Splits a single line (multiline handling is done independently) into multiple parts
    /// </summary>
    internal sealed class CsvLineSplitter
    {
        private static readonly Dictionary<(char Separator, bool AllowSingleQuoteToEncloseFieldValues), CsvLineSplitter> splitterCache = new Dictionary<(char, bool), CsvLineSplitter>();

        private static readonly object syncRoot = new object();

        private readonly char separator;
        private readonly Regex splitter;

        private CsvLineSplitter(char separator, Regex splitter)
        {
            this.separator = separator;
            this.splitter = splitter;
        }

        public static CsvLineSplitter Get(CsvOptions options)
        {
            CsvLineSplitter? splitter;
            lock (syncRoot)
            {
                var key = (options.Separator, options.AllowSingleQuoteToEncloseFieldValues);
                if (!splitterCache.TryGetValue(key, out splitter))
                    splitterCache[key] = splitter = Create(options);
            }

            return splitter;
        }

        private static CsvLineSplitter Create(CsvOptions options)
        {
            const string patternEscape = @"(?>(?(IQ)(?(ESC).(?<-ESC>)|\\(?<ESC>))|(?!))|(?(IQ)\k<QUOTE>(?<-IQ>)|(?<=^|{0})(?<QUOTE>[{1}])(?<IQ>))|(?(IQ).|[^{0}]))+|^(?={0})|(?<={0})(?={0})|(?<={0})$";
            const string patternNoEscape = @"(?>(?(IQ)\k<QUOTE>(?<-IQ>)|(?<=^|{0})(?<QUOTE>[{1}])(?<IQ>))|(?(IQ).|[^{0}]))+|^(?={0})|(?<={0})(?={0})|(?<={0})$";
            var separator = Regex.Escape(options.Separator.ToString());
            var quoteChars = options.AllowSingleQuoteToEncloseFieldValues ? "\"'" : "\"";
            const RegexOptions regexOptions = RegexOptions.Singleline | RegexOptions.Compiled;
            if (options.AllowBackSlashToEscapeQuote)
                return new CsvLineSplitter(options.Separator, new Regex(string.Format(patternEscape, separator, quoteChars), regexOptions));
            return new CsvLineSplitter(options.Separator, new Regex(string.Format(patternNoEscape, separator, quoteChars), regexOptions));
        }

        public static bool IsUnterminatedQuotedValue(SpanText value, CsvOptions options)
        {
            if (value.Length == 0)
                return false;

            char quoteChar;
            if (value[0] == '"')
            {
                quoteChar = '"';
            }
            else if (options.AllowSingleQuoteToEncloseFieldValues && value[0] == '\'')
            {
                quoteChar = '\'';
            }
            else
            {
                return false;
            }

            var regex = options.AllowBackSlashToEscapeQuote ? $@"\\?{quoteChar}+$" : $@"{quoteChar}+$";
            var trailingQuotes = StringHelpers.RegexMatch(value[1..], regex);
            // if the first trailing quote is escaped, ignore it
            if (options.AllowBackSlashToEscapeQuote && trailingQuotes.StartsWith("\\"))
            {
                trailingQuotes = trailingQuotes[2..];
            }
            // the value is properly terminated if there are an odd number of unescaped quotes at the end
            return trailingQuotes.Length % 2 == 0;
        }

        public IList<MemoryText> Split(MemoryText line, CsvOptions options)
        {
            var matches = splitter.Matches(line.AsString());
            var values = new List<MemoryText>(matches.Count);
            var p = -1;
            // ReSharper disable once ForCanBeConvertedToForeach
            for (var i = 0; i < matches.Count; i++)
            {
                var value = line.Slice(matches[i].Index, matches[i].Length);
                if (p >= 0 && IsUnterminatedQuotedValue(values[p].AsSpan(), options))
                {
                    values[p] = StringHelpers.Concat(values[p], separator.ToString(), value);
                }
                else
                {
                    values.Add(value);
                    p++;
                }
            }
            return values;
        }
    }
}