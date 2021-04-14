using System;
using System.IO;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.LogUploaderProxy
{
	public class LogHeaderFormatter
	{
		public LogHeaderFormatter(LogSchema schema) : this(schema, LogHeaderCsvOption.NotCsvCompatible)
		{
		}

		public LogHeaderFormatter(LogSchema schema, bool csvCompatible) : this(schema, csvCompatible ? LogHeaderCsvOption.CsvCompatible : LogHeaderCsvOption.NotCsvCompatible)
		{
		}

		public LogHeaderFormatter(LogSchema schema, LogHeaderCsvOption csvOption)
		{
			this.logHeaderFormatterImpl = new LogHeaderFormatter(schema.LogSchemaImpl, (LogHeaderCsvOption)csvOption);
		}

		public LogHeaderCsvOption CsvOption
		{
			get
			{
				return (LogHeaderCsvOption)this.logHeaderFormatterImpl.CsvOption;
			}
		}

		internal LogHeaderFormatter LogHeaderFormatterImpl
		{
			get
			{
				return this.logHeaderFormatterImpl;
			}
		}

		internal void Write(Stream output, DateTime date)
		{
			this.logHeaderFormatterImpl.Write(output, date);
		}

		public const string DateTimeFormatSpecifier = "yyyy-MM-ddTHH\\:mm\\:ss.fffZ";

		public const string TimeSpanFormatSpecifier = "g";

		private LogHeaderFormatter logHeaderFormatterImpl;
	}
}
