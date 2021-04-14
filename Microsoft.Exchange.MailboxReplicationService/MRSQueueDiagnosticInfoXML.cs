using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[XmlType(TypeName = "Queue")]
	public class MRSQueueDiagnosticInfoXML : XMLSerializableBase
	{
		[XmlAttribute(AttributeName = "Guid")]
		public Guid MdbGuid { get; set; }

		[XmlAttribute(AttributeName = "DBName")]
		public string MdbName { get; set; }

		[XmlAttribute(AttributeName = "LastJobPickup")]
		public DateTime LastJobPickup { get; set; }

		[XmlAttribute(AttributeName = "LastInteractiveJobPickup")]
		public DateTime LastInteractiveJobPickup { get; set; }

		[XmlAttribute(AttributeName = "QueuedJobs")]
		public int QueuedJobsCount { get; set; }

		[XmlAttribute(AttributeName = "InProgressJobs")]
		public int InProgressJobsCount { get; set; }

		[XmlText]
		public string LastScanFailure { get; set; }

		[XmlAttribute(AttributeName = "MdbDiscovery")]
		public DateTime MdbDiscoveryTimestamp { get; set; }

		[XmlAttribute(AttributeName = "LastScan")]
		public DateTime LastScanTimestamp { get; set; }

		[XmlAttribute(AttributeName = "LastScanDurationMs")]
		public int LastScanDurationMs { get; set; }

		[XmlAttribute(AttributeName = "NextScan")]
		public DateTime NextRecommendedScan { get; set; }

		[XmlAttribute(AttributeName = "lastfinishtime")]
		public DateTime LastActiveJobFinishTime { get; set; }

		[XmlAttribute(AttributeName = "lastfinishjob")]
		public Guid LastActiveJobFinished { get; set; }

		[XmlArray("LastScanResults")]
		public List<JobPickupRec> LastScanResults { get; set; }

		public bool IsEmpty()
		{
			return this.QueuedJobsCount == 0 && this.InProgressJobsCount == 0;
		}
	}
}
