using System;
using System.CodeDom.Compiler;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.HostedServices.AdminCenter.UI.Services
{
	[DataContract(Name = "InheritanceSettings", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Exchange.HostedServices.AdminCenter.UI.Services")]
	[GeneratedCode("System.Runtime.Serialization", "3.0.0.0")]
	[Flags]
	internal enum InheritanceSettings
	{
		[EnumMember]
		InheritInboundIPConfig = 1,
		[EnumMember]
		InheritOutboundIPConfig = 2,
		[EnumMember]
		InheritOnPremiseGatewayIPConfig = 4,
		[EnumMember]
		InheritInternalServerIPConfig = 8,
		[EnumMember]
		InheritRecipientLevelRoutingConfig = 16,
		[EnumMember]
		InheritSkipListConfig = 32,
		[EnumMember]
		InheritDirectoryEdgeBlockModeConfig = 64,
		[EnumMember]
		InheritSubscriptions = 128
	}
}
