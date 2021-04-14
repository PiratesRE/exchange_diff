using System;
using System.CodeDom.Compiler;
using System.Runtime.Serialization;

namespace schemas.microsoft.com.O365Filtering.GlobalLocatorService.Data
{
	[Flags]
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	[DataContract(Name = "DomainKeyType", Namespace = "http://schemas.microsoft.com/O365Filtering/GlobalLocatorService/Data")]
	public enum DomainKeyType
	{
		[EnumMember]
		NotDefined = 0,
		[EnumMember]
		InitialDomain = 1,
		[EnumMember]
		CustomDomain = 2,
		[EnumMember]
		UseExisting = 3
	}
}
