using System;
using System.CodeDom.Compiler;
using System.Runtime.Serialization;

namespace Microsoft.Online.Administration
{
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	[DataContract(Name = "GroupType", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.Administration")]
	public enum GroupType
	{
		[EnumMember]
		DistributionList,
		[EnumMember]
		Security,
		[EnumMember]
		MailEnabledSecurity
	}
}
