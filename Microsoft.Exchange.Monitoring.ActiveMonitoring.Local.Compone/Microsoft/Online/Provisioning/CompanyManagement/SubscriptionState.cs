using System;
using System.CodeDom.Compiler;
using System.Runtime.Serialization;

namespace Microsoft.Online.Provisioning.CompanyManagement
{
	[DataContract(Name = "SubscriptionState", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.Provisioning.CompanyManagement")]
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	public enum SubscriptionState
	{
		[EnumMember]
		Active,
		[EnumMember]
		Warning,
		[EnumMember]
		Suspend,
		[EnumMember]
		Delete,
		[EnumMember]
		LockOut
	}
}
