using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.UM.UMCommon
{
	[CollectionDataContract(Namespace = "http://schemas.microsoft.com/v1.0/UMReportAggregatedData", ItemName = "AggregatedDataEntry", KeyName = "UMReportTuple", ValueName = "UMReportTupleData")]
	internal class AggregatedDataDictionary : Dictionary<UMReportTuple, UMReportTupleData>
	{
		public AggregatedDataDictionary()
		{
		}

		public AggregatedDataDictionary(int count) : base(count)
		{
		}
	}
}
