using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.OnlineMeetings.ResourceContract
{
	[Namespace("Events")]
	[DataContract(Name = "EventsResponse")]
	internal class EventsResponse : ResponseContainer
	{
		[DataMember(Name = "Events", EmitDefaultValue = false)]
		public EventsEntity EventsResource { get; set; }

		public const string ContentType = "multipart/related";
	}
}
