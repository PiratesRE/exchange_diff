using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType("ArrayOfResponseMessagesType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[Serializable]
	public class ArrayOfResponseMessages
	{
		[XmlElement("SendNotificationResponseMessage", typeof(SendNotificationResponseMessage))]
		[XmlElement("ExecuteDiagnosticMethodResponseMessage", typeof(ExecuteDiagnosticMethodResponseMessage))]
		[XmlElement("SyncFolderHierarchyResponseMessage", typeof(SyncFolderHierarchyResponseMessage))]
		[XmlElement("FindMailboxStatisticsByKeywordsResponseMessage", typeof(FindMailboxStatisticsByKeywordsResponseMessage))]
		[XmlElement("SearchMailboxesResponseMessage", typeof(SearchMailboxesResponseMessage))]
		[XmlElement("ConvertIdResponseMessage", typeof(ConvertIdResponseMessage))]
		[XmlElement("ApplyConversationActionResponseMessage", typeof(ApplyConversationActionResponseMessage))]
		[XmlElement("FindPeopleResponseMessage", typeof(FindPeopleResponseMessage))]
		[XmlElement("GetPersonaResponseMessage", typeof(GetPersonaResponseMessage))]
		[XmlElement("FindFolderResponseMessage", typeof(FindFolderResponseMessage))]
		[XmlElement("MarkAllItemsAsReadResponseMessage", typeof(ResponseMessage))]
		[XmlElement("GetUserPhotoResponseMessage", typeof(GetUserPhotoResponseMessage))]
		[XmlElement("GetPeopleICommunicateWithResponseMessage", typeof(GetPeopleICommunicateWithResponseMessage))]
		[XmlElement("CreateManagedFolderResponseMessage", typeof(FolderInfoResponseMessage))]
		[XmlElement("CreateFolderPathResponseMessage", typeof(FolderInfoResponseMessage))]
		[XmlElement("UploadItemsResponseMessage", typeof(UploadItemsResponseMessage))]
		[XmlElement("CreateUserConfigurationResponseMessage", typeof(ResponseMessage))]
		[XmlElement("ExportItemsResponseMessage", typeof(ExportItemsResponseMessage))]
		[XmlElement("SetClientExtensionResponseMessage", typeof(ResponseMessage))]
		[XmlElement("DeleteFolderResponseMessage", typeof(ResponseMessage))]
		[XmlElement("DeleteItemResponseMessage", typeof(DeleteItemResponseMessage))]
		[XmlElement("EmptyFolderResponseMessage", typeof(ResponseMessage))]
		[XmlElement("CreateFolderResponseMessage", typeof(FolderInfoResponseMessage))]
		[XmlElement("CopyItemResponseMessage", typeof(ItemInfoResponseMessage))]
		[XmlElement("GetStreamingEventsResponseMessage", typeof(GetStreamingEventsResponseMessage))]
		[XmlElement("CreateItemResponseMessage", typeof(ItemInfoResponseMessage))]
		[XmlElement("MarkAsJunkResponseMessage", typeof(MarkAsJunkResponseMessage))]
		[XmlElement("SubscribeResponseMessage", typeof(SubscribeResponseMessage))]
		[XmlElement("UnsubscribeResponseMessage", typeof(ResponseMessage))]
		[XmlElement("UpdateFolderResponseMessage", typeof(FolderInfoResponseMessage))]
		[XmlElement("UpdateItemResponseMessage", typeof(UpdateItemResponseMessage))]
		[XmlElement("PostModernGroupItemResponseMessage", typeof(ItemInfoResponseMessage))]
		[XmlElement("UpdateItemInRecoverableItemsResponseMessage", typeof(UpdateItemInRecoverableItemsResponseMessage))]
		[XmlElement("UpdateMailboxAssociationResponseMessage", typeof(ResponseMessage))]
		[XmlElement("UpdateGroupMailboxResponseMessage", typeof(ResponseMessage))]
		[XmlElement("DeleteUserConfigurationResponseMessage", typeof(ResponseMessage))]
		[XmlElement("ExpandDLResponseMessage", typeof(ExpandDLResponseMessage))]
		[XmlElement("GetConversationItemsResponseMessage", typeof(GetConversationItemsResponseMessage))]
		[XmlElement("FindItemResponseMessage", typeof(FindItemResponseMessage))]
		[XmlChoiceIdentifier("ItemsElementName")]
		[DataMember]
		[XmlElement("GetEventsResponseMessage", typeof(GetEventsResponseMessage))]
		[XmlElement("GetClientAccessTokenResponseMessage", typeof(GetClientAccessTokenResponseMessage))]
		[XmlElement("GetFolderResponseMessage", typeof(FolderInfoResponseMessage))]
		[XmlElement("GetMailTipsResponseMessage", typeof(GetMailTipsResponseMessage))]
		[XmlElement("GetItemResponseMessage", typeof(ItemInfoResponseMessage))]
		[XmlElement("GetServerTimeZonesResponseMessage", typeof(GetServerTimeZonesResponseMessage))]
		[XmlElement("GetUserConfigurationResponseMessage", typeof(GetUserConfigurationResponseMessage))]
		[XmlElement("MoveFolderResponseMessage", typeof(FolderInfoResponseMessage))]
		[XmlElement("MoveItemResponseMessage", typeof(ItemInfoResponseMessage))]
		[XmlElement("ResolveNamesResponseMessage", typeof(ResolveNamesResponseMessage))]
		[XmlElement("DeleteAttachmentResponseMessage", typeof(DeleteAttachmentResponseMessage))]
		[XmlElement("SendItemResponseMessage", typeof(ResponseMessage))]
		[XmlElement("CopyFolderResponseMessage", typeof(FolderInfoResponseMessage))]
		[XmlElement("GetAttachmentResponseMessage", typeof(AttachmentInfoResponseMessage))]
		[XmlElement("UpdateUserConfigurationResponseMessage", typeof(ResponseMessage))]
		[XmlElement("CreateAttachmentResponseMessage", typeof(AttachmentInfoResponseMessage))]
		[XmlElement("SyncFolderItemsResponseMessage", typeof(SyncFolderItemsResponseMessage))]
		[XmlElement("ArchiveItemResponseMessage", typeof(ItemInfoResponseMessage))]
		public ResponseMessage[] Items
		{
			get
			{
				return this.items.ToArray();
			}
			set
			{
				this.items.Clear();
				this.items.AddRange(value);
			}
		}

		[OnDeserializing]
		private void OnDeserializing(StreamingContext context)
		{
			this.Initialize();
		}

		public ArrayOfResponseMessages()
		{
			this.Initialize();
		}

		[XmlElement("ItemsElementName")]
		[XmlIgnore]
		public ResponseType[] ItemsElementName
		{
			get
			{
				return this.itemsElementName.ToArray();
			}
			set
			{
				this.itemsElementName.Clear();
				this.itemsElementName.AddRange(value);
			}
		}

		internal void AddResponse(ResponseMessage message, ResponseType messageType)
		{
			this.items.Add(message);
			this.itemsElementName.Add(messageType);
		}

		private void Initialize()
		{
			this.items = new List<ResponseMessage>();
			this.itemsElementName = new List<ResponseType>();
		}

		private List<ResponseMessage> items;

		private List<ResponseType> itemsElementName;
	}
}
