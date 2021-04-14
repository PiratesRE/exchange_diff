using System;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.OnlineMeetings.ResourceContract
{
	[DataContract(Name = "EventsEntity")]
	internal class EventsEntity : HyperReference
	{
		[DataMember(Name = "event", EmitDefaultValue = false)]
		public Collection<EventSenderEntity> Senders { get; set; }

		public Link Link { get; set; }

		public const string Token = "events";

		public const string ContentType = "multipart/related";
	}
}
