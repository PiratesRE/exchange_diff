using System;
using System.CodeDom.Compiler;
using System.Runtime.Serialization;

namespace Microsoft.Online.Administration
{
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	[DataContract(Name = "DomainCapabilities", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.Administration")]
	[Flags]
	public enum DomainCapabilities
	{
		[EnumMember]
		None = 0,
		[EnumMember]
		Email = 1,
		[EnumMember]
		Sharepoint = 2,
		[EnumMember]
		OfficeCommunicationsOnline = 4,
		[EnumMember]
		SharepointDefault = 8,
		[EnumMember]
		FullRedelegation = 16,
		[EnumMember]
		All = 31
	}
}
