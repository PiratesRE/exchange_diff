using System;
using System.CodeDom.Compiler;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.HostedServices.AdminCenter.UI.Services
{
	[GeneratedCode("System.Runtime.Serialization", "3.0.0.0")]
	[DataContract(Name = "EdgeBlockMode", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Exchange.HostedServices.AdminCenter.UI.Services")]
	internal enum EdgeBlockMode
	{
		[EnumMember]
		Reject = 1,
		[EnumMember]
		Test = 3,
		[EnumMember]
		Disabled = 5
	}
}
