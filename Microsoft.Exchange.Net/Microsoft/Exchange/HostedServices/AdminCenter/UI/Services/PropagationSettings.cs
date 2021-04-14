using System;
using System.CodeDom.Compiler;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.HostedServices.AdminCenter.UI.Services
{
	[GeneratedCode("System.Runtime.Serialization", "3.0.0.0")]
	[DataContract(Name = "PropagationSettings", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Exchange.HostedServices.AdminCenter.UI.Services")]
	[Flags]
	internal enum PropagationSettings
	{
		[EnumMember]
		PropagateOutboundIPConfig = 1,
		[EnumMember]
		PropagateRecipientLevelRoutingConfig = 2,
		[EnumMember]
		PropagateSkipListConfig = 4,
		[EnumMember]
		PropagateDirectoryEdgeBlockModeConfig = 8
	}
}
