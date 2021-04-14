using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.SoapWebClient.EWS;

namespace Microsoft.Exchange.UM.UMCommon
{
	[XmlType(TypeName = "UMReportRawCountersType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DataContract(Name = "UMReportRawCounters", Namespace = "http://schemas.microsoft.com/v1.0/UMReportAggregatedData")]
	public class UMReportRawCounters
	{
		[DataMember(Name = "AutoAttendantCalls")]
		[XmlElement]
		public ulong AutoAttendantCalls { get; set; }

		[DataMember(Name = "FailedCalls")]
		[XmlElement]
		public ulong FailedCalls { get; set; }

		[DataMember(Name = "FaxCalls")]
		[XmlElement]
		public ulong FaxCalls { get; set; }

		[DataMember(Name = "MissedCalls")]
		[XmlElement]
		public ulong MissedCalls { get; set; }

		[DataMember(Name = "OtherCalls")]
		[XmlElement]
		public ulong OtherCalls { get; set; }

		[DataMember(Name = "OutboundCalls")]
		[XmlElement]
		public ulong OutboundCalls { get; set; }

		[XmlElement]
		[DataMember(Name = "SubscriberAccessCalls")]
		public ulong SubscriberAccessCalls { get; set; }

		[XmlElement]
		[DataMember(Name = "VoiceMailCalls")]
		public ulong VoiceMailCalls { get; set; }

		[XmlElement]
		[DataMember(Name = "TotalCalls")]
		public ulong TotalCalls { get; set; }

		[DataMember(Name = "Date")]
		[XmlElement]
		public DateTime Date { get; set; }

		[DataMember(Name = "AudioMetricsAverages")]
		[XmlElement]
		public UMReportAudioMetricsAverageCounters AudioMetricsAverages { get; set; }

		public UMReportRawCounters() : this(default(DateTime))
		{
		}

		public UMReportRawCounters(DateTime dateTime)
		{
			this.AutoAttendantCalls = 0UL;
			this.Date = dateTime;
			this.FailedCalls = 0UL;
			this.FaxCalls = 0UL;
			this.MissedCalls = 0UL;
			this.OutboundCalls = 0UL;
			this.OtherCalls = 0UL;
			this.SubscriberAccessCalls = 0UL;
			this.VoiceMailCalls = 0UL;
			this.TotalCalls = 0UL;
			this.AudioMetricsAverages = new UMReportAudioMetricsAverageCounters();
		}

		public void AddCDR(CDRData cdrData)
		{
			this.TotalCalls += 1UL;
			this.AudioMetricsAverages.AddAudioQualityMetrics(cdrData);
			if (!this.TryIncrementFailedCalls(cdrData))
			{
				string callType;
				switch (callType = cdrData.CallType)
				{
				case "AutoAttendant":
					this.AutoAttendantCalls += 1UL;
					return;
				case "CallAnsweringMissedCall":
					this.MissedCalls += 1UL;
					return;
				case "CallAnsweringVoiceMessage":
					this.VoiceMailCalls += 1UL;
					return;
				case "Fax":
					this.FaxCalls += 1UL;
					return;
				case "PlayOnPhone":
				case "PlayOnPhonePAAGreeting":
				case "FindMe":
					this.OutboundCalls += 1UL;
					return;
				case "SubscriberAccess":
					this.SubscriberAccessCalls += 1UL;
					return;
				case "PromptProvisioning":
				case "UnAuthenticatedPilotNumber":
				case "VirtualNumberCall":
					this.OtherCalls += 1UL;
					return;
				}
				this.OtherCalls += 1UL;
			}
		}

		private bool TryIncrementFailedCalls(CDRData cdrData)
		{
			if (string.Equals(cdrData.CallType, "None", StringComparison.OrdinalIgnoreCase) || string.Equals(cdrData.DropCallReason, "SystemError", StringComparison.OrdinalIgnoreCase) || string.Equals(cdrData.OfferResult, "Reject", StringComparison.OrdinalIgnoreCase) || string.Equals(cdrData.DropCallReason, "OutboundFailedCall", StringComparison.OrdinalIgnoreCase))
			{
				this.FailedCalls += 1UL;
				return true;
			}
			return false;
		}

		public UMReportRawCounters(UMReportRawCountersType rawCountersType)
		{
			this.AutoAttendantCalls = (ulong)rawCountersType.AutoAttendantCalls;
			this.Date = rawCountersType.Date;
			this.FailedCalls = (ulong)rawCountersType.FailedCalls;
			this.FaxCalls = (ulong)rawCountersType.FaxCalls;
			this.MissedCalls = (ulong)rawCountersType.MissedCalls;
			this.OutboundCalls = (ulong)rawCountersType.OutboundCalls;
			this.OtherCalls = (ulong)rawCountersType.OtherCalls;
			this.SubscriberAccessCalls = (ulong)rawCountersType.SubscriberAccessCalls;
			this.VoiceMailCalls = (ulong)rawCountersType.VoiceMailCalls;
			this.TotalCalls = (ulong)rawCountersType.TotalCalls;
			this.AudioMetricsAverages = new UMReportAudioMetricsAverageCounters(rawCountersType.AudioMetricsAverages);
		}

		[OnDeserialized]
		private void Initialize(StreamingContext context)
		{
			if (this.AudioMetricsAverages == null)
			{
				this.AudioMetricsAverages = new UMReportAudioMetricsAverageCounters();
			}
		}
	}
}
