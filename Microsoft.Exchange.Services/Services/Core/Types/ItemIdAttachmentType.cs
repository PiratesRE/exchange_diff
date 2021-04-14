using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange", Name = "ItemIdAttachment")]
	[Serializable]
	public class ItemIdAttachmentType : AttachmentType
	{
		[DataMember(Name = "ItemId")]
		public ItemId ItemId { get; set; }

		[DataMember(Name = "AttachmentIdToAttach")]
		public string AttachmentIdToAttach { get; set; }
	}
}
