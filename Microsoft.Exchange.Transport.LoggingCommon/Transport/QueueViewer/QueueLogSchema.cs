using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.QueueViewer
{
	internal static class QueueLogSchema
	{
		public static CsvTable Schema
		{
			get
			{
				return QueueLogSchema.schema;
			}
		}

		public static string LogPrefix
		{
			get
			{
				return "TRANSPORTQUEUE";
			}
		}

		public static List<Version> SupportedVersionsInAscendingOrder
		{
			get
			{
				return QueueLogSchema.supportedVersionsInAscendingOrder;
			}
		}

		public const string Timestamp = "Timestamp";

		public const string SequenceNumber = "SequenceNumber";

		public const string EventId = "EventId";

		public const string QueueIdentity = "QueueIdentity";

		public const string QueueStatus = "QueueStatus";

		public const string DeliveryType = "DeliveryType";

		public const string NextHopDomain = "NextHopDomain";

		public const string NextHopKey = "NextHopKey";

		public const string ActiveMessageCount = "ActiveMessageCount";

		public const string LockedMessageCount = "LockedMessageCount";

		public const string DeferredMessageCount = "DeferredMessageCount";

		public const string IncomingRate = "IncomingRate";

		public const string NextHopCategory = "NextHopCategory";

		public const string OutgoingRate = "OutgoingRate";

		public const string RiskLevel = "RiskLevel";

		public const string OutboundIPPool = "OutboundIPPool";

		public const string LastError = "LastError";

		public const string Velocity = "Velocity";

		public const string NextHopConnector = "NextHopConnector";

		public const string TlsDomain = "TlsDomain";

		public const string Data = "Data";

		public const string CustomData = "CustomData";

		public static readonly Version E15Version = new Version(15, 0, 0, 0);

		private static readonly List<Version> supportedVersionsInAscendingOrder = new List<Version>
		{
			QueueLogSchema.E15Version
		};

		private static readonly CsvTable schema = new CsvTable(new CsvField[]
		{
			new CsvField("Timestamp", typeof(string), QueueLogSchema.E15Version),
			new CsvField("SequenceNumber", typeof(string), QueueLogSchema.E15Version),
			new CsvField("EventId", typeof(string), QueueLogSchema.E15Version),
			new CsvField("QueueIdentity", typeof(string), QueueLogSchema.E15Version),
			new CsvField("QueueStatus", typeof(string), QueueLogSchema.E15Version),
			new CsvField("DeliveryType", typeof(string), QueueLogSchema.E15Version),
			new CsvField("NextHopDomain", typeof(string), QueueLogSchema.E15Version),
			new CsvField("NextHopKey", typeof(string), QueueLogSchema.E15Version),
			new CsvField("ActiveMessageCount", typeof(string), QueueLogSchema.E15Version),
			new CsvField("DeferredMessageCount", typeof(string), QueueLogSchema.E15Version),
			new CsvField("LockedMessageCount", typeof(string), QueueLogSchema.E15Version),
			new CsvField("IncomingRate", typeof(string), QueueLogSchema.E15Version),
			new CsvField("OutgoingRate", typeof(string), QueueLogSchema.E15Version),
			new CsvField("Velocity", typeof(string), QueueLogSchema.E15Version),
			new CsvField("NextHopCategory", typeof(string), QueueLogSchema.E15Version),
			new CsvField("RiskLevel", typeof(string), QueueLogSchema.E15Version),
			new CsvField("OutboundIPPool", typeof(string), QueueLogSchema.E15Version),
			new CsvField("NextHopConnector", typeof(string), QueueLogSchema.E15Version),
			new CsvField("TlsDomain", typeof(string), QueueLogSchema.E15Version),
			new CsvField("LastError", typeof(string), QueueLogSchema.E15Version),
			new CsvField("Data", typeof(string), QueueLogSchema.E15Version),
			new CsvField("CustomData", typeof(KeyValuePair<string, object>[]), QueueLogSchema.E15Version)
		});
	}
}
