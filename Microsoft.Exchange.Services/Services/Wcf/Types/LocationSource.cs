using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[DataContract]
	public enum LocationSource
	{
		[EnumMember]
		None,
		[EnumMember]
		LocationServices,
		[EnumMember]
		PhonebookServices,
		[EnumMember]
		Device,
		[EnumMember]
		Contact,
		[EnumMember]
		Resource
	}
}
