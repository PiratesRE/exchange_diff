using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Filtering
{
	internal static class TextExtractionSchema
	{
		public static int TimeStampFieldIndex
		{
			get
			{
				return 0;
			}
		}

		public static CsvTable DefaultSchema
		{
			get
			{
				return TextExtractionSchema.schema;
			}
		}

		public static Version DefaultVersion
		{
			get
			{
				return TextExtractionSchema.E15Version;
			}
		}

		private static readonly Version E15Version = new Version(15, 0, 0, 0);

		private static readonly CsvTable schema = new CsvTable(new CsvField[]
		{
			new CsvField(TextExtractionLogLineFields.Timestamp.ToString(), typeof(DateTime), TextExtractionSchema.E15Version),
			new CsvField(TextExtractionLogLineFields.ExMessageId.ToString(), typeof(string), TextExtractionSchema.E15Version),
			new CsvField(TextExtractionLogLineFields.StreamId.ToString(), typeof(int), TextExtractionSchema.E15Version),
			new CsvField(TextExtractionLogLineFields.StreamSize.ToString(), typeof(long), TextExtractionSchema.E15Version),
			new CsvField(TextExtractionLogLineFields.ParentId.ToString(), typeof(int), TextExtractionSchema.E15Version),
			new CsvField(TextExtractionLogLineFields.TeTypes.ToString(), typeof(string), true, TextExtractionSchema.E15Version),
			new CsvField(TextExtractionLogLineFields.TeModuleUsed.ToString(), typeof(string), true, TextExtractionSchema.E15Version),
			new CsvField(TextExtractionLogLineFields.TeResult.ToString(), typeof(int), true, TextExtractionSchema.E15Version),
			new CsvField(TextExtractionLogLineFields.TeSkippedModules.ToString(), typeof(string), true, TextExtractionSchema.E15Version),
			new CsvField(TextExtractionLogLineFields.TeFailedModules.ToString(), typeof(string), true, TextExtractionSchema.E15Version),
			new CsvField(TextExtractionLogLineFields.TeDisabledModules.ToString(), typeof(string), true, TextExtractionSchema.E15Version),
			new CsvField(TextExtractionLogLineFields.AdditionalInformation.ToString(), typeof(string), true, TextExtractionSchema.E15Version)
		});
	}
}
