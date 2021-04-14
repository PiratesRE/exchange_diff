using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.UM.UMCommon;
using Microsoft.Exchange.UM.UMCommon.CrossServerMailboxAccess;

namespace Microsoft.Exchange.Management.Tasks.UM
{
	[Cmdlet("Get", "UMCallSummaryReport")]
	public sealed class GetUMCallSummaryReport : UMReportsTaskBase<MailboxIdParameter>
	{
		private new MailboxIdParameter Identity
		{
			get
			{
				return base.Identity;
			}
			set
			{
				base.Identity = value;
			}
		}

		[Parameter(Mandatory = false)]
		[ValidateNotNullOrEmpty]
		public UMDialPlanIdParameter UMDialPlan
		{
			get
			{
				return (UMDialPlanIdParameter)base.Fields["UMDialPlan"];
			}
			set
			{
				base.Fields["UMDialPlan"] = value;
			}
		}

		[ValidateNotNullOrEmpty]
		[Parameter(Mandatory = false)]
		public UMIPGatewayIdParameter UMIPGateway
		{
			get
			{
				return (UMIPGatewayIdParameter)base.Fields["UMIPGateway"];
			}
			set
			{
				base.Fields["UMIPGateway"] = value;
			}
		}

		[Parameter(Mandatory = true)]
		public GroupBy GroupBy
		{
			get
			{
				return (GroupBy)base.Fields["GroupBy"];
			}
			set
			{
				base.Fields["GroupBy"] = value;
			}
		}

		protected override void InternalBeginProcessing()
		{
			base.InternalBeginProcessing();
			base.ValidateCommonParamsAndSetOrg(this.UMDialPlan, this.UMIPGateway, out this.dialPlanGuid, out this.gatewayGuid, out this.dialPlanName, out this.gatewayName);
		}

		protected override void ProcessMailbox()
		{
			try
			{
				using (IUMCallDataRecordStorage umcallDataRecordsAcessor = InterServerMailboxAccessor.GetUMCallDataRecordsAcessor(this.DataObject))
				{
					UMReportRawCounters[] umcallSummary = umcallDataRecordsAcessor.GetUMCallSummary(this.dialPlanGuid, this.gatewayGuid, this.GroupBy);
					if (umcallSummary != null)
					{
						this.WriteAsConfigObjects(umcallSummary);
					}
				}
			}
			catch (StorageTransientException exception)
			{
				base.WriteError(exception, ErrorCategory.ReadError, null);
			}
			catch (StoragePermanentException exception2)
			{
				base.WriteError(exception2, ErrorCategory.ReadError, null);
			}
			catch (CDROperationException exception3)
			{
				base.WriteError(exception3, ErrorCategory.ReadError, null);
			}
			catch (EWSUMMailboxAccessException exception4)
			{
				base.WriteError(exception4, ErrorCategory.ReadError, null);
			}
		}

		private void WriteAsConfigObjects(UMReportRawCounters[] counters)
		{
			foreach (UMReportRawCounters umreportRawCounters in counters)
			{
				UMCallSummaryReport umcallSummaryReport = new UMCallSummaryReport(this.DataObject.Identity);
				switch (this.GroupBy)
				{
				case GroupBy.Day:
					umcallSummaryReport.Date = umreportRawCounters.Date.ToShortDateString();
					break;
				case GroupBy.Month:
					umcallSummaryReport.Date = umreportRawCounters.Date.ToString("MMM/yyyy");
					break;
				case GroupBy.Total:
					umcallSummaryReport.Date = "---";
					break;
				default:
					throw new NotImplementedException("Value of GroupBy is unknown.");
				}
				umcallSummaryReport.AutoAttendant = umreportRawCounters.AutoAttendantCalls;
				umcallSummaryReport.FailedOrRejectedCalls = umreportRawCounters.FailedCalls;
				umcallSummaryReport.Fax = umreportRawCounters.FaxCalls;
				umcallSummaryReport.MissedCalls = umreportRawCounters.MissedCalls;
				umcallSummaryReport.OtherCalls = umreportRawCounters.OtherCalls;
				umcallSummaryReport.Outbound = umreportRawCounters.OutboundCalls;
				umcallSummaryReport.SubscriberAccess = umreportRawCounters.SubscriberAccessCalls;
				umcallSummaryReport.VoiceMessages = umreportRawCounters.VoiceMailCalls;
				umcallSummaryReport.TotalCalls = umreportRawCounters.TotalCalls;
				umcallSummaryReport.UMDialPlanName = this.dialPlanName;
				umcallSummaryReport.UMIPGatewayName = this.gatewayName;
				umcallSummaryReport.NMOS = Utils.GetNullableAudioQualityMetric((float)umreportRawCounters.AudioMetricsAverages.NMOS.Average);
				umcallSummaryReport.NMOSDegradation = Utils.GetNullableAudioQualityMetric((float)umreportRawCounters.AudioMetricsAverages.NMOSDegradation.Average);
				umcallSummaryReport.PercentPacketLoss = Utils.GetNullableAudioQualityMetric((float)umreportRawCounters.AudioMetricsAverages.PercentPacketLoss.Average);
				umcallSummaryReport.Jitter = Utils.GetNullableAudioQualityMetric((float)umreportRawCounters.AudioMetricsAverages.Jitter.Average);
				umcallSummaryReport.RoundTripMilliseconds = Utils.GetNullableAudioQualityMetric((float)umreportRawCounters.AudioMetricsAverages.RoundTrip.Average);
				umcallSummaryReport.BurstLossDurationMilliseconds = Utils.GetNullableAudioQualityMetric((float)umreportRawCounters.AudioMetricsAverages.BurstLossDuration.Average);
				umcallSummaryReport.TotalAudioQualityCallsSampled = umreportRawCounters.AudioMetricsAverages.TotalAudioQualityCallsSampled;
				base.WriteObject(umcallSummaryReport);
			}
		}

		private const string TotalReportDateString = "---";

		private const string DateFormat = "MMM/yyyy";

		private const string FixedFormatString = "F1";

		private Guid dialPlanGuid;

		private Guid gatewayGuid;

		private string dialPlanName;

		private string gatewayName;
	}
}
