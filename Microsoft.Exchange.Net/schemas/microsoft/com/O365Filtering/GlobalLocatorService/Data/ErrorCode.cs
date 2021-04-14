using System;
using System.CodeDom.Compiler;
using System.Runtime.Serialization;

namespace schemas.microsoft.com.O365Filtering.GlobalLocatorService.Data
{
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	[DataContract(Name = "ErrorCode", Namespace = "http://schemas.microsoft.com/O365Filtering/GlobalLocatorService/Data")]
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
		Server,
		[EnumMember]
		DataNotFound
	}
}
