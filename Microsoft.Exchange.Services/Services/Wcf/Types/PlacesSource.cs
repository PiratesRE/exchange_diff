using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.Wcf.Types
{
	[Flags]
	[DataContract]
	public enum PlacesSource
	{
		None = 0,
		[EnumMember]
		History = 1,
		[EnumMember]
		LocationServices = 2,
		[EnumMember]
		PhonebookServices = 4
	}
}
