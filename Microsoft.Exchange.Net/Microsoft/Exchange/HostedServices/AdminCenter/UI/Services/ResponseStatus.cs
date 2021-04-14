using System;
using System.CodeDom.Compiler;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.HostedServices.AdminCenter.UI.Services
{
	[DataContract(Name = "ResponseStatus", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Exchange.HostedServices.AdminCenter.UI.Services")]
	[GeneratedCode("System.Runtime.Serialization", "3.0.0.0")]
	internal enum ResponseStatus
	{
		[EnumMember]
		Success = 1,
		[EnumMember]
		TransientFailure,
		[EnumMember]
		PermanentFailure,
		[EnumMember]
		ContractError
	}
}
