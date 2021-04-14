using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class CreateReferenceAttachmentFromLocalFileRequest
	{
		[DataMember]
		public ItemId ParentItemId { get; set; }

		[DataMember]
		public string FileName { get; set; }

		[DataMember]
		public string FileContentToUpload { get; set; }

		[DataMember]
		public string SubscriptionId { get; set; }

		[DataMember]
		public string ChannelId { get; set; }

		[DataMember]
		public string CancellationId { get; set; }
	}
}
