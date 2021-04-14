using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Servicelets.GlobalLocatorCache
{
	[DataContract(Namespace = "http://schemas.microsoft.com/O365Filtering/GlobalLocatorService/Data")]
	public enum ErrorCode
	{
		[EnumMember]
		UnknownError,
		[EnumMember]
		Authentication,
		[EnumMember]
		Authorization,
		[EnumMember]
		Arguments,
		[EnumMember]
		Server
	}
}
