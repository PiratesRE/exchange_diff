using System;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.OnlineMeetings.ResourceContract
{
	[DataContract(Name = "MultipartRelatedEventsResponse")]
	[Namespace("MultipartRelatedEventsResponse")]
	internal class MultipartRelatedEventsResponse : ResponseContainer
	{
		[DataMember(Name = "EventsResponse", EmitDefaultValue = false)]
		public EventsResponse EventsResponse { get; set; }

		[DataMember(Name = "Part", EmitDefaultValue = false)]
		public Collection<object> Parts { get; set; }
	}
}
