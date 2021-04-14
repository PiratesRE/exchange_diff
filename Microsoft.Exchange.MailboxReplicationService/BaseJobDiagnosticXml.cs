using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[XmlType(TypeName = "job")]
	public class BaseJobDiagnosticXml : XMLSerializableBase
	{
		[XmlAttribute(AttributeName = "PickupTime")]
		public DateTime JobPickupTimestamp { get; set; }

		[XmlAttribute(AttributeName = "Guid")]
		public Guid RequestGuid { get; set; }

		[XmlAttribute(AttributeName = "Queue")]
		public string RequestQueue { get; set; }

		[XmlAttribute(AttributeName = "Type")]
		public string RequestType { get; set; }

		[XmlAttribute(AttributeName = "RetryCount")]
		public int RetryCount { get; set; }

		[XmlAttribute(AttributeName = "TotalRetryCount")]
		public int TotalRetryCount { get; set; }

		[XmlAttribute(AttributeName = "SyncStage")]
		public SyncStage SyncStage { get; set; }

		[XmlAttribute(AttributeName = "ThrottledBy")]
		public string CurrentlyThrottledResource { get; set; }

		[XmlAttribute(AttributeName = "ThrottledByMetricType")]
		public int CurrentlyThrottledResourceMetricType { get; set; }

		[XmlAttribute(AttributeName = "ThrottledSince")]
		public DateTime ThrottledSince { get; set; }

		[XmlAttribute(AttributeName = "BadItemsEncountered")]
		public int BadItemsEncountered { get; set; }

		[XmlAttribute(AttributeName = "LargeItemsEncountered")]
		public int LargeItemsEncountered { get; set; }

		[XmlAttribute(AttributeName = "MissingItemsEncountered")]
		public int MissingItemsEncountered { get; set; }

		[XmlAttribute(AttributeName = "LastProgressTimestamp")]
		public DateTime LastProgressTimestamp { get; set; }

		[XmlElement("Progress")]
		public BaseJobDiagnosticXml.JobTransferProgress JobTransferProgressRec { get; set; }

		[XmlAttribute(AttributeName = "TimeTrackerCurrentState")]
		public string TimeTrackerCurrentState { get; set; }

		[XmlArray("Reservations")]
		public List<BaseJobDiagnosticXml.ReservationRec> ReservationRecs;

		[XmlArray("Warnings")]
		public List<string> Warnings;

		[XmlType(TypeName = "Progress")]
		public class JobTransferProgress : XMLSerializableBase
		{
			[XmlAttribute(AttributeName = "MsgsWritten")]
			public int MessagesWritten { get; set; }

			[XmlAttribute(AttributeName = "MsgSizeWritten")]
			public ulong MessageSizeWritten { get; set; }

			[XmlAttribute(AttributeName = "TotalMsgs")]
			public int TotalMessages { get; set; }

			[XmlAttribute(AttributeName = "TotalMsgByteSize")]
			public ulong TotalMessageByteSize { get; set; }

			[XmlAttribute(AttributeName = "OverallProgress")]
			public int OverallProgress { get; set; }

			[XmlElement("ThroughputProgressTracker")]
			public TransferProgressTrackerXML ThroughputProgressTracker { get; set; }
		}

		[XmlType(TypeName = "ReservationRec")]
		public class ReservationRec : XMLSerializableBase
		{
			[XmlAttribute(AttributeName = "Id")]
			public Guid Id { get; set; }

			[XmlAttribute(AttributeName = "Flags")]
			public string Flags { get; set; }

			[XmlAttribute(AttributeName = "Resource")]
			public Guid ResourceId { get; set; }
		}
	}
}
