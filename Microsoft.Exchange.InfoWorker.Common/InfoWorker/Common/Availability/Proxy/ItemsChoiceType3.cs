using System;
using System.CodeDom.Compiler;
using System.Xml.Serialization;

namespace Microsoft.Exchange.InfoWorker.Common.Availability.Proxy
{
	[GeneratedCode("wsdl", "2.0.50727.1432")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages", IncludeInSchema = false)]
	[Serializable]
	public enum ItemsChoiceType3
	{
		ConvertIdResponseMessage,
		CopyFolderResponseMessage,
		CopyItemResponseMessage,
		CreateAttachmentResponseMessage,
		CreateFolderResponseMessage,
		CreateItemResponseMessage,
		CreateManagedFolderResponseMessage,
		DeleteAttachmentResponseMessage,
		DeleteFolderResponseMessage,
		DeleteItemResponseMessage,
		ExpandDLResponseMessage,
		FindFolderResponseMessage,
		FindItemResponseMessage,
		GetAttachmentResponseMessage,
		GetEventsResponseMessage,
		GetFolderResponseMessage,
		GetItemResponseMessage,
		GetSharingMetadataResponseMessage,
		MoveFolderResponseMessage,
		MoveItemResponseMessage,
		RefreshSharingFolderResponseMessage,
		ResolveNamesResponseMessage,
		SendItemResponseMessage,
		SendNotificationResponseMessage,
		SubscribeResponseMessage,
		SyncFolderHierarchyResponseMessage,
		SyncFolderItemsResponseMessage,
		UnsubscribeResponseMessage,
		UpdateFolderResponseMessage,
		UpdateItemResponseMessage,
		Items
	}
}
