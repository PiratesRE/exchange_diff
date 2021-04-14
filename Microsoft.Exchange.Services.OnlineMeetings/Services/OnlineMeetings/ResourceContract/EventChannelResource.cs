using System;

namespace Microsoft.Exchange.Services.OnlineMeetings.ResourceContract
{
	[Parent("application")]
	[Get(typeof(EventChannelResource))]
	internal class EventChannelResource : Resource
	{
		public EventChannelResource(string selfUri) : base(selfUri)
		{
		}

		public const string Token = "event";

		public const bool IsXmlSupported = false;

		public const bool IsJsonSupported = false;

		public const string ContentType = "multipart/related+json";
	}
}
