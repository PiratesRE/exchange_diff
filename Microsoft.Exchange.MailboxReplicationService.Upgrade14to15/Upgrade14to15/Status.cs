using System;
using System.CodeDom.Compiler;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.MailboxReplicationService.Upgrade14to15
{
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	[DataContract(Name = "Status", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.BOX.OrchestrationEngine.Common.Database.ObjectModel")]
	public enum Status
	{
		[EnumMember]
		Cancelled = 1,
		[EnumMember]
		Complete,
		[EnumMember]
		ForceComplete,
		[EnumMember]
		NotReady,
		[EnumMember]
		NotStarted,
		[EnumMember]
		InProgress,
		[EnumMember]
		Warning,
		[EnumMember]
		Error
	}
}
