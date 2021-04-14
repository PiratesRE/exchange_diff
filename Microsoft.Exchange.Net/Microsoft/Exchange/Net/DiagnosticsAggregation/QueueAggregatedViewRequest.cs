using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Net.DiagnosticsAggregation
{
	[DataContract]
	internal class QueueAggregatedViewRequest
	{
		public QueueAggregatedViewRequest(QueueDigestGroupBy groupByKey, DetailsLevel detailsLevel, string queueFilter)
		{
			this.GroupByKey = groupByKey.ToString();
			this.DetailsLevel = detailsLevel.ToString();
			this.QueueFilter = queueFilter;
		}

		[DataMember(IsRequired = true)]
		public string QueueFilter { get; private set; }

		[DataMember(IsRequired = true)]
		public string GroupByKey { get; private set; }

		[DataMember(IsRequired = true)]
		public string DetailsLevel { get; private set; }
	}
}
