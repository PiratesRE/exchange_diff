using System;

namespace Microsoft.Exchange.Services.OnlineMeetings.ResourceContract
{
	[Link("scheduled/summaries")]
	[Parent("Application")]
	[Link("scheduled/conferences")]
	[Link("scheduled/schedulingoptions")]
	internal class MyMeetingsResource : Resource
	{
		public MyMeetingsResource(string selfUri) : base(selfUri)
		{
		}

		public const string Token = "onlineMeetings";
	}
}
