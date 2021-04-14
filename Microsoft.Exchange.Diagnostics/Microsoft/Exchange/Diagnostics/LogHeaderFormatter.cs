using System;
using System.Globalization;
using System.IO;

namespace Microsoft.Exchange.Diagnostics
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
			this.schema = schema;
			this.CsvOption = csvOption;
		}

		public LogHeaderCsvOption CsvOption { get; private set; }

		internal void Write(Stream output, DateTime date)
		{
			if (output.Position == 0L)
			{
				if (this.CsvOption != LogHeaderCsvOption.NotCsvCompatible)
				{
					Utf8Csv.WriteHeaderRow(output, this.schema.Fields);
				}
				else
				{
					Utf8Csv.WriteBom(output);
				}
			}
			if (this.CsvOption != LogHeaderCsvOption.CsvStrict)
			{
				Utf8Csv.EncodeAndWrite(output, "#Software: ");
				Utf8Csv.EncodeAndWriteLine(output, this.schema.Software);
				Utf8Csv.EncodeAndWrite(output, "#Version: ");
				Utf8Csv.EncodeAndWriteLine(output, this.schema.Version);
				Utf8Csv.EncodeAndWrite(output, "#Log-type: ");
				Utf8Csv.EncodeAndWriteLine(output, this.schema.LogType);
				Utf8Csv.EncodeAndWrite(output, "#Date: ");
				Utf8Csv.EncodeAndWriteLine(output, date.ToString("yyyy-MM-ddTHH\\:mm\\:ss.fffZ", DateTimeFormatInfo.InvariantInfo));
				Utf8Csv.EncodeAndWrite(output, "#Fields: ");
				Utf8Csv.WriteHeaderRow(output, this.schema.Fields);
			}
		}

		public const string DateTimeFormatSpecifier = "yyyy-MM-ddTHH\\:mm\\:ss.fffZ";

		public const string TimeSpanFormatSpecifier = "g";

		private LogSchema schema;
	}
}
