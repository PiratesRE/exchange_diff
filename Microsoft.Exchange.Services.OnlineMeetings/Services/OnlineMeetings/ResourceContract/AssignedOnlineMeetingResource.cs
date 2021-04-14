using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.OnlineMeetings.ResourceContract
{
	[Parent("onlineMeetingEligibleValues")]
	[Get(typeof(OnlineMeetingResource))]
	[DataContract(Name = "myAssignedOnlineMeetingResource")]
	internal class AssignedOnlineMeetingResource : OnlineMeetingResource
	{
		public AssignedOnlineMeetingResource(string selfUri) : base(selfUri)
		{
		}

		public new const string Token = "myAssignedOnlineMeeting";
	}
}
