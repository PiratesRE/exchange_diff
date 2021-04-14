using System;
using System.IO;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Compliance.Xml;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Services.Wcf.Types;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlInclude(typeof(UpdateItemInRecoverableItemsResponseMessage))]
	[XmlInclude(typeof(DeleteAttachmentResponseMessage))]
	[XmlInclude(typeof(FindFolderResponseMessage))]
	[KnownType(typeof(AddImGroupResponseMessage))]
	[KnownType(typeof(RemoveImContactFromGroupResponseMessage))]
	[KnownType(typeof(AddImContactToGroupResponseMessage))]
	[XmlInclude(typeof(PerformInstantSearchResponse))]
	[XmlInclude(typeof(ExpandDLResponseMessage))]
	[XmlInclude(typeof(FindMailboxStatisticsByKeywordsResponseMessage))]
	[XmlInclude(typeof(GetDiscoverySearchConfigurationResponse))]
	[XmlInclude(typeof(GetEventsResponseMessage))]
	[XmlInclude(typeof(GetHoldOnMailboxesResponse))]
	[XmlInclude(typeof(GetNonIndexableItemDetailsResponse))]
	[XmlInclude(typeof(GetNonIndexableItemStatisticsResponse))]
	[XmlInclude(typeof(GetPasswordExpirationDateResponse))]
	[XmlInclude(typeof(GetSearchableMailboxesResponse))]
	[XmlInclude(typeof(GetStreamingEventsResponseMessage))]
	[XmlInclude(typeof(GetUserConfigurationResponseMessage))]
	[XmlInclude(typeof(GetUserRetentionPolicyTagsResponse))]
	[XmlInclude(typeof(ResolveNamesResponseMessage))]
	[XmlInclude(typeof(SearchMailboxesResponseMessage))]
	[XmlInclude(typeof(SetHoldOnMailboxesResponse))]
	[XmlInclude(typeof(SetImListMigrationCompletedResponseMessage))]
	[XmlInclude(typeof(SubscribeResponseMessage))]
	[XmlInclude(typeof(SyncFolderHierarchyResponseMessage))]
	[XmlInclude(typeof(SyncFolderItemsResponseMessage))]
	[XmlInclude(typeof(InstallAppResponseMessage))]
	[XmlInclude(typeof(UninstallAppResponseMessage))]
	[XmlInclude(typeof(GetClientExtensionResponse))]
	[XmlInclude(typeof(GetEncryptionConfigurationResponse))]
	[XmlInclude(typeof(SetEncryptionConfigurationResponse))]
	[XmlInclude(typeof(GetUserPhotoResponseMessage))]
	[XmlInclude(typeof(GetPeopleICommunicateWithResponseMessage))]
	[XmlInclude(typeof(DisableAppResponseMessage))]
	[XmlInclude(typeof(GetAppMarketplaceUrlResponseMessage))]
	[XmlInclude(typeof(AddAggregatedAccountResponseMessage))]
	[XmlInclude(typeof(MarkAsJunkResponseMessage))]
	[XmlInclude(typeof(GetAggregatedAccountResponseMessage))]
	[XmlInclude(typeof(RemoveAggregatedAccountResponseMessage))]
	[XmlInclude(typeof(SetAggregatedAccountResponseMessage))]
	[XmlInclude(typeof(ApplyConversationActionResponseMessage))]
	[XmlInclude(typeof(DeleteItemResponseMessage))]
	[KnownType(typeof(CreateUnifiedMailboxResponseMessage))]
	[KnownType(typeof(ExpandDLResponseMessage))]
	[KnownType(typeof(FindMailboxStatisticsByKeywordsResponseMessage))]
	[KnownType(typeof(GetClientAccessTokenResponseMessage))]
	[KnownType(typeof(GetDiscoverySearchConfigurationResponse))]
	[KnownType(typeof(GetEventsResponseMessage))]
	[KnownType(typeof(GetHoldOnMailboxesResponse))]
	[KnownType(typeof(GetPasswordExpirationDateResponse))]
	[KnownType(typeof(GetSearchableMailboxesResponse))]
	[KnownType(typeof(GetStreamingEventsResponseMessage))]
	[KnownType(typeof(GetUserConfigurationResponseMessage))]
	[KnownType(typeof(GetUserRetentionPolicyTagsResponse))]
	[KnownType(typeof(ResolveNamesResponseMessage))]
	[KnownType(typeof(SearchMailboxesResponseMessage))]
	[KnownType(typeof(SetHoldOnMailboxesResponse))]
	[KnownType(typeof(SetImListMigrationCompletedResponseMessage))]
	[KnownType(typeof(SubscribeResponseMessage))]
	[KnownType(typeof(SyncFolderHierarchyResponseMessage))]
	[KnownType(typeof(SyncFolderItemsResponseMessage))]
	[KnownType(typeof(InstallAppResponseMessage))]
	[KnownType(typeof(UninstallAppResponseMessage))]
	[KnownType(typeof(GetClientExtensionResponse))]
	[KnownType(typeof(GetEncryptionConfigurationResponse))]
	[KnownType(typeof(SetEncryptionConfigurationResponse))]
	[KnownType(typeof(GetUserPhotoResponseMessage))]
	[KnownType(typeof(GetPeopleICommunicateWithResponseMessage))]
	[KnownType(typeof(DisableAppResponseMessage))]
	[KnownType(typeof(GetAppMarketplaceUrlResponseMessage))]
	[KnownType(typeof(AddAggregatedAccountResponseMessage))]
	[KnownType(typeof(SubscribeToPushNotificationResponse))]
	[KnownType(typeof(UnsubscribeToPushNotificationResponse))]
	[KnownType(typeof(MarkAsJunkResponseMessage))]
	[KnownType(typeof(GetAggregatedAccountResponseMessage))]
	[KnownType(typeof(RemoveAggregatedAccountResponseMessage))]
	[KnownType(typeof(SetAggregatedAccountResponseMessage))]
	[KnownType(typeof(GetInboxRulesResponse))]
	[KnownType(typeof(ApplyConversationActionResponseMessage))]
	[KnownType(typeof(DeleteItemResponseMessage))]
	[XmlInclude(typeof(AddDistributionGroupToImListResponseMessage))]
	[XmlInclude(typeof(AddImContactToGroupResponseMessage))]
	[XmlInclude(typeof(RemoveImContactFromGroupResponseMessage))]
	[XmlInclude(typeof(AddImGroupResponseMessage))]
	[XmlInclude(typeof(AddNewImContactToGroupResponseMessage))]
	[XmlInclude(typeof(AddNewTelUriContactToGroupResponseMessage))]
	[XmlInclude(typeof(AttachmentInfoResponseMessage))]
	[XmlInclude(typeof(ConvertIdResponseMessage))]
	[XmlInclude(typeof(FindItemResponseMessage))]
	[XmlInclude(typeof(FindPeopleResponseMessage))]
	[XmlInclude(typeof(FolderInfoResponseMessage))]
	[XmlInclude(typeof(GetConversationItemsResponseMessage))]
	[XmlInclude(typeof(GetImItemListResponseMessage))]
	[XmlInclude(typeof(GetImItemsResponseMessage))]
	[XmlInclude(typeof(GetInboxRulesResponse))]
	[XmlInclude(typeof(GetPersonaResponseMessage))]
	[XmlInclude(typeof(ItemInfoResponseMessage))]
	[XmlInclude(typeof(RemoveContactFromImListResponseMessage))]
	[XmlInclude(typeof(RemoveDistributionGroupFromImListResponseMessage))]
	[XmlInclude(typeof(RemoveImGroupResponseMessage))]
	[XmlInclude(typeof(SetImGroupResponseMessage))]
	[XmlInclude(typeof(UpdateInboxRulesResponse))]
	[XmlInclude(typeof(UpdateItemResponseMessage))]
	[XmlInclude(typeof(EndInstantSearchSessionResponse))]
	[XmlInclude(typeof(GetUserUnifiedGroupsResponseMessage))]
	[XmlType("ResponseMessageType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[KnownType(typeof(AddDistributionGroupToImListResponseMessage))]
	[XmlInclude(typeof(CreateUnifiedMailboxResponseMessage))]
	[KnownType(typeof(AddNewImContactToGroupResponseMessage))]
	[KnownType(typeof(AddNewTelUriContactToGroupResponseMessage))]
	[KnownType(typeof(AttachmentInfoResponseMessage))]
	[KnownType(typeof(ConvertIdResponseMessage))]
	[KnownType(typeof(DeleteAttachmentResponseMessage))]
	[KnownType(typeof(FindFolderResponseMessage))]
	[KnownType(typeof(FindItemResponseMessage))]
	[KnownType(typeof(FindPeopleResponseMessage))]
	[KnownType(typeof(FolderInfoResponseMessage))]
	[KnownType(typeof(GetConversationItemsResponseMessage))]
	[KnownType(typeof(GetThreadedConversationItemsResponseMessage))]
	[KnownType(typeof(GetConversationItemsDiagnosticsResponseMessage))]
	[KnownType(typeof(GetImItemListResponseMessage))]
	[KnownType(typeof(GetImItemsResponseMessage))]
	[KnownType(typeof(GetPersonaResponseMessage))]
	[KnownType(typeof(ItemInfoResponseMessage))]
	[KnownType(typeof(RemoveContactFromImListResponseMessage))]
	[KnownType(typeof(RemoveDistributionGroupFromImListResponseMessage))]
	[KnownType(typeof(RemoveImGroupResponseMessage))]
	[KnownType(typeof(SetImGroupResponseMessage))]
	[KnownType(typeof(UpdateInboxRulesResponse))]
	[KnownType(typeof(UpdateItemResponseMessage))]
	[KnownType(typeof(GetUserUnifiedGroupsResponseMessage))]
	[KnownType(typeof(GetClutterStateResponse))]
	[KnownType(typeof(SetClutterStateResponse))]
	[KnownType(typeof(PerformInstantSearchResponse))]
	[KnownType(typeof(EndInstantSearchSessionResponse))]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[KnownType(typeof(GetModernConversationAttachmentsResponseMessage))]
	[KnownType(typeof(LogPushNotificationDataResponse))]
	public class ResponseMessage : IExchangeWebMethodResponse
	{
		private void InternalInitialize(ServiceError error)
		{
			this.messageXml = error.MessageXml;
		}

		private string GetMessageXmlAsString()
		{
			XmlNodeArray xmlNodeArray = this.MessageXml;
			if (xmlNodeArray != null)
			{
				SafeXmlSerializer safeXmlSerializer = new SafeXmlSerializer(xmlNodeArray.GetType(), "http://www.w3.org/2001/XMLSchema-instance");
				using (MemoryStream memoryStream = new MemoryStream())
				{
					try
					{
						safeXmlSerializer.Serialize(memoryStream, xmlNodeArray, ResponseMessage.namespaces);
						memoryStream.Position = 0L;
						using (StreamReader streamReader = new StreamReader(memoryStream))
						{
							return streamReader.ReadToEnd();
						}
					}
					catch (InvalidOperationException ex)
					{
						ExTraceGlobals.UtilCallTracer.TraceError<string, string>((long)this.GetHashCode(), "GetMessageXmlAsString failed to serialize MessageXml. Exception class: {0}, Message: {1}", ex.GetType().FullName, ex.Message);
					}
				}
			}
			return null;
		}

		static ResponseMessage()
		{
			ResponseMessage.namespaces.Add("m", "http://schemas.microsoft.com/exchange/services/2006/messages");
			ResponseMessage.namespaces.Add("t", "http://schemas.microsoft.com/exchange/services/2006/types");
		}

		public ResponseMessage()
		{
		}

		internal ResponseMessage(ServiceResultCode code, ServiceError error)
		{
			this.Initialize(code, error);
		}

		internal bool StopsBatchProcessing
		{
			get
			{
				return this.error != null && this.error.StopsBatchProcessing;
			}
		}

		internal void Initialize(ServiceResultCode code, ServiceError error)
		{
			switch (code)
			{
			case ServiceResultCode.Success:
				this.responseClass = ResponseClass.Success;
				return;
			case ServiceResultCode.Warning:
				this.error = error;
				this.InternalInitialize(error);
				this.responseClass = ResponseClass.Warning;
				return;
			case ServiceResultCode.Error:
				this.error = error;
				this.InternalInitialize(error);
				this.responseClass = ResponseClass.Error;
				return;
			default:
				return;
			}
		}

		protected void ExecuteServiceCommandIfRequired()
		{
			if (!this.hasServiceCommandExecuted)
			{
				try
				{
					ServiceDiagnostics.SendWatsonReportOnUnhandledException(delegate
					{
						this.InternalExecuteServiceCommand();
					});
				}
				finally
				{
					this.MessageText = ((this.error != null) ? this.error.MessageText : null);
					this.ResponseCode = ((this.error != null) ? this.error.MessageKey : ResponseCodeType.NoError);
					this.DescriptiveLinkKey = ((this.error != null) ? this.error.DescriptiveLinkId : 0);
				}
			}
		}

		protected virtual void InternalExecuteServiceCommand()
		{
		}

		[XmlElement("MessageText", IsNullable = false)]
		[DataMember(EmitDefaultValue = false, Order = 1)]
		public string MessageText
		{
			get
			{
				this.ExecuteServiceCommandIfRequired();
				return this.messageText;
			}
			set
			{
				this.hasServiceCommandExecuted = true;
				this.messageText = value;
			}
		}

		[IgnoreDataMember]
		[XmlElement("ResponseCode")]
		public ResponseCodeType ResponseCode
		{
			get
			{
				this.ExecuteServiceCommandIfRequired();
				return this.responseCode;
			}
			set
			{
				this.hasServiceCommandExecuted = true;
				this.responseCode = value;
			}
		}

		[DataMember(Name = "ResponseCode", Order = 2)]
		[XmlIgnore]
		public string ResponseCodeString
		{
			get
			{
				return EnumUtilities.ToString<ResponseCodeType>(this.ResponseCode);
			}
			set
			{
				this.ResponseCode = EnumUtilities.Parse<ResponseCodeType>(value);
			}
		}

		[DataMember(EmitDefaultValue = false, Order = 3)]
		[XmlElement("DescriptiveLinkKey")]
		public int DescriptiveLinkKey
		{
			get
			{
				this.ExecuteServiceCommandIfRequired();
				return this.descriptiveLinkKey;
			}
			set
			{
				this.hasServiceCommandExecuted = true;
				this.descriptiveLinkKey = value;
			}
		}

		[XmlElement("MessageXml")]
		[IgnoreDataMember]
		public XmlNodeArray MessageXml
		{
			get
			{
				this.ExecuteServiceCommandIfRequired();
				return this.messageXml;
			}
			set
			{
				this.messageXml = value;
			}
		}

		[XmlIgnore]
		[DataMember(Name = "MessageXml", EmitDefaultValue = false, Order = 4)]
		public string MessageXmlString
		{
			get
			{
				if (this.messageXmlStringSet)
				{
					return this.messageXmlString;
				}
				return this.GetMessageXmlAsString();
			}
			set
			{
				this.messageXmlStringSet = true;
				this.messageXmlString = value;
			}
		}

		[XmlAttribute("ResponseClass")]
		[IgnoreDataMember]
		public ResponseClass ResponseClass
		{
			get
			{
				this.ExecuteServiceCommandIfRequired();
				return this.responseClass;
			}
			set
			{
				this.hasServiceCommandExecuted = true;
				this.responseClass = value;
			}
		}

		[XmlIgnore]
		[DataMember(Name = "ResponseClass", Order = 5)]
		public string ResponseClassString
		{
			get
			{
				return EnumUtilities.ToString<ResponseClass>(this.ResponseClass);
			}
			set
			{
				this.ResponseClass = EnumUtilities.Parse<ResponseClass>(value);
			}
		}

		[XmlIgnore]
		[IgnoreDataMember]
		public bool MessageIndexSpecified
		{
			get
			{
				this.ExecuteServiceCommandIfRequired();
				return this.error != null;
			}
			set
			{
				throw new InvalidOperationException("ResponseMessage.MessageIndexSpecified.set should never be called.");
			}
		}

		[XmlIgnore]
		[IgnoreDataMember]
		public bool DescriptiveLinkKeySpecified
		{
			get
			{
				this.ExecuteServiceCommandIfRequired();
				return this.error != null;
			}
			set
			{
			}
		}

		public virtual ResponseType GetResponseType()
		{
			ExTraceGlobals.UtilCallTracer.TraceError((long)this.GetHashCode(), "Override GetResponseType in your response derived class, the base class doesn't provide implementation.");
			throw new InvalidOperationException("Override GetResponseType in your response derived class");
		}

		ResponseCodeType IExchangeWebMethodResponse.GetErrorCodeToLog()
		{
			return this.ResponseCode;
		}

		private ServiceError error;

		private ResponseClass responseClass;

		private string messageText;

		private ResponseCodeType responseCode;

		private int descriptiveLinkKey;

		private XmlNodeArray messageXml;

		private bool messageXmlStringSet;

		private string messageXmlString;

		protected static XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();

		private bool hasServiceCommandExecuted;
	}
}
