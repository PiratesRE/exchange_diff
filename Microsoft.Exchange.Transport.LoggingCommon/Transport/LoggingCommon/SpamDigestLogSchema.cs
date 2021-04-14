using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.LoggingCommon
{
	internal static class SpamDigestLogSchema
	{
		public static CsvTable Schema
		{
			get
			{
				return SpamDigestLogSchema.schema;
			}
		}

		public static CsvTable DefaultSchema
		{
			get
			{
				return SpamDigestLogSchema.schema;
			}
		}

		public static Version DefaultVersion
		{
			get
			{
				return SpamDigestLogSchema.E15Version;
			}
		}

		public static List<Version> SupportedVersionsInAscendingOrder
		{
			get
			{
				return SpamDigestLogSchema.supportedVersionsInAscendingOrder;
			}
		}

		public const string Timestamp = "Timestamp";

		public const string EventId = "EventId";

		public const string Source = "Source";

		public const string Status = "Status";

		public const string SourceContext = "Source-Context";

		public const string TenantId = "TenantId";

		public const string ExMessageId = "ExMessageId";

		public const string MessageId = "MessageId";

		public const string Sender = "Sender";

		public const string Recipient = "Recipient";

		public const string Subject = "Subject";

		public const string Error = "Error";

		public const string Data = "Data";

		public const string CustomData = "CustomData";

		public static readonly Version E15Version = new Version(15, 0, 0, 0);

		private static readonly List<Version> supportedVersionsInAscendingOrder = new List<Version>
		{
			SpamDigestLogSchema.E15Version
		};

		private static readonly CsvTable schema = new CsvTable(new CsvField[]
		{
			new CsvField("Timestamp", typeof(DateTime), SpamDigestLogSchema.E15Version),
			new CsvField("EventId", typeof(string), SpamDigestLogSchema.E15Version),
			new CsvField("Source", typeof(string), SpamDigestLogSchema.E15Version),
			new CsvField("Status", typeof(string), SpamDigestLogSchema.E15Version),
			new CsvField("Source-Context", typeof(string), SpamDigestLogSchema.E15Version),
			new CsvField("TenantId", typeof(string), SpamDigestLogSchema.E15Version),
			new CsvField("ExMessageId", typeof(string), SpamDigestLogSchema.E15Version),
			new CsvField("MessageId", typeof(string), SpamDigestLogSchema.E15Version),
			new CsvField("Sender", typeof(string), SpamDigestLogSchema.E15Version),
			new CsvField("Recipient", typeof(string), SpamDigestLogSchema.E15Version),
			new CsvField("Subject", typeof(string), SpamDigestLogSchema.E15Version),
			new CsvField("Error", typeof(string), SpamDigestLogSchema.E15Version),
			new CsvField("Data", typeof(string), SpamDigestLogSchema.E15Version),
			new CsvField("CustomData", typeof(KeyValuePair<string, object>[]), SpamDigestLogSchema.E15Version)
		});
	}
}
