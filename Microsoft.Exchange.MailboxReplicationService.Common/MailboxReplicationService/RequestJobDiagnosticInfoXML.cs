using System;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[XmlType(TypeName = "DiagnosticInfo")]
	public sealed class RequestJobDiagnosticInfoXML : XMLSerializableBase
	{
		[XmlAttribute(AttributeName = "PoisonCount")]
		public int PoisonCount { get; set; }

		[XmlAttribute(AttributeName = "LastPickupTime")]
		public DateTime LastPickupTime { get; set; }

		[XmlAttribute(AttributeName = "DoNotPickUntil")]
		public DateTime DoNotPickUntil { get; set; }

		[XmlAttribute(AttributeName = "LastProgressTime")]
		public DateTime LastProgressTime { get; set; }

		[XmlAttribute(AttributeName = "IsCanceled")]
		public bool IsCanceled { get; set; }

		[XmlAttribute(AttributeName = "RetryCount")]
		public int RetryCount { get; set; }

		[XmlAttribute(AttributeName = "TotalRetryCount")]
		public int TotalRetryCount { get; set; }

		[XmlAttribute(AttributeName = "DomainController")]
		public string DomainController { get; set; }

		[XmlElement(ElementName = "JobPickupFailureMessage")]
		public string JobPickupFailureMessage { get; set; }

		[XmlElement(ElementName = "SkippedItems")]
		public SkippedItemCounts SkippedItems { get; set; }

		[XmlElement(ElementName = "FailureHistory")]
		public FailureHistory FailureHistory { get; set; }

		[XmlElement(ElementName = "TimeTracker")]
		public RequestJobTimeTrackerXML TimeTracker { get; set; }

		[XmlElement(ElementName = "ProgressTracker")]
		public TransferProgressTrackerXML ProgressTracker { get; set; }
	}
}
