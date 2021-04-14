using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.UM.UMCommon
{
	[CollectionDataContract(Namespace = "http://schemas.microsoft.com/v1.0/UMReportAggregatedData", ItemName = "DailyReportEntry", KeyName = "Day", ValueName = "UMReportRawCounters")]
	internal class DailyReportDictionary : UMReportDictionaryBase
	{
		public override int MaxItemsInDictionary
		{
			get
			{
				return 90;
			}
		}

		private const int MaxDailyData = 90;
	}
}
