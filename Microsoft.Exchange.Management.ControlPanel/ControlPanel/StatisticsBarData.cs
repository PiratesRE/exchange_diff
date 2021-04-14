using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class StatisticsBarData
	{
		public StatisticsBarData(uint usagePercentage, StatisticsBarState usageState, string usageText) : this(usagePercentage, usageState, usageText, null)
		{
		}

		public StatisticsBarData(uint usagePercentage, StatisticsBarState usageState, string usageText, string additionalInfoText)
		{
			this.UsagePercentage = usagePercentage;
			this.UsageState = usageState;
			this.UsageText = usageText;
			this.AdditionalInfoText = additionalInfoText;
		}

		[DataMember]
		public uint UsagePercentage { get; private set; }

		[DataMember]
		public StatisticsBarState UsageState { get; private set; }

		[DataMember]
		public string UsageText { get; private set; }

		[DataMember]
		public string AdditionalInfoText { get; private set; }

		public bool Equals(StatisticsBarData statisticsBardata)
		{
			return statisticsBardata != null && (string.Compare(this.UsageText, statisticsBardata.UsageText) == 0 && this.UsageState == statisticsBardata.UsageState && this.UsagePercentage == statisticsBardata.UsagePercentage) && string.Compare(this.AdditionalInfoText, statisticsBardata.AdditionalInfoText) == 0;
		}

		public override bool Equals(object obj)
		{
			return this.Equals((StatisticsBarData)obj);
		}

		public override int GetHashCode()
		{
			return (this.UsageText + this.UsageState.ToString() + this.UsagePercentage.ToString() + this.AdditionalInfoText).GetHashCode();
		}
	}
}
