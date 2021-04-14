using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.OnlineMeetings.ResourceContract
{
	[DataContract(Name = "GenericPolicy")]
	internal enum GenericPolicy
	{
		[EnumMember]
		None,
		[EnumMember]
		Disabled,
		[EnumMember]
		Enabled
	}
}
