using System;
using System.Globalization;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class MethodCallLatencyStatisticsLogger : StatisticsLogger
	{
		protected MethodCallLatencyStatisticsLogger()
		{
		}

		public static MethodCallLatencyStatisticsLogger Instance
		{
			get
			{
				return MethodCallLatencyStatisticsLogger.instance;
			}
		}

		protected override StatisticsLogger.StatisticsLogSchema LogSchema
		{
			get
			{
				return this.logSchema;
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<MethodCallLatencyStatisticsLogger>(this);
		}

		private readonly StatisticsLogger.StatisticsLogSchema logSchema = new MethodCallLatencyStatisticsLogger.MethodCallLatencyStatisticsLogSchema();

		private static MethodCallLatencyStatisticsLogger instance = new MethodCallLatencyStatisticsLogger();

		private enum Field
		{
			RequestId,
			StartTime,
			Latency,
			MethodName,
			UserObjectGuid,
			TenantGuid,
			Tag
		}

		public class MethodCallLatencyStatisticsLogSchema : StatisticsLogger.StatisticsLogSchema
		{
			public MethodCallLatencyStatisticsLogSchema() : this("MethodCallLatencyStatistics")
			{
			}

			protected MethodCallLatencyStatisticsLogSchema(string logType) : base("1.0", logType, MethodCallLatencyStatisticsLogger.MethodCallLatencyStatisticsLogSchema.columns)
			{
			}

			public const string MethodCallLatencyStatisticsLogType = "MethodCallLatencyStatistics";

			public const string MethodCallLatencyStatisticsLogVersion = "1.0";

			private static readonly StatisticsLogger.StatisticsLogColumn[] columns = new StatisticsLogger.StatisticsLogColumn[]
			{
				new StatisticsLogger.StatisticsLogColumn(MethodCallLatencyStatisticsLogger.Field.RequestId.ToString(), false),
				new StatisticsLogger.StatisticsLogColumn(MethodCallLatencyStatisticsLogger.Field.StartTime.ToString(), false),
				new StatisticsLogger.StatisticsLogColumn(MethodCallLatencyStatisticsLogger.Field.Latency.ToString(), false),
				new StatisticsLogger.StatisticsLogColumn(MethodCallLatencyStatisticsLogger.Field.MethodName.ToString(), false),
				new StatisticsLogger.StatisticsLogColumn(MethodCallLatencyStatisticsLogger.Field.UserObjectGuid.ToString(), false),
				new StatisticsLogger.StatisticsLogColumn(MethodCallLatencyStatisticsLogger.Field.TenantGuid.ToString(), false),
				new StatisticsLogger.StatisticsLogColumn(MethodCallLatencyStatisticsLogger.Field.Tag.ToString(), false)
			};
		}

		public class MethodCallLatencyStatisticsLogRow : StatisticsLogger.StatisticsLogRow
		{
			public MethodCallLatencyStatisticsLogRow() : base(MethodCallLatencyStatisticsLogger.Instance.LogSchema)
			{
			}

			public Guid RequestId { get; set; }

			public ExDateTime StartTime { get; set; }

			public TimeSpan Latency { get; set; }

			public string MethodName { get; set; }

			public Guid? UserObjectGuid { get; set; }

			public Guid? TenantGuid { get; set; }

			public string Tag { get; set; }

			public override void PopulateFields()
			{
				base.Fields[0] = this.RequestId.ToString();
				base.Fields[1] = this.StartTime.ToString("o");
				base.Fields[2] = this.Latency.TotalSeconds.ToString(CultureInfo.InvariantCulture);
				base.Fields[3] = this.MethodName;
				base.Fields[4] = ((this.UserObjectGuid == null) ? string.Empty : this.UserObjectGuid.ToString());
				base.Fields[5] = ((this.TenantGuid == null) ? string.Empty : this.TenantGuid.ToString());
				base.Fields[6] = this.Tag;
			}
		}
	}
}
