using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[DataContract]
	[Serializable]
	public class FlowItem
	{
		[DataMember]
		public string ItemBody { get; set; }

		[DataMember]
		public ItemId ItemId { get; set; }

		[DataMember]
		public string ReceivedTimeUtc { get; set; }

		[DataMember]
		public EmailAddressWrapper Sender { get; set; }

		[DataMember]
		public bool IsRead { get; set; }
	}
}
