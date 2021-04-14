using System;

namespace Microsoft.Exchange.Services.OnlineMeetings.ResourceContract
{
	[Post(typeof(MakeMeAvailableSettings))]
	[Parent("communication")]
	internal class MakeMeAvailableResource : Resource
	{
		public MakeMeAvailableResource(string selfUri) : base(selfUri)
		{
		}

		public const string Token = "MakeMeAvailable";
	}
}
