using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Net.DiagnosticsAggregation
{
	[DataContract]
	[Serializable]
	public class LocalQueueInfo
	{
		[DataMember(IsRequired = true)]
		public string QueueIdentity { get; set; }

		[DataMember(IsRequired = true)]
		public string ServerIdentity { get; set; }

		[DataMember(IsRequired = true)]
		public int MessageCount { get; set; }

		[DataMember(IsRequired = true)]
		public int DeferredMessageCount { get; set; }

		[DataMember(IsRequired = true)]
		public int LockedMessageCount { get; set; }

		[DataMember(IsRequired = true)]
		public double IncomingRate { get; set; }

		[DataMember(IsRequired = true)]
		public double OutgoingRate { get; set; }

		[DataMember(IsRequired = true)]
		public double Velocity { get; set; }

		[DataMember(IsRequired = true)]
		public string NextHopDomain { get; set; }

		[DataMember(IsRequired = true)]
		public string NextHopCategory { get; set; }

		[DataMember(IsRequired = true)]
		public string DeliveryType { get; set; }

		[DataMember(IsRequired = true)]
		public string Status { get; set; }

		[DataMember(IsRequired = true)]
		public string RiskLevel { get; set; }

		[DataMember(IsRequired = true)]
		public string OutboundIPPool { get; set; }

		[DataMember(IsRequired = true)]
		public string LastError { get; set; }

		[DataMember(IsRequired = true)]
		public string NextHopKey { get; set; }

		[DataMember(IsRequired = true)]
		public Guid NextHopConnector { get; set; }

		[DataMember(IsRequired = true)]
		public string TlsDomain { get; set; }

		public override string ToString()
		{
			return this.QueueIdentity;
		}
	}
}
