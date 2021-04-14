using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[DebuggerStepThrough]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public class UMReportRawCountersType
	{
		public long AutoAttendantCalls
		{
			get
			{
				return this.autoAttendantCallsField;
			}
			set
			{
				this.autoAttendantCallsField = value;
			}
		}

		public long FailedCalls
		{
			get
			{
				return this.failedCallsField;
			}
			set
			{
				this.failedCallsField = value;
			}
		}

		public long FaxCalls
		{
			get
			{
				return this.faxCallsField;
			}
			set
			{
				this.faxCallsField = value;
			}
		}

		public long MissedCalls
		{
			get
			{
				return this.missedCallsField;
			}
			set
			{
				this.missedCallsField = value;
			}
		}

		public long OtherCalls
		{
			get
			{
				return this.otherCallsField;
			}
			set
			{
				this.otherCallsField = value;
			}
		}

		public long OutboundCalls
		{
			get
			{
				return this.outboundCallsField;
			}
			set
			{
				this.outboundCallsField = value;
			}
		}

		public long SubscriberAccessCalls
		{
			get
			{
				return this.subscriberAccessCallsField;
			}
			set
			{
				this.subscriberAccessCallsField = value;
			}
		}

		public long VoiceMailCalls
		{
			get
			{
				return this.voiceMailCallsField;
			}
			set
			{
				this.voiceMailCallsField = value;
			}
		}

		public long TotalCalls
		{
			get
			{
				return this.totalCallsField;
			}
			set
			{
				this.totalCallsField = value;
			}
		}

		public DateTime Date
		{
			get
			{
				return this.dateField;
			}
			set
			{
				this.dateField = value;
			}
		}

		public UMReportAudioMetricsAverageCountersType AudioMetricsAverages
		{
			get
			{
				return this.audioMetricsAveragesField;
			}
			set
			{
				this.audioMetricsAveragesField = value;
			}
		}

		private long autoAttendantCallsField;

		private long failedCallsField;

		private long faxCallsField;

		private long missedCallsField;

		private long otherCallsField;

		private long outboundCallsField;

		private long subscriberAccessCallsField;

		private long voiceMailCallsField;

		private long totalCallsField;

		private DateTime dateField;

		private UMReportAudioMetricsAverageCountersType audioMetricsAveragesField;
	}
}
