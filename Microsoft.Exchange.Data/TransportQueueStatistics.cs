using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data
{
	internal class TransportQueueStatistics : ConfigurableObject
	{
		public TransportQueueStatistics() : base(new SimpleProviderPropertyBag())
		{
		}

		public override ObjectId Identity
		{
			get
			{
				return new ConfigObjectId(this.identity.ToString());
			}
		}

		public string ServerName
		{
			get
			{
				return (string)this[TransportQueueStatisticsSchema.ServerNameProperty];
			}
		}

		public string TlsDomain
		{
			get
			{
				return (string)this[TransportQueueStatisticsSchema.TlsDomainProperty];
			}
		}

		public string NextHopDomain
		{
			get
			{
				return (string)this[TransportQueueStatisticsSchema.NextHopDomainProperty];
			}
		}

		public string NextHopKey
		{
			get
			{
				return (string)this[TransportQueueStatisticsSchema.NextHopKeyProperty];
			}
		}

		public string NextHopCategory
		{
			get
			{
				return (string)this[TransportQueueStatisticsSchema.NextHopCategoryProperty];
			}
		}

		public string DeliveryType
		{
			get
			{
				return (string)this[TransportQueueStatisticsSchema.DeliveryTypeProperty];
			}
		}

		public string RiskLevel
		{
			get
			{
				return (string)this[TransportQueueStatisticsSchema.RiskLevelProperty];
			}
		}

		public string OutboundIPPool
		{
			get
			{
				return (string)this[TransportQueueStatisticsSchema.OutboundIPPoolProperty];
			}
		}

		public string Status
		{
			get
			{
				return (string)this[TransportQueueStatisticsSchema.StatusProperty];
			}
		}

		public string LastError
		{
			get
			{
				return (string)this[TransportQueueStatisticsSchema.LastErrorProperty];
			}
		}

		public int QueueCount
		{
			get
			{
				return (int)this[TransportQueueStatisticsSchema.QueueCountProperty];
			}
		}

		public int MessageCount
		{
			get
			{
				return (int)this[TransportQueueStatisticsSchema.MessageCountProperty];
			}
		}

		public int DeferredMessageCount
		{
			get
			{
				return (int)this[TransportQueueStatisticsSchema.DeferredMessageCountProperty];
			}
		}

		public int LockedMessageCount
		{
			get
			{
				return (int)this[TransportQueueStatisticsSchema.LockedMessageCountProperty];
			}
		}

		public double IncomingRate
		{
			get
			{
				return (double)this[TransportQueueStatisticsSchema.IncomingRateProperty];
			}
		}

		public double OutgoingRate
		{
			get
			{
				return (double)this[TransportQueueStatisticsSchema.OutgoingRateProperty];
			}
		}

		public double Velocity
		{
			get
			{
				return (double)this[TransportQueueStatisticsSchema.VelocityProperty];
			}
		}

		public MultiValuedProperty<TransportQueueLog> QueueLogs
		{
			get
			{
				if (this.queueLogs == null)
				{
					this.queueLogs = TransportQueueLog.Parse(this[TransportQueueStatisticsSchema.TransportQueueLogsProperty] as string);
				}
				return this.queueLogs;
			}
		}

		internal override ObjectSchema ObjectSchema
		{
			get
			{
				return TransportQueueStatistics.SchemaObject;
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2010;
			}
		}

		private static readonly TransportQueueStatisticsSchema SchemaObject = ObjectSchema.GetInstance<TransportQueueStatisticsSchema>();

		private readonly Guid identity = CombGuidGenerator.NewGuid();

		private MultiValuedProperty<TransportQueueLog> queueLogs;
	}
}
