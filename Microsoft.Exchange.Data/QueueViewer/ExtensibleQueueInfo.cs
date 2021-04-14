using System;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Extensibility.Internal;

namespace Microsoft.Exchange.Data.QueueViewer
{
	[Serializable]
	public abstract class ExtensibleQueueInfo : ConfigurableObject, PagedDataObject, IConfigurable
	{
		internal ExtensibleQueueInfo(QueueInfoPropertyBag propertyBag) : base(propertyBag)
		{
		}

		public QueueIdentity QueueIdentity
		{
			get
			{
				return (QueueIdentity)this.Identity;
			}
		}

		public string[] PriorityDescriptions
		{
			get
			{
				return EnumUtils.DeliveryPriorityEnumNames;
			}
		}

		internal override ObjectSchema ObjectSchema
		{
			get
			{
				return ExtensibleQueueInfo.schema;
			}
		}

		public void ConvertDatesToLocalTime()
		{
			if (this.LastRetryTime != null)
			{
				this.LastRetryTime = new DateTime?(this.LastRetryTime.Value.ToLocalTime());
			}
			if (this.NextRetryTime != null)
			{
				this.NextRetryTime = new DateTime?(this.NextRetryTime.Value.ToLocalTime());
			}
			if (this.FirstRetryTime != null)
			{
				this.FirstRetryTime = new DateTime?(this.FirstRetryTime.Value.ToLocalTime());
			}
			string lastError;
			if (!string.IsNullOrEmpty(this.LastError) && Microsoft.Exchange.Extensibility.Internal.LastError.TryConvertLastRetryTimeToLocalTime(this.LastError, out lastError))
			{
				this.LastError = lastError;
			}
		}

		public void ConvertDatesToUniversalTime()
		{
			if (this.LastRetryTime != null)
			{
				this.LastRetryTime = new DateTime?(this.LastRetryTime.Value.ToUniversalTime());
			}
			if (this.NextRetryTime != null)
			{
				this.NextRetryTime = new DateTime?(this.NextRetryTime.Value.ToUniversalTime());
			}
			string lastError;
			if (!string.IsNullOrEmpty(this.LastError) && Microsoft.Exchange.Extensibility.Internal.LastError.TryConvertLastRetryTimeToUniversalTime(this.LastError, out lastError))
			{
				this.LastError = lastError;
			}
		}

		public abstract bool IsDeliveryQueue();

		public abstract bool IsSubmissionQueue();

		public abstract bool IsPoisonQueue();

		public abstract bool IsShadowQueue();

		public abstract DeliveryType DeliveryType { get; internal set; }

		public abstract string NextHopDomain { get; internal set; }

		public abstract string TlsDomain { get; internal set; }

		public abstract Guid NextHopConnector { get; internal set; }

		public abstract QueueStatus Status { get; internal set; }

		public abstract int MessageCount { get; internal set; }

		public abstract int[] MessageCountsPerPriority { get; internal set; }

		public abstract string LastError { get; internal set; }

		public abstract int RetryCount { get; internal set; }

		public abstract DateTime? LastRetryTime { get; internal set; }

		public abstract DateTime? NextRetryTime { get; internal set; }

		public abstract DateTime? FirstRetryTime { get; internal set; }

		public abstract int DeferredMessageCount { get; internal set; }

		public abstract int[] DeferredMessageCountsPerPriority { get; internal set; }

		public abstract int LockedMessageCount { get; internal set; }

		public abstract RiskLevel RiskLevel { get; internal set; }

		public abstract int OutboundIPPool { get; internal set; }

		public abstract NextHopCategory NextHopCategory { get; internal set; }

		public abstract double IncomingRate { get; internal set; }

		public abstract double OutgoingRate { get; internal set; }

		public abstract double Velocity { get; internal set; }

		public abstract string OverrideSource { get; internal set; }

		private static ExtensibleQueueInfoSchema schema = ObjectSchema.GetInstance<ExtensibleQueueInfoSchema>();
	}
}
