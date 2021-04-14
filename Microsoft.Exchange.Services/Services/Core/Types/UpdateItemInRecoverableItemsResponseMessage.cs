using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType("UpdateItemInRecoverableItemsResponseMessageType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class UpdateItemInRecoverableItemsResponseMessage : ItemInfoResponseMessage
	{
		public UpdateItemInRecoverableItemsResponseMessage()
		{
		}

		internal UpdateItemInRecoverableItemsResponseMessage(ServiceResultCode code, ServiceError error, ItemType item, AttachmentType[] attachments, ConflictResults conflictResults) : base(code, error, item)
		{
			this.Attachments = attachments;
			this.ConflictResults = conflictResults;
		}

		[XmlArray("Attachments", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		[DataMember(EmitDefaultValue = false, Name = "Attachments")]
		[XmlArrayItem(ElementName = "FileAttachment", Type = typeof(FileAttachmentType), Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
		[XmlArrayItem(ElementName = "ItemAttachment", Type = typeof(ItemAttachmentType), Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
		[XmlArrayItem(ElementName = "ReferenceAttachment", Type = typeof(ReferenceAttachmentType), Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
		public AttachmentType[] Attachments { get; set; }

		[XmlElement("ConflictResults", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		[DataMember(EmitDefaultValue = false)]
		public ConflictResults ConflictResults { get; set; }
	}
}
