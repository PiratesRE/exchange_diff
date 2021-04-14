using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class UMCallSummaryReportRow : UMCallBaseRow
	{
		public UMCallSummaryReportRow(UMCallSummaryReport report) : base(report)
		{
			this.UMCallSummaryReport = report;
		}

		private UMCallSummaryReport UMCallSummaryReport { get; set; }

		[DataMember]
		public string Date
		{
			get
			{
				return this.UMCallSummaryReport.Date;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string AutoAttendant
		{
			get
			{
				return this.ConvertToPercentage(this.UMCallSummaryReport.AutoAttendant);
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string Failed
		{
			get
			{
				return this.ConvertToPercentage(this.UMCallSummaryReport.FailedOrRejectedCalls);
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string Fax
		{
			get
			{
				return this.ConvertToPercentage(this.UMCallSummaryReport.Fax);
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string MissedCalls
		{
			get
			{
				return this.ConvertToPercentage(this.UMCallSummaryReport.MissedCalls);
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string Outbound
		{
			get
			{
				return this.ConvertToPercentage(this.UMCallSummaryReport.Outbound);
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string Other
		{
			get
			{
				return this.ConvertToPercentage(this.UMCallSummaryReport.OtherCalls);
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string SubscriberAccess
		{
			get
			{
				return this.ConvertToPercentage(this.UMCallSummaryReport.SubscriberAccess);
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string VoiceMessages
		{
			get
			{
				return this.ConvertToPercentage(this.UMCallSummaryReport.VoiceMessages);
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string TotalCalls
		{
			get
			{
				return this.UMCallSummaryReport.TotalCalls.ToString();
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string TotalAudioQualityCallsSampled
		{
			get
			{
				return this.UMCallSummaryReport.TotalAudioQualityCallsSampled.ToString();
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string UMDialPlanName
		{
			get
			{
				if (string.IsNullOrEmpty(this.UMCallSummaryReport.UMDialPlanName))
				{
					return Strings.AllDialplans;
				}
				return this.UMCallSummaryReport.UMDialPlanName;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string UMIPGatewayName
		{
			get
			{
				if (string.IsNullOrEmpty(this.UMCallSummaryReport.UMIPGatewayName))
				{
					return Strings.AllGateways;
				}
				return this.UMCallSummaryReport.UMIPGatewayName;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public bool IsExportCallDataEnabled { get; set; }

		[DataMember]
		public string UMDialPlanID { get; set; }

		[DataMember]
		public string UMIPGatewayID { get; set; }

		private string ConvertToPercentage(ulong val)
		{
			if (this.UMCallSummaryReport.TotalCalls > 0UL)
			{
				return (val / this.UMCallSummaryReport.TotalCalls).ToString("#0.0%");
			}
			return "-";
		}
	}
}
