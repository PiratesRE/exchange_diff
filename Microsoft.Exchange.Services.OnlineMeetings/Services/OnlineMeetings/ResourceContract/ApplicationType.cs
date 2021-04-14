using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.OnlineMeetings.ResourceContract
{
	[DataContract(Name = "applicationType")]
	internal enum ApplicationType
	{
		[EnumMember]
		Browser,
		[EnumMember]
		Phone,
		[EnumMember]
		Tablet
	}
}
