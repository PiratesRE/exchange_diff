using System;
using System.CodeDom.Compiler;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.HostedServices.AdminCenter.UI.Services
{
	[DataContract(Name = "MailServerType", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Exchange.HostedServices.AdminCenter.UI.Services")]
	[GeneratedCode("System.Runtime.Serialization", "3.0.0.0")]
	internal enum MailServerType
	{
		[EnumMember]
		None,
		[EnumMember]
		Exchange2007,
		[EnumMember]
		Exchange2003SP2,
		[EnumMember]
		Exchange2003SP1,
		[EnumMember]
		Exchange2000SP3,
		[EnumMember]
		Exchange2000SP2,
		[EnumMember]
		Exchange2000SP1,
		[EnumMember]
		Exchange55,
		[EnumMember]
		Other
	}
}
