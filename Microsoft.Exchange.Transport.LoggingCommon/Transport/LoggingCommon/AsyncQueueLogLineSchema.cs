using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.LoggingCommon
{
	internal static class AsyncQueueLogLineSchema
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
				return AsyncQueueLogLineSchema.schema;
			}
		}

		public static Version DefaultVersion
		{
			get
			{
				return AsyncQueueLogLineSchema.E15Version;
			}
		}

		public static List<Version> SupportedVersionsInAscendingOrder
		{
			get
			{
				return new List<Version>
				{
					AsyncQueueLogLineSchema.E15Version
				};
			}
		}

		internal const string Timestamp = "Timestamp";

		internal const string OrganizationalUnitRoot = "OrganizationalUnitRoot";

		internal const string StepTransactionId = "StepTransactionId";

		internal const string ProcessStartDatetime = "ProcessStartDatetime";

		internal const string PrimaryProperties = "PrimaryProperties";

		internal const string ExtendedProperties = "ExtendedProperties";

		private static readonly Version E15Version = new Version(15, 0, 0, 0);

		private static readonly CsvTable schema = new CsvTable(new CsvField[]
		{
			new CsvField("Timestamp", typeof(DateTime), AsyncQueueLogLineSchema.E15Version),
			new CsvField("OrganizationalUnitRoot", typeof(string), true, AsyncQueueLogLineSchema.E15Version),
			new CsvField("StepTransactionId", typeof(string), true, AsyncQueueLogLineSchema.E15Version),
			new CsvField("ProcessStartDatetime", typeof(DateTime), true, AsyncQueueLogLineSchema.E15Version),
			new CsvField("PrimaryProperties", typeof(KeyValuePair<string, object>[]), AsyncQueueLogLineSchema.E15Version),
			new CsvField("ExtendedProperties", typeof(KeyValuePair<string, object>[]), AsyncQueueLogLineSchema.E15Version)
		});
	}
}
