using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxLoadBalance.Band;
using Microsoft.Exchange.MailboxLoadBalance.Config;
using Microsoft.Exchange.MailboxLoadBalance.Data;

namespace Microsoft.Exchange.MailboxLoadBalance.Logging
{
	internal class BandRebalanceLog : ObjectLog<BandRebalanceLogEntry>
	{
		private BandRebalanceLog() : base(new BandRebalanceLog.BandRebalanceLogSchema(), new LoadBalanceLoggingConfig("BandBalance"))
		{
		}

		public static void Write(BandMailboxRebalanceData request)
		{
			foreach (LoadMetric loadMetric in request.RebalanceInformation.Metrics)
			{
				BandRebalanceLogEntry bandRebalanceLogEntry = new BandRebalanceLogEntry();
				bandRebalanceLogEntry.SourceDatabase = request.SourceDatabase.Name;
				bandRebalanceLogEntry.TargetDatabase = request.TargetDatabase.Name;
				bandRebalanceLogEntry.BatchName = request.RebalanceBatchName;
				bandRebalanceLogEntry.Metric = loadMetric.ToString();
				bandRebalanceLogEntry.RebalanceUnits = request.RebalanceInformation[loadMetric];
				BandRebalanceLog.Instance.LogObject(bandRebalanceLogEntry);
			}
		}

		private static readonly BandRebalanceLog Instance = new BandRebalanceLog();

		private class BandRebalanceLogData : ConfigurableObject
		{
			public BandRebalanceLogData(PropertyBag propertyBag) : base(propertyBag)
			{
			}

			internal override ObjectSchema ObjectSchema
			{
				get
				{
					return new DummyObjectSchema();
				}
			}
		}

		private class BandRebalanceLogSchema : ConfigurableObjectLogSchema<BandRebalanceLog.BandRebalanceLogData, DummyObjectSchema>
		{
			public override string LogType
			{
				get
				{
					return "Band Rebalance Requests";
				}
			}

			public override string Software
			{
				get
				{
					return "Mailbox Load Balancing";
				}
			}

			public static readonly ObjectLogSimplePropertyDefinition<BandRebalanceLogEntry> SourceDatabase = new ObjectLogSimplePropertyDefinition<BandRebalanceLogEntry>("SourceDatabase", (BandRebalanceLogEntry r) => r.SourceDatabase);

			public static readonly ObjectLogSimplePropertyDefinition<BandRebalanceLogEntry> TargetDatabase = new ObjectLogSimplePropertyDefinition<BandRebalanceLogEntry>("TargetDatabase", (BandRebalanceLogEntry r) => r.TargetDatabase);

			public static readonly ObjectLogSimplePropertyDefinition<BandRebalanceLogEntry> Metric = new ObjectLogSimplePropertyDefinition<BandRebalanceLogEntry>("Metric", (BandRebalanceLogEntry r) => r.Metric);

			public static readonly ObjectLogSimplePropertyDefinition<BandRebalanceLogEntry> Units = new ObjectLogSimplePropertyDefinition<BandRebalanceLogEntry>("Units", (BandRebalanceLogEntry r) => r.RebalanceUnits);

			public static readonly ObjectLogSimplePropertyDefinition<BandRebalanceLogEntry> BatchName = new ObjectLogSimplePropertyDefinition<BandRebalanceLogEntry>("BatchName", (BandRebalanceLogEntry r) => r.BatchName);
		}
	}
}
