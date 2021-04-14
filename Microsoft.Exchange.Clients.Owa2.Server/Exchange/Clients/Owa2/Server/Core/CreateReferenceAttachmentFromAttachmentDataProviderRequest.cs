using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class CreateReferenceAttachmentFromAttachmentDataProviderRequest
	{
		[DataMember(Name = "itemId", IsRequired = true)]
		public ItemId ItemId { get; set; }

		[DataMember(Name = "attachmentDataProviderId", IsRequired = true)]
		public string AttachmentDataProviderId { get; set; }

		[DataMember(Name = "location", IsRequired = false)]
		public string Location { get; set; }

		[DataMember(Name = "attachmentId", IsRequired = true)]
		public string AttachmentId { get; set; }

		[DataMember(Name = "subscriptionId", IsRequired = false)]
		public string SubscriptionId { get; set; }

		[DataMember(Name = "dataProviderParentItemId", IsRequired = false)]
		public string DataProviderParentItemId { get; set; }

		[DataMember(Name = "providerEndpointUrl", IsRequired = false)]
		public string ProviderEndpointUrl { get; set; }

		[DataMember(Name = "cancellationId", IsRequired = false)]
		public string CancellationId { get; set; }
	}
}
