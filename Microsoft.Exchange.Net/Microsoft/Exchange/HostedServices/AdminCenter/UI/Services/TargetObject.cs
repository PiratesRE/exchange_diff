using System;
using System.CodeDom.Compiler;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.HostedServices.AdminCenter.UI.Services
{
	[DataContract(Name = "TargetObject", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Exchange.HostedServices.AdminCenter.UI.Services")]
	[GeneratedCode("System.Runtime.Serialization", "3.0.0.0")]
	internal enum TargetObject
	{
		[EnumMember]
		None,
		[EnumMember]
		Company,
		[EnumMember]
		Domain,
		[EnumMember]
		SmtpProfile,
		[EnumMember]
		OutboundIP,
		[EnumMember]
		OnPremiseGatewayIP,
		[EnumMember]
		InternalServerIP,
		[EnumMember]
		RecipientLevelRoutingConfig,
		[EnumMember]
		SkipListConfig,
		[EnumMember]
		DirectoryEdgeBlockConfig,
		[EnumMember]
		CompanyConfigurationSettings,
		[EnumMember]
		DomainConfigurationSettings,
		[EnumMember]
		Subscription
	}
}
