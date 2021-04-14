using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.LoggingCommon
{
	internal static class SpamEngineOpticsLogSchema
	{
		public static CsvTable Schema
		{
			get
			{
				return SpamEngineOpticsLogSchema.schema;
			}
		}

		public static CsvTable DefaultSchema
		{
			get
			{
				return SpamEngineOpticsLogSchema.schema;
			}
		}

		public static Version DefaultVersion
		{
			get
			{
				return SpamEngineOpticsLogSchema.E15Version;
			}
		}

		public static List<Version> SupportedVersionsInAscendingOrder
		{
			get
			{
				return SpamEngineOpticsLogSchema.supportedVersionsInAscendingOrder;
			}
		}

		public const string Source = "source";

		public const string EntityID = "entityt-id";

		public const string Category = "category";

		public const string CustomData = "custom-data";

		public static readonly Version E15Version = new Version(15, 0, 0, 0);

		private static readonly List<Version> supportedVersionsInAscendingOrder = new List<Version>
		{
			SpamEngineOpticsLogSchema.E15Version
		};

		private static readonly CsvTable schema = new CsvTable(new CsvField[]
		{
			new CsvField("date-time", typeof(DateTime), SpamEngineOpticsLogSchema.E15Version),
			new CsvField("source", typeof(string), SpamEngineOpticsLogSchema.E15Version),
			new CsvField("entityt-id", typeof(string), SpamEngineOpticsLogSchema.E15Version),
			new CsvField("category", typeof(string), SpamEngineOpticsLogSchema.E15Version),
			new CsvField("custom-data", typeof(KeyValuePair<string, object>[]), SpamEngineOpticsLogSchema.E15Version)
		});
	}
}
