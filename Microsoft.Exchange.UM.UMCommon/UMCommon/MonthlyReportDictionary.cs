using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.UM.UMCommon
{
	[CollectionDataContract(Namespace = "http://schemas.microsoft.com/v1.0/UMReportAggregatedData", ItemName = "MonthlyReportEntry", KeyName = "Month", ValueName = "UMReportRawCounters")]
	internal class MonthlyReportDictionary : UMReportDictionaryBase
	{
		public override int MaxItemsInDictionary
		{
			get
			{
				return 12;
			}
		}

		private const int MaxMonthlyData = 12;
	}
}
