using System;

namespace Microsoft.Exchange.Services.OnlineMeetings.ResourceContract
{
	internal class EventEntity
	{
		public Link Link { get; set; }

		public Link In { get; set; }

		public EventOperation Relationship { get; set; }

		public EventStatus Status { get; set; }

		public Resource EmbeddedResource { get; set; }

		public ErrorInformation Error { get; set; }
	}
}
