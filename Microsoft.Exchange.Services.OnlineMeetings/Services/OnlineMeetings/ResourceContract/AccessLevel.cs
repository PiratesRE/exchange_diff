using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.OnlineMeetings.ResourceContract
{
	[DataContract(Name = "accessLevel")]
	internal enum AccessLevel
	{
		[EnumMember]
		SameEnterprise = 1,
		[EnumMember]
		None = 0,
		[EnumMember]
		Locked = 2,
		[EnumMember]
		Invited,
		[EnumMember]
		Everyone
	}
}
