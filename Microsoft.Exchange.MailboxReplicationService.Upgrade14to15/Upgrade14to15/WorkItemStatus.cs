using System;
using System.CodeDom.Compiler;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.MailboxReplicationService.Upgrade14to15
{
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	[DataContract(Name = "WorkItemStatus", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.BOX.OrchestrationEngine.WcfService.Contract.WorkloadService")]
	public enum WorkItemStatus
	{
		[EnumMember]
		NotStarted,
		[EnumMember]
		InProgress,
		[EnumMember]
		Warning,
		[EnumMember]
		Error,
		[EnumMember]
		Cancelled,
		[EnumMember]
		Complete,
		[EnumMember]
		ForceComplete
	}
}
