using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[Serializable]
	public class ArrayOfResponseMessagesType
	{
		[XmlElement("CopyFolderResponseMessage", typeof(FolderInfoResponseMessageType))]
		[XmlElement("SendNotificationResponseMessage", typeof(SendNotificationResponseMessageType))]
		[XmlElement("SetClientExtensionResponseMessage", typeof(ResponseMessageType))]
		[XmlElement("SetEncryptionConfigurationResponseMessage", typeof(ResponseMessageType))]
		[XmlElement("SetHoldOnMailboxesResponseMessage", typeof(SetHoldOnMailboxesResponseMessageType))]
		[XmlElement("SubscribeResponseMessage", typeof(SubscribeResponseMessageType))]
		[XmlElement("SyncFolderHierarchyResponseMessage", typeof(SyncFolderHierarchyResponseMessageType))]
		[XmlElement("SyncFolderItemsResponseMessage", typeof(SyncFolderItemsResponseMessageType))]
		[XmlElement("UnsubscribeResponseMessage", typeof(ResponseMessageType))]
		[XmlElement("UpdateFolderResponseMessage", typeof(FolderInfoResponseMessageType))]
		[XmlElement("UpdateGroupMailboxResponseMessage", typeof(ResponseMessageType))]
		[XmlElement("UpdateItemInRecoverableItemsResponseMessage", typeof(UpdateItemInRecoverableItemsResponseMessageType))]
		[XmlElement("UpdateItemResponseMessage", typeof(UpdateItemResponseMessageType))]
		[XmlElement("UpdateMailboxAssociationResponseMessage", typeof(ResponseMessageType))]
		[XmlElement("UpdateUserConfigurationResponseMessage", typeof(ResponseMessageType))]
		[XmlElement("UploadItemsResponseMessage", typeof(UploadItemsResponseMessageType))]
		[XmlChoiceIdentifier("ItemsElementName")]
		[XmlElement("GetStreamingEventsResponseMessage", typeof(GetStreamingEventsResponseMessageType))]
		[XmlElement("SendItemResponseMessage", typeof(ResponseMessageType))]
		[XmlElement("CopyItemResponseMessage", typeof(ItemInfoResponseMessageType))]
		[XmlElement("CreateAttachmentResponseMessage", typeof(AttachmentInfoResponseMessageType))]
		[XmlElement("CreateFolderPathResponseMessage", typeof(FolderInfoResponseMessageType))]
		[XmlElement("CreateFolderResponseMessage", typeof(FolderInfoResponseMessageType))]
		[XmlElement("CreateItemResponseMessage", typeof(ItemInfoResponseMessageType))]
		[XmlElement("CreateManagedFolderResponseMessage", typeof(FolderInfoResponseMessageType))]
		[XmlElement("ApplyConversationActionResponseMessage", typeof(ApplyConversationActionResponseMessageType))]
		[XmlElement("ArchiveItemResponseMessage", typeof(ItemInfoResponseMessageType))]
		[XmlElement("CreateUserConfigurationResponseMessage", typeof(ResponseMessageType))]
		[XmlElement("DeleteAttachmentResponseMessage", typeof(DeleteAttachmentResponseMessageType))]
		[XmlElement("ConvertIdResponseMessage", typeof(ConvertIdResponseMessageType))]
		[XmlElement("DeleteFolderResponseMessage", typeof(ResponseMessageType))]
		[XmlElement("DeleteItemResponseMessage", typeof(DeleteItemResponseMessageType))]
		[XmlElement("DeleteUserConfigurationResponseMessage", typeof(ResponseMessageType))]
		[XmlElement("EmptyFolderResponseMessage", typeof(ResponseMessageType))]
		[XmlElement("ExpandDLResponseMessage", typeof(ExpandDLResponseMessageType))]
		[XmlElement("ExportItemsResponseMessage", typeof(ExportItemsResponseMessageType))]
		[XmlElement("FindFolderResponseMessage", typeof(FindFolderResponseMessageType))]
		[XmlElement("FindItemResponseMessage", typeof(FindItemResponseMessageType))]
		[XmlElement("FindMailboxStatisticsByKeywordsResponseMessage", typeof(FindMailboxStatisticsByKeywordsResponseMessageType))]
		[XmlElement("FindPeopleResponseMessage", typeof(FindPeopleResponseMessageType))]
		[XmlElement("GetAppManifestsResponseMessage", typeof(ResponseMessageType))]
		[XmlElement("GetAttachmentResponseMessage", typeof(AttachmentInfoResponseMessageType))]
		[XmlElement("GetClientAccessTokenResponseMessage", typeof(GetClientAccessTokenResponseMessageType))]
		[XmlElement("GetClientExtensionResponseMessage", typeof(ResponseMessageType))]
		[XmlElement("GetConversationItemsResponseMessage", typeof(GetConversationItemsResponseMessageType))]
		[XmlElement("GetDiscoverySearchConfigurationResponseMessage", typeof(GetDiscoverySearchConfigurationResponseMessageType))]
		[XmlElement("GetEncryptionConfigurationResponseMessage", typeof(ResponseMessageType))]
		[XmlElement("GetEventsResponseMessage", typeof(GetEventsResponseMessageType))]
		[XmlElement("GetFolderResponseMessage", typeof(FolderInfoResponseMessageType))]
		[XmlElement("GetHoldOnMailboxesResponseMessage", typeof(GetHoldOnMailboxesResponseMessageType))]
		[XmlElement("GetItemResponseMessage", typeof(ItemInfoResponseMessageType))]
		[XmlElement("GetNonIndexableItemDetailsResponseMessage", typeof(GetNonIndexableItemDetailsResponseMessageType))]
		[XmlElement("GetNonIndexableItemStatisticsResponseMessage", typeof(GetNonIndexableItemStatisticsResponseMessageType))]
		[XmlElement("GetPasswordExpirationDateResponse", typeof(GetPasswordExpirationDateResponseMessageType))]
		[XmlElement("GetPersonaResponseMessage", typeof(GetPersonaResponseMessageType))]
		[XmlElement("GetRemindersResponse", typeof(GetRemindersResponseMessageType))]
		[XmlElement("GetRoomListsResponse", typeof(GetRoomListsResponseMessageType))]
		[XmlElement("GetRoomsResponse", typeof(GetRoomsResponseMessageType))]
		[XmlElement("GetSearchableMailboxesResponseMessage", typeof(GetSearchableMailboxesResponseMessageType))]
		[XmlElement("GetServerTimeZonesResponseMessage", typeof(GetServerTimeZonesResponseMessageType))]
		[XmlElement("GetSharingFolderResponseMessage", typeof(GetSharingFolderResponseMessageType))]
		[XmlElement("GetSharingMetadataResponseMessage", typeof(GetSharingMetadataResponseMessageType))]
		[XmlElement("GetUserConfigurationResponseMessage", typeof(GetUserConfigurationResponseMessageType))]
		[XmlElement("GetUserPhotoResponseMessage", typeof(GetUserPhotoResponseMessageType))]
		[XmlElement("GetUserRetentionPolicyTagsResponseMessage", typeof(GetUserRetentionPolicyTagsResponseMessageType))]
		[XmlElement("MarkAllItemsAsReadResponseMessage", typeof(ResponseMessageType))]
		[XmlElement("MarkAsJunkResponseMessage", typeof(MarkAsJunkResponseMessageType))]
		[XmlElement("MoveFolderResponseMessage", typeof(FolderInfoResponseMessageType))]
		[XmlElement("MoveItemResponseMessage", typeof(ItemInfoResponseMessageType))]
		[XmlElement("PerformReminderActionResponse", typeof(PerformReminderActionResponseMessageType))]
		[XmlElement("PostModernGroupItemResponseMessage", typeof(ResponseMessageType))]
		[XmlElement("RefreshSharingFolderResponseMessage", typeof(RefreshSharingFolderResponseMessageType))]
		[XmlElement("ResolveNamesResponseMessage", typeof(ResolveNamesResponseMessageType))]
		[XmlElement("SearchMailboxesResponseMessage", typeof(SearchMailboxesResponseMessageType))]
		public ResponseMessageType[] Items
		{
			get
			{
				return this.itemsField;
			}
			set
			{
				this.itemsField = value;
			}
		}

		[XmlIgnore]
		[XmlElement("ItemsElementName")]
		public ItemsChoiceType4[] ItemsElementName
		{
			get
			{
				return this.itemsElementNameField;
			}
			set
			{
				this.itemsElementNameField = value;
			}
		}

		private ResponseMessageType[] itemsField;

		private ItemsChoiceType4[] itemsElementNameField;
	}
}
