using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.OnlineMeetings.ResourceContract
{
	[DataContract]
	internal abstract class HyperReference
	{
		[IgnoreDataMember]
		public string Relationship { get; set; }

		[IgnoreDataMember]
		public string Href { get; set; }
	}
}
