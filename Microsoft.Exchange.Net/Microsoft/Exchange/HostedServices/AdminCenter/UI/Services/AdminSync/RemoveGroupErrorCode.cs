using System;
using System.CodeDom.Compiler;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.HostedServices.AdminCenter.UI.Services.AdminSync
{
	[DataContract(Name = "RemoveGroupErrorCode", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Exchange.HostedServices.AdminCenter.UI.Services.AdminSync")]
	[GeneratedCode("System.Runtime.Serialization", "3.0.0.0")]
	internal enum RemoveGroupErrorCode
	{
		[EnumMember]
		InvalidFormat,
		[EnumMember]
		InternalServerError,
		[EnumMember]
		GroupDoesNotExist,
		[EnumMember]
		AccessDenied
	}
}
