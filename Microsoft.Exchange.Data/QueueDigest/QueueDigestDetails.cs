using System;
using Microsoft.Exchange.Net.DiagnosticsAggregation;

namespace Microsoft.Exchange.Data.QueueDigest
{
	[Serializable]
	public class QueueDigestDetails
	{
		internal QueueDigestDetails(AggregatedQueueNormalDetails details)
		{
			this.QueueIdentity = details.QueueIdentity;
			this.ServerIdentity = details.ServerIdentity;
			this.MessageCount = details.MessageCount;
		}

		internal QueueDigestDetails(AggregatedQueueVerboseDetails details)
		{
			this.QueueIdentity = details.QueueIdentity;
			this.ServerIdentity = details.ServerIdentity;
			this.MessageCount = details.MessageCount;
			this.DeferredMessageCount = new int?(details.DeferredMessageCount);
			this.LockedMessageCount = new int?(details.LockedMessageCount);
			this.IncomingRate = new double?(details.IncomingRate);
			this.OutgoingRate = new double?(details.OutgoingRate);
			this.Velocity = new double?(details.Velocity);
			this.NextHopDomain = details.NextHopDomain;
			this.NextHopCategory = details.NextHopCategory;
			this.DeliveryType = details.DeliveryType;
			this.Status = details.Status;
			this.RiskLevel = details.RiskLevel;
			this.OutboundIPPool = details.OutboundIPPool;
			this.LastError = details.LastError;
			this.NextHopConnector = new Guid?(details.NextHopConnector);
			this.TlsDomain = details.TlsDomain;
		}

		internal QueueDigestDetails(TransportQueueLog details)
		{
			this.QueueIdentity = details.QueueName;
			this.MessageCount = details.MessageCount;
			this.DeferredMessageCount = new int?(details.DeferredMessageCount);
			this.LockedMessageCount = new int?(details.LockedMessageCount);
			this.IncomingRate = new double?(details.IncomingRate);
			this.OutgoingRate = new double?(details.OutgoingRate);
			this.Velocity = new double?(details.Velocity);
			this.NextHopDomain = details.NextHopDomain;
			this.NextHopCategory = details.NextHopCategory;
			this.DeliveryType = details.DeliveryType;
			this.Status = details.Status;
			this.RiskLevel = details.RiskLevel;
			this.OutboundIPPool = details.OutboundIPPool;
			this.LastError = details.LastError;
			this.NextHopConnector = new Guid?(details.NextHopConnector);
			this.TlsDomain = details.TlsDomain;
		}

		public string QueueIdentity { get; private set; }

		public string ServerIdentity { get; private set; }

		public int MessageCount { get; private set; }

		public int? DeferredMessageCount { get; private set; }

		public int? LockedMessageCount { get; private set; }

		public double? IncomingRate { get; private set; }

		public double? OutgoingRate { get; private set; }

		public double? Velocity { get; private set; }

		public string NextHopDomain { get; private set; }

		public string NextHopCategory { get; private set; }

		public Guid? NextHopConnector { get; private set; }

		public string DeliveryType { get; private set; }

		public string Status { get; private set; }

		public string RiskLevel { get; private set; }

		public string OutboundIPPool { get; private set; }

		public string LastError { get; private set; }

		public string TlsDomain { get; private set; }

		public override string ToString()
		{
			return this.QueueIdentity;
		}
	}
}
