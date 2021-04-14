using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.OnlineMeetings.ResourceContract
{
	[DataContract(Name = "conferencingRole")]
	internal enum ConferencingRole
	{
		[EnumMember]
		None,
		[EnumMember]
		Attendee,
		[EnumMember]
		Leader
	}
}
