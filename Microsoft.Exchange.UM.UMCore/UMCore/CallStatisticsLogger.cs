using System;
using System.Globalization;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class CallStatisticsLogger : StatisticsLogger
	{
		protected CallStatisticsLogger()
		{
		}

		public static CallStatisticsLogger Instance
		{
			get
			{
				return CallStatisticsLogger.instance;
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
			return DisposeTracker.Get<CallStatisticsLogger>(this);
		}

		private readonly StatisticsLogger.StatisticsLogSchema logSchema = new CallStatisticsLogger.CallStatisticsLogSchema();

		private static CallStatisticsLogger instance = new CallStatisticsLogger();

		private enum Field
		{
			CallStartTime,
			CallType,
			CallId,
			ParentCallIdentity,
			UMServerName,
			DialPlanGuid,
			DialPlanName,
			CallDuration,
			IPGatewayAddress,
			GatewayGuid,
			CalledPhoneNumber,
			CallerPhoneNumber,
			OfferResult,
			DropCallReason,
			ReasonForCall,
			TransferredNumber,
			DialedString,
			CallerMailboxAlias,
			CalleeMailboxAlias,
			AutoAttendantName,
			OrganizationId,
			AudioCodec,
			AudioQualityBurstDensity,
			AudioQualityBurstDuration,
			AudioQualityJitter,
			AudioQualityNMOS,
			AudioQualityNMOSDegradation,
			AudioQualityNMOSDegradationJitter,
			AudioQualityNMOSDegradationPacketLoss,
			AudioQualityPacketLoss,
			AudioQualityRoundTrip
		}

		public class CallStatisticsLogSchema : StatisticsLogger.StatisticsLogSchema
		{
			public CallStatisticsLogSchema() : this("CallStatistics")
			{
			}

			protected CallStatisticsLogSchema(string logType) : base("1.2", logType, CallStatisticsLogger.CallStatisticsLogSchema.columns)
			{
			}

			private const string CallStatisticsLogType = "CallStatistics";

			private const string CallStatisticsLogVersion = "1.2";

			private static readonly StatisticsLogger.StatisticsLogColumn[] columns = new StatisticsLogger.StatisticsLogColumn[]
			{
				new StatisticsLogger.StatisticsLogColumn(CallStatisticsLogger.Field.CallStartTime.ToString(), false),
				new StatisticsLogger.StatisticsLogColumn(CallStatisticsLogger.Field.CallType.ToString(), false),
				new StatisticsLogger.StatisticsLogColumn(CallStatisticsLogger.Field.CallId.ToString(), false),
				new StatisticsLogger.StatisticsLogColumn(CallStatisticsLogger.Field.ParentCallIdentity.ToString(), true),
				new StatisticsLogger.StatisticsLogColumn(CallStatisticsLogger.Field.UMServerName.ToString(), false),
				new StatisticsLogger.StatisticsLogColumn(CallStatisticsLogger.Field.DialPlanGuid.ToString(), true),
				new StatisticsLogger.StatisticsLogColumn(CallStatisticsLogger.Field.DialPlanName.ToString(), true),
				new StatisticsLogger.StatisticsLogColumn(CallStatisticsLogger.Field.CallDuration.ToString(), false),
				new StatisticsLogger.StatisticsLogColumn(CallStatisticsLogger.Field.IPGatewayAddress.ToString(), false),
				new StatisticsLogger.StatisticsLogColumn(CallStatisticsLogger.Field.GatewayGuid.ToString(), false),
				new StatisticsLogger.StatisticsLogColumn(CallStatisticsLogger.Field.CalledPhoneNumber.ToString(), true),
				new StatisticsLogger.StatisticsLogColumn(CallStatisticsLogger.Field.CallerPhoneNumber.ToString(), true),
				new StatisticsLogger.StatisticsLogColumn(CallStatisticsLogger.Field.OfferResult.ToString(), false),
				new StatisticsLogger.StatisticsLogColumn(CallStatisticsLogger.Field.DropCallReason.ToString(), false),
				new StatisticsLogger.StatisticsLogColumn(CallStatisticsLogger.Field.ReasonForCall.ToString(), false),
				new StatisticsLogger.StatisticsLogColumn(CallStatisticsLogger.Field.TransferredNumber.ToString(), true),
				new StatisticsLogger.StatisticsLogColumn(CallStatisticsLogger.Field.DialedString.ToString(), true),
				new StatisticsLogger.StatisticsLogColumn(CallStatisticsLogger.Field.CallerMailboxAlias.ToString(), true),
				new StatisticsLogger.StatisticsLogColumn(CallStatisticsLogger.Field.CalleeMailboxAlias.ToString(), true),
				new StatisticsLogger.StatisticsLogColumn(CallStatisticsLogger.Field.AutoAttendantName.ToString(), true),
				new StatisticsLogger.StatisticsLogColumn(CallStatisticsLogger.Field.OrganizationId.ToString(), false),
				new StatisticsLogger.StatisticsLogColumn(CallStatisticsLogger.Field.AudioCodec.ToString(), false),
				new StatisticsLogger.StatisticsLogColumn(CallStatisticsLogger.Field.AudioQualityBurstDensity.ToString(), false),
				new StatisticsLogger.StatisticsLogColumn(CallStatisticsLogger.Field.AudioQualityBurstDuration.ToString(), false),
				new StatisticsLogger.StatisticsLogColumn(CallStatisticsLogger.Field.AudioQualityJitter.ToString(), false),
				new StatisticsLogger.StatisticsLogColumn(CallStatisticsLogger.Field.AudioQualityNMOS.ToString(), false),
				new StatisticsLogger.StatisticsLogColumn(CallStatisticsLogger.Field.AudioQualityNMOSDegradation.ToString(), false),
				new StatisticsLogger.StatisticsLogColumn(CallStatisticsLogger.Field.AudioQualityNMOSDegradationJitter.ToString(), false),
				new StatisticsLogger.StatisticsLogColumn(CallStatisticsLogger.Field.AudioQualityNMOSDegradationPacketLoss.ToString(), false),
				new StatisticsLogger.StatisticsLogColumn(CallStatisticsLogger.Field.AudioQualityPacketLoss.ToString(), false),
				new StatisticsLogger.StatisticsLogColumn(CallStatisticsLogger.Field.AudioQualityRoundTrip.ToString(), false)
			};
		}

		public class CallStatisticsLogRow : StatisticsLogger.StatisticsLogRow
		{
			public CallStatisticsLogRow() : base(CallStatisticsLogger.Instance.LogSchema)
			{
			}

			public DateTime CallStartTime { get; set; }

			public string CallType { get; set; }

			public string CallIdentity { get; set; }

			public string ParentCallIdentity { get; set; }

			public string UMServerName { get; set; }

			public Guid DialPlanGuid { get; set; }

			public string DialPlanName { get; set; }

			public int CallDuration { get; set; }

			public string IPGatewayAddress { get; set; }

			public Guid GatewayGuid { get; set; }

			public string CalledPhoneNumber { get; set; }

			public string CallerPhoneNumber { get; set; }

			public string OfferResult { get; set; }

			public string DropCallReason { get; set; }

			public string ReasonForCall { get; set; }

			public string TransferredNumber { get; set; }

			public string DialedString { get; set; }

			public string CallerMailboxAlias { get; set; }

			public string CalleeMailboxAlias { get; set; }

			public string AutoAttendantName { get; set; }

			public string OrganizationId { get; set; }

			public string AudioCodec { get; set; }

			public float AudioQualityBurstDensity { get; set; }

			public float AudioQualityBurstDuration { get; set; }

			public float AudioQualityJitter { get; set; }

			public float AudioQualityNMOS { get; set; }

			public float AudioQualityNMOSDegradation { get; set; }

			public float AudioQualityNMOSDegradationJitter { get; set; }

			public float AudioQualityNMOSDegradationPacketLoss { get; set; }

			public float AudioQualityPacketLoss { get; set; }

			public float AudioQualityRoundTrip { get; set; }

			public override void PopulateFields()
			{
				base.Fields[0] = this.CallStartTime.ToString(CultureInfo.InvariantCulture);
				base.Fields[1] = this.CallType;
				base.Fields[2] = this.CallIdentity;
				base.Fields[3] = this.ParentCallIdentity;
				base.Fields[4] = this.UMServerName;
				base.Fields[5] = this.DialPlanGuid.ToString();
				base.Fields[6] = this.DialPlanName;
				base.Fields[7] = this.CallDuration.ToString();
				base.Fields[8] = this.IPGatewayAddress;
				base.Fields[9] = this.GatewayGuid.ToString();
				base.Fields[10] = this.CalledPhoneNumber;
				base.Fields[11] = this.CallerPhoneNumber;
				base.Fields[12] = this.OfferResult;
				base.Fields[13] = this.DropCallReason;
				base.Fields[14] = this.ReasonForCall;
				base.Fields[15] = this.TransferredNumber;
				base.Fields[16] = this.DialedString;
				base.Fields[17] = this.CallerMailboxAlias;
				base.Fields[18] = this.CalleeMailboxAlias;
				base.Fields[19] = this.AutoAttendantName;
				base.Fields[20] = this.OrganizationId;
				base.Fields[21] = this.AudioCodec;
				base.Fields[22] = this.AudioQualityBurstDensity.ToString();
				base.Fields[23] = this.AudioQualityBurstDuration.ToString();
				base.Fields[24] = this.AudioQualityJitter.ToString();
				base.Fields[25] = this.AudioQualityNMOS.ToString();
				base.Fields[26] = this.AudioQualityNMOSDegradation.ToString();
				base.Fields[27] = this.AudioQualityNMOSDegradationJitter.ToString();
				base.Fields[28] = this.AudioQualityNMOSDegradationPacketLoss.ToString();
				base.Fields[29] = this.AudioQualityPacketLoss.ToString();
				base.Fields[30] = this.AudioQualityRoundTrip.ToString();
			}
		}
	}
}
