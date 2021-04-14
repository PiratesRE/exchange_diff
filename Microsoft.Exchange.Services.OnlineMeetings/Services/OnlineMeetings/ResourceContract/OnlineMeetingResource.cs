using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.OnlineMeetings.ResourceContract
{
	[Get(typeof(OnlineMeetingResource))]
	[DataContract(Name = "onlineMeetingResource")]
	[Put(typeof(OnlineMeetingResource), typeof(OnlineMeetingResource))]
	[Parent("onlineMeetingEligibleValues")]
	[Delete]
	internal class OnlineMeetingResource : MeetingResource, IEtagProvider
	{
		public OnlineMeetingResource(string selfUri) : base(selfUri)
		{
		}

		public string ETag { get; set; }

		public const string Token = "myOnlineMeeting";
	}
}
