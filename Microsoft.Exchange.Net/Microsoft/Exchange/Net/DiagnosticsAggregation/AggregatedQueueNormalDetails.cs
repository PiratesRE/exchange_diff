using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Net.DiagnosticsAggregation
{
	[DataContract]
	[Serializable]
	public class AggregatedQueueNormalDetails
	{
		[DataMember(IsRequired = true)]
		public string QueueIdentity { get; set; }

		[DataMember(IsRequired = true)]
		public string ServerIdentity { get; set; }

		[DataMember(IsRequired = true)]
		public int MessageCount { get; set; }
	}
}
