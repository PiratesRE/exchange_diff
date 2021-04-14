using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class CallPerformanceLogger : StatisticsLogger
	{
		protected CallPerformanceLogger()
		{
		}

		public static CallPerformanceLogger Instance
		{
			get
			{
				return CallPerformanceLogger.instance;
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
			return DisposeTracker.Get<CallPerformanceLogger>(this);
		}

		private readonly StatisticsLogger.StatisticsLogSchema logSchema = new CallPerformanceLogger.CallPerformanceLogSchema();

		private static CallPerformanceLogger instance = new CallPerformanceLogger();

		private enum Field
		{
			CallId,
			UMServerName,
			Component,
			CallStartTime,
			Duration,
			AdCount,
			AdLatency,
			MServeCount,
			MServeLatency,
			UserDataRpcCount,
			UserDataRpcLatency,
			UserDataAdCount,
			UserDataAdLatency,
			UserDataDuration,
			UserDataTimedOut,
			UserAgent
		}

		public class CallPerformanceLogSchema : StatisticsLogger.StatisticsLogSchema
		{
			public CallPerformanceLogSchema() : this("CallPerformance")
			{
			}

			protected CallPerformanceLogSchema(string logType) : base("1.1", logType, CallPerformanceLogger.CallPerformanceLogSchema.columns)
			{
			}

			private const string CallPerformanceLogType = "CallPerformance";

			private const string CallPerformanceLogVersion = "1.1";

			private static readonly StatisticsLogger.StatisticsLogColumn[] columns = new StatisticsLogger.StatisticsLogColumn[]
			{
				new StatisticsLogger.StatisticsLogColumn(CallPerformanceLogger.Field.CallId.ToString(), false),
				new StatisticsLogger.StatisticsLogColumn(CallPerformanceLogger.Field.UMServerName.ToString(), false),
				new StatisticsLogger.StatisticsLogColumn(CallPerformanceLogger.Field.Component.ToString(), false),
				new StatisticsLogger.StatisticsLogColumn(CallPerformanceLogger.Field.CallStartTime.ToString(), false),
				new StatisticsLogger.StatisticsLogColumn(CallPerformanceLogger.Field.Duration.ToString(), false),
				new StatisticsLogger.StatisticsLogColumn(CallPerformanceLogger.Field.AdCount.ToString(), false),
				new StatisticsLogger.StatisticsLogColumn(CallPerformanceLogger.Field.AdLatency.ToString(), false),
				new StatisticsLogger.StatisticsLogColumn(CallPerformanceLogger.Field.MServeCount.ToString(), false),
				new StatisticsLogger.StatisticsLogColumn(CallPerformanceLogger.Field.MServeLatency.ToString(), false),
				new StatisticsLogger.StatisticsLogColumn(CallPerformanceLogger.Field.UserDataRpcCount.ToString(), false),
				new StatisticsLogger.StatisticsLogColumn(CallPerformanceLogger.Field.UserDataRpcLatency.ToString(), false),
				new StatisticsLogger.StatisticsLogColumn(CallPerformanceLogger.Field.UserDataAdCount.ToString(), false),
				new StatisticsLogger.StatisticsLogColumn(CallPerformanceLogger.Field.UserDataAdLatency.ToString(), false),
				new StatisticsLogger.StatisticsLogColumn(CallPerformanceLogger.Field.UserDataDuration.ToString(), false),
				new StatisticsLogger.StatisticsLogColumn(CallPerformanceLogger.Field.UserDataTimedOut.ToString(), false),
				new StatisticsLogger.StatisticsLogColumn(CallPerformanceLogger.Field.UserAgent.ToString(), false)
			};
		}

		public class CallPerformanceLogRow : StatisticsLogger.StatisticsLogRow
		{
			public CallPerformanceLogRow() : base(CallPerformanceLogger.Instance.LogSchema)
			{
			}

			public string CallId { get; set; }

			public string UMServerName { get; set; }

			public string Component { get; set; }

			public DateTime CallStartTime { get; set; }

			public int Duration { get; set; }

			public uint AdCount { get; set; }

			public int AdLatency { get; set; }

			public uint MServeCount { get; set; }

			public int MServeLatency { get; set; }

			public uint UserDataRpcCount { get; set; }

			public int UserDataRpcLatency { get; set; }

			public uint UserDataAdCount { get; set; }

			public int UserDataAdLatency { get; set; }

			public int UserDataDuration { get; set; }

			public bool UserDataTimedOut { get; set; }

			public string UserAgent { get; set; }

			public override void PopulateFields()
			{
				base.Fields[0] = this.CallId;
				base.Fields[1] = this.UMServerName;
				base.Fields[2] = this.Component.ToString();
				base.Fields[3] = this.CallStartTime.ToString("o");
				base.Fields[4] = this.Duration.ToString();
				base.Fields[5] = this.AdCount.ToString();
				base.Fields[6] = this.AdLatency.ToString();
				base.Fields[7] = this.MServeCount.ToString();
				base.Fields[8] = this.MServeLatency.ToString();
				base.Fields[9] = this.UserDataRpcCount.ToString();
				base.Fields[10] = this.UserDataRpcLatency.ToString();
				base.Fields[11] = this.UserDataAdCount.ToString();
				base.Fields[12] = this.UserDataAdLatency.ToString();
				base.Fields[13] = this.UserDataDuration.ToString();
				base.Fields[14] = this.UserDataTimedOut.ToString();
				base.Fields[15] = this.UserAgent.ToString();
			}
		}
	}
}
