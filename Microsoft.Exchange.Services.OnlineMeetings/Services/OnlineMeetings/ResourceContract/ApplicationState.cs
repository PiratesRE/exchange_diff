using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.OnlineMeetings.ResourceContract
{
	[DataContract(Name = "applicationState")]
	internal enum ApplicationState
	{
		[EnumMember]
		Idle,
		[EnumMember]
		Establishing,
		[EnumMember]
		Established,
		[EnumMember]
		Terminating,
		[EnumMember]
		Terminated
	}
}
