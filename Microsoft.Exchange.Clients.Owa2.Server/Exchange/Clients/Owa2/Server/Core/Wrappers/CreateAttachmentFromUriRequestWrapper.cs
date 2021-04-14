using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core.Wrappers
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class CreateAttachmentFromUriRequestWrapper
	{
		[DataMember(Name = "itemId")]
		public ItemId ItemId { get; set; }

		[DataMember(Name = "uri")]
		public string Uri { get; set; }

		[DataMember(Name = "name")]
		public string Name { get; set; }

		[DataMember(Name = "subscriptionId")]
		public string SubscriptionId { get; set; }
	}
}
