using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.SharePointSignalStore
{
	internal static class OfficeGraphLogSchema
	{
		public static CsvTable Schema
		{
			get
			{
				return OfficeGraphLogSchema.schema;
			}
		}

		public static string LogPrefix
		{
			get
			{
				return "OFFICEGRAPH";
			}
		}

		public static List<Version> SupportedVersionsInAscendingOrder
		{
			get
			{
				return OfficeGraphLogSchema.supportedVersionsInAscendingOrder;
			}
		}

		public static Version DefaultVersion
		{
			get
			{
				return OfficeGraphLogSchema.E15Version;
			}
		}

		public const string TimeStamp = "TimeStamp";

		public const string SignalType = "SignalType";

		public const string Signal = "Signal";

		public const string OrganizationId = "OrganizationId";

		public const string SharePointUrl = "SharePointUrl";

		public static readonly Version E15Version = new Version(15, 0, 0, 0);

		private static readonly List<Version> supportedVersionsInAscendingOrder = new List<Version>
		{
			OfficeGraphLogSchema.E15Version
		};

		private static readonly CsvTable schema = new CsvTable(new CsvField[]
		{
			new CsvField("TimeStamp", typeof(DateTime), OfficeGraphLogSchema.E15Version),
			new CsvField("SignalType", typeof(string), OfficeGraphLogSchema.E15Version),
			new CsvField("Signal", typeof(string), OfficeGraphLogSchema.E15Version),
			new CsvField("OrganizationId", typeof(string), OfficeGraphLogSchema.E15Version),
			new CsvField("SharePointUrl", typeof(string), OfficeGraphLogSchema.E15Version)
		});
	}
}
