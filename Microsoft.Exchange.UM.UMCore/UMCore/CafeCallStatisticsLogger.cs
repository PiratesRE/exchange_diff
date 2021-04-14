using System;
using System.Globalization;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class CafeCallStatisticsLogger : StatisticsLogger
	{
		protected CafeCallStatisticsLogger()
		{
		}

		public static CafeCallStatisticsLogger Instance
		{
			get
			{
				return CafeCallStatisticsLogger.instance;
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
			return DisposeTracker.Get<CafeCallStatisticsLogger>(this);
		}

		private readonly StatisticsLogger.StatisticsLogSchema logSchema = new CafeCallStatisticsLogger.CafeCallStatisticsLogSchema();

		private static CafeCallStatisticsLogger instance = new CafeCallStatisticsLogger();

		private enum Field
		{
			CallStartTime,
			CallLatency,
			CallType,
			CallId,
			CafeServerName,
			DialPlanGuid,
			DialPlanType,
			CalledPhoneNumber,
			CallerPhoneNumber,
			OfferResult,
			OrganizationId
		}

		public class CafeCallStatisticsLogSchema : StatisticsLogger.StatisticsLogSchema
		{
			public CafeCallStatisticsLogSchema() : this("UmCallRouter")
			{
			}

			protected CafeCallStatisticsLogSchema(string logType) : base("1.0", logType, CafeCallStatisticsLogger.CafeCallStatisticsLogSchema.columns)
			{
			}

			private const string CallStatisticsLogType = "UmCallRouter";

			private const string CallStatisticsLogVersion = "1.0";

			private static readonly StatisticsLogger.StatisticsLogColumn[] columns = new StatisticsLogger.StatisticsLogColumn[]
			{
				new StatisticsLogger.StatisticsLogColumn(CafeCallStatisticsLogger.Field.CallStartTime.ToString(), false),
				new StatisticsLogger.StatisticsLogColumn(CafeCallStatisticsLogger.Field.CallLatency.ToString(), false),
				new StatisticsLogger.StatisticsLogColumn(CafeCallStatisticsLogger.Field.CallType.ToString(), false),
				new StatisticsLogger.StatisticsLogColumn(CafeCallStatisticsLogger.Field.CallId.ToString(), false),
				new StatisticsLogger.StatisticsLogColumn(CafeCallStatisticsLogger.Field.CafeServerName.ToString(), false),
				new StatisticsLogger.StatisticsLogColumn(CafeCallStatisticsLogger.Field.DialPlanGuid.ToString(), true),
				new StatisticsLogger.StatisticsLogColumn(CafeCallStatisticsLogger.Field.DialPlanType.ToString(), false),
				new StatisticsLogger.StatisticsLogColumn(CafeCallStatisticsLogger.Field.CalledPhoneNumber.ToString(), true),
				new StatisticsLogger.StatisticsLogColumn(CafeCallStatisticsLogger.Field.CallerPhoneNumber.ToString(), true),
				new StatisticsLogger.StatisticsLogColumn(CafeCallStatisticsLogger.Field.OfferResult.ToString(), false),
				new StatisticsLogger.StatisticsLogColumn(CafeCallStatisticsLogger.Field.OrganizationId.ToString(), false)
			};
		}

		public class CafeCallStatisticsLogRow : StatisticsLogger.StatisticsLogRow
		{
			public CafeCallStatisticsLogRow() : base(CafeCallStatisticsLogger.Instance.LogSchema)
			{
			}

			public DateTime CallStartTime { get; set; }

			public TimeSpan CallLatency { get; set; }

			public string CallType { get; set; }

			public string CallIdentity { get; set; }

			public string CafeServerName { get; set; }

			public Guid DialPlanGuid { get; set; }

			public string DialPlanType { get; set; }

			public string CalledPhoneNumber { get; set; }

			public string CallerPhoneNumber { get; set; }

			public string OfferResult { get; set; }

			public string OrganizationId { get; set; }

			public override void PopulateFields()
			{
				base.Fields[0] = this.CallStartTime.ToString(CultureInfo.InvariantCulture);
				base.Fields[1] = this.CallLatency.TotalSeconds.ToString(CultureInfo.InvariantCulture);
				base.Fields[2] = this.CallType;
				base.Fields[3] = this.CallIdentity;
				base.Fields[4] = this.CafeServerName;
				base.Fields[5] = this.DialPlanGuid.ToString();
				base.Fields[6] = this.DialPlanType;
				base.Fields[7] = this.CalledPhoneNumber;
				base.Fields[8] = this.CallerPhoneNumber;
				base.Fields[9] = this.OfferResult;
				base.Fields[10] = this.OrganizationId;
			}
		}
	}
}
