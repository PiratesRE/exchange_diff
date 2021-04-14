using System;
using System.CodeDom.Compiler;
using System.Runtime.Serialization;

namespace Microsoft.BDM.Pets.SharedLibrary.Enums
{
	[DataContract(Name = "ResourceRecordType", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.BDM.Pets.SharedLibrary.Enums")]
	[GeneratedCode("System.Runtime.Serialization", "3.0.0.0")]
	public enum ResourceRecordType
	{
		[EnumMember]
		DNS_TYPE_ZERO,
		[EnumMember]
		DNS_TYPE_A,
		[EnumMember]
		DNS_TYPE_NS,
		[EnumMember]
		DNS_TYPE_CNAME = 5,
		[EnumMember]
		DNS_TYPE_SOA,
		[EnumMember]
		DNS_TYPE_MX = 15,
		[EnumMember]
		DNS_TYPE_TEXT,
		[EnumMember]
		DNS_TYPE_SRV = 33
	}
}
