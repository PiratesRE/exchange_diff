using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.LoggingCommon
{
	internal static class AvEngineUpdateSchema
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
				return AvEngineUpdateSchema.schema;
			}
		}

		public static Version DefaultVersion
		{
			get
			{
				return AvEngineUpdateSchema.E15Version;
			}
		}

		private static readonly Version E15Version = new Version(15, 0, 0, 0);

		private static readonly CsvTable schema = new CsvTable(new CsvField[]
		{
			new CsvField(AvEngineUpdateLogLineFields.Timestamp.ToString(), typeof(DateTime), AvEngineUpdateSchema.E15Version),
			new CsvField(AvEngineUpdateLogLineFields.EngineCategory.ToString(), typeof(string), true, AvEngineUpdateSchema.E15Version),
			new CsvField(AvEngineUpdateLogLineFields.EngineName.ToString(), typeof(string), true, AvEngineUpdateSchema.E15Version),
			new CsvField(AvEngineUpdateLogLineFields.EngineVersion.ToString(), typeof(string), true, AvEngineUpdateSchema.E15Version),
			new CsvField(AvEngineUpdateLogLineFields.SignatureVersion.ToString(), typeof(string), true, AvEngineUpdateSchema.E15Version),
			new CsvField(AvEngineUpdateLogLineFields.SignatureDateTime.ToString(), typeof(DateTime), true, AvEngineUpdateSchema.E15Version),
			new CsvField(AvEngineUpdateLogLineFields.RUSId.ToString(), typeof(string), AvEngineUpdateSchema.E15Version),
			new CsvField(AvEngineUpdateLogLineFields.UpdatedDateTime.ToString(), typeof(DateTime), AvEngineUpdateSchema.E15Version)
		});
	}
}
