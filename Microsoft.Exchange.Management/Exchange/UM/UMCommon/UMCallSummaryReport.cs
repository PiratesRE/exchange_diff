using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.UM.UMCommon
{
	[Serializable]
	public class UMCallSummaryReport : UMCallReportBase
	{
		public UMCallSummaryReport(ObjectId identity) : base(identity)
		{
		}

		public ulong AutoAttendant
		{
			get
			{
				return (ulong)this[UMCallSummaryReportSchema.AutoAttendant];
			}
			internal set
			{
				this[UMCallSummaryReportSchema.AutoAttendant] = value;
			}
		}

		public ulong FailedOrRejectedCalls
		{
			get
			{
				return (ulong)this[UMCallSummaryReportSchema.FailedOrRejectedCalls];
			}
			internal set
			{
				this[UMCallSummaryReportSchema.FailedOrRejectedCalls] = value;
			}
		}

		public ulong Fax
		{
			get
			{
				return (ulong)this[UMCallSummaryReportSchema.Fax];
			}
			internal set
			{
				this[UMCallSummaryReportSchema.Fax] = value;
			}
		}

		public ulong MissedCalls
		{
			get
			{
				return (ulong)this[UMCallSummaryReportSchema.MissedCalls];
			}
			internal set
			{
				this[UMCallSummaryReportSchema.MissedCalls] = value;
			}
		}

		public ulong OtherCalls
		{
			get
			{
				return (ulong)this[UMCallSummaryReportSchema.OtherCalls];
			}
			internal set
			{
				this[UMCallSummaryReportSchema.OtherCalls] = value;
			}
		}

		public ulong Outbound
		{
			get
			{
				return (ulong)this[UMCallSummaryReportSchema.Outbound];
			}
			internal set
			{
				this[UMCallSummaryReportSchema.Outbound] = value;
			}
		}

		public ulong SubscriberAccess
		{
			get
			{
				return (ulong)this[UMCallSummaryReportSchema.SubscriberAccess];
			}
			internal set
			{
				this[UMCallSummaryReportSchema.SubscriberAccess] = value;
			}
		}

		public ulong VoiceMessages
		{
			get
			{
				return (ulong)this[UMCallSummaryReportSchema.VoiceMessages];
			}
			internal set
			{
				this[UMCallSummaryReportSchema.VoiceMessages] = value;
			}
		}

		public ulong TotalCalls
		{
			get
			{
				return (ulong)this[UMCallSummaryReportSchema.TotalCalls];
			}
			internal set
			{
				this[UMCallSummaryReportSchema.TotalCalls] = value;
			}
		}

		public string Date
		{
			get
			{
				return (string)this[UMCallSummaryReportSchema.Date];
			}
			internal set
			{
				this[UMCallSummaryReportSchema.Date] = value;
			}
		}

		public ulong TotalAudioQualityCallsSampled
		{
			get
			{
				return (ulong)this[UMCallSummaryReportSchema.TotalAudioQualityCallsSampled];
			}
			internal set
			{
				this[UMCallSummaryReportSchema.TotalAudioQualityCallsSampled] = value;
			}
		}

		public string UMDialPlanName
		{
			get
			{
				return (string)this[UMCallSummaryReportSchema.UMDialPlanName];
			}
			internal set
			{
				this[UMCallSummaryReportSchema.UMDialPlanName] = value;
			}
		}

		public string UMIPGatewayName
		{
			get
			{
				return (string)this[UMCallSummaryReportSchema.UMIPGatewayName];
			}
			internal set
			{
				this[UMCallSummaryReportSchema.UMIPGatewayName] = value;
			}
		}

		internal override ObjectSchema ObjectSchema
		{
			get
			{
				return UMCallSummaryReport.schema;
			}
		}

		private static UMCallSummaryReportSchema schema = ObjectSchema.GetInstance<UMCallSummaryReportSchema>();
	}
}
