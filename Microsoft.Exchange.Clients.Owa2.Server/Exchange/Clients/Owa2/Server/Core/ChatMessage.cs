using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class ChatMessage
	{
		[DataMember(IsRequired = true, EmitDefaultValue = false, Order = 1)]
		public string Body { get; set; }

		[DataMember(IsRequired = false, EmitDefaultValue = false, Order = 2)]
		public string Format { get; set; }

		[DataMember(IsRequired = false, EmitDefaultValue = false, Order = 3)]
		public string Subject { get; set; }

		[DataMember(IsRequired = false, EmitDefaultValue = false, Order = 4)]
		public int ChatSessionId { get; set; }

		[DataMember(IsRequired = true, EmitDefaultValue = false, Order = 5)]
		public string[] Recipients { get; set; }
	}
}
