using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.OnlineMeetings.ResourceContract
{
	[DataContract(Name = "onlineMeetingRel")]
	internal enum OnlineMeetingRel
	{
		[EnumMember]
		MyOnlineMeetings,
		[EnumMember]
		MyAssignedOnlineMeeting
	}
}
