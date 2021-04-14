using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.OnlineMeetings.ResourceContract
{
	[DataContract(Name = "entryExitAnnouncement")]
	internal enum EntryExitAnnouncement
	{
		[EnumMember]
		Unsupported,
		[EnumMember]
		Disabled,
		[EnumMember]
		Enabled
	}
}
