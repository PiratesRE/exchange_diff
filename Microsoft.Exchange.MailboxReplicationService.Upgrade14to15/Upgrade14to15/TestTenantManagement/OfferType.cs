using System;
using System.CodeDom.Compiler;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.MailboxReplicationService.Upgrade14to15.TestTenantManagement
{
	[DataContract(Name = "OfferType", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.BOX.SyntheticSvc.Contracts")]
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	public enum OfferType
	{
		[EnumMember]
		Invalid,
		[EnumMember]
		ExchangePlan1Edu,
		[EnumMember]
		E3,
		[EnumMember]
		E1,
		[EnumMember]
		P1,
		[EnumMember]
		K1,
		[EnumMember]
		EopEnterprise,
		[EnumMember]
		EopEnterprisePreminum
	}
}
