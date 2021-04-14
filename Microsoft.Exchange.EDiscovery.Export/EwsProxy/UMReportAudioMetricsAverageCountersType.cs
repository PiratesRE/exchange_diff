using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DesignerCategory("code")]
	[DebuggerStepThrough]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public class UMReportAudioMetricsAverageCountersType
	{
		public AudioMetricsAverageType NMOS
		{
			get
			{
				return this.nMOSField;
			}
			set
			{
				this.nMOSField = value;
			}
		}

		public AudioMetricsAverageType NMOSDegradation
		{
			get
			{
				return this.nMOSDegradationField;
			}
			set
			{
				this.nMOSDegradationField = value;
			}
		}

		public AudioMetricsAverageType Jitter
		{
			get
			{
				return this.jitterField;
			}
			set
			{
				this.jitterField = value;
			}
		}

		public AudioMetricsAverageType PercentPacketLoss
		{
			get
			{
				return this.percentPacketLossField;
			}
			set
			{
				this.percentPacketLossField = value;
			}
		}

		public AudioMetricsAverageType RoundTrip
		{
			get
			{
				return this.roundTripField;
			}
			set
			{
				this.roundTripField = value;
			}
		}

		public AudioMetricsAverageType BurstLossDuration
		{
			get
			{
				return this.burstLossDurationField;
			}
			set
			{
				this.burstLossDurationField = value;
			}
		}

		public long TotalAudioQualityCallsSampled
		{
			get
			{
				return this.totalAudioQualityCallsSampledField;
			}
			set
			{
				this.totalAudioQualityCallsSampledField = value;
			}
		}

		private AudioMetricsAverageType nMOSField;

		private AudioMetricsAverageType nMOSDegradationField;

		private AudioMetricsAverageType jitterField;

		private AudioMetricsAverageType percentPacketLossField;

		private AudioMetricsAverageType roundTripField;

		private AudioMetricsAverageType burstLossDurationField;

		private long totalAudioQualityCallsSampledField;
	}
}
