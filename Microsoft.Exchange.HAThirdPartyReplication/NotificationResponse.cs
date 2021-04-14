using System;
using System.CodeDom.Compiler;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.ThirdPartyReplication
{
	[DataContract(Name = "NotificationResponse", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Exchange.ThirdPartyReplication")]
	[GeneratedCode("System.Runtime.Serialization", "3.0.0.0")]
	public enum NotificationResponse
	{
		[EnumMember]
		Complete,
		[EnumMember]
		Incomplete
	}
}
