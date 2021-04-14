using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Services.Wcf.Types;

namespace Microsoft.Exchange.Services.Core.Types
{
	[KnownType(typeof(SubscribeResponse))]
	[XmlInclude(typeof(CreateFolderResponse))]
	[XmlInclude(typeof(CreateItemResponse))]
	[XmlInclude(typeof(GetConversationItemsResponse))]
	[XmlInclude(typeof(ArchiveItemResponse))]
	[XmlInclude(typeof(CreateManagedFolderResponse))]
	[XmlInclude(typeof(CreateUserConfigurationResponse))]
	[XmlInclude(typeof(CreateUnifiedMailboxResponse))]
	[XmlInclude(typeof(DeleteUserConfigurationResponse))]
	[XmlInclude(typeof(DeprovisionResponse))]
	[XmlInclude(typeof(ExpandDLResponse))]
	[XmlInclude(typeof(ExportItemsResponse))]
	[XmlInclude(typeof(GetClientAccessTokenResponse))]
	[XmlInclude(typeof(GetEventsResponse))]
	[XmlInclude(typeof(GetUserConfigurationResponse))]
	[XmlInclude(typeof(ProvisionResponse))]
	[XmlInclude(typeof(ResolveNamesResponse))]
	[XmlInclude(typeof(SearchMailboxesResponse))]
	[XmlInclude(typeof(SubscribeResponse))]
	[XmlInclude(typeof(UnsubscribeResponse))]
	[XmlInclude(typeof(UpdateUserConfigurationResponse))]
	[XmlInclude(typeof(UploadItemsResponse))]
	[XmlInclude(typeof(InstallAppResponse))]
	[XmlInclude(typeof(UninstallAppResponse))]
	[XmlInclude(typeof(SetClientExtensionResponse))]
	[XmlInclude(typeof(GetUserPhotoResponse))]
	[XmlInclude(typeof(DisableAppResponse))]
	[XmlInclude(typeof(GetAppMarketplaceUrlResponse))]
	[XmlInclude(typeof(AddAggregatedAccountResponse))]
	[XmlInclude(typeof(MarkAsJunkResponse))]
	[XmlInclude(typeof(GetAggregatedAccountResponse))]
	[XmlInclude(typeof(RemoveAggregatedAccountResponse))]
	[XmlInclude(typeof(SetAggregatedAccountResponse))]
	[XmlInclude(typeof(ApplyConversationActionResponseMessage))]
	[XmlInclude(typeof(DeleteItemResponseMessage))]
	[KnownType(typeof(ArchiveItemResponse))]
	[KnownType(typeof(CreateManagedFolderResponse))]
	[KnownType(typeof(CreateUserConfigurationResponse))]
	[KnownType(typeof(CreateUnifiedMailboxResponse))]
	[KnownType(typeof(DeleteUserConfigurationResponse))]
	[KnownType(typeof(DeprovisionResponse))]
	[KnownType(typeof(ExpandDLResponse))]
	[KnownType(typeof(ExportItemsResponse))]
	[KnownType(typeof(GetClientAccessTokenResponse))]
	[KnownType(typeof(GetEventsResponse))]
	[KnownType(typeof(GetUserConfigurationResponse))]
	[KnownType(typeof(ProvisionResponse))]
	[KnownType(typeof(ResolveNamesResponse))]
	[KnownType(typeof(SearchMailboxesResponse))]
	[KnownType(typeof(UnsubscribeResponse))]
	[KnownType(typeof(UpdateUserConfigurationResponse))]
	[KnownType(typeof(UploadItemsResponse))]
	[KnownType(typeof(GetUserPhotoResponse))]
	[KnownType(typeof(InstallAppResponse))]
	[KnownType(typeof(UninstallAppResponse))]
	[KnownType(typeof(SetClientExtensionResponse))]
	[KnownType(typeof(DisableAppResponse))]
	[KnownType(typeof(GetAppMarketplaceUrlResponse))]
	[KnownType(typeof(AddAggregatedAccountResponse))]
	[KnownType(typeof(MarkAsJunkResponse))]
	[KnownType(typeof(GetAggregatedAccountResponse))]
	[KnownType(typeof(RemoveAggregatedAccountResponse))]
	[KnownType(typeof(SetAggregatedAccountResponse))]
	[KnownType(typeof(ApplyConversationActionResponseMessage))]
	[KnownType(typeof(DeleteItemResponseMessage))]
	[XmlInclude(typeof(ConvertIdResponse))]
	[XmlInclude(typeof(CopyFolderResponse))]
	[XmlInclude(typeof(CopyItemResponse))]
	[XmlInclude(typeof(CreateAttachmentResponse))]
	[KnownType(typeof(SendItemResponse))]
	[XmlInclude(typeof(CreateFolderPathResponse))]
	[XmlInclude(typeof(DeleteAttachmentResponse))]
	[XmlInclude(typeof(DeleteFolderResponse))]
	[XmlInclude(typeof(DeleteItemResponse))]
	[XmlInclude(typeof(EmptyFolderResponse))]
	[XmlInclude(typeof(FindFolderResponse))]
	[XmlInclude(typeof(FindItemResponse))]
	[XmlInclude(typeof(GetAttachmentResponse))]
	[KnownType(typeof(GetModernConversationAttachmentsResponse))]
	[KnownType(typeof(UpdateFolderResponse))]
	[KnownType(typeof(GetServerTimeZonesResponse))]
	[XmlInclude(typeof(GetServerTimeZonesResponse))]
	[XmlInclude(typeof(MoveFolderResponse))]
	[XmlInclude(typeof(MoveItemResponse))]
	[XmlInclude(typeof(SendItemResponse))]
	[XmlInclude(typeof(SendNotificationResponse))]
	[XmlInclude(typeof(UpdateFolderResponse))]
	[XmlInclude(typeof(UpdateItemResponse))]
	[XmlType("BaseResponseMessageType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[KnownType(typeof(ConvertIdResponse))]
	[KnownType(typeof(CopyFolderResponse))]
	[KnownType(typeof(CopyItemResponse))]
	[KnownType(typeof(CreateAttachmentResponse))]
	[KnownType(typeof(CreateFolderResponse))]
	[KnownType(typeof(CreateItemResponse))]
	[KnownType(typeof(CreateFolderPathResponse))]
	[KnownType(typeof(DeleteAttachmentResponse))]
	[KnownType(typeof(DeleteFolderResponse))]
	[KnownType(typeof(DeleteItemResponse))]
	[KnownType(typeof(EmptyFolderResponse))]
	[KnownType(typeof(FindFolderResponse))]
	[KnownType(typeof(SendNotificationResponse))]
	[KnownType(typeof(GetAttachmentResponse))]
	[KnownType(typeof(GetConversationItemsResponse))]
	[KnownType(typeof(GetThreadedConversationItemsResponse))]
	[KnownType(typeof(GetConversationItemsDiagnosticsResponseType))]
	[KnownType(typeof(GetFolderResponse))]
	[KnownType(typeof(GetItemResponse))]
	[KnownType(typeof(MoveFolderResponse))]
	[KnownType(typeof(MoveItemResponse))]
	[KnownType(typeof(PostModernGroupItemResponse))]
	[XmlInclude(typeof(GetItemResponse))]
	[KnownType(typeof(FindItemResponse))]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlInclude(typeof(GetFolderResponse))]
	[KnownType(typeof(UpdateItemResponse))]
	[KnownType(typeof(UpdateAndPostModernGroupItemResponse))]
	[KnownType(typeof(CreateResponseFromModernGroupResponse))]
	[Serializable]
	public class BaseResponseMessage : IExchangeWebMethodResponse
	{
		[OnDeserializing]
		private void OnDeserializing(StreamingContext context)
		{
			this.Initialize();
		}

		static BaseResponseMessage()
		{
			BaseResponseMessage.namespaces.Add("m", "http://schemas.microsoft.com/exchange/services/2006/messages");
			BaseResponseMessage.namespaces.Add("t", "http://schemas.microsoft.com/exchange/services/2006/types");
		}

		public BaseResponseMessage()
		{
			this.Initialize();
		}

		internal BaseResponseMessage(ResponseType responseType) : this()
		{
			this.ResponseType = responseType;
		}

		private void Initialize()
		{
			this.responseMessages = new ArrayOfResponseMessages();
		}

		[XmlNamespaceDeclarations]
		public XmlSerializerNamespaces Namespaces
		{
			get
			{
				return BaseResponseMessage.namespaces;
			}
			set
			{
			}
		}

		[DataMember]
		public ArrayOfResponseMessages ResponseMessages
		{
			get
			{
				return this.responseMessages;
			}
			set
			{
				this.responseMessages = value;
			}
		}

		[XmlIgnore]
		public ResponseType ResponseType
		{
			get
			{
				return this.responseType;
			}
			set
			{
				this.responseType = value;
			}
		}

		internal virtual void ProcessServiceResult(ServiceResult<ServiceResultNone> result)
		{
			this.AddResponse(new ResponseMessage(result.Code, result.Error));
		}

		internal void AddResponse(ResponseMessage message)
		{
			this.ResponseMessages.AddResponse(message, this.ResponseType);
		}

		internal void BuildForNoReturnValue(ServiceResult<ServiceResultNone>[] serviceResults)
		{
			ServiceResult<ServiceResultNone>.ProcessServiceResults(serviceResults, new ProcessServiceResult<ServiceResultNone>(this.ProcessServiceResult));
		}

		ResponseType IExchangeWebMethodResponse.GetResponseType()
		{
			return this.ResponseType;
		}

		ResponseCodeType IExchangeWebMethodResponse.GetErrorCodeToLog()
		{
			foreach (ResponseMessage responseMessage in this.ResponseMessages.Items)
			{
				if (responseMessage.ResponseCode != ResponseCodeType.NoError)
				{
					return responseMessage.ResponseCode;
				}
			}
			return ResponseCodeType.NoError;
		}

		private static XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();

		private ResponseType responseType;

		private ArrayOfResponseMessages responseMessages;
	}
}
