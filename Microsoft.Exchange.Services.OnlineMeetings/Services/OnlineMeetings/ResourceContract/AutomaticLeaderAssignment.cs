using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.OnlineMeetings.ResourceContract
{
	[DataContract(Name = "automaticLeaderAssignment")]
	internal enum AutomaticLeaderAssignment : long
	{
		[EnumMember]
		Disabled,
		[EnumMember]
		SameEnterprise = 32768L,
		[EnumMember]
		Everyone = 2147483648L
	}
}
