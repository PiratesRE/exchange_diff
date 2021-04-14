using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.MailboxLoadBalance.CapacityData;
using Microsoft.Exchange.MailboxLoadBalance.Data;
using Microsoft.Exchange.MailboxReplicationService;

namespace Microsoft.Exchange.MailboxLoadBalanceClient
{
	public sealed class CapacitySummary : ConfigurableObject
	{
		public CapacitySummary() : base(new SimpleProviderPropertyBag())
		{
		}

		internal CapacitySummary(PropertyBag propertyBag) : base(propertyBag)
		{
		}

		public new string Identity
		{
			get
			{
				return (string)this[CapacitySummarySchema.Identity];
			}
			internal set
			{
				this[CapacitySummarySchema.Identity] = value;
			}
		}

		public Report.ListWithToString<LoadMetricValue> LoadMetrics { get; internal set; }

		public ByteQuantifiedSize LogicalSize
		{
			get
			{
				return (ByteQuantifiedSize)this[CapacitySummarySchema.LogicalSize];
			}
			internal set
			{
				this[CapacitySummarySchema.LogicalSize] = value;
			}
		}

		public ByteQuantifiedSize MaximumSize
		{
			get
			{
				return (ByteQuantifiedSize)this[CapacitySummarySchema.MaximumSize];
			}
			internal set
			{
				this[CapacitySummarySchema.MaximumSize] = value;
			}
		}

		public ByteQuantifiedSize PhysicalSize
		{
			get
			{
				return (ByteQuantifiedSize)this[CapacitySummarySchema.PhysicalSize];
			}
			internal set
			{
				this[CapacitySummarySchema.PhysicalSize] = value;
			}
		}

		public DateTime RetrievedTimeStamp
		{
			get
			{
				return (DateTime)this[CapacitySummarySchema.RetrievedTimestamp];
			}
			set
			{
				this[CapacitySummarySchema.RetrievedTimestamp] = value;
			}
		}

		public long TotalMailboxCount
		{
			get
			{
				return (long)this[CapacitySummarySchema.TotalMailboxCount];
			}
			internal set
			{
				this[CapacitySummarySchema.TotalMailboxCount] = value;
			}
		}

		internal override ObjectSchema ObjectSchema
		{
			get
			{
				return ObjectSchema.GetInstance<CapacitySummarySchema>();
			}
		}

		internal static CapacitySummary FromDatum(HeatMapCapacityData capacityDatum)
		{
			CapacitySummary capacitySummary = new CapacitySummary();
			capacitySummary.SetExchangeVersion(ExchangeObjectVersion.Current);
			capacitySummary.Identity = capacityDatum.Identity.Name;
			capacitySummary.PhysicalSize = capacityDatum.PhysicalSize;
			capacitySummary.LogicalSize = capacityDatum.LogicalSize;
			capacitySummary.MaximumSize = capacityDatum.TotalCapacity;
			capacitySummary.TotalMailboxCount = capacityDatum.TotalMailboxCount;
			capacitySummary.RetrievedTimeStamp = capacityDatum.RetrievedTimestamp;
			Report.ListWithToString<LoadMetricValue> listWithToString = new Report.ListWithToString<LoadMetricValue>();
			foreach (LoadMetric loadMetric in capacityDatum.LoadMetrics.Metrics)
			{
				LoadMetricValue item = new LoadMetricValue(loadMetric, capacityDatum.LoadMetrics[loadMetric]);
				listWithToString.Add(item);
			}
			capacitySummary.LoadMetrics = listWithToString;
			return capacitySummary;
		}
	}
}
