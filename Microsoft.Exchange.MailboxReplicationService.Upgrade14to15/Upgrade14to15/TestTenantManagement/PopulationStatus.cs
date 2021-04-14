using System;
using System.CodeDom.Compiler;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.MailboxReplicationService.Upgrade14to15.TestTenantManagement
{
	[DataContract(Name = "PopulationStatus", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.BOX.SyntheticSvc.Contracts")]
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	public enum PopulationStatus
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
		Complete
	}
}
