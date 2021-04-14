using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class UMCallDataRecordRow : UMCallBaseRow
	{
		public UMCallDataRecordRow(UMCallDataRecord report) : base(report)
		{
			this.UMCallDataRecord = report;
		}

		private UMCallDataRecord UMCallDataRecord { get; set; }

		[DataMember]
		public string Date
		{
			get
			{
				return ((ExDateTime)this.UMCallDataRecord.Date).ToUserDateTimeString();
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string Duration
		{
			get
			{
				return this.UMCallDataRecord.Duration.ToString();
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string AudioCodec
		{
			get
			{
				return UMUtils.FormatAudioQualityMetricDisplay(this.UMCallDataRecord.AudioCodec);
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string DialPlan
		{
			get
			{
				return this.UMCallDataRecord.DialPlan;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string CallType
		{
			get
			{
				return this.GetLocalizedCallType(this.UMCallDataRecord.CallType);
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string CallingNumber
		{
			get
			{
				return this.UMCallDataRecord.CallingNumber;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string CalledNumber
		{
			get
			{
				return this.UMCallDataRecord.CalledNumber;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string Gateway
		{
			get
			{
				return this.UMCallDataRecord.Gateway;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string UserMailboxName
		{
			get
			{
				return this.UMCallDataRecord.UserMailboxName;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		private string GetLocalizedCallType(string callType)
		{
			switch (callType)
			{
			case "CallAnsweringMissedCall":
				return Strings.UMReportingCAMissedCall;
			case "CallAnsweringVoiceMessage":
				return Strings.UMReportingCAVoiceMessage;
			case "FindMe":
				return Strings.UMReportingFindMe;
			case "PlayOnPhone":
				return Strings.UMReportingPlayOnPhone;
			case "PlayOnPhonePAAGreeting":
				return Strings.UMReportingPlayOnPhonePAAGreeting;
			case "PromptProvisioning":
				return Strings.UMReportingPromptProvisioning;
			case "SubscriberAccess":
				return Strings.UMReportingSubscriberAccess;
			case "UnAuthenticatedPilotNumber":
				return Strings.UMReportingUnAuthenticatedPilotNumber;
			case "VirtualNumberCall":
				return Strings.UMReportingVirtualNumberCall;
			}
			return callType;
		}
	}
}
