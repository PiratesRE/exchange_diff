using System;
using System.Collections;
using System.IO;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.LogUploaderProxy
{
	public class LogRowFormatter
	{
		public LogRowFormatter(LogSchema schema) : this(schema, LogRowFormatter.DefaultEscapeLineBreaks)
		{
		}

		public LogRowFormatter(LogSchema schema, bool escapeLineBreaks) : this(schema, escapeLineBreaks, true)
		{
		}

		public LogRowFormatter(LogSchema schema, bool escapeLineBreaks, bool escapeRawData)
		{
			this.logRowFormatterImpl = new LogRowFormatter(schema.LogSchemaImpl, escapeLineBreaks, escapeRawData);
		}

		public LogRowFormatter(LogRowFormatter copy)
		{
			this.logRowFormatterImpl = new LogRowFormatter(copy.logRowFormatterImpl);
		}

		internal LogRowFormatter LogRowFormatterImpl
		{
			get
			{
				return this.logRowFormatterImpl;
			}
		}

		public object this[int index]
		{
			get
			{
				return this.logRowFormatterImpl[index];
			}
			set
			{
				this.logRowFormatterImpl[index] = value;
			}
		}

		public static string FormatCollection(IEnumerable data)
		{
			return LogRowFormatter.FormatCollection(data);
		}

		public static string FormatCollection(IEnumerable data, out bool needsEscaping)
		{
			return LogRowFormatter.FormatCollection(data, LogRowFormatter.DefaultEscapeLineBreaks, out needsEscaping);
		}

		public static string FormatCollection(IEnumerable data, bool escapeLineBreaks, out bool needsEscaping)
		{
			return LogRowFormatter.FormatCollection(data, escapeLineBreaks, out needsEscaping);
		}

		public static string Format(object data)
		{
			return LogRowFormatter.Format(data);
		}

		internal void Write(Stream output)
		{
			this.logRowFormatterImpl.Write(output);
		}

		public static readonly bool DefaultEscapeLineBreaks = LogRowFormatter.DefaultEscapeLineBreaks;

		private LogRowFormatter logRowFormatterImpl;
	}
}
