using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[XmlInclude(typeof(DeleteItemResponseMessageType))]
	[XmlInclude(typeof(UpdateDelegateResponseMessageType))]
	[XmlInclude(typeof(GetRemindersResponseMessageType))]
	[XmlInclude(typeof(GetUMSubscriberCallAnsweringDataResponseMessageType))]
	[XmlInclude(typeof(GetClientIntentResponseMessageType))]
	[XmlInclude(typeof(GetUMPinResponseMessageType))]
	[XmlInclude(typeof(SaveUMPinResponseMessageType))]
	[XmlInclude(typeof(ValidateUMPinResponseMessageType))]
	[XmlInclude(typeof(ResetUMMailboxResponseMessageType))]
	[XmlInclude(typeof(InitUMMailboxResponseMessageType))]
	[XmlInclude(typeof(GetUMCallSummaryResponseMessageType))]
	[XmlInclude(typeof(GetUMCallDataRecordsResponseMessageType))]
	[XmlInclude(typeof(CreateUMCallDataRecordResponseMessageType))]
	[XmlInclude(typeof(CompleteFindInGALSpeechRecognitionResponseMessageType))]
	[XmlInclude(typeof(StartFindInGALSpeechRecognitionResponseMessageType))]
	[XmlInclude(typeof(GetUserPhotoResponseMessageType))]
	[XmlInclude(typeof(GetUserRetentionPolicyTagsResponseMessageType))]
	[XmlInclude(typeof(SetImListMigrationCompletedResponseMessageType))]
	[XmlInclude(typeof(SetImGroupResponseMessageType))]
	[XmlInclude(typeof(RemoveImGroupResponseMessageType))]
	[XmlInclude(typeof(RemoveDistributionGroupFromImListResponseMessageType))]
	[XmlInclude(typeof(RemoveContactFromImListResponseMessageType))]
	[XmlInclude(typeof(GetImItemsResponseMessageType))]
	[XmlInclude(typeof(GetImItemListResponseMessageType))]
	[XmlInclude(typeof(AddDistributionGroupToImListResponseMessageType))]
	[XmlInclude(typeof(AddImGroupResponseMessageType))]
	[XmlInclude(typeof(RemoveImContactFromGroupResponseMessageType))]
	[XmlInclude(typeof(AddImContactToGroupResponseMessageType))]
	[XmlInclude(typeof(AddNewTelUriContactToGroupResponseMessageType))]
	[XmlInclude(typeof(AddNewImContactToGroupResponseMessageType))]
	[XmlInclude(typeof(DisableAppResponseType))]
	[XmlInclude(typeof(UninstallAppResponseType))]
	[XmlInclude(typeof(InstallAppResponseType))]
	[XmlInclude(typeof(MarkAsJunkResponseMessageType))]
	[XmlInclude(typeof(GetAppMarketplaceUrlResponseMessageType))]
	[XmlInclude(typeof(GetAppManifestsResponseType))]
	[XmlInclude(typeof(SetEncryptionConfigurationResponseType))]
	[XmlInclude(typeof(EncryptionConfigurationResponseType))]
	[XmlInclude(typeof(ClientExtensionResponseType))]
	[XmlInclude(typeof(GetConversationItemsResponseMessageType))]
	[XmlInclude(typeof(GetNonIndexableItemDetailsResponseMessageType))]
	[XmlInclude(typeof(GetNonIndexableItemStatisticsResponseMessageType))]
	[XmlInclude(typeof(SetHoldOnMailboxesResponseMessageType))]
	[XmlInclude(typeof(GetHoldOnMailboxesResponseMessageType))]
	[XmlInclude(typeof(GetDiscoverySearchConfigurationResponseMessageType))]
	[XmlInclude(typeof(SearchMailboxesResponseMessageType))]
	[XmlInclude(typeof(GetSearchableMailboxesResponseMessageType))]
	[XmlInclude(typeof(FindMailboxStatisticsByKeywordsResponseMessageType))]
	[XmlInclude(typeof(UpdateInboxRulesResponseType))]
	[XmlInclude(typeof(GetInboxRulesResponseType))]
	[XmlInclude(typeof(GetMessageTrackingReportResponseMessageType))]
	[XmlInclude(typeof(FindMessageTrackingReportResponseMessageType))]
	[XmlInclude(typeof(ServiceConfigurationResponseMessageType))]
	[XmlInclude(typeof(GetServiceConfigurationResponseMessageType))]
	[XmlInclude(typeof(PerformReminderActionResponseMessageType))]
	[XmlInclude(typeof(DisconnectPhoneCallResponseMessageType))]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[XmlInclude(typeof(GetRoomListsResponseMessageType))]
	[XmlInclude(typeof(UnpinTeamMailboxResponseMessageType))]
	[XmlInclude(typeof(SetTeamMailboxResponseMessageType))]
	[XmlInclude(typeof(GetUserConfigurationResponseMessageType))]
	[XmlInclude(typeof(GetSharingFolderResponseMessageType))]
	[XmlInclude(typeof(RefreshSharingFolderResponseMessageType))]
	[XmlInclude(typeof(GetSharingMetadataResponseMessageType))]
	[XmlInclude(typeof(BaseDelegateResponseMessageType))]
	[XmlInclude(typeof(RemoveDelegateResponseMessageType))]
	[XmlInclude(typeof(AddDelegateResponseMessageType))]
	[XmlInclude(typeof(GetDelegateResponseMessageType))]
	[XmlInclude(typeof(DelegateUserResponseMessageType))]
	[XmlInclude(typeof(ConvertIdResponseMessageType))]
	[XmlInclude(typeof(SyncFolderItemsResponseMessageType))]
	[XmlInclude(typeof(SyncFolderHierarchyResponseMessageType))]
	[XmlInclude(typeof(SendNotificationResponseMessageType))]
	[XmlInclude(typeof(GetStreamingEventsResponseMessageType))]
	[XmlInclude(typeof(GetEventsResponseMessageType))]
	[XmlInclude(typeof(SubscribeResponseMessageType))]
	[XmlInclude(typeof(GetServerTimeZonesResponseMessageType))]
	[XmlInclude(typeof(ExpandDLResponseMessageType))]
	[XmlInclude(typeof(GetUMPromptResponseMessageType))]
	[XmlInclude(typeof(GetUMPromptNamesResponseMessageType))]
	[XmlInclude(typeof(CreateUMPromptResponseMessageType))]
	[XmlInclude(typeof(DeleteUMPromptsResponseMessageType))]
	[XmlInclude(typeof(GetRoomsResponseMessageType))]
	[XmlInclude(typeof(GetPhoneCallInformationResponseMessageType))]
	[XmlInclude(typeof(PlayOnPhoneResponseMessageType))]
	[XmlInclude(typeof(MailTipsResponseMessageType))]
	[XmlInclude(typeof(GetMailTipsResponseMessageType))]
	[XmlInclude(typeof(GetPasswordExpirationDateResponseMessageType))]
	[XmlInclude(typeof(ResolveNamesResponseMessageType))]
	[XmlInclude(typeof(GetClientAccessTokenResponseMessageType))]
	[XmlInclude(typeof(FindItemResponseMessageType))]
	[XmlInclude(typeof(GetPersonaResponseMessageType))]
	[XmlInclude(typeof(FindPeopleResponseMessageType))]
	[XmlInclude(typeof(ApplyConversationActionResponseMessageType))]
	[XmlInclude(typeof(FindConversationResponseMessageType))]
	[XmlInclude(typeof(DeleteAttachmentResponseMessageType))]
	[XmlInclude(typeof(AttachmentInfoResponseMessageType))]
	[XmlInclude(typeof(ItemInfoResponseMessageType))]
	[XmlInclude(typeof(UpdateItemInRecoverableItemsResponseMessageType))]
	[XmlInclude(typeof(UpdateItemResponseMessageType))]
	[XmlInclude(typeof(FindFolderResponseMessageType))]
	[XmlInclude(typeof(FolderInfoResponseMessageType))]
	[XmlInclude(typeof(ExportItemsResponseMessageType))]
	[XmlInclude(typeof(UploadItemsResponseMessageType))]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[Serializable]
	public class ResponseMessageType
	{
		public string MessageText
		{
			get
			{
				return this.messageTextField;
			}
			set
			{
				this.messageTextField = value;
			}
		}

		public ResponseCodeType ResponseCode
		{
			get
			{
				return this.responseCodeField;
			}
			set
			{
				this.responseCodeField = value;
			}
		}

		[XmlIgnore]
		public bool ResponseCodeSpecified
		{
			get
			{
				return this.responseCodeFieldSpecified;
			}
			set
			{
				this.responseCodeFieldSpecified = value;
			}
		}

		public int DescriptiveLinkKey
		{
			get
			{
				return this.descriptiveLinkKeyField;
			}
			set
			{
				this.descriptiveLinkKeyField = value;
			}
		}

		[XmlIgnore]
		public bool DescriptiveLinkKeySpecified
		{
			get
			{
				return this.descriptiveLinkKeyFieldSpecified;
			}
			set
			{
				this.descriptiveLinkKeyFieldSpecified = value;
			}
		}

		public ResponseMessageTypeMessageXml MessageXml
		{
			get
			{
				return this.messageXmlField;
			}
			set
			{
				this.messageXmlField = value;
			}
		}

		[XmlAttribute]
		public ResponseClassType ResponseClass
		{
			get
			{
				return this.responseClassField;
			}
			set
			{
				this.responseClassField = value;
			}
		}

		private string messageTextField;

		private ResponseCodeType responseCodeField;

		private bool responseCodeFieldSpecified;

		private int descriptiveLinkKeyField;

		private bool descriptiveLinkKeyFieldSpecified;

		private ResponseMessageTypeMessageXml messageXmlField;

		private ResponseClassType responseClassField;
	}
}
