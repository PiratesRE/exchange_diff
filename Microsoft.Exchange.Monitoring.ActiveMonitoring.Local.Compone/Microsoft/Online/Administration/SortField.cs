using System;
using System.CodeDom.Compiler;
using System.Runtime.Serialization;

namespace Microsoft.Online.Administration
{
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	[DataContract(Name = "SortField", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.Administration")]
	public enum SortField
	{
		[EnumMember]
		DisplayName,
		[EnumMember]
		UserPrincipalName,
		[EnumMember]
		None
	}
}
