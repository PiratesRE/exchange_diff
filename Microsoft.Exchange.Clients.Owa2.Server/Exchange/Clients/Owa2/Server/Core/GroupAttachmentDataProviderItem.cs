using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Services.Wcf;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class GroupAttachmentDataProviderItem : AttachmentDataProviderItem
	{
		public GroupAttachmentDataProviderItem()
		{
			base.Type = AttachmentDataProviderItemType.Group;
		}

		[DataMember]
		public AttachmentItemsPagingMetadata PagingMetadata { get; set; }

		[DataMember]
		public string GroupCategory { get; set; }

		[DataMember]
		[DateTimeString]
		public string JoinDate { get; set; }
	}
}
