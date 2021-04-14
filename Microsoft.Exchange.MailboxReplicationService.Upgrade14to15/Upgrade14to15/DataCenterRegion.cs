using System;
using System.CodeDom.Compiler;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.MailboxReplicationService.Upgrade14to15
{
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	[DataContract(Name = "DataCenterRegion", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.BOX.OrchestrationEngine.Common.DataContract")]
	public enum DataCenterRegion
	{
		[EnumMember]
		NONE,
		[EnumMember]
		EU,
		[EnumMember]
		LATAM,
		[EnumMember]
		NA,
		[EnumMember]
		SEA
	}
}
