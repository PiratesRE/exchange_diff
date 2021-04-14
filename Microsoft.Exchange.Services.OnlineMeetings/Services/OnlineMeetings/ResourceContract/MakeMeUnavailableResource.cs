using System;

namespace Microsoft.Exchange.Services.OnlineMeetings.ResourceContract
{
	[Parent("communication")]
	[Post]
	internal class MakeMeUnavailableResource : Resource
	{
		public MakeMeUnavailableResource(string selfUri) : base(selfUri)
		{
		}

		public const string Token = "MakeMeUnavailable";
	}
}
