using System;
using System.CodeDom.Compiler;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.HostedServices.AdminCenter.UI.Services.AdminSync
{
	[DataContract(Name = "InvalidContractCode", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Exchange.HostedServices.AdminCenter.UI.Services.AdminSync")]
	[GeneratedCode("System.Runtime.Serialization", "3.0.0.0")]
	internal enum InvalidContractCode
	{
		[EnumMember]
		BatchSizeExceededLimit,
		[EnumMember]
		NullOrEmptyParameterSpecified,
		[EnumMember]
		InvalidParameterSpecified
	}
}
