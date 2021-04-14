using System;
using System.CodeDom.Compiler;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.HostedServices.AdminCenter.UI.Services
{
	[GeneratedCode("System.Runtime.Serialization", "3.0.0.0")]
	[DataContract(Name = "DomainMailFlowType", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Exchange.HostedServices.AdminCenter.UI.Services")]
	[Flags]
	internal enum DomainMailFlowType
	{
		[EnumMember]
		Inbound = 1,
		[EnumMember]
		Outbound = 2,
		[EnumMember]
		InboundOutbound = 5
	}
}
