using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.OnlineMeetings.Autodiscover.DataContract
{
	internal enum AccessLocation
	{
		[EnumMember]
		Internal,
		[EnumMember]
		External
	}
}
