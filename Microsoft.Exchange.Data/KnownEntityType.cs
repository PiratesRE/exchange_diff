using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Data
{
	[DataContract]
	public enum KnownEntityType
	{
		[EnumMember]
		MeetingSuggestion = 1,
		[EnumMember]
		TaskSuggestion,
		[EnumMember]
		Address,
		[EnumMember]
		Url,
		[EnumMember]
		PhoneNumber,
		[EnumMember]
		EmailAddress,
		[EnumMember]
		Contact
	}
}
