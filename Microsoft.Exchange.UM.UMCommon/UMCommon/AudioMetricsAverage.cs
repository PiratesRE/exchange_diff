using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.SoapWebClient.EWS;
using Microsoft.Exchange.UM.TroubleshootingTool.Shared;

namespace Microsoft.Exchange.UM.UMCommon
{
	[DataContract(Name = "AudioMetricsAverage", Namespace = "http://schemas.microsoft.com/v1.0/UMReportAggregatedData")]
	[XmlType(TypeName = "AudioMetricsAverageType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	public class AudioMetricsAverage
	{
		public AudioMetricsAverage()
		{
			this.TotalValue = 0.0;
			this.TotalCount = 0.0;
		}

		public AudioMetricsAverage(AudioMetricsAverageType audioMetricsAverageType)
		{
			this.TotalCount = audioMetricsAverageType.TotalCount;
			this.TotalValue = audioMetricsAverageType.TotalValue;
		}

		[DataMember(Name = "TotalValue")]
		[XmlElement]
		public double TotalValue { get; set; }

		[XmlElement]
		[DataMember(Name = "TotalCount")]
		public double TotalCount { get; set; }

		[DataMember(Name = "Average")]
		public double Average
		{
			get
			{
				if (this.TotalCount == 0.0)
				{
					return (double)AudioQuality.UnknownValue;
				}
				return this.TotalValue / this.TotalCount;
			}
			set
			{
			}
		}

		public void Add(double value)
		{
			this.TotalCount += 1.0;
			this.TotalValue += value;
		}
	}
}
