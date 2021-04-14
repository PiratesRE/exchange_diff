using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Net.DiagnosticsAggregation
{
	[DataContract]
	public class AggregatedQueueInfo
	{
		public AggregatedQueueInfo()
		{
			this.NormalDetails = new List<AggregatedQueueNormalDetails>();
			this.VerboseDetails = new List<AggregatedQueueVerboseDetails>();
		}

		[DataMember(IsRequired = true)]
		public string GroupByValue { get; set; }

		[DataMember(IsRequired = true)]
		public int MessageCount { get; set; }

		[DataMember(IsRequired = true)]
		public int DeferredMessageCount { get; set; }

		[DataMember(IsRequired = true)]
		public int LockedMessageCount { get; set; }

		[DataMember(IsRequired = true)]
		public int StaleMessageCount { get; set; }

		[DataMember(IsRequired = true)]
		public double IncomingRate { get; set; }

		[DataMember(IsRequired = true)]
		public double OutgoingRate { get; set; }

		[DataMember(IsRequired = true)]
		public List<AggregatedQueueNormalDetails> NormalDetails { get; set; }

		[DataMember(IsRequired = true)]
		public List<AggregatedQueueVerboseDetails> VerboseDetails { get; set; }

		public AggregatedQueueInfo Clone()
		{
			return new AggregatedQueueInfo
			{
				GroupByValue = this.GroupByValue,
				MessageCount = this.MessageCount,
				DeferredMessageCount = this.DeferredMessageCount,
				LockedMessageCount = this.LockedMessageCount,
				StaleMessageCount = this.StaleMessageCount,
				IncomingRate = this.IncomingRate,
				OutgoingRate = this.OutgoingRate,
				NormalDetails = new List<AggregatedQueueNormalDetails>(this.NormalDetails),
				VerboseDetails = new List<AggregatedQueueVerboseDetails>(this.VerboseDetails)
			};
		}
	}
}
