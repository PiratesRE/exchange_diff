using System;

namespace Microsoft.Exchange.Data.Reporting
{
	internal class TenantThrottleInfo : ConfigurableObject
	{
		public TenantThrottleInfo() : base(new SimpleProviderPropertyBag())
		{
		}

		public override ObjectId Identity
		{
			get
			{
				return new ConfigObjectId(this.TenantId.ToString());
			}
		}

		public Guid TenantId
		{
			get
			{
				return (Guid)this[TenantThrottleInfoSchema.TenantIdProperty];
			}
			set
			{
				this[TenantThrottleInfoSchema.TenantIdProperty] = value;
			}
		}

		public DateTime TimeStamp
		{
			get
			{
				return (DateTime)this[TenantThrottleInfoSchema.TimeStampProperty];
			}
			set
			{
				this[TenantThrottleInfoSchema.TimeStampProperty] = value;
			}
		}

		public TenantThrottleState ThrottleState
		{
			get
			{
				return (TenantThrottleState)this[TenantThrottleInfoSchema.ThrottleStateProperty];
			}
			set
			{
				this[TenantThrottleInfoSchema.ThrottleStateProperty] = value;
			}
		}

		public bool IsThrottled
		{
			get
			{
				return this.ThrottleState == TenantThrottleState.Throttled || (this.ThrottleState == TenantThrottleState.Auto && this.ThrottlingFactor > 0.0);
			}
		}

		public int MessageCount
		{
			get
			{
				return (int)this[TenantThrottleInfoSchema.MessageCountProperty];
			}
			set
			{
				this[TenantThrottleInfoSchema.MessageCountProperty] = value;
			}
		}

		public double AverageMessageSizeKb
		{
			get
			{
				return (double)this[TenantThrottleInfoSchema.AvgMessageSizeKbProperty];
			}
			set
			{
				this[TenantThrottleInfoSchema.AvgMessageSizeKbProperty] = value;
			}
		}

		public double AverageMessageCostMs
		{
			get
			{
				return (double)this[TenantThrottleInfoSchema.AvgMessageCostMsProperty];
			}
			set
			{
				this[TenantThrottleInfoSchema.AvgMessageCostMsProperty] = value;
			}
		}

		public double ThrottlingFactor
		{
			get
			{
				return (double)this[TenantThrottleInfoSchema.ThrottlingFactorProperty];
			}
			set
			{
				this[TenantThrottleInfoSchema.ThrottlingFactorProperty] = value;
			}
		}

		public int PartitionTenantCount
		{
			get
			{
				return (int)this[TenantThrottleInfoSchema.PartitionTenantCountProperty];
			}
			set
			{
				this[TenantThrottleInfoSchema.PartitionTenantCountProperty] = value;
			}
		}

		public int PartitionMessageCount
		{
			get
			{
				return (int)this[TenantThrottleInfoSchema.PartitionMessageCountProperty];
			}
			set
			{
				this[TenantThrottleInfoSchema.PartitionMessageCountProperty] = value;
			}
		}

		public double PartitionAverageMessageSizeKb
		{
			get
			{
				return (double)this[TenantThrottleInfoSchema.PartitionAvgMessageSizeKbProperty];
			}
			set
			{
				this[TenantThrottleInfoSchema.PartitionAvgMessageSizeKbProperty] = value;
			}
		}

		public double PartitionAverageMessageCostMs
		{
			get
			{
				return (double)this[TenantThrottleInfoSchema.PartitionAvgMessageCostMsProperty];
			}
			set
			{
				this[TenantThrottleInfoSchema.PartitionAvgMessageCostMsProperty] = value;
			}
		}

		public double StandardDeviation
		{
			get
			{
				return (double)this[TenantThrottleInfoSchema.StandardDeviationProperty];
			}
			set
			{
				this[TenantThrottleInfoSchema.StandardDeviationProperty] = value;
			}
		}

		internal override ObjectSchema ObjectSchema
		{
			get
			{
				return TenantThrottleInfo.SchemaObject;
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2010;
			}
		}

		private static readonly TenantThrottleInfoSchema SchemaObject = ObjectSchema.GetInstance<TenantThrottleInfoSchema>();
	}
}
