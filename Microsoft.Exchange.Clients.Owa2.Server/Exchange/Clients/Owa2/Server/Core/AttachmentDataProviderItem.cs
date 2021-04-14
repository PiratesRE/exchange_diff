using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Services.Wcf;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[KnownType(typeof(FileAttachmentDataProviderItem))]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[KnownType(typeof(GroupAttachmentDataProviderItem))]
	[KnownType(typeof(FolderAttachmentDataProviderItem))]
	public class AttachmentDataProviderItem
	{
		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public virtual long Size { get; set; }

		[DataMember]
		public string Id { get; set; }

		[DataMember]
		public string ParentId { get; set; }

		[DataMember]
		public string Location { get; set; }

		[DataMember]
		public AttachmentDataProviderType ProviderType { get; set; }

		[DataMember]
		public string ProviderEndpointUrl { get; set; }

		[DateTimeString]
		[DataMember]
		public string Modified { get; set; }

		[DataMember]
		public string ModifiedBy { get; set; }

		[DataMember]
		public virtual int ChildCount { get; set; }

		[DataMember]
		public AttachmentDataProviderItemType Type { get; set; }

		[DataMember]
		public string AttachmentProviderId { get; set; }
	}
}
