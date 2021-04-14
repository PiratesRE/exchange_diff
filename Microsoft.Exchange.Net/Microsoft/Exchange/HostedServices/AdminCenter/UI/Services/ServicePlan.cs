using System;
using System.CodeDom.Compiler;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.HostedServices.AdminCenter.UI.Services
{
	[GeneratedCode("System.Runtime.Serialization", "3.0.0.0")]
	[DataContract(Name = "ServicePlan", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Exchange.HostedServices.AdminCenter.UI.Services")]
	internal enum ServicePlan
	{
		[EnumMember]
		None,
		[EnumMember]
		Office365Enterprises,
		[EnumMember]
		Office365EDU,
		[EnumMember]
		Office365SB
	}
}
