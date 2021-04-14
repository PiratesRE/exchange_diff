using System;
using System.CodeDom.Compiler;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.MailboxReplicationService.Upgrade14to15.TestTenantManagement
{
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	[DataContract(Name = "ValidationStatus", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.BOX.SyntheticSvc.Contracts")]
	public enum ValidationStatus
	{
		[EnumMember]
		Invalid,
		[EnumMember]
		NotStarted,
		[EnumMember]
		InProgress,
		[EnumMember]
		Error,
		[EnumMember]
		Success,
		[EnumMember]
		Failure
	}
}
