using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.OnlineMeetings.ResourceContract
{
	[Parent("onlineMeeting")]
	[Post(typeof(OnlineMeetingExtensionResource), typeof(OnlineMeetingExtensionResource))]
	[DataContract(Name = "OnlineMeetingExtensionsResource")]
	[Get(typeof(OnlineMeetingExtensionsResource))]
	internal class OnlineMeetingExtensionsResource : CollectionContainerResource<OnlineMeetingExtensionResource>
	{
		public OnlineMeetingExtensionsResource(string selfUri) : base("onlineMeetingExtension", selfUri)
		{
		}

		public const string Token = "onlineMeetingExtensions";
	}
}
