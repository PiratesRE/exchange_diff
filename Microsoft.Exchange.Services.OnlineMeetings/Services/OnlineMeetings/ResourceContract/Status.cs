using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.OnlineMeetings.ResourceContract
{
	[DataContract(Name = "Status")]
	internal enum Status
	{
		[EnumMember]
		Pending,
		[EnumMember]
		Failed,
		[EnumMember]
		Succeeded
	}
}
