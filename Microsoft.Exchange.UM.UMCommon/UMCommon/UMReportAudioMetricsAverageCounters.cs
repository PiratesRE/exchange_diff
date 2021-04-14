using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.SoapWebClient.EWS;
using Microsoft.Exchange.UM.TroubleshootingTool.Shared;

namespace Microsoft.Exchange.UM.UMCommon
{
	[XmlType(TypeName = "UMReportAudioMetricsAverageCountersType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[DataContract(Name = "UMReportAudioMetricsAverageCounters", Namespace = "http://schemas.microsoft.com/v1.0/UMReportAggregatedData")]
	public class UMReportAudioMetricsAverageCounters
	{
		[XmlElement]
		[DataMember(Name = "NMOS")]
		public AudioMetricsAverage NMOS { get; set; }

		[DataMember(Name = "NMOSDegradation")]
		[XmlElement]
		public AudioMetricsAverage NMOSDegradation { get; set; }

		[DataMember(Name = "Jitter")]
		[XmlElement]
		public AudioMetricsAverage Jitter { get; set; }

		[XmlElement]
		[DataMember(Name = "PercentPacketLoss")]
		public AudioMetricsAverage PercentPacketLoss { get; set; }

		[DataMember(Name = "RoundTrip")]
		[XmlElement]
		public AudioMetricsAverage RoundTrip { get; set; }

		[DataMember(Name = "BurstLossDuration")]
		[XmlElement]
		public AudioMetricsAverage BurstLossDuration { get; set; }

		[DataMember(Name = "TotalAudioQualityCallsSampled")]
		[XmlElement]
		public ulong TotalAudioQualityCallsSampled { get; set; }

		public UMReportAudioMetricsAverageCounters()
		{
			this.NMOS = new AudioMetricsAverage();
			this.NMOSDegradation = new AudioMetricsAverage();
			this.Jitter = new AudioMetricsAverage();
			this.PercentPacketLoss = new AudioMetricsAverage();
			this.RoundTrip = new AudioMetricsAverage();
			this.BurstLossDuration = new AudioMetricsAverage();
		}

		public UMReportAudioMetricsAverageCounters(UMReportAudioMetricsAverageCountersType ewsType)
		{
			this.NMOS = new AudioMetricsAverage(ewsType.NMOS);
			this.Jitter = new AudioMetricsAverage(ewsType.Jitter);
			this.NMOSDegradation = new AudioMetricsAverage(ewsType.NMOSDegradation);
			this.PercentPacketLoss = new AudioMetricsAverage(ewsType.PercentPacketLoss);
			this.RoundTrip = new AudioMetricsAverage(ewsType.RoundTrip);
			this.BurstLossDuration = new AudioMetricsAverage(ewsType.BurstLossDuration);
			this.TotalAudioQualityCallsSampled = (ulong)ewsType.TotalAudioQualityCallsSampled;
		}

		public void AddAudioQualityMetrics(CDRData cdrData)
		{
			bool flag = false;
			this.AddAudioMetricToCounter(cdrData.AudioQualityMetrics.NMOS, this.NMOS, ref flag);
			this.AddAudioMetricToCounter(cdrData.AudioQualityMetrics.NMOSDegradation, this.NMOSDegradation, ref flag);
			this.AddAudioMetricToCounter(cdrData.AudioQualityMetrics.Jitter, this.Jitter, ref flag);
			this.AddAudioMetricToCounter(cdrData.AudioQualityMetrics.PacketLoss, this.PercentPacketLoss, ref flag);
			this.AddAudioMetricToCounter(cdrData.AudioQualityMetrics.RoundTrip, this.RoundTrip, ref flag);
			this.AddAudioMetricToCounter(cdrData.AudioQualityMetrics.BurstDuration, this.BurstLossDuration, ref flag);
			if (flag)
			{
				this.TotalAudioQualityCallsSampled += 1UL;
			}
		}

		private void AddAudioMetricToCounter(float metric, AudioMetricsAverage averageCounter, ref bool cdrSampledForAudioQuality)
		{
			if (metric != AudioQuality.UnknownValue)
			{
				averageCounter.Add((double)metric);
				cdrSampledForAudioQuality = true;
			}
		}
	}
}
