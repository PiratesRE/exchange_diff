using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.OnlineMeetings.ResourceContract
{
	[DataContract(Name = "phoneUserAdmission")]
	internal enum PhoneUserAdmission
	{
		[EnumMember]
		Disabled,
		[EnumMember]
		Enabled
	}
}
