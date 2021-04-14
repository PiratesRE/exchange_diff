using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Directory.ResourceHealth;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType("UpdateItemInRecoverableItemsType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	public class UpdateItemInRecoverableItemsRequest : BaseRequest
	{
		[DataMember(Name = "ItemId", IsRequired = true)]
		[XmlElement("ItemId", typeof(ItemId))]
		public ItemId ItemId { get; set; }

		[XmlArrayItem("SetItemField", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", Type = typeof(SetItemPropertyUpdate))]
		[XmlArrayItem("DeleteItemField", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", Type = typeof(DeleteItemPropertyUpdate))]
		[XmlArrayItem("AppendToItemField", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", Type = typeof(AppendItemPropertyUpdate))]
		[DataMember(Name = "Updates", IsRequired = true)]
		[XmlArray("Updates")]
		public PropertyUpdate[] PropertyUpdates { get; set; }

		[XmlArrayItem("ItemAttachment", typeof(ItemAttachmentType), Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		[XmlArrayItem("ReferenceAttachment", typeof(ReferenceAttachmentType), Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		[XmlArray("Attachments")]
		[XmlArrayItem("FileAttachment", typeof(FileAttachmentType), Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		[DataMember(EmitDefaultValue = false)]
		public AttachmentType[] Attachments { get; set; }

		[DataMember(Name = "MakeItemImmutable", IsRequired = false)]
		[XmlElement("MakeItemImmutable", typeof(bool))]
		public bool MakeItemImmutable { get; set; }

		internal override ServiceCommandBase GetServiceCommand(CallContext callContext)
		{
			return new UpdateItemInRecoverableItems(callContext, this);
		}

		internal override BaseServerIdInfo GetProxyInfo(CallContext callContext)
		{
			return BaseRequest.GetServerInfoForItemId(callContext, this.ItemId);
		}

		internal override ResourceKey[] GetResources(CallContext callContext, int taskStep)
		{
			if (this.resourceKeys == null)
			{
				BaseServerIdInfo serverInfoForFolderId = BaseRequest.GetServerInfoForFolderId(callContext, new DistinguishedFolderId
				{
					Id = DistinguishedFolderIdName.recoverableitemspurges
				});
				BaseServerIdInfo serverInfoForItemId = BaseRequest.GetServerInfoForItemId(callContext, this.ItemId);
				this.resourceKeys = BaseRequest.ServerInfosToResourceKeys(true, new BaseServerIdInfo[]
				{
					serverInfoForFolderId,
					serverInfoForItemId
				});
			}
			return this.resourceKeys;
		}

		private ResourceKey[] resourceKeys;
	}
}
