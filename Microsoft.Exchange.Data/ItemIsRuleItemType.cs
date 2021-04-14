using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Data
{
	[DataContract]
	public enum ItemIsRuleItemType
	{
		[EnumMember]
		Message = 1,
		[EnumMember]
		Appointment
	}
}
