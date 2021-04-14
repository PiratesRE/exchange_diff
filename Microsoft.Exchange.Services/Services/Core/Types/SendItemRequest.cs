using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Directory.ResourceHealth;

namespace Microsoft.Exchange.Services.Core.Types
{
	[KnownType(typeof(ItemId))]
	[KnownType(typeof(OccurrenceItemId))]
	[KnownType(typeof(RecurringMasterItemId))]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType("SendItemType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	public class SendItemRequest : BaseRequest
	{
		[DataMember(Name = "ItemIds", IsRequired = true, Order = 1)]
		[XmlArrayItem("ItemId", typeof(ItemId), Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		[XmlArrayItem("OccurrenceItemId", typeof(OccurrenceItemId), Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		[XmlArrayItem("RecurringMasterItemId", typeof(RecurringMasterItemId), Namespace = "http://schemas.microsoft.com/exchange/services/2006/types", IsNullable = false)]
		[XmlArray("ItemIds", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public BaseItemId[] Ids { get; set; }

		[XmlElement("SavedItemFolderId")]
		[DataMember(IsRequired = false, Order = 2)]
		public TargetFolderId SavedItemFolderId { get; set; }

		[DataMember(IsRequired = false, Order = 3)]
		[XmlAttribute(AttributeName = "SaveItemToFolder")]
		public bool SaveItemToFolder { get; set; }

		internal override ServiceCommandBase GetServiceCommand(CallContext callContext)
		{
			return new SendItem(callContext, this);
		}

		internal override BaseServerIdInfo GetProxyInfo(CallContext callContext)
		{
			if (this.Ids == null)
			{
				return null;
			}
			return BaseRequest.GetServerInfoForItemIdList(callContext, this.Ids);
		}

		internal override ResourceKey[] GetResources(CallContext callContext, int taskStep)
		{
			if (this.targetFolderServerInfo == null && this.SavedItemFolderId != null)
			{
				this.targetFolderServerInfo = BaseRequest.GetServerInfoForFolderId(callContext, this.SavedItemFolderId.BaseFolderId);
			}
			BaseServerIdInfo serverInfoForItemId = BaseRequest.GetServerInfoForItemId(callContext, this.Ids[taskStep]);
			if (this.targetFolderServerInfo != null)
			{
				return BaseRequest.ServerInfosToResourceKeys(true, new BaseServerIdInfo[]
				{
					this.targetFolderServerInfo,
					serverInfoForItemId
				});
			}
			return BaseRequest.ServerInfoToResourceKeys(true, serverInfoForItemId);
		}

		protected override List<ServiceObjectId> GetAllIds()
		{
			if (this.Ids != null)
			{
				return new List<ServiceObjectId>(this.Ids);
			}
			return null;
		}

		internal const string ItemIdsElementName = "ItemIds";

		internal const string SavedItemFolderIdElementName = "SavedItemFolderId";

		private BaseServerIdInfo targetFolderServerInfo;
	}
}
