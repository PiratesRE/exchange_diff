using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.LoggingCommon
{
	public static class MessageTrackingSchema
	{
		public static CsvTable MessageTrackingEvent
		{
			get
			{
				return MessageTrackingSchema.schema;
			}
		}

		public static List<Version> SupportedVersionsInAscendingOrder
		{
			get
			{
				return MessageTrackingSchema.supportedVersionsInAscendingOrder;
			}
		}

		public static Version E15FirstVersion
		{
			get
			{
				return MessageTrackingSchema.E15Version;
			}
		}

		public const string ClientHostName = "client-hostname";

		public const string ServerHostName = "server-hostname";

		public const string SourceContext = "source-context";

		public const string ConnectorId = "connector-id";

		public const string Source = "source";

		public const string InternalMsgID = "internal-message-id";

		public const string NetworkMsgID = "network-message-id";

		public const string RecipientStatuses = "recipient-status";

		public const string RelatedRecipientAddress = "related-recipient-address";

		public const string Reference = "reference";

		public const string ReturnPath = "return-path";

		public const string MessageInfo = "message-info";

		public const string Directionality = "directionality";

		public const string CustomData = "custom-data";

		private static readonly Version E12Version = new Version(12, 0, 0, 0);

		private static readonly Version E14InterfaceUpdateVersion = new Version(14, 0, 533);

		private static readonly Version CustomDataAddedVersion = new Version(14, 0, 552);

		private static readonly Version E15Version = new Version(15, 0, 0, 0);

		private static readonly List<Version> supportedVersionsInAscendingOrder = new List<Version>
		{
			MessageTrackingSchema.CustomDataAddedVersion,
			MessageTrackingSchema.E15Version
		};

		private static readonly CsvTable schema = new CsvTable(new CsvField[]
		{
			new CsvField("date-time", typeof(DateTime), MessageTrackingSchema.E12Version),
			new CsvField("client-ip", typeof(string), MessageTrackingSchema.E12Version),
			new CsvField("client-hostname", typeof(string), MessageTrackingSchema.E12Version),
			new CsvField("server-ip", typeof(string), MessageTrackingSchema.E12Version),
			new CsvField("server-hostname", typeof(string), MessageTrackingSchema.E12Version),
			new CsvField("source-context", typeof(string), MessageTrackingSchema.E12Version),
			new CsvField("connector-id", typeof(string), MessageTrackingSchema.E12Version),
			new CsvField("source", typeof(string), MessageTrackingSchema.E12Version),
			new CsvField("event-id", typeof(string), MessageTrackingSchema.E12Version),
			new CsvField("internal-message-id", typeof(string), MessageTrackingSchema.E12Version),
			new CsvField("message-id", typeof(string), true, MessageTrackingSchema.E12Version, new NormalizeColumnDataMethod(CsvFieldCache.NormalizeMessageID)),
			new CsvField("network-message-id", typeof(string), MessageTrackingSchema.E15Version),
			new CsvField("recipient-address", typeof(string[]), MessageTrackingSchema.E12Version),
			new CsvField("recipient-status", typeof(string[]), MessageTrackingSchema.E12Version),
			new CsvField("total-bytes", typeof(int), MessageTrackingSchema.E12Version),
			new CsvField("recipient-count", typeof(int), MessageTrackingSchema.E12Version),
			new CsvField("related-recipient-address", typeof(string), MessageTrackingSchema.E12Version),
			new CsvField("reference", typeof(string[]), MessageTrackingSchema.E12Version),
			new CsvField("message-subject", typeof(string), MessageTrackingSchema.E12Version),
			new CsvField("sender-address", typeof(string), true, MessageTrackingSchema.E12Version, new NormalizeColumnDataMethod(CsvFieldCache.NormalizeEmailAddress)),
			new CsvField("return-path", typeof(string), MessageTrackingSchema.E12Version),
			new CsvField("message-info", typeof(string), MessageTrackingSchema.E12Version),
			new CsvField("directionality", typeof(string), MessageTrackingSchema.E14InterfaceUpdateVersion),
			new CsvField("tenant-id", typeof(string), MessageTrackingSchema.E14InterfaceUpdateVersion),
			new CsvField("original-client-ip", typeof(string), MessageTrackingSchema.E14InterfaceUpdateVersion),
			new CsvField("original-server-ip", typeof(string), MessageTrackingSchema.E14InterfaceUpdateVersion),
			new CsvField("custom-data", typeof(KeyValuePair<string, object>[]), MessageTrackingSchema.CustomDataAddedVersion)
		});
	}
}
