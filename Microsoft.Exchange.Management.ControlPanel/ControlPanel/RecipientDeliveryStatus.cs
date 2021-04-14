using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public enum RecipientDeliveryStatus
	{
		[EnumMember]
		Unsuccessful,
		[EnumMember]
		Pending,
		[EnumMember]
		Delivered,
		[EnumMember]
		Transferred,
		[EnumMember]
		Read,
		[EnumMember]
		All = 99
	}
}
