using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public sealed class GetFlowConversationResponse
	{
		[DataMember]
		public FlowConversationItem[] Conversations { get; set; }
	}
}
