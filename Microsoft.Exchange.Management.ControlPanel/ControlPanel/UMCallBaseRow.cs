using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public abstract class UMCallBaseRow : BaseRow
	{
		public UMCallBaseRow(UMCallReportBase report) : base(UMUtils.CreateUniqueUMReportingRowIdentity(), report)
		{
			this.UMCallReportBase = report;
			this.audioQualityIcon = UMUtils.GetAudioQualityIconAndAlternateText(this.UMCallReportBase.NMOS, out this.audioQualityAltText);
		}

		private UMCallReportBase UMCallReportBase { get; set; }

		[DataMember]
		public string NMOS
		{
			get
			{
				return UMUtils.FormatAudioQualityMetricDisplay(UMUtils.FormatFloat(this.UMCallReportBase.NMOS));
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string NMOSDegradation
		{
			get
			{
				return UMUtils.FormatAudioQualityMetricDisplay(UMUtils.FormatFloat(this.UMCallReportBase.NMOSDegradation));
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string PacketLoss
		{
			get
			{
				string metric = string.Empty;
				if (this.UMCallReportBase.PercentPacketLoss != null)
				{
					metric = (this.UMCallReportBase.PercentPacketLoss.Value / 100f).ToString("#0.0%");
				}
				return UMUtils.FormatAudioQualityMetricDisplay(metric);
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string Jitter
		{
			get
			{
				return UMUtils.FormatAudioQualityMetricDisplay(UMUtils.AppendMillisecondSuffix(UMUtils.FormatFloat(this.UMCallReportBase.Jitter)));
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string RoundTrip
		{
			get
			{
				return UMUtils.FormatAudioQualityMetricDisplay(UMUtils.AppendMillisecondSuffix(UMUtils.FormatFloat(this.UMCallReportBase.RoundTripMilliseconds)));
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string BurstLossDuration
		{
			get
			{
				return UMUtils.FormatAudioQualityMetricDisplay(UMUtils.AppendMillisecondSuffix(UMUtils.FormatFloat(this.UMCallReportBase.BurstLossDurationMilliseconds)));
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string AudioQuality
		{
			get
			{
				return this.audioQualityIcon;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string AudioQualityAlternateText
		{
			get
			{
				return this.audioQualityAltText;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		private string audioQualityIcon;

		private string audioQualityAltText;
	}
}
