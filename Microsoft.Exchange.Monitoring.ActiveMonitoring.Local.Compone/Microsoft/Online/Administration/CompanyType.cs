using System;
using System.CodeDom.Compiler;
using System.Runtime.Serialization;

namespace Microsoft.Online.Administration
{
	[DataContract(Name = "CompanyType", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.Administration")]
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	public enum CompanyType
	{
		[EnumMember]
		CompanyTenant,
		[EnumMember]
		MicrosoftSupportTenant,
		[EnumMember]
		SyndicatePartnerTenant,
		[EnumMember]
		SupportPartnerTenant
	}
}
