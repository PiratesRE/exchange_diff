using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Net.Security;
using System.Threading;
using System.Web.Services;
using System.Web.Services.Description;
using System.Web.Services.Protocols;
using System.Xml.Serialization;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;
using Microsoft.Exchange.Net.WSTrust;

namespace Microsoft.Exchange.SoapWebClient.EWS
{
	[XmlInclude(typeof(BaseResponseMessageType))]
	[XmlInclude(typeof(BaseFolderType))]
	[XmlInclude(typeof(BaseRequestType))]
	[CLSCompliant(false)]
	[XmlInclude(typeof(BaseItemIdType))]
	[XmlInclude(typeof(BaseEmailAddressType))]
	[XmlInclude(typeof(BaseFolderIdType))]
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[DebuggerStepThrough]
	[DesignerCategory("code")]
	[WebServiceBinding(Name = "ExchangeServiceBinding", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[XmlInclude(typeof(AttendeeConflictData))]
	[XmlInclude(typeof(ServiceConfiguration))]
	[XmlInclude(typeof(DirectoryEntryType))]
	[XmlInclude(typeof(BaseCalendarItemStateDefinitionType))]
	[XmlInclude(typeof(RuleOperationType))]
	[XmlInclude(typeof(BaseSubscriptionRequestType))]
	[XmlInclude(typeof(MailboxLocatorType))]
	[XmlInclude(typeof(BaseGroupByType))]
	[XmlInclude(typeof(RecurrenceRangeBaseType))]
	[XmlInclude(typeof(RecurrencePatternBaseType))]
	[XmlInclude(typeof(AttachmentType))]
	[XmlInclude(typeof(ChangeDescriptionType))]
	[XmlInclude(typeof(BasePagingType))]
	[XmlInclude(typeof(BasePermissionType))]
	public class ExchangeServiceBinding : CustomSoapHttpClientProtocol, IExchangeService
	{
		public ExchangeServiceBinding()
		{
		}

		public event ResolveNamesCompletedEventHandler ResolveNamesCompleted;

		public event ExpandDLCompletedEventHandler ExpandDLCompleted;

		public event GetServerTimeZonesCompletedEventHandler GetServerTimeZonesCompleted;

		public event FindFolderCompletedEventHandler FindFolderCompleted;

		public event FindItemCompletedEventHandler FindItemCompleted;

		public event GetFolderCompletedEventHandler GetFolderCompleted;

		public event UploadItemsCompletedEventHandler UploadItemsCompleted;

		public event ExportItemsCompletedEventHandler ExportItemsCompleted;

		public event ConvertIdCompletedEventHandler ConvertIdCompleted;

		public event CreateFolderCompletedEventHandler CreateFolderCompleted;

		public event CreateFolderPathCompletedEventHandler CreateFolderPathCompleted;

		public event DeleteFolderCompletedEventHandler DeleteFolderCompleted;

		public event EmptyFolderCompletedEventHandler EmptyFolderCompleted;

		public event UpdateFolderCompletedEventHandler UpdateFolderCompleted;

		public event MoveFolderCompletedEventHandler MoveFolderCompleted;

		public event CopyFolderCompletedEventHandler CopyFolderCompleted;

		public event SubscribeCompletedEventHandler SubscribeCompleted;

		public event UnsubscribeCompletedEventHandler UnsubscribeCompleted;

		public event GetEventsCompletedEventHandler GetEventsCompleted;

		public event GetStreamingEventsCompletedEventHandler GetStreamingEventsCompleted;

		public event SyncFolderHierarchyCompletedEventHandler SyncFolderHierarchyCompleted;

		public event SyncFolderItemsCompletedEventHandler SyncFolderItemsCompleted;

		public event CreateManagedFolderCompletedEventHandler CreateManagedFolderCompleted;

		public event GetItemCompletedEventHandler GetItemCompleted;

		public event CreateItemCompletedEventHandler CreateItemCompleted;

		public event DeleteItemCompletedEventHandler DeleteItemCompleted;

		public event UpdateItemCompletedEventHandler UpdateItemCompleted;

		public event UpdateItemInRecoverableItemsCompletedEventHandler UpdateItemInRecoverableItemsCompleted;

		public event SendItemCompletedEventHandler SendItemCompleted;

		public event MoveItemCompletedEventHandler MoveItemCompleted;

		public event CopyItemCompletedEventHandler CopyItemCompleted;

		public event ArchiveItemCompletedEventHandler ArchiveItemCompleted;

		public event CreateAttachmentCompletedEventHandler CreateAttachmentCompleted;

		public event DeleteAttachmentCompletedEventHandler DeleteAttachmentCompleted;

		public event GetAttachmentCompletedEventHandler GetAttachmentCompleted;

		public event GetClientAccessTokenCompletedEventHandler GetClientAccessTokenCompleted;

		public event GetDelegateCompletedEventHandler GetDelegateCompleted;

		public event AddDelegateCompletedEventHandler AddDelegateCompleted;

		public event RemoveDelegateCompletedEventHandler RemoveDelegateCompleted;

		public event UpdateDelegateCompletedEventHandler UpdateDelegateCompleted;

		public event CreateUserConfigurationCompletedEventHandler CreateUserConfigurationCompleted;

		public event DeleteUserConfigurationCompletedEventHandler DeleteUserConfigurationCompleted;

		public event GetUserConfigurationCompletedEventHandler GetUserConfigurationCompleted;

		public event UpdateUserConfigurationCompletedEventHandler UpdateUserConfigurationCompleted;

		public event GetUserAvailabilityCompletedEventHandler GetUserAvailabilityCompleted;

		public event GetUserOofSettingsCompletedEventHandler GetUserOofSettingsCompleted;

		public event SetUserOofSettingsCompletedEventHandler SetUserOofSettingsCompleted;

		public event GetServiceConfigurationCompletedEventHandler GetServiceConfigurationCompleted;

		public event GetMailTipsCompletedEventHandler GetMailTipsCompleted;

		public event PlayOnPhoneCompletedEventHandler PlayOnPhoneCompleted;

		public event GetPhoneCallInformationCompletedEventHandler GetPhoneCallInformationCompleted;

		public event DisconnectPhoneCallCompletedEventHandler DisconnectPhoneCallCompleted;

		public event GetSharingMetadataCompletedEventHandler GetSharingMetadataCompleted;

		public event RefreshSharingFolderCompletedEventHandler RefreshSharingFolderCompleted;

		public event GetSharingFolderCompletedEventHandler GetSharingFolderCompleted;

		public event SetTeamMailboxCompletedEventHandler SetTeamMailboxCompleted;

		public event UnpinTeamMailboxCompletedEventHandler UnpinTeamMailboxCompleted;

		public event GetRoomListsCompletedEventHandler GetRoomListsCompleted;

		public event GetRoomsCompletedEventHandler GetRoomsCompleted;

		public event FindMessageTrackingReportCompletedEventHandler FindMessageTrackingReportCompleted;

		public event GetMessageTrackingReportCompletedEventHandler GetMessageTrackingReportCompleted;

		public event FindConversationCompletedEventHandler FindConversationCompleted;

		public event ApplyConversationActionCompletedEventHandler ApplyConversationActionCompleted;

		public event GetConversationItemsCompletedEventHandler GetConversationItemsCompleted;

		public event FindPeopleCompletedEventHandler FindPeopleCompleted;

		public event GetPersonaCompletedEventHandler GetPersonaCompleted;

		public event GetInboxRulesCompletedEventHandler GetInboxRulesCompleted;

		public event UpdateInboxRulesCompletedEventHandler UpdateInboxRulesCompleted;

		public event GetPasswordExpirationDateCompletedEventHandler GetPasswordExpirationDateCompleted;

		public event GetSearchableMailboxesCompletedEventHandler GetSearchableMailboxesCompleted;

		public event SearchMailboxesCompletedEventHandler SearchMailboxesCompleted;

		public event GetDiscoverySearchConfigurationCompletedEventHandler GetDiscoverySearchConfigurationCompleted;

		public event GetHoldOnMailboxesCompletedEventHandler GetHoldOnMailboxesCompleted;

		public event SetHoldOnMailboxesCompletedEventHandler SetHoldOnMailboxesCompleted;

		public event GetNonIndexableItemStatisticsCompletedEventHandler GetNonIndexableItemStatisticsCompleted;

		public event GetNonIndexableItemDetailsCompletedEventHandler GetNonIndexableItemDetailsCompleted;

		public event MarkAllItemsAsReadCompletedEventHandler MarkAllItemsAsReadCompleted;

		public event MarkAsJunkCompletedEventHandler MarkAsJunkCompleted;

		public event GetAppManifestsCompletedEventHandler GetAppManifestsCompleted;

		public event AddNewImContactToGroupCompletedEventHandler AddNewImContactToGroupCompleted;

		public event AddNewTelUriContactToGroupCompletedEventHandler AddNewTelUriContactToGroupCompleted;

		public event AddImContactToGroupCompletedEventHandler AddImContactToGroupCompleted;

		public event RemoveImContactFromGroupCompletedEventHandler RemoveImContactFromGroupCompleted;

		public event AddImGroupCompletedEventHandler AddImGroupCompleted;

		public event AddDistributionGroupToImListCompletedEventHandler AddDistributionGroupToImListCompleted;

		public event GetImItemListCompletedEventHandler GetImItemListCompleted;

		public event GetImItemsCompletedEventHandler GetImItemsCompleted;

		public event RemoveContactFromImListCompletedEventHandler RemoveContactFromImListCompleted;

		public event RemoveDistributionGroupFromImListCompletedEventHandler RemoveDistributionGroupFromImListCompleted;

		public event RemoveImGroupCompletedEventHandler RemoveImGroupCompleted;

		public event SetImGroupCompletedEventHandler SetImGroupCompleted;

		public event SetImListMigrationCompletedCompletedEventHandler SetImListMigrationCompletedCompleted;

		public event GetUserRetentionPolicyTagsCompletedEventHandler GetUserRetentionPolicyTagsCompleted;

		public event InstallAppCompletedEventHandler InstallAppCompleted;

		public event UninstallAppCompletedEventHandler UninstallAppCompleted;

		public event DisableAppCompletedEventHandler DisableAppCompleted;

		public event GetAppMarketplaceUrlCompletedEventHandler GetAppMarketplaceUrlCompleted;

		public event GetUserPhotoCompletedEventHandler GetUserPhotoCompleted;

		[SoapHeader("MailboxCulture")]
		[SoapHeader("ServerVersionInfoValue", Direction = SoapHeaderDirection.Out)]
		[SoapHeader("RequestServerVersionValue")]
		[SoapDocumentMethod("http://schemas.microsoft.com/exchange/services/2006/messages/ResolveNames", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Bare)]
		[SoapHeader("ExchangeImpersonation")]
		[SoapHttpClientTraceExtension]
		[return: XmlElement("ResolveNamesResponse", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public ResolveNamesResponseType ResolveNames([XmlElement("ResolveNames", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")] ResolveNamesType ResolveNames1)
		{
			object[] array = this.Invoke("ResolveNames", new object[]
			{
				ResolveNames1
			});
			return (ResolveNamesResponseType)array[0];
		}

		public IAsyncResult BeginResolveNames(ResolveNamesType ResolveNames1, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("ResolveNames", new object[]
			{
				ResolveNames1
			}, callback, asyncState);
		}

		public ResolveNamesResponseType EndResolveNames(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (ResolveNamesResponseType)array[0];
		}

		public void ResolveNamesAsync(ResolveNamesType ResolveNames1)
		{
			this.ResolveNamesAsync(ResolveNames1, null);
		}

		public void ResolveNamesAsync(ResolveNamesType ResolveNames1, object userState)
		{
			if (this.ResolveNamesOperationCompleted == null)
			{
				this.ResolveNamesOperationCompleted = new SendOrPostCallback(this.OnResolveNamesOperationCompleted);
			}
			base.InvokeAsync("ResolveNames", new object[]
			{
				ResolveNames1
			}, this.ResolveNamesOperationCompleted, userState);
		}

		private void OnResolveNamesOperationCompleted(object arg)
		{
			if (this.ResolveNamesCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.ResolveNamesCompleted(this, new ResolveNamesCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapHeader("ServerVersionInfoValue", Direction = SoapHeaderDirection.Out)]
		[SoapHeader("ExchangeImpersonation")]
		[SoapDocumentMethod("http://schemas.microsoft.com/exchange/services/2006/messages/ExpandDL", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Bare)]
		[SoapHeader("MailboxCulture")]
		[SoapHttpClientTraceExtension]
		[SoapHeader("RequestServerVersionValue")]
		[return: XmlElement("ExpandDLResponse", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public ExpandDLResponseType ExpandDL([XmlElement("ExpandDL", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")] ExpandDLType ExpandDL1)
		{
			object[] array = this.Invoke("ExpandDL", new object[]
			{
				ExpandDL1
			});
			return (ExpandDLResponseType)array[0];
		}

		public IAsyncResult BeginExpandDL(ExpandDLType ExpandDL1, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("ExpandDL", new object[]
			{
				ExpandDL1
			}, callback, asyncState);
		}

		public ExpandDLResponseType EndExpandDL(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (ExpandDLResponseType)array[0];
		}

		public void ExpandDLAsync(ExpandDLType ExpandDL1)
		{
			this.ExpandDLAsync(ExpandDL1, null);
		}

		public void ExpandDLAsync(ExpandDLType ExpandDL1, object userState)
		{
			if (this.ExpandDLOperationCompleted == null)
			{
				this.ExpandDLOperationCompleted = new SendOrPostCallback(this.OnExpandDLOperationCompleted);
			}
			base.InvokeAsync("ExpandDL", new object[]
			{
				ExpandDL1
			}, this.ExpandDLOperationCompleted, userState);
		}

		private void OnExpandDLOperationCompleted(object arg)
		{
			if (this.ExpandDLCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.ExpandDLCompleted(this, new ExpandDLCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://schemas.microsoft.com/exchange/services/2006/messages/GetServerTimeZones", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Bare)]
		[SoapHeader("RequestServerVersionValue")]
		[SoapHttpClientTraceExtension]
		[SoapHeader("ServerVersionInfoValue", Direction = SoapHeaderDirection.Out)]
		[SoapHeader("MailboxCulture")]
		[return: XmlElement("GetServerTimeZonesResponse", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public GetServerTimeZonesResponseType GetServerTimeZones([XmlElement("GetServerTimeZones", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")] GetServerTimeZonesType GetServerTimeZones1)
		{
			object[] array = this.Invoke("GetServerTimeZones", new object[]
			{
				GetServerTimeZones1
			});
			return (GetServerTimeZonesResponseType)array[0];
		}

		public IAsyncResult BeginGetServerTimeZones(GetServerTimeZonesType GetServerTimeZones1, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("GetServerTimeZones", new object[]
			{
				GetServerTimeZones1
			}, callback, asyncState);
		}

		public GetServerTimeZonesResponseType EndGetServerTimeZones(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (GetServerTimeZonesResponseType)array[0];
		}

		public void GetServerTimeZonesAsync(GetServerTimeZonesType GetServerTimeZones1)
		{
			this.GetServerTimeZonesAsync(GetServerTimeZones1, null);
		}

		public void GetServerTimeZonesAsync(GetServerTimeZonesType GetServerTimeZones1, object userState)
		{
			if (this.GetServerTimeZonesOperationCompleted == null)
			{
				this.GetServerTimeZonesOperationCompleted = new SendOrPostCallback(this.OnGetServerTimeZonesOperationCompleted);
			}
			base.InvokeAsync("GetServerTimeZones", new object[]
			{
				GetServerTimeZones1
			}, this.GetServerTimeZonesOperationCompleted, userState);
		}

		private void OnGetServerTimeZonesOperationCompleted(object arg)
		{
			if (this.GetServerTimeZonesCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.GetServerTimeZonesCompleted(this, new GetServerTimeZonesCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapHeader("MailboxCulture")]
		[SoapHeader("RequestServerVersionValue")]
		[SoapHeader("ExchangeImpersonation")]
		[SoapHeader("ManagementRole")]
		[SoapHeader("TimeZoneContext")]
		[SoapHttpClientTraceExtension]
		[SoapHeader("ServerVersionInfoValue", Direction = SoapHeaderDirection.Out)]
		[SoapDocumentMethod("http://schemas.microsoft.com/exchange/services/2006/messages/FindFolder", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Bare)]
		[return: XmlElement("FindFolderResponse", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public FindFolderResponseType FindFolder([XmlElement("FindFolder", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")] FindFolderType FindFolder1)
		{
			object[] array = this.Invoke("FindFolder", new object[]
			{
				FindFolder1
			});
			return (FindFolderResponseType)array[0];
		}

		public IAsyncResult BeginFindFolder(FindFolderType FindFolder1, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("FindFolder", new object[]
			{
				FindFolder1
			}, callback, asyncState);
		}

		public FindFolderResponseType EndFindFolder(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (FindFolderResponseType)array[0];
		}

		public void FindFolderAsync(FindFolderType FindFolder1)
		{
			this.FindFolderAsync(FindFolder1, null);
		}

		public void FindFolderAsync(FindFolderType FindFolder1, object userState)
		{
			if (this.FindFolderOperationCompleted == null)
			{
				this.FindFolderOperationCompleted = new SendOrPostCallback(this.OnFindFolderOperationCompleted);
			}
			base.InvokeAsync("FindFolder", new object[]
			{
				FindFolder1
			}, this.FindFolderOperationCompleted, userState);
		}

		private void OnFindFolderOperationCompleted(object arg)
		{
			if (this.FindFolderCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.FindFolderCompleted(this, new FindFolderCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapHeader("ManagementRole")]
		[SoapHeader("TimeZoneContext")]
		[SoapHeader("ServerVersionInfoValue", Direction = SoapHeaderDirection.Out)]
		[SoapHeader("RequestServerVersionValue")]
		[SoapHeader("MailboxCulture")]
		[SoapDocumentMethod("http://schemas.microsoft.com/exchange/services/2006/messages/FindItem", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Bare)]
		[SoapHttpClientTraceExtension]
		[SoapHeader("DateTimePrecision")]
		[SoapHeader("ExchangeImpersonation")]
		[return: XmlElement("FindItemResponse", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public FindItemResponseType FindItem([XmlElement("FindItem", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")] FindItemType FindItem1)
		{
			object[] array = this.Invoke("FindItem", new object[]
			{
				FindItem1
			});
			return (FindItemResponseType)array[0];
		}

		public IAsyncResult BeginFindItem(FindItemType FindItem1, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("FindItem", new object[]
			{
				FindItem1
			}, callback, asyncState);
		}

		public FindItemResponseType EndFindItem(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (FindItemResponseType)array[0];
		}

		public void FindItemAsync(FindItemType FindItem1)
		{
			this.FindItemAsync(FindItem1, null);
		}

		public void FindItemAsync(FindItemType FindItem1, object userState)
		{
			if (this.FindItemOperationCompleted == null)
			{
				this.FindItemOperationCompleted = new SendOrPostCallback(this.OnFindItemOperationCompleted);
			}
			base.InvokeAsync("FindItem", new object[]
			{
				FindItem1
			}, this.FindItemOperationCompleted, userState);
		}

		private void OnFindItemOperationCompleted(object arg)
		{
			if (this.FindItemCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.FindItemCompleted(this, new FindItemCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapHeader("ManagementRole")]
		[SoapDocumentMethod("http://schemas.microsoft.com/exchange/services/2006/messages/GetFolder", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Bare)]
		[SoapHttpClientTraceExtension]
		[SoapHeader("TimeZoneContext")]
		[SoapHeader("ExchangeImpersonation")]
		[SoapHeader("MailboxCulture")]
		[SoapHeader("ServerVersionInfoValue", Direction = SoapHeaderDirection.Out)]
		[SoapHeader("RequestServerVersionValue")]
		[return: XmlElement("GetFolderResponse", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public GetFolderResponseType GetFolder([XmlElement("GetFolder", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")] GetFolderType GetFolder1)
		{
			object[] array = this.Invoke("GetFolder", new object[]
			{
				GetFolder1
			});
			return (GetFolderResponseType)array[0];
		}

		public IAsyncResult BeginGetFolder(GetFolderType GetFolder1, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("GetFolder", new object[]
			{
				GetFolder1
			}, callback, asyncState);
		}

		public GetFolderResponseType EndGetFolder(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (GetFolderResponseType)array[0];
		}

		public void GetFolderAsync(GetFolderType GetFolder1)
		{
			this.GetFolderAsync(GetFolder1, null);
		}

		public void GetFolderAsync(GetFolderType GetFolder1, object userState)
		{
			if (this.GetFolderOperationCompleted == null)
			{
				this.GetFolderOperationCompleted = new SendOrPostCallback(this.OnGetFolderOperationCompleted);
			}
			base.InvokeAsync("GetFolder", new object[]
			{
				GetFolder1
			}, this.GetFolderOperationCompleted, userState);
		}

		private void OnGetFolderOperationCompleted(object arg)
		{
			if (this.GetFolderCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.GetFolderCompleted(this, new GetFolderCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapHeader("MailboxCulture")]
		[SoapHeader("ServerVersionInfoValue", Direction = SoapHeaderDirection.Out)]
		[SoapHttpClientTraceExtension]
		[SoapHeader("RequestServerVersionValue")]
		[SoapHeader("ExchangeImpersonation")]
		[SoapDocumentMethod("http://schemas.microsoft.com/exchange/services/2006/messages/UploadItems", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Bare)]
		[return: XmlElement("UploadItemsResponse", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public UploadItemsResponseType UploadItems([XmlElement("UploadItems", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")] UploadItemsType UploadItems1)
		{
			object[] array = this.Invoke("UploadItems", new object[]
			{
				UploadItems1
			});
			return (UploadItemsResponseType)array[0];
		}

		public IAsyncResult BeginUploadItems(UploadItemsType UploadItems1, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("UploadItems", new object[]
			{
				UploadItems1
			}, callback, asyncState);
		}

		public UploadItemsResponseType EndUploadItems(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (UploadItemsResponseType)array[0];
		}

		public void UploadItemsAsync(UploadItemsType UploadItems1)
		{
			this.UploadItemsAsync(UploadItems1, null);
		}

		public void UploadItemsAsync(UploadItemsType UploadItems1, object userState)
		{
			if (this.UploadItemsOperationCompleted == null)
			{
				this.UploadItemsOperationCompleted = new SendOrPostCallback(this.OnUploadItemsOperationCompleted);
			}
			base.InvokeAsync("UploadItems", new object[]
			{
				UploadItems1
			}, this.UploadItemsOperationCompleted, userState);
		}

		private void OnUploadItemsOperationCompleted(object arg)
		{
			if (this.UploadItemsCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.UploadItemsCompleted(this, new UploadItemsCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapHttpClientTraceExtension]
		[SoapHeader("RequestServerVersionValue")]
		[SoapHeader("ManagementRole")]
		[SoapHeader("MailboxCulture")]
		[SoapHeader("ServerVersionInfoValue", Direction = SoapHeaderDirection.Out)]
		[SoapHeader("ExchangeImpersonation")]
		[SoapDocumentMethod("http://schemas.microsoft.com/exchange/services/2006/messages/ExportItems", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Bare)]
		[return: XmlElement("ExportItemsResponse", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public ExportItemsResponseType ExportItems([XmlElement("ExportItems", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")] ExportItemsType ExportItems1)
		{
			object[] array = this.Invoke("ExportItems", new object[]
			{
				ExportItems1
			});
			return (ExportItemsResponseType)array[0];
		}

		public IAsyncResult BeginExportItems(ExportItemsType ExportItems1, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("ExportItems", new object[]
			{
				ExportItems1
			}, callback, asyncState);
		}

		public ExportItemsResponseType EndExportItems(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (ExportItemsResponseType)array[0];
		}

		public void ExportItemsAsync(ExportItemsType ExportItems1)
		{
			this.ExportItemsAsync(ExportItems1, null);
		}

		public void ExportItemsAsync(ExportItemsType ExportItems1, object userState)
		{
			if (this.ExportItemsOperationCompleted == null)
			{
				this.ExportItemsOperationCompleted = new SendOrPostCallback(this.OnExportItemsOperationCompleted);
			}
			base.InvokeAsync("ExportItems", new object[]
			{
				ExportItems1
			}, this.ExportItemsOperationCompleted, userState);
		}

		private void OnExportItemsOperationCompleted(object arg)
		{
			if (this.ExportItemsCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.ExportItemsCompleted(this, new ExportItemsCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapHeader("ExchangeImpersonation")]
		[SoapHeader("ServerVersionInfoValue", Direction = SoapHeaderDirection.Out)]
		[SoapHeader("RequestServerVersionValue")]
		[SoapHttpClientTraceExtension]
		[SoapDocumentMethod("http://schemas.microsoft.com/exchange/services/2006/messages/ConvertId", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Bare)]
		[return: XmlElement("ConvertIdResponse", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public ConvertIdResponseType ConvertId([XmlElement("ConvertId", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")] ConvertIdType ConvertId1)
		{
			object[] array = this.Invoke("ConvertId", new object[]
			{
				ConvertId1
			});
			return (ConvertIdResponseType)array[0];
		}

		public IAsyncResult BeginConvertId(ConvertIdType ConvertId1, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("ConvertId", new object[]
			{
				ConvertId1
			}, callback, asyncState);
		}

		public ConvertIdResponseType EndConvertId(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (ConvertIdResponseType)array[0];
		}

		public void ConvertIdAsync(ConvertIdType ConvertId1)
		{
			this.ConvertIdAsync(ConvertId1, null);
		}

		public void ConvertIdAsync(ConvertIdType ConvertId1, object userState)
		{
			if (this.ConvertIdOperationCompleted == null)
			{
				this.ConvertIdOperationCompleted = new SendOrPostCallback(this.OnConvertIdOperationCompleted);
			}
			base.InvokeAsync("ConvertId", new object[]
			{
				ConvertId1
			}, this.ConvertIdOperationCompleted, userState);
		}

		private void OnConvertIdOperationCompleted(object arg)
		{
			if (this.ConvertIdCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.ConvertIdCompleted(this, new ConvertIdCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapHttpClientTraceExtension]
		[SoapHeader("ExchangeImpersonation")]
		[SoapHeader("TimeZoneContext")]
		[SoapHeader("RequestServerVersionValue")]
		[SoapHeader("MailboxCulture")]
		[SoapDocumentMethod("http://schemas.microsoft.com/exchange/services/2006/messages/CreateFolder", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Bare)]
		[SoapHeader("ServerVersionInfoValue", Direction = SoapHeaderDirection.Out)]
		[return: XmlElement("CreateFolderResponse", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public CreateFolderResponseType CreateFolder([XmlElement("CreateFolder", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")] CreateFolderType CreateFolder1)
		{
			object[] array = this.Invoke("CreateFolder", new object[]
			{
				CreateFolder1
			});
			return (CreateFolderResponseType)array[0];
		}

		public IAsyncResult BeginCreateFolder(CreateFolderType CreateFolder1, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("CreateFolder", new object[]
			{
				CreateFolder1
			}, callback, asyncState);
		}

		public CreateFolderResponseType EndCreateFolder(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (CreateFolderResponseType)array[0];
		}

		public void CreateFolderAsync(CreateFolderType CreateFolder1)
		{
			this.CreateFolderAsync(CreateFolder1, null);
		}

		public void CreateFolderAsync(CreateFolderType CreateFolder1, object userState)
		{
			if (this.CreateFolderOperationCompleted == null)
			{
				this.CreateFolderOperationCompleted = new SendOrPostCallback(this.OnCreateFolderOperationCompleted);
			}
			base.InvokeAsync("CreateFolder", new object[]
			{
				CreateFolder1
			}, this.CreateFolderOperationCompleted, userState);
		}

		private void OnCreateFolderOperationCompleted(object arg)
		{
			if (this.CreateFolderCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.CreateFolderCompleted(this, new CreateFolderCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapHeader("ExchangeImpersonation")]
		[SoapHeader("ServerVersionInfoValue", Direction = SoapHeaderDirection.Out)]
		[SoapDocumentMethod("http://schemas.microsoft.com/exchange/services/2006/messages/CreateFolderPath", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Bare)]
		[SoapHttpClientTraceExtension]
		[SoapHeader("RequestServerVersionValue")]
		[SoapHeader("TimeZoneContext")]
		[SoapHeader("MailboxCulture")]
		[return: XmlElement("CreateFolderPathResponse", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public CreateFolderPathResponseType CreateFolderPath([XmlElement("CreateFolderPath", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")] CreateFolderPathType CreateFolderPath1)
		{
			object[] array = this.Invoke("CreateFolderPath", new object[]
			{
				CreateFolderPath1
			});
			return (CreateFolderPathResponseType)array[0];
		}

		public IAsyncResult BeginCreateFolderPath(CreateFolderPathType CreateFolderPath1, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("CreateFolderPath", new object[]
			{
				CreateFolderPath1
			}, callback, asyncState);
		}

		public CreateFolderPathResponseType EndCreateFolderPath(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (CreateFolderPathResponseType)array[0];
		}

		public void CreateFolderPathAsync(CreateFolderPathType CreateFolderPath1)
		{
			this.CreateFolderPathAsync(CreateFolderPath1, null);
		}

		public void CreateFolderPathAsync(CreateFolderPathType CreateFolderPath1, object userState)
		{
			if (this.CreateFolderPathOperationCompleted == null)
			{
				this.CreateFolderPathOperationCompleted = new SendOrPostCallback(this.OnCreateFolderPathOperationCompleted);
			}
			base.InvokeAsync("CreateFolderPath", new object[]
			{
				CreateFolderPath1
			}, this.CreateFolderPathOperationCompleted, userState);
		}

		private void OnCreateFolderPathOperationCompleted(object arg)
		{
			if (this.CreateFolderPathCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.CreateFolderPathCompleted(this, new CreateFolderPathCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://schemas.microsoft.com/exchange/services/2006/messages/DeleteFolder", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Bare)]
		[SoapHeader("ExchangeImpersonation")]
		[SoapHeader("ServerVersionInfoValue", Direction = SoapHeaderDirection.Out)]
		[SoapHeader("MailboxCulture")]
		[SoapHeader("RequestServerVersionValue")]
		[SoapHttpClientTraceExtension]
		[return: XmlElement("DeleteFolderResponse", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public DeleteFolderResponseType DeleteFolder([XmlElement("DeleteFolder", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")] DeleteFolderType DeleteFolder1)
		{
			object[] array = this.Invoke("DeleteFolder", new object[]
			{
				DeleteFolder1
			});
			return (DeleteFolderResponseType)array[0];
		}

		public IAsyncResult BeginDeleteFolder(DeleteFolderType DeleteFolder1, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("DeleteFolder", new object[]
			{
				DeleteFolder1
			}, callback, asyncState);
		}

		public DeleteFolderResponseType EndDeleteFolder(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (DeleteFolderResponseType)array[0];
		}

		public void DeleteFolderAsync(DeleteFolderType DeleteFolder1)
		{
			this.DeleteFolderAsync(DeleteFolder1, null);
		}

		public void DeleteFolderAsync(DeleteFolderType DeleteFolder1, object userState)
		{
			if (this.DeleteFolderOperationCompleted == null)
			{
				this.DeleteFolderOperationCompleted = new SendOrPostCallback(this.OnDeleteFolderOperationCompleted);
			}
			base.InvokeAsync("DeleteFolder", new object[]
			{
				DeleteFolder1
			}, this.DeleteFolderOperationCompleted, userState);
		}

		private void OnDeleteFolderOperationCompleted(object arg)
		{
			if (this.DeleteFolderCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.DeleteFolderCompleted(this, new DeleteFolderCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapHeader("MailboxCulture")]
		[SoapHeader("ServerVersionInfoValue", Direction = SoapHeaderDirection.Out)]
		[SoapDocumentMethod("http://schemas.microsoft.com/exchange/services/2006/messages/EmptyFolder", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Bare)]
		[SoapHeader("ExchangeImpersonation")]
		[SoapHttpClientTraceExtension]
		[SoapHeader("RequestServerVersionValue")]
		[return: XmlElement("EmptyFolderResponse", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public EmptyFolderResponseType EmptyFolder([XmlElement("EmptyFolder", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")] EmptyFolderType EmptyFolder1)
		{
			object[] array = this.Invoke("EmptyFolder", new object[]
			{
				EmptyFolder1
			});
			return (EmptyFolderResponseType)array[0];
		}

		public IAsyncResult BeginEmptyFolder(EmptyFolderType EmptyFolder1, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("EmptyFolder", new object[]
			{
				EmptyFolder1
			}, callback, asyncState);
		}

		public EmptyFolderResponseType EndEmptyFolder(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (EmptyFolderResponseType)array[0];
		}

		public void EmptyFolderAsync(EmptyFolderType EmptyFolder1)
		{
			this.EmptyFolderAsync(EmptyFolder1, null);
		}

		public void EmptyFolderAsync(EmptyFolderType EmptyFolder1, object userState)
		{
			if (this.EmptyFolderOperationCompleted == null)
			{
				this.EmptyFolderOperationCompleted = new SendOrPostCallback(this.OnEmptyFolderOperationCompleted);
			}
			base.InvokeAsync("EmptyFolder", new object[]
			{
				EmptyFolder1
			}, this.EmptyFolderOperationCompleted, userState);
		}

		private void OnEmptyFolderOperationCompleted(object arg)
		{
			if (this.EmptyFolderCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.EmptyFolderCompleted(this, new EmptyFolderCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapHeader("TimeZoneContext")]
		[SoapHeader("MailboxCulture")]
		[SoapHeader("ExchangeImpersonation")]
		[SoapDocumentMethod("http://schemas.microsoft.com/exchange/services/2006/messages/UpdateFolder", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Bare)]
		[SoapHeader("RequestServerVersionValue")]
		[SoapHeader("ServerVersionInfoValue", Direction = SoapHeaderDirection.Out)]
		[SoapHttpClientTraceExtension]
		[return: XmlElement("UpdateFolderResponse", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public UpdateFolderResponseType UpdateFolder([XmlElement("UpdateFolder", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")] UpdateFolderType UpdateFolder1)
		{
			object[] array = this.Invoke("UpdateFolder", new object[]
			{
				UpdateFolder1
			});
			return (UpdateFolderResponseType)array[0];
		}

		public IAsyncResult BeginUpdateFolder(UpdateFolderType UpdateFolder1, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("UpdateFolder", new object[]
			{
				UpdateFolder1
			}, callback, asyncState);
		}

		public UpdateFolderResponseType EndUpdateFolder(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (UpdateFolderResponseType)array[0];
		}

		public void UpdateFolderAsync(UpdateFolderType UpdateFolder1)
		{
			this.UpdateFolderAsync(UpdateFolder1, null);
		}

		public void UpdateFolderAsync(UpdateFolderType UpdateFolder1, object userState)
		{
			if (this.UpdateFolderOperationCompleted == null)
			{
				this.UpdateFolderOperationCompleted = new SendOrPostCallback(this.OnUpdateFolderOperationCompleted);
			}
			base.InvokeAsync("UpdateFolder", new object[]
			{
				UpdateFolder1
			}, this.UpdateFolderOperationCompleted, userState);
		}

		private void OnUpdateFolderOperationCompleted(object arg)
		{
			if (this.UpdateFolderCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.UpdateFolderCompleted(this, new UpdateFolderCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapHttpClientTraceExtension]
		[SoapHeader("RequestServerVersionValue")]
		[SoapHeader("ServerVersionInfoValue", Direction = SoapHeaderDirection.Out)]
		[SoapHeader("ExchangeImpersonation")]
		[SoapHeader("MailboxCulture")]
		[SoapDocumentMethod("http://schemas.microsoft.com/exchange/services/2006/messages/MoveFolder", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Bare)]
		[return: XmlElement("MoveFolderResponse", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public MoveFolderResponseType MoveFolder([XmlElement("MoveFolder", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")] MoveFolderType MoveFolder1)
		{
			object[] array = this.Invoke("MoveFolder", new object[]
			{
				MoveFolder1
			});
			return (MoveFolderResponseType)array[0];
		}

		public IAsyncResult BeginMoveFolder(MoveFolderType MoveFolder1, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("MoveFolder", new object[]
			{
				MoveFolder1
			}, callback, asyncState);
		}

		public MoveFolderResponseType EndMoveFolder(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (MoveFolderResponseType)array[0];
		}

		public void MoveFolderAsync(MoveFolderType MoveFolder1)
		{
			this.MoveFolderAsync(MoveFolder1, null);
		}

		public void MoveFolderAsync(MoveFolderType MoveFolder1, object userState)
		{
			if (this.MoveFolderOperationCompleted == null)
			{
				this.MoveFolderOperationCompleted = new SendOrPostCallback(this.OnMoveFolderOperationCompleted);
			}
			base.InvokeAsync("MoveFolder", new object[]
			{
				MoveFolder1
			}, this.MoveFolderOperationCompleted, userState);
		}

		private void OnMoveFolderOperationCompleted(object arg)
		{
			if (this.MoveFolderCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.MoveFolderCompleted(this, new MoveFolderCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://schemas.microsoft.com/exchange/services/2006/messages/CopyFolder", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Bare)]
		[SoapHeader("ExchangeImpersonation")]
		[SoapHeader("ServerVersionInfoValue", Direction = SoapHeaderDirection.Out)]
		[SoapHeader("RequestServerVersionValue")]
		[SoapHeader("MailboxCulture")]
		[SoapHttpClientTraceExtension]
		[return: XmlElement("CopyFolderResponse", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public CopyFolderResponseType CopyFolder([XmlElement("CopyFolder", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")] CopyFolderType CopyFolder1)
		{
			object[] array = this.Invoke("CopyFolder", new object[]
			{
				CopyFolder1
			});
			return (CopyFolderResponseType)array[0];
		}

		public IAsyncResult BeginCopyFolder(CopyFolderType CopyFolder1, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("CopyFolder", new object[]
			{
				CopyFolder1
			}, callback, asyncState);
		}

		public CopyFolderResponseType EndCopyFolder(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (CopyFolderResponseType)array[0];
		}

		public void CopyFolderAsync(CopyFolderType CopyFolder1)
		{
			this.CopyFolderAsync(CopyFolder1, null);
		}

		public void CopyFolderAsync(CopyFolderType CopyFolder1, object userState)
		{
			if (this.CopyFolderOperationCompleted == null)
			{
				this.CopyFolderOperationCompleted = new SendOrPostCallback(this.OnCopyFolderOperationCompleted);
			}
			base.InvokeAsync("CopyFolder", new object[]
			{
				CopyFolder1
			}, this.CopyFolderOperationCompleted, userState);
		}

		private void OnCopyFolderOperationCompleted(object arg)
		{
			if (this.CopyFolderCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.CopyFolderCompleted(this, new CopyFolderCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapHeader("ServerVersionInfoValue", Direction = SoapHeaderDirection.Out)]
		[SoapHttpClientTraceExtension]
		[SoapHeader("MailboxCulture")]
		[SoapDocumentMethod("http://schemas.microsoft.com/exchange/services/2006/messages/Subscribe", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Bare)]
		[SoapHeader("ExchangeImpersonation")]
		[SoapHeader("RequestServerVersionValue")]
		[return: XmlElement("SubscribeResponse", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public SubscribeResponseType Subscribe([XmlElement("Subscribe", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")] SubscribeType Subscribe1)
		{
			object[] array = this.Invoke("Subscribe", new object[]
			{
				Subscribe1
			});
			return (SubscribeResponseType)array[0];
		}

		public IAsyncResult BeginSubscribe(SubscribeType Subscribe1, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("Subscribe", new object[]
			{
				Subscribe1
			}, callback, asyncState);
		}

		public SubscribeResponseType EndSubscribe(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (SubscribeResponseType)array[0];
		}

		public void SubscribeAsync(SubscribeType Subscribe1)
		{
			this.SubscribeAsync(Subscribe1, null);
		}

		public void SubscribeAsync(SubscribeType Subscribe1, object userState)
		{
			if (this.SubscribeOperationCompleted == null)
			{
				this.SubscribeOperationCompleted = new SendOrPostCallback(this.OnSubscribeOperationCompleted);
			}
			base.InvokeAsync("Subscribe", new object[]
			{
				Subscribe1
			}, this.SubscribeOperationCompleted, userState);
		}

		private void OnSubscribeOperationCompleted(object arg)
		{
			if (this.SubscribeCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.SubscribeCompleted(this, new SubscribeCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapHeader("ExchangeImpersonation")]
		[SoapHeader("ServerVersionInfoValue", Direction = SoapHeaderDirection.Out)]
		[SoapHeader("RequestServerVersionValue")]
		[SoapHttpClientTraceExtension]
		[SoapHeader("MailboxCulture")]
		[SoapDocumentMethod("http://schemas.microsoft.com/exchange/services/2006/messages/Unsubscribe", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Bare)]
		[return: XmlElement("UnsubscribeResponse", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public UnsubscribeResponseType Unsubscribe([XmlElement("Unsubscribe", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")] UnsubscribeType Unsubscribe1)
		{
			object[] array = this.Invoke("Unsubscribe", new object[]
			{
				Unsubscribe1
			});
			return (UnsubscribeResponseType)array[0];
		}

		public IAsyncResult BeginUnsubscribe(UnsubscribeType Unsubscribe1, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("Unsubscribe", new object[]
			{
				Unsubscribe1
			}, callback, asyncState);
		}

		public UnsubscribeResponseType EndUnsubscribe(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (UnsubscribeResponseType)array[0];
		}

		public void UnsubscribeAsync(UnsubscribeType Unsubscribe1)
		{
			this.UnsubscribeAsync(Unsubscribe1, null);
		}

		public void UnsubscribeAsync(UnsubscribeType Unsubscribe1, object userState)
		{
			if (this.UnsubscribeOperationCompleted == null)
			{
				this.UnsubscribeOperationCompleted = new SendOrPostCallback(this.OnUnsubscribeOperationCompleted);
			}
			base.InvokeAsync("Unsubscribe", new object[]
			{
				Unsubscribe1
			}, this.UnsubscribeOperationCompleted, userState);
		}

		private void OnUnsubscribeOperationCompleted(object arg)
		{
			if (this.UnsubscribeCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.UnsubscribeCompleted(this, new UnsubscribeCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://schemas.microsoft.com/exchange/services/2006/messages/GetEvents", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Bare)]
		[SoapHttpClientTraceExtension]
		[SoapHeader("MailboxCulture")]
		[SoapHeader("ServerVersionInfoValue", Direction = SoapHeaderDirection.Out)]
		[SoapHeader("ExchangeImpersonation")]
		[SoapHeader("RequestServerVersionValue")]
		[return: XmlElement("GetEventsResponse", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public GetEventsResponseType GetEvents([XmlElement("GetEvents", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")] GetEventsType GetEvents1)
		{
			object[] array = this.Invoke("GetEvents", new object[]
			{
				GetEvents1
			});
			return (GetEventsResponseType)array[0];
		}

		public IAsyncResult BeginGetEvents(GetEventsType GetEvents1, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("GetEvents", new object[]
			{
				GetEvents1
			}, callback, asyncState);
		}

		public GetEventsResponseType EndGetEvents(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (GetEventsResponseType)array[0];
		}

		public void GetEventsAsync(GetEventsType GetEvents1)
		{
			this.GetEventsAsync(GetEvents1, null);
		}

		public void GetEventsAsync(GetEventsType GetEvents1, object userState)
		{
			if (this.GetEventsOperationCompleted == null)
			{
				this.GetEventsOperationCompleted = new SendOrPostCallback(this.OnGetEventsOperationCompleted);
			}
			base.InvokeAsync("GetEvents", new object[]
			{
				GetEvents1
			}, this.GetEventsOperationCompleted, userState);
		}

		private void OnGetEventsOperationCompleted(object arg)
		{
			if (this.GetEventsCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.GetEventsCompleted(this, new GetEventsCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapHeader("RequestServerVersionValue")]
		[SoapHeader("MailboxCulture")]
		[SoapDocumentMethod("http://schemas.microsoft.com/exchange/services/2006/messages/GetEvents", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Bare)]
		[SoapHttpClientTraceExtension]
		[SoapHeader("ServerVersionInfoValue", Direction = SoapHeaderDirection.Out)]
		[SoapHeader("ExchangeImpersonation")]
		[return: XmlElement("GetStreamingEventsResponse", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public GetStreamingEventsResponseType GetStreamingEvents([XmlElement("GetStreamingEvents", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")] GetStreamingEventsType GetStreamingEvents1)
		{
			object[] array = this.Invoke("GetStreamingEvents", new object[]
			{
				GetStreamingEvents1
			});
			return (GetStreamingEventsResponseType)array[0];
		}

		public IAsyncResult BeginGetStreamingEvents(GetStreamingEventsType GetStreamingEvents1, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("GetStreamingEvents", new object[]
			{
				GetStreamingEvents1
			}, callback, asyncState);
		}

		public GetStreamingEventsResponseType EndGetStreamingEvents(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (GetStreamingEventsResponseType)array[0];
		}

		public void GetStreamingEventsAsync(GetStreamingEventsType GetStreamingEvents1)
		{
			this.GetStreamingEventsAsync(GetStreamingEvents1, null);
		}

		public void GetStreamingEventsAsync(GetStreamingEventsType GetStreamingEvents1, object userState)
		{
			if (this.GetStreamingEventsOperationCompleted == null)
			{
				this.GetStreamingEventsOperationCompleted = new SendOrPostCallback(this.OnGetStreamingEventsOperationCompleted);
			}
			base.InvokeAsync("GetStreamingEvents", new object[]
			{
				GetStreamingEvents1
			}, this.GetStreamingEventsOperationCompleted, userState);
		}

		private void OnGetStreamingEventsOperationCompleted(object arg)
		{
			if (this.GetStreamingEventsCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.GetStreamingEventsCompleted(this, new GetStreamingEventsCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapHttpClientTraceExtension]
		[SoapHeader("ServerVersionInfoValue", Direction = SoapHeaderDirection.Out)]
		[SoapHeader("RequestServerVersionValue")]
		[SoapHeader("ExchangeImpersonation")]
		[SoapDocumentMethod("http://schemas.microsoft.com/exchange/services/2006/messages/SyncFolderHierarchy", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Bare)]
		[SoapHeader("MailboxCulture")]
		[return: XmlElement("SyncFolderHierarchyResponse", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public SyncFolderHierarchyResponseType SyncFolderHierarchy([XmlElement("SyncFolderHierarchy", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")] SyncFolderHierarchyType SyncFolderHierarchy1)
		{
			object[] array = this.Invoke("SyncFolderHierarchy", new object[]
			{
				SyncFolderHierarchy1
			});
			return (SyncFolderHierarchyResponseType)array[0];
		}

		public IAsyncResult BeginSyncFolderHierarchy(SyncFolderHierarchyType SyncFolderHierarchy1, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("SyncFolderHierarchy", new object[]
			{
				SyncFolderHierarchy1
			}, callback, asyncState);
		}

		public SyncFolderHierarchyResponseType EndSyncFolderHierarchy(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (SyncFolderHierarchyResponseType)array[0];
		}

		public void SyncFolderHierarchyAsync(SyncFolderHierarchyType SyncFolderHierarchy1)
		{
			this.SyncFolderHierarchyAsync(SyncFolderHierarchy1, null);
		}

		public void SyncFolderHierarchyAsync(SyncFolderHierarchyType SyncFolderHierarchy1, object userState)
		{
			if (this.SyncFolderHierarchyOperationCompleted == null)
			{
				this.SyncFolderHierarchyOperationCompleted = new SendOrPostCallback(this.OnSyncFolderHierarchyOperationCompleted);
			}
			base.InvokeAsync("SyncFolderHierarchy", new object[]
			{
				SyncFolderHierarchy1
			}, this.SyncFolderHierarchyOperationCompleted, userState);
		}

		private void OnSyncFolderHierarchyOperationCompleted(object arg)
		{
			if (this.SyncFolderHierarchyCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.SyncFolderHierarchyCompleted(this, new SyncFolderHierarchyCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://schemas.microsoft.com/exchange/services/2006/messages/SyncFolderItems", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Bare)]
		[SoapHeader("ServerVersionInfoValue", Direction = SoapHeaderDirection.Out)]
		[SoapHeader("ExchangeImpersonation")]
		[SoapHttpClientTraceExtension]
		[SoapHeader("RequestServerVersionValue")]
		[SoapHeader("MailboxCulture")]
		[return: XmlElement("SyncFolderItemsResponse", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public SyncFolderItemsResponseType SyncFolderItems([XmlElement("SyncFolderItems", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")] SyncFolderItemsType SyncFolderItems1)
		{
			object[] array = this.Invoke("SyncFolderItems", new object[]
			{
				SyncFolderItems1
			});
			return (SyncFolderItemsResponseType)array[0];
		}

		public IAsyncResult BeginSyncFolderItems(SyncFolderItemsType SyncFolderItems1, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("SyncFolderItems", new object[]
			{
				SyncFolderItems1
			}, callback, asyncState);
		}

		public SyncFolderItemsResponseType EndSyncFolderItems(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (SyncFolderItemsResponseType)array[0];
		}

		public void SyncFolderItemsAsync(SyncFolderItemsType SyncFolderItems1)
		{
			this.SyncFolderItemsAsync(SyncFolderItems1, null);
		}

		public void SyncFolderItemsAsync(SyncFolderItemsType SyncFolderItems1, object userState)
		{
			if (this.SyncFolderItemsOperationCompleted == null)
			{
				this.SyncFolderItemsOperationCompleted = new SendOrPostCallback(this.OnSyncFolderItemsOperationCompleted);
			}
			base.InvokeAsync("SyncFolderItems", new object[]
			{
				SyncFolderItems1
			}, this.SyncFolderItemsOperationCompleted, userState);
		}

		private void OnSyncFolderItemsOperationCompleted(object arg)
		{
			if (this.SyncFolderItemsCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.SyncFolderItemsCompleted(this, new SyncFolderItemsCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapHeader("MailboxCulture")]
		[SoapHeader("ExchangeImpersonation")]
		[SoapDocumentMethod("http://schemas.microsoft.com/exchange/services/2006/messages/CreateManagedFolder", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Bare)]
		[SoapHeader("RequestServerVersionValue")]
		[SoapHeader("ServerVersionInfoValue", Direction = SoapHeaderDirection.Out)]
		[SoapHttpClientTraceExtension]
		[return: XmlElement("CreateManagedFolderResponse", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public CreateManagedFolderResponseType CreateManagedFolder([XmlElement("CreateManagedFolder", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")] CreateManagedFolderRequestType CreateManagedFolder1)
		{
			object[] array = this.Invoke("CreateManagedFolder", new object[]
			{
				CreateManagedFolder1
			});
			return (CreateManagedFolderResponseType)array[0];
		}

		public IAsyncResult BeginCreateManagedFolder(CreateManagedFolderRequestType CreateManagedFolder1, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("CreateManagedFolder", new object[]
			{
				CreateManagedFolder1
			}, callback, asyncState);
		}

		public CreateManagedFolderResponseType EndCreateManagedFolder(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (CreateManagedFolderResponseType)array[0];
		}

		public void CreateManagedFolderAsync(CreateManagedFolderRequestType CreateManagedFolder1)
		{
			this.CreateManagedFolderAsync(CreateManagedFolder1, null);
		}

		public void CreateManagedFolderAsync(CreateManagedFolderRequestType CreateManagedFolder1, object userState)
		{
			if (this.CreateManagedFolderOperationCompleted == null)
			{
				this.CreateManagedFolderOperationCompleted = new SendOrPostCallback(this.OnCreateManagedFolderOperationCompleted);
			}
			base.InvokeAsync("CreateManagedFolder", new object[]
			{
				CreateManagedFolder1
			}, this.CreateManagedFolderOperationCompleted, userState);
		}

		private void OnCreateManagedFolderOperationCompleted(object arg)
		{
			if (this.CreateManagedFolderCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.CreateManagedFolderCompleted(this, new CreateManagedFolderCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapHeader("MailboxCulture")]
		[SoapHeader("ServerVersionInfoValue", Direction = SoapHeaderDirection.Out)]
		[SoapHeader("ManagementRole")]
		[SoapHeader("RequestServerVersionValue")]
		[SoapHeader("DateTimePrecision")]
		[SoapDocumentMethod("http://schemas.microsoft.com/exchange/services/2006/messages/GetItem", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Bare)]
		[SoapHttpClientTraceExtension]
		[SoapHeader("TimeZoneContext")]
		[SoapHeader("ExchangeImpersonation")]
		[return: XmlElement("GetItemResponse", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public GetItemResponseType GetItem([XmlElement("GetItem", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")] GetItemType GetItem1)
		{
			object[] array = this.Invoke("GetItem", new object[]
			{
				GetItem1
			});
			return (GetItemResponseType)array[0];
		}

		public IAsyncResult BeginGetItem(GetItemType GetItem1, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("GetItem", new object[]
			{
				GetItem1
			}, callback, asyncState);
		}

		public GetItemResponseType EndGetItem(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (GetItemResponseType)array[0];
		}

		public void GetItemAsync(GetItemType GetItem1)
		{
			this.GetItemAsync(GetItem1, null);
		}

		public void GetItemAsync(GetItemType GetItem1, object userState)
		{
			if (this.GetItemOperationCompleted == null)
			{
				this.GetItemOperationCompleted = new SendOrPostCallback(this.OnGetItemOperationCompleted);
			}
			base.InvokeAsync("GetItem", new object[]
			{
				GetItem1
			}, this.GetItemOperationCompleted, userState);
		}

		private void OnGetItemOperationCompleted(object arg)
		{
			if (this.GetItemCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.GetItemCompleted(this, new GetItemCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapHeader("TimeZoneContext")]
		[SoapHeader("ExchangeImpersonation")]
		[SoapHttpClientTraceExtension]
		[SoapHeader("MailboxCulture")]
		[SoapDocumentMethod("http://schemas.microsoft.com/exchange/services/2006/messages/CreateItem", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Bare)]
		[SoapHeader("RequestServerVersionValue")]
		[SoapHeader("ServerVersionInfoValue", Direction = SoapHeaderDirection.Out)]
		[return: XmlElement("CreateItemResponse", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public CreateItemResponseType CreateItem([XmlElement("CreateItem", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")] CreateItemType CreateItem1)
		{
			object[] array = this.Invoke("CreateItem", new object[]
			{
				CreateItem1
			});
			return (CreateItemResponseType)array[0];
		}

		public IAsyncResult BeginCreateItem(CreateItemType CreateItem1, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("CreateItem", new object[]
			{
				CreateItem1
			}, callback, asyncState);
		}

		public CreateItemResponseType EndCreateItem(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (CreateItemResponseType)array[0];
		}

		public void CreateItemAsync(CreateItemType CreateItem1)
		{
			this.CreateItemAsync(CreateItem1, null);
		}

		public void CreateItemAsync(CreateItemType CreateItem1, object userState)
		{
			if (this.CreateItemOperationCompleted == null)
			{
				this.CreateItemOperationCompleted = new SendOrPostCallback(this.OnCreateItemOperationCompleted);
			}
			base.InvokeAsync("CreateItem", new object[]
			{
				CreateItem1
			}, this.CreateItemOperationCompleted, userState);
		}

		private void OnCreateItemOperationCompleted(object arg)
		{
			if (this.CreateItemCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.CreateItemCompleted(this, new CreateItemCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://schemas.microsoft.com/exchange/services/2006/messages/DeleteItem", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Bare)]
		[SoapHeader("RequestServerVersionValue")]
		[SoapHttpClientTraceExtension]
		[SoapHeader("MailboxCulture")]
		[SoapHeader("ServerVersionInfoValue", Direction = SoapHeaderDirection.Out)]
		[SoapHeader("ExchangeImpersonation")]
		[return: XmlElement("DeleteItemResponse", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public DeleteItemResponseType DeleteItem([XmlElement("DeleteItem", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")] DeleteItemType DeleteItem1)
		{
			object[] array = this.Invoke("DeleteItem", new object[]
			{
				DeleteItem1
			});
			return (DeleteItemResponseType)array[0];
		}

		public IAsyncResult BeginDeleteItem(DeleteItemType DeleteItem1, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("DeleteItem", new object[]
			{
				DeleteItem1
			}, callback, asyncState);
		}

		public DeleteItemResponseType EndDeleteItem(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (DeleteItemResponseType)array[0];
		}

		public void DeleteItemAsync(DeleteItemType DeleteItem1)
		{
			this.DeleteItemAsync(DeleteItem1, null);
		}

		public void DeleteItemAsync(DeleteItemType DeleteItem1, object userState)
		{
			if (this.DeleteItemOperationCompleted == null)
			{
				this.DeleteItemOperationCompleted = new SendOrPostCallback(this.OnDeleteItemOperationCompleted);
			}
			base.InvokeAsync("DeleteItem", new object[]
			{
				DeleteItem1
			}, this.DeleteItemOperationCompleted, userState);
		}

		private void OnDeleteItemOperationCompleted(object arg)
		{
			if (this.DeleteItemCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.DeleteItemCompleted(this, new DeleteItemCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapHeader("ServerVersionInfoValue", Direction = SoapHeaderDirection.Out)]
		[SoapHeader("MailboxCulture")]
		[SoapHttpClientTraceExtension]
		[SoapDocumentMethod("http://schemas.microsoft.com/exchange/services/2006/messages/UpdateItem", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Bare)]
		[SoapHeader("ExchangeImpersonation")]
		[SoapHeader("RequestServerVersionValue")]
		[SoapHeader("TimeZoneContext")]
		[return: XmlElement("UpdateItemResponse", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public UpdateItemResponseType UpdateItem([XmlElement("UpdateItem", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")] UpdateItemType UpdateItem1)
		{
			object[] array = this.Invoke("UpdateItem", new object[]
			{
				UpdateItem1
			});
			return (UpdateItemResponseType)array[0];
		}

		public IAsyncResult BeginUpdateItem(UpdateItemType UpdateItem1, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("UpdateItem", new object[]
			{
				UpdateItem1
			}, callback, asyncState);
		}

		public UpdateItemResponseType EndUpdateItem(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (UpdateItemResponseType)array[0];
		}

		public void UpdateItemAsync(UpdateItemType UpdateItem1)
		{
			this.UpdateItemAsync(UpdateItem1, null);
		}

		public void UpdateItemAsync(UpdateItemType UpdateItem1, object userState)
		{
			if (this.UpdateItemOperationCompleted == null)
			{
				this.UpdateItemOperationCompleted = new SendOrPostCallback(this.OnUpdateItemOperationCompleted);
			}
			base.InvokeAsync("UpdateItem", new object[]
			{
				UpdateItem1
			}, this.UpdateItemOperationCompleted, userState);
		}

		private void OnUpdateItemOperationCompleted(object arg)
		{
			if (this.UpdateItemCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.UpdateItemCompleted(this, new UpdateItemCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapHttpClientTraceExtension]
		[SoapHeader("ServerVersionInfoValue", Direction = SoapHeaderDirection.Out)]
		[SoapHeader("MailboxCulture")]
		[SoapHeader("RequestServerVersionValue")]
		[SoapHeader("ManagementRole")]
		[SoapDocumentMethod("http://schemas.microsoft.com/exchange/services/2006/messages/UpdateItemInRecoverableItems", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Bare)]
		[SoapHeader("TimeZoneContext")]
		[SoapHeader("ExchangeImpersonation")]
		[return: XmlElement("UpdateItemInRecoverableItemsResponse", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public UpdateItemInRecoverableItemsResponseType UpdateItemInRecoverableItems([XmlElement("UpdateItemInRecoverableItems", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")] UpdateItemInRecoverableItemsType UpdateItemInRecoverableItems1)
		{
			object[] array = this.Invoke("UpdateItemInRecoverableItems", new object[]
			{
				UpdateItemInRecoverableItems1
			});
			return (UpdateItemInRecoverableItemsResponseType)array[0];
		}

		public IAsyncResult BeginUpdateItemInRecoverableItems(UpdateItemInRecoverableItemsType UpdateItemInRecoverableItems1, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("UpdateItemInRecoverableItems", new object[]
			{
				UpdateItemInRecoverableItems1
			}, callback, asyncState);
		}

		public UpdateItemInRecoverableItemsResponseType EndUpdateItemInRecoverableItems(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (UpdateItemInRecoverableItemsResponseType)array[0];
		}

		public void UpdateItemInRecoverableItemsAsync(UpdateItemInRecoverableItemsType UpdateItemInRecoverableItems1)
		{
			this.UpdateItemInRecoverableItemsAsync(UpdateItemInRecoverableItems1, null);
		}

		public void UpdateItemInRecoverableItemsAsync(UpdateItemInRecoverableItemsType UpdateItemInRecoverableItems1, object userState)
		{
			if (this.UpdateItemInRecoverableItemsOperationCompleted == null)
			{
				this.UpdateItemInRecoverableItemsOperationCompleted = new SendOrPostCallback(this.OnUpdateItemInRecoverableItemsOperationCompleted);
			}
			base.InvokeAsync("UpdateItemInRecoverableItems", new object[]
			{
				UpdateItemInRecoverableItems1
			}, this.UpdateItemInRecoverableItemsOperationCompleted, userState);
		}

		private void OnUpdateItemInRecoverableItemsOperationCompleted(object arg)
		{
			if (this.UpdateItemInRecoverableItemsCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.UpdateItemInRecoverableItemsCompleted(this, new UpdateItemInRecoverableItemsCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapHeader("RequestServerVersionValue")]
		[SoapHeader("MailboxCulture")]
		[SoapHttpClientTraceExtension]
		[SoapDocumentMethod("http://schemas.microsoft.com/exchange/services/2006/messages/SendItem", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Bare)]
		[SoapHeader("ServerVersionInfoValue", Direction = SoapHeaderDirection.Out)]
		[SoapHeader("ExchangeImpersonation")]
		[return: XmlElement("SendItemResponse", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public SendItemResponseType SendItem([XmlElement("SendItem", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")] SendItemType SendItem1)
		{
			object[] array = this.Invoke("SendItem", new object[]
			{
				SendItem1
			});
			return (SendItemResponseType)array[0];
		}

		public IAsyncResult BeginSendItem(SendItemType SendItem1, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("SendItem", new object[]
			{
				SendItem1
			}, callback, asyncState);
		}

		public SendItemResponseType EndSendItem(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (SendItemResponseType)array[0];
		}

		public void SendItemAsync(SendItemType SendItem1)
		{
			this.SendItemAsync(SendItem1, null);
		}

		public void SendItemAsync(SendItemType SendItem1, object userState)
		{
			if (this.SendItemOperationCompleted == null)
			{
				this.SendItemOperationCompleted = new SendOrPostCallback(this.OnSendItemOperationCompleted);
			}
			base.InvokeAsync("SendItem", new object[]
			{
				SendItem1
			}, this.SendItemOperationCompleted, userState);
		}

		private void OnSendItemOperationCompleted(object arg)
		{
			if (this.SendItemCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.SendItemCompleted(this, new SendItemCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapHttpClientTraceExtension]
		[SoapHeader("ServerVersionInfoValue", Direction = SoapHeaderDirection.Out)]
		[SoapHeader("ExchangeImpersonation")]
		[SoapDocumentMethod("http://schemas.microsoft.com/exchange/services/2006/messages/MoveItem", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Bare)]
		[SoapHeader("RequestServerVersionValue")]
		[SoapHeader("MailboxCulture")]
		[return: XmlElement("MoveItemResponse", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public MoveItemResponseType MoveItem([XmlElement("MoveItem", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")] MoveItemType MoveItem1)
		{
			object[] array = this.Invoke("MoveItem", new object[]
			{
				MoveItem1
			});
			return (MoveItemResponseType)array[0];
		}

		public IAsyncResult BeginMoveItem(MoveItemType MoveItem1, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("MoveItem", new object[]
			{
				MoveItem1
			}, callback, asyncState);
		}

		public MoveItemResponseType EndMoveItem(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (MoveItemResponseType)array[0];
		}

		public void MoveItemAsync(MoveItemType MoveItem1)
		{
			this.MoveItemAsync(MoveItem1, null);
		}

		public void MoveItemAsync(MoveItemType MoveItem1, object userState)
		{
			if (this.MoveItemOperationCompleted == null)
			{
				this.MoveItemOperationCompleted = new SendOrPostCallback(this.OnMoveItemOperationCompleted);
			}
			base.InvokeAsync("MoveItem", new object[]
			{
				MoveItem1
			}, this.MoveItemOperationCompleted, userState);
		}

		private void OnMoveItemOperationCompleted(object arg)
		{
			if (this.MoveItemCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.MoveItemCompleted(this, new MoveItemCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapHttpClientTraceExtension]
		[SoapHeader("RequestServerVersionValue")]
		[SoapDocumentMethod("http://schemas.microsoft.com/exchange/services/2006/messages/CopyItem", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Bare)]
		[SoapHeader("ServerVersionInfoValue", Direction = SoapHeaderDirection.Out)]
		[SoapHeader("MailboxCulture")]
		[SoapHeader("ExchangeImpersonation")]
		[return: XmlElement("CopyItemResponse", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public CopyItemResponseType CopyItem([XmlElement("CopyItem", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")] CopyItemType CopyItem1)
		{
			object[] array = this.Invoke("CopyItem", new object[]
			{
				CopyItem1
			});
			return (CopyItemResponseType)array[0];
		}

		public IAsyncResult BeginCopyItem(CopyItemType CopyItem1, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("CopyItem", new object[]
			{
				CopyItem1
			}, callback, asyncState);
		}

		public CopyItemResponseType EndCopyItem(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (CopyItemResponseType)array[0];
		}

		public void CopyItemAsync(CopyItemType CopyItem1)
		{
			this.CopyItemAsync(CopyItem1, null);
		}

		public void CopyItemAsync(CopyItemType CopyItem1, object userState)
		{
			if (this.CopyItemOperationCompleted == null)
			{
				this.CopyItemOperationCompleted = new SendOrPostCallback(this.OnCopyItemOperationCompleted);
			}
			base.InvokeAsync("CopyItem", new object[]
			{
				CopyItem1
			}, this.CopyItemOperationCompleted, userState);
		}

		private void OnCopyItemOperationCompleted(object arg)
		{
			if (this.CopyItemCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.CopyItemCompleted(this, new CopyItemCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapHeader("ServerVersionInfoValue", Direction = SoapHeaderDirection.Out)]
		[SoapHeader("ExchangeImpersonation")]
		[SoapDocumentMethod("http://schemas.microsoft.com/exchange/services/2006/messages/ArchiveItem", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Bare)]
		[SoapHttpClientTraceExtension]
		[SoapHeader("RequestServerVersionValue")]
		[SoapHeader("MailboxCulture")]
		[return: XmlElement("ArchiveItemResponse", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public ArchiveItemResponseType ArchiveItem([XmlElement("ArchiveItem", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")] ArchiveItemType ArchiveItem1)
		{
			object[] array = this.Invoke("ArchiveItem", new object[]
			{
				ArchiveItem1
			});
			return (ArchiveItemResponseType)array[0];
		}

		public IAsyncResult BeginArchiveItem(ArchiveItemType ArchiveItem1, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("ArchiveItem", new object[]
			{
				ArchiveItem1
			}, callback, asyncState);
		}

		public ArchiveItemResponseType EndArchiveItem(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (ArchiveItemResponseType)array[0];
		}

		public void ArchiveItemAsync(ArchiveItemType ArchiveItem1)
		{
			this.ArchiveItemAsync(ArchiveItem1, null);
		}

		public void ArchiveItemAsync(ArchiveItemType ArchiveItem1, object userState)
		{
			if (this.ArchiveItemOperationCompleted == null)
			{
				this.ArchiveItemOperationCompleted = new SendOrPostCallback(this.OnArchiveItemOperationCompleted);
			}
			base.InvokeAsync("ArchiveItem", new object[]
			{
				ArchiveItem1
			}, this.ArchiveItemOperationCompleted, userState);
		}

		private void OnArchiveItemOperationCompleted(object arg)
		{
			if (this.ArchiveItemCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.ArchiveItemCompleted(this, new ArchiveItemCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapHttpClientTraceExtension]
		[SoapHeader("TimeZoneContext")]
		[SoapHeader("ExchangeImpersonation")]
		[SoapDocumentMethod("http://schemas.microsoft.com/exchange/services/2006/messages/CreateAttachment", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Bare)]
		[SoapHeader("MailboxCulture")]
		[SoapHeader("ServerVersionInfoValue", Direction = SoapHeaderDirection.Out)]
		[SoapHeader("RequestServerVersionValue")]
		[return: XmlElement("CreateAttachmentResponse", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public CreateAttachmentResponseType CreateAttachment([XmlElement("CreateAttachment", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")] CreateAttachmentType CreateAttachment1)
		{
			object[] array = this.Invoke("CreateAttachment", new object[]
			{
				CreateAttachment1
			});
			return (CreateAttachmentResponseType)array[0];
		}

		public IAsyncResult BeginCreateAttachment(CreateAttachmentType CreateAttachment1, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("CreateAttachment", new object[]
			{
				CreateAttachment1
			}, callback, asyncState);
		}

		public CreateAttachmentResponseType EndCreateAttachment(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (CreateAttachmentResponseType)array[0];
		}

		public void CreateAttachmentAsync(CreateAttachmentType CreateAttachment1)
		{
			this.CreateAttachmentAsync(CreateAttachment1, null);
		}

		public void CreateAttachmentAsync(CreateAttachmentType CreateAttachment1, object userState)
		{
			if (this.CreateAttachmentOperationCompleted == null)
			{
				this.CreateAttachmentOperationCompleted = new SendOrPostCallback(this.OnCreateAttachmentOperationCompleted);
			}
			base.InvokeAsync("CreateAttachment", new object[]
			{
				CreateAttachment1
			}, this.CreateAttachmentOperationCompleted, userState);
		}

		private void OnCreateAttachmentOperationCompleted(object arg)
		{
			if (this.CreateAttachmentCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.CreateAttachmentCompleted(this, new CreateAttachmentCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapHeader("MailboxCulture")]
		[SoapHeader("ServerVersionInfoValue", Direction = SoapHeaderDirection.Out)]
		[SoapHttpClientTraceExtension]
		[SoapHeader("ExchangeImpersonation")]
		[SoapHeader("RequestServerVersionValue")]
		[SoapDocumentMethod("http://schemas.microsoft.com/exchange/services/2006/messages/DeleteAttachment", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Bare)]
		[return: XmlElement("DeleteAttachmentResponse", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public DeleteAttachmentResponseType DeleteAttachment([XmlElement("DeleteAttachment", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")] DeleteAttachmentType DeleteAttachment1)
		{
			object[] array = this.Invoke("DeleteAttachment", new object[]
			{
				DeleteAttachment1
			});
			return (DeleteAttachmentResponseType)array[0];
		}

		public IAsyncResult BeginDeleteAttachment(DeleteAttachmentType DeleteAttachment1, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("DeleteAttachment", new object[]
			{
				DeleteAttachment1
			}, callback, asyncState);
		}

		public DeleteAttachmentResponseType EndDeleteAttachment(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (DeleteAttachmentResponseType)array[0];
		}

		public void DeleteAttachmentAsync(DeleteAttachmentType DeleteAttachment1)
		{
			this.DeleteAttachmentAsync(DeleteAttachment1, null);
		}

		public void DeleteAttachmentAsync(DeleteAttachmentType DeleteAttachment1, object userState)
		{
			if (this.DeleteAttachmentOperationCompleted == null)
			{
				this.DeleteAttachmentOperationCompleted = new SendOrPostCallback(this.OnDeleteAttachmentOperationCompleted);
			}
			base.InvokeAsync("DeleteAttachment", new object[]
			{
				DeleteAttachment1
			}, this.DeleteAttachmentOperationCompleted, userState);
		}

		private void OnDeleteAttachmentOperationCompleted(object arg)
		{
			if (this.DeleteAttachmentCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.DeleteAttachmentCompleted(this, new DeleteAttachmentCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapHeader("MailboxCulture")]
		[SoapHeader("TimeZoneContext")]
		[SoapDocumentMethod("http://schemas.microsoft.com/exchange/services/2006/messages/GetAttachment", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Bare)]
		[SoapHttpClientTraceExtension]
		[SoapHeader("ExchangeImpersonation")]
		[SoapHeader("RequestServerVersionValue")]
		[SoapHeader("ServerVersionInfoValue", Direction = SoapHeaderDirection.Out)]
		[return: XmlElement("GetAttachmentResponse", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public GetAttachmentResponseType GetAttachment([XmlElement("GetAttachment", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")] GetAttachmentType GetAttachment1)
		{
			object[] array = this.Invoke("GetAttachment", new object[]
			{
				GetAttachment1
			});
			return (GetAttachmentResponseType)array[0];
		}

		public IAsyncResult BeginGetAttachment(GetAttachmentType GetAttachment1, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("GetAttachment", new object[]
			{
				GetAttachment1
			}, callback, asyncState);
		}

		public GetAttachmentResponseType EndGetAttachment(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (GetAttachmentResponseType)array[0];
		}

		public void GetAttachmentAsync(GetAttachmentType GetAttachment1)
		{
			this.GetAttachmentAsync(GetAttachment1, null);
		}

		public void GetAttachmentAsync(GetAttachmentType GetAttachment1, object userState)
		{
			if (this.GetAttachmentOperationCompleted == null)
			{
				this.GetAttachmentOperationCompleted = new SendOrPostCallback(this.OnGetAttachmentOperationCompleted);
			}
			base.InvokeAsync("GetAttachment", new object[]
			{
				GetAttachment1
			}, this.GetAttachmentOperationCompleted, userState);
		}

		private void OnGetAttachmentOperationCompleted(object arg)
		{
			if (this.GetAttachmentCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.GetAttachmentCompleted(this, new GetAttachmentCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapHttpClientTraceExtension]
		[SoapDocumentMethod("http://schemas.microsoft.com/exchange/services/2006/messages/GetClientAccessToken", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Bare)]
		[SoapHeader("RequestServerVersionValue")]
		[SoapHeader("ServerVersionInfoValue", Direction = SoapHeaderDirection.Out)]
		[return: XmlElement("GetClientAccessTokenResponse", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public GetClientAccessTokenResponseType GetClientAccessToken([XmlElement("GetClientAccessToken", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")] GetClientAccessTokenType GetClientAccessToken1)
		{
			object[] array = this.Invoke("GetClientAccessToken", new object[]
			{
				GetClientAccessToken1
			});
			return (GetClientAccessTokenResponseType)array[0];
		}

		public IAsyncResult BeginGetClientAccessToken(GetClientAccessTokenType GetClientAccessToken1, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("GetClientAccessToken", new object[]
			{
				GetClientAccessToken1
			}, callback, asyncState);
		}

		public GetClientAccessTokenResponseType EndGetClientAccessToken(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (GetClientAccessTokenResponseType)array[0];
		}

		public void GetClientAccessTokenAsync(GetClientAccessTokenType GetClientAccessToken1)
		{
			this.GetClientAccessTokenAsync(GetClientAccessToken1, null);
		}

		public void GetClientAccessTokenAsync(GetClientAccessTokenType GetClientAccessToken1, object userState)
		{
			if (this.GetClientAccessTokenOperationCompleted == null)
			{
				this.GetClientAccessTokenOperationCompleted = new SendOrPostCallback(this.OnGetClientAccessTokenOperationCompleted);
			}
			base.InvokeAsync("GetClientAccessToken", new object[]
			{
				GetClientAccessToken1
			}, this.GetClientAccessTokenOperationCompleted, userState);
		}

		private void OnGetClientAccessTokenOperationCompleted(object arg)
		{
			if (this.GetClientAccessTokenCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.GetClientAccessTokenCompleted(this, new GetClientAccessTokenCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapHttpClientTraceExtension]
		[SoapHeader("ExchangeImpersonation")]
		[SoapHeader("RequestServerVersionValue")]
		[SoapHeader("MailboxCulture")]
		[SoapDocumentMethod("http://schemas.microsoft.com/exchange/services/2006/messages/GetDelegate", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Bare)]
		[SoapHeader("ServerVersionInfoValue", Direction = SoapHeaderDirection.Out)]
		[return: XmlElement("GetDelegateResponse", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public GetDelegateResponseMessageType GetDelegate([XmlElement("GetDelegate", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")] GetDelegateType GetDelegate1)
		{
			object[] array = this.Invoke("GetDelegate", new object[]
			{
				GetDelegate1
			});
			return (GetDelegateResponseMessageType)array[0];
		}

		public IAsyncResult BeginGetDelegate(GetDelegateType GetDelegate1, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("GetDelegate", new object[]
			{
				GetDelegate1
			}, callback, asyncState);
		}

		public GetDelegateResponseMessageType EndGetDelegate(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (GetDelegateResponseMessageType)array[0];
		}

		public void GetDelegateAsync(GetDelegateType GetDelegate1)
		{
			this.GetDelegateAsync(GetDelegate1, null);
		}

		public void GetDelegateAsync(GetDelegateType GetDelegate1, object userState)
		{
			if (this.GetDelegateOperationCompleted == null)
			{
				this.GetDelegateOperationCompleted = new SendOrPostCallback(this.OnGetDelegateOperationCompleted);
			}
			base.InvokeAsync("GetDelegate", new object[]
			{
				GetDelegate1
			}, this.GetDelegateOperationCompleted, userState);
		}

		private void OnGetDelegateOperationCompleted(object arg)
		{
			if (this.GetDelegateCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.GetDelegateCompleted(this, new GetDelegateCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapHeader("MailboxCulture")]
		[SoapHeader("ExchangeImpersonation")]
		[SoapDocumentMethod("http://schemas.microsoft.com/exchange/services/2006/messages/AddDelegate", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Bare)]
		[SoapHttpClientTraceExtension]
		[SoapHeader("RequestServerVersionValue")]
		[SoapHeader("ServerVersionInfoValue", Direction = SoapHeaderDirection.Out)]
		[return: XmlElement("AddDelegateResponse", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public AddDelegateResponseMessageType AddDelegate([XmlElement("AddDelegate", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")] AddDelegateType AddDelegate1)
		{
			object[] array = this.Invoke("AddDelegate", new object[]
			{
				AddDelegate1
			});
			return (AddDelegateResponseMessageType)array[0];
		}

		public IAsyncResult BeginAddDelegate(AddDelegateType AddDelegate1, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("AddDelegate", new object[]
			{
				AddDelegate1
			}, callback, asyncState);
		}

		public AddDelegateResponseMessageType EndAddDelegate(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (AddDelegateResponseMessageType)array[0];
		}

		public void AddDelegateAsync(AddDelegateType AddDelegate1)
		{
			this.AddDelegateAsync(AddDelegate1, null);
		}

		public void AddDelegateAsync(AddDelegateType AddDelegate1, object userState)
		{
			if (this.AddDelegateOperationCompleted == null)
			{
				this.AddDelegateOperationCompleted = new SendOrPostCallback(this.OnAddDelegateOperationCompleted);
			}
			base.InvokeAsync("AddDelegate", new object[]
			{
				AddDelegate1
			}, this.AddDelegateOperationCompleted, userState);
		}

		private void OnAddDelegateOperationCompleted(object arg)
		{
			if (this.AddDelegateCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.AddDelegateCompleted(this, new AddDelegateCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapHttpClientTraceExtension]
		[SoapHeader("ServerVersionInfoValue", Direction = SoapHeaderDirection.Out)]
		[SoapHeader("ExchangeImpersonation")]
		[SoapDocumentMethod("http://schemas.microsoft.com/exchange/services/2006/messages/RemoveDelegate", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Bare)]
		[SoapHeader("RequestServerVersionValue")]
		[SoapHeader("MailboxCulture")]
		[return: XmlElement("RemoveDelegateResponse", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public RemoveDelegateResponseMessageType RemoveDelegate([XmlElement("RemoveDelegate", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")] RemoveDelegateType RemoveDelegate1)
		{
			object[] array = this.Invoke("RemoveDelegate", new object[]
			{
				RemoveDelegate1
			});
			return (RemoveDelegateResponseMessageType)array[0];
		}

		public IAsyncResult BeginRemoveDelegate(RemoveDelegateType RemoveDelegate1, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("RemoveDelegate", new object[]
			{
				RemoveDelegate1
			}, callback, asyncState);
		}

		public RemoveDelegateResponseMessageType EndRemoveDelegate(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (RemoveDelegateResponseMessageType)array[0];
		}

		public void RemoveDelegateAsync(RemoveDelegateType RemoveDelegate1)
		{
			this.RemoveDelegateAsync(RemoveDelegate1, null);
		}

		public void RemoveDelegateAsync(RemoveDelegateType RemoveDelegate1, object userState)
		{
			if (this.RemoveDelegateOperationCompleted == null)
			{
				this.RemoveDelegateOperationCompleted = new SendOrPostCallback(this.OnRemoveDelegateOperationCompleted);
			}
			base.InvokeAsync("RemoveDelegate", new object[]
			{
				RemoveDelegate1
			}, this.RemoveDelegateOperationCompleted, userState);
		}

		private void OnRemoveDelegateOperationCompleted(object arg)
		{
			if (this.RemoveDelegateCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.RemoveDelegateCompleted(this, new RemoveDelegateCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapHeader("ServerVersionInfoValue", Direction = SoapHeaderDirection.Out)]
		[SoapHttpClientTraceExtension]
		[SoapHeader("MailboxCulture")]
		[SoapHeader("RequestServerVersionValue")]
		[SoapHeader("ExchangeImpersonation")]
		[SoapDocumentMethod("http://schemas.microsoft.com/exchange/services/2006/messages/UpdateDelegate", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Bare)]
		[return: XmlElement("UpdateDelegateResponse", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public UpdateDelegateResponseMessageType UpdateDelegate([XmlElement("UpdateDelegate", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")] UpdateDelegateType UpdateDelegate1)
		{
			object[] array = this.Invoke("UpdateDelegate", new object[]
			{
				UpdateDelegate1
			});
			return (UpdateDelegateResponseMessageType)array[0];
		}

		public IAsyncResult BeginUpdateDelegate(UpdateDelegateType UpdateDelegate1, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("UpdateDelegate", new object[]
			{
				UpdateDelegate1
			}, callback, asyncState);
		}

		public UpdateDelegateResponseMessageType EndUpdateDelegate(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (UpdateDelegateResponseMessageType)array[0];
		}

		public void UpdateDelegateAsync(UpdateDelegateType UpdateDelegate1)
		{
			this.UpdateDelegateAsync(UpdateDelegate1, null);
		}

		public void UpdateDelegateAsync(UpdateDelegateType UpdateDelegate1, object userState)
		{
			if (this.UpdateDelegateOperationCompleted == null)
			{
				this.UpdateDelegateOperationCompleted = new SendOrPostCallback(this.OnUpdateDelegateOperationCompleted);
			}
			base.InvokeAsync("UpdateDelegate", new object[]
			{
				UpdateDelegate1
			}, this.UpdateDelegateOperationCompleted, userState);
		}

		private void OnUpdateDelegateOperationCompleted(object arg)
		{
			if (this.UpdateDelegateCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.UpdateDelegateCompleted(this, new UpdateDelegateCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapHeader("ExchangeImpersonation")]
		[SoapHttpClientTraceExtension]
		[SoapHeader("ServerVersionInfoValue", Direction = SoapHeaderDirection.Out)]
		[SoapDocumentMethod("http://schemas.microsoft.com/exchange/services/2006/messages/CreateUserConfiguration", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Bare)]
		[SoapHeader("MailboxCulture")]
		[SoapHeader("RequestServerVersionValue")]
		[return: XmlElement("CreateUserConfigurationResponse", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public CreateUserConfigurationResponseType CreateUserConfiguration([XmlElement("CreateUserConfiguration", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")] CreateUserConfigurationType CreateUserConfiguration1)
		{
			object[] array = this.Invoke("CreateUserConfiguration", new object[]
			{
				CreateUserConfiguration1
			});
			return (CreateUserConfigurationResponseType)array[0];
		}

		public IAsyncResult BeginCreateUserConfiguration(CreateUserConfigurationType CreateUserConfiguration1, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("CreateUserConfiguration", new object[]
			{
				CreateUserConfiguration1
			}, callback, asyncState);
		}

		public CreateUserConfigurationResponseType EndCreateUserConfiguration(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (CreateUserConfigurationResponseType)array[0];
		}

		public void CreateUserConfigurationAsync(CreateUserConfigurationType CreateUserConfiguration1)
		{
			this.CreateUserConfigurationAsync(CreateUserConfiguration1, null);
		}

		public void CreateUserConfigurationAsync(CreateUserConfigurationType CreateUserConfiguration1, object userState)
		{
			if (this.CreateUserConfigurationOperationCompleted == null)
			{
				this.CreateUserConfigurationOperationCompleted = new SendOrPostCallback(this.OnCreateUserConfigurationOperationCompleted);
			}
			base.InvokeAsync("CreateUserConfiguration", new object[]
			{
				CreateUserConfiguration1
			}, this.CreateUserConfigurationOperationCompleted, userState);
		}

		private void OnCreateUserConfigurationOperationCompleted(object arg)
		{
			if (this.CreateUserConfigurationCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.CreateUserConfigurationCompleted(this, new CreateUserConfigurationCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapHttpClientTraceExtension]
		[SoapDocumentMethod("http://schemas.microsoft.com/exchange/services/2006/messages/DeleteUserConfiguration", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Bare)]
		[SoapHeader("ExchangeImpersonation")]
		[SoapHeader("ServerVersionInfoValue", Direction = SoapHeaderDirection.Out)]
		[SoapHeader("RequestServerVersionValue")]
		[SoapHeader("MailboxCulture")]
		[return: XmlElement("DeleteUserConfigurationResponse", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public DeleteUserConfigurationResponseType DeleteUserConfiguration([XmlElement("DeleteUserConfiguration", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")] DeleteUserConfigurationType DeleteUserConfiguration1)
		{
			object[] array = this.Invoke("DeleteUserConfiguration", new object[]
			{
				DeleteUserConfiguration1
			});
			return (DeleteUserConfigurationResponseType)array[0];
		}

		public IAsyncResult BeginDeleteUserConfiguration(DeleteUserConfigurationType DeleteUserConfiguration1, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("DeleteUserConfiguration", new object[]
			{
				DeleteUserConfiguration1
			}, callback, asyncState);
		}

		public DeleteUserConfigurationResponseType EndDeleteUserConfiguration(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (DeleteUserConfigurationResponseType)array[0];
		}

		public void DeleteUserConfigurationAsync(DeleteUserConfigurationType DeleteUserConfiguration1)
		{
			this.DeleteUserConfigurationAsync(DeleteUserConfiguration1, null);
		}

		public void DeleteUserConfigurationAsync(DeleteUserConfigurationType DeleteUserConfiguration1, object userState)
		{
			if (this.DeleteUserConfigurationOperationCompleted == null)
			{
				this.DeleteUserConfigurationOperationCompleted = new SendOrPostCallback(this.OnDeleteUserConfigurationOperationCompleted);
			}
			base.InvokeAsync("DeleteUserConfiguration", new object[]
			{
				DeleteUserConfiguration1
			}, this.DeleteUserConfigurationOperationCompleted, userState);
		}

		private void OnDeleteUserConfigurationOperationCompleted(object arg)
		{
			if (this.DeleteUserConfigurationCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.DeleteUserConfigurationCompleted(this, new DeleteUserConfigurationCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapHeader("MailboxCulture")]
		[SoapHeader("ExchangeImpersonation")]
		[SoapHttpClientTraceExtension]
		[SoapDocumentMethod("http://schemas.microsoft.com/exchange/services/2006/messages/GetUserConfiguration", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Bare)]
		[SoapHeader("RequestServerVersionValue")]
		[SoapHeader("ServerVersionInfoValue", Direction = SoapHeaderDirection.Out)]
		[return: XmlElement("GetUserConfigurationResponse", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public GetUserConfigurationResponseType GetUserConfiguration([XmlElement("GetUserConfiguration", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")] GetUserConfigurationType GetUserConfiguration1)
		{
			object[] array = this.Invoke("GetUserConfiguration", new object[]
			{
				GetUserConfiguration1
			});
			return (GetUserConfigurationResponseType)array[0];
		}

		public IAsyncResult BeginGetUserConfiguration(GetUserConfigurationType GetUserConfiguration1, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("GetUserConfiguration", new object[]
			{
				GetUserConfiguration1
			}, callback, asyncState);
		}

		public GetUserConfigurationResponseType EndGetUserConfiguration(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (GetUserConfigurationResponseType)array[0];
		}

		public void GetUserConfigurationAsync(GetUserConfigurationType GetUserConfiguration1)
		{
			this.GetUserConfigurationAsync(GetUserConfiguration1, null);
		}

		public void GetUserConfigurationAsync(GetUserConfigurationType GetUserConfiguration1, object userState)
		{
			if (this.GetUserConfigurationOperationCompleted == null)
			{
				this.GetUserConfigurationOperationCompleted = new SendOrPostCallback(this.OnGetUserConfigurationOperationCompleted);
			}
			base.InvokeAsync("GetUserConfiguration", new object[]
			{
				GetUserConfiguration1
			}, this.GetUserConfigurationOperationCompleted, userState);
		}

		private void OnGetUserConfigurationOperationCompleted(object arg)
		{
			if (this.GetUserConfigurationCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.GetUserConfigurationCompleted(this, new GetUserConfigurationCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://schemas.microsoft.com/exchange/services/2006/messages/UpdateUserConfiguration", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Bare)]
		[SoapHeader("ExchangeImpersonation")]
		[SoapHeader("RequestServerVersionValue")]
		[SoapHeader("ServerVersionInfoValue", Direction = SoapHeaderDirection.Out)]
		[SoapHttpClientTraceExtension]
		[SoapHeader("MailboxCulture")]
		[return: XmlElement("UpdateUserConfigurationResponse", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public UpdateUserConfigurationResponseType UpdateUserConfiguration([XmlElement("UpdateUserConfiguration", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")] UpdateUserConfigurationType UpdateUserConfiguration1)
		{
			object[] array = this.Invoke("UpdateUserConfiguration", new object[]
			{
				UpdateUserConfiguration1
			});
			return (UpdateUserConfigurationResponseType)array[0];
		}

		public IAsyncResult BeginUpdateUserConfiguration(UpdateUserConfigurationType UpdateUserConfiguration1, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("UpdateUserConfiguration", new object[]
			{
				UpdateUserConfiguration1
			}, callback, asyncState);
		}

		public UpdateUserConfigurationResponseType EndUpdateUserConfiguration(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (UpdateUserConfigurationResponseType)array[0];
		}

		public void UpdateUserConfigurationAsync(UpdateUserConfigurationType UpdateUserConfiguration1)
		{
			this.UpdateUserConfigurationAsync(UpdateUserConfiguration1, null);
		}

		public void UpdateUserConfigurationAsync(UpdateUserConfigurationType UpdateUserConfiguration1, object userState)
		{
			if (this.UpdateUserConfigurationOperationCompleted == null)
			{
				this.UpdateUserConfigurationOperationCompleted = new SendOrPostCallback(this.OnUpdateUserConfigurationOperationCompleted);
			}
			base.InvokeAsync("UpdateUserConfiguration", new object[]
			{
				UpdateUserConfiguration1
			}, this.UpdateUserConfigurationOperationCompleted, userState);
		}

		private void OnUpdateUserConfigurationOperationCompleted(object arg)
		{
			if (this.UpdateUserConfigurationCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.UpdateUserConfigurationCompleted(this, new UpdateUserConfigurationCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapHeader("ExchangeImpersonation")]
		[SoapHeader("TimeZoneContext")]
		[SoapHeader("RequestServerVersionValue")]
		[SoapHttpClientTraceExtension]
		[SoapDocumentMethod("http://schemas.microsoft.com/exchange/services/2006/messages/GetUserAvailability", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Bare)]
		[SoapHeader("ServerVersionInfoValue", Direction = SoapHeaderDirection.Out)]
		[return: XmlElement("GetUserAvailabilityResponse", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public GetUserAvailabilityResponseType GetUserAvailability([XmlElement(Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")] GetUserAvailabilityRequestType GetUserAvailabilityRequest)
		{
			object[] array = this.Invoke("GetUserAvailability", new object[]
			{
				GetUserAvailabilityRequest
			});
			return (GetUserAvailabilityResponseType)array[0];
		}

		public IAsyncResult BeginGetUserAvailability(GetUserAvailabilityRequestType GetUserAvailabilityRequest, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("GetUserAvailability", new object[]
			{
				GetUserAvailabilityRequest
			}, callback, asyncState);
		}

		public GetUserAvailabilityResponseType EndGetUserAvailability(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (GetUserAvailabilityResponseType)array[0];
		}

		public void GetUserAvailabilityAsync(GetUserAvailabilityRequestType GetUserAvailabilityRequest)
		{
			this.GetUserAvailabilityAsync(GetUserAvailabilityRequest, null);
		}

		public void GetUserAvailabilityAsync(GetUserAvailabilityRequestType GetUserAvailabilityRequest, object userState)
		{
			if (this.GetUserAvailabilityOperationCompleted == null)
			{
				this.GetUserAvailabilityOperationCompleted = new SendOrPostCallback(this.OnGetUserAvailabilityOperationCompleted);
			}
			base.InvokeAsync("GetUserAvailability", new object[]
			{
				GetUserAvailabilityRequest
			}, this.GetUserAvailabilityOperationCompleted, userState);
		}

		private void OnGetUserAvailabilityOperationCompleted(object arg)
		{
			if (this.GetUserAvailabilityCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.GetUserAvailabilityCompleted(this, new GetUserAvailabilityCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapHeader("ServerVersionInfoValue", Direction = SoapHeaderDirection.Out)]
		[SoapHttpClientTraceExtension]
		[SoapHeader("ExchangeImpersonation")]
		[SoapDocumentMethod("http://schemas.microsoft.com/exchange/services/2006/messages/GetUserOofSettings", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Bare)]
		[return: XmlElement("GetUserOofSettingsResponse", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public GetUserOofSettingsResponse GetUserOofSettings([XmlElement(Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")] GetUserOofSettingsRequest GetUserOofSettingsRequest)
		{
			object[] array = this.Invoke("GetUserOofSettings", new object[]
			{
				GetUserOofSettingsRequest
			});
			return (GetUserOofSettingsResponse)array[0];
		}

		public IAsyncResult BeginGetUserOofSettings(GetUserOofSettingsRequest GetUserOofSettingsRequest, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("GetUserOofSettings", new object[]
			{
				GetUserOofSettingsRequest
			}, callback, asyncState);
		}

		public GetUserOofSettingsResponse EndGetUserOofSettings(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (GetUserOofSettingsResponse)array[0];
		}

		public void GetUserOofSettingsAsync(GetUserOofSettingsRequest GetUserOofSettingsRequest)
		{
			this.GetUserOofSettingsAsync(GetUserOofSettingsRequest, null);
		}

		public void GetUserOofSettingsAsync(GetUserOofSettingsRequest GetUserOofSettingsRequest, object userState)
		{
			if (this.GetUserOofSettingsOperationCompleted == null)
			{
				this.GetUserOofSettingsOperationCompleted = new SendOrPostCallback(this.OnGetUserOofSettingsOperationCompleted);
			}
			base.InvokeAsync("GetUserOofSettings", new object[]
			{
				GetUserOofSettingsRequest
			}, this.GetUserOofSettingsOperationCompleted, userState);
		}

		private void OnGetUserOofSettingsOperationCompleted(object arg)
		{
			if (this.GetUserOofSettingsCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.GetUserOofSettingsCompleted(this, new GetUserOofSettingsCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapHttpClientTraceExtension]
		[SoapDocumentMethod("http://schemas.microsoft.com/exchange/services/2006/messages/SetUserOofSettings", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Bare)]
		[SoapHeader("ExchangeImpersonation")]
		[SoapHeader("ServerVersionInfoValue", Direction = SoapHeaderDirection.Out)]
		[return: XmlElement("SetUserOofSettingsResponse", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public SetUserOofSettingsResponse SetUserOofSettings([XmlElement(Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")] SetUserOofSettingsRequest SetUserOofSettingsRequest)
		{
			object[] array = this.Invoke("SetUserOofSettings", new object[]
			{
				SetUserOofSettingsRequest
			});
			return (SetUserOofSettingsResponse)array[0];
		}

		public IAsyncResult BeginSetUserOofSettings(SetUserOofSettingsRequest SetUserOofSettingsRequest, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("SetUserOofSettings", new object[]
			{
				SetUserOofSettingsRequest
			}, callback, asyncState);
		}

		public SetUserOofSettingsResponse EndSetUserOofSettings(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (SetUserOofSettingsResponse)array[0];
		}

		public void SetUserOofSettingsAsync(SetUserOofSettingsRequest SetUserOofSettingsRequest)
		{
			this.SetUserOofSettingsAsync(SetUserOofSettingsRequest, null);
		}

		public void SetUserOofSettingsAsync(SetUserOofSettingsRequest SetUserOofSettingsRequest, object userState)
		{
			if (this.SetUserOofSettingsOperationCompleted == null)
			{
				this.SetUserOofSettingsOperationCompleted = new SendOrPostCallback(this.OnSetUserOofSettingsOperationCompleted);
			}
			base.InvokeAsync("SetUserOofSettings", new object[]
			{
				SetUserOofSettingsRequest
			}, this.SetUserOofSettingsOperationCompleted, userState);
		}

		private void OnSetUserOofSettingsOperationCompleted(object arg)
		{
			if (this.SetUserOofSettingsCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.SetUserOofSettingsCompleted(this, new SetUserOofSettingsCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapHttpClientTraceExtension]
		[SoapDocumentMethod("http://schemas.microsoft.com/exchange/services/2006/messages/GetServiceConfiguration", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Bare)]
		[SoapHeader("ExchangeImpersonation")]
		[SoapHeader("MailboxCulture")]
		[SoapHeader("RequestServerVersionValue")]
		[SoapHeader("ServerVersionInfoValue", Direction = SoapHeaderDirection.Out)]
		[return: XmlElement("GetServiceConfigurationResponse", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public GetServiceConfigurationResponseMessageType GetServiceConfiguration([XmlElement("GetServiceConfiguration", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")] GetServiceConfigurationType GetServiceConfiguration1)
		{
			object[] array = this.Invoke("GetServiceConfiguration", new object[]
			{
				GetServiceConfiguration1
			});
			return (GetServiceConfigurationResponseMessageType)array[0];
		}

		public IAsyncResult BeginGetServiceConfiguration(GetServiceConfigurationType GetServiceConfiguration1, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("GetServiceConfiguration", new object[]
			{
				GetServiceConfiguration1
			}, callback, asyncState);
		}

		public GetServiceConfigurationResponseMessageType EndGetServiceConfiguration(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (GetServiceConfigurationResponseMessageType)array[0];
		}

		public void GetServiceConfigurationAsync(GetServiceConfigurationType GetServiceConfiguration1)
		{
			this.GetServiceConfigurationAsync(GetServiceConfiguration1, null);
		}

		public void GetServiceConfigurationAsync(GetServiceConfigurationType GetServiceConfiguration1, object userState)
		{
			if (this.GetServiceConfigurationOperationCompleted == null)
			{
				this.GetServiceConfigurationOperationCompleted = new SendOrPostCallback(this.OnGetServiceConfigurationOperationCompleted);
			}
			base.InvokeAsync("GetServiceConfiguration", new object[]
			{
				GetServiceConfiguration1
			}, this.GetServiceConfigurationOperationCompleted, userState);
		}

		private void OnGetServiceConfigurationOperationCompleted(object arg)
		{
			if (this.GetServiceConfigurationCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.GetServiceConfigurationCompleted(this, new GetServiceConfigurationCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://schemas.microsoft.com/exchange/services/2006/messages/GetMailTips", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Bare)]
		[SoapHttpClientTraceExtension]
		[SoapHeader("ServerVersionInfoValue", Direction = SoapHeaderDirection.Out)]
		[SoapHeader("RequestServerVersionValue")]
		[SoapHeader("MailboxCulture")]
		[return: XmlElement("GetMailTipsResponse", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public GetMailTipsResponseMessageType GetMailTips([XmlElement("GetMailTips", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")] GetMailTipsType GetMailTips1)
		{
			object[] array = this.Invoke("GetMailTips", new object[]
			{
				GetMailTips1
			});
			return (GetMailTipsResponseMessageType)array[0];
		}

		public IAsyncResult BeginGetMailTips(GetMailTipsType GetMailTips1, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("GetMailTips", new object[]
			{
				GetMailTips1
			}, callback, asyncState);
		}

		public GetMailTipsResponseMessageType EndGetMailTips(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (GetMailTipsResponseMessageType)array[0];
		}

		public void GetMailTipsAsync(GetMailTipsType GetMailTips1)
		{
			this.GetMailTipsAsync(GetMailTips1, null);
		}

		public void GetMailTipsAsync(GetMailTipsType GetMailTips1, object userState)
		{
			if (this.GetMailTipsOperationCompleted == null)
			{
				this.GetMailTipsOperationCompleted = new SendOrPostCallback(this.OnGetMailTipsOperationCompleted);
			}
			base.InvokeAsync("GetMailTips", new object[]
			{
				GetMailTips1
			}, this.GetMailTipsOperationCompleted, userState);
		}

		private void OnGetMailTipsOperationCompleted(object arg)
		{
			if (this.GetMailTipsCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.GetMailTipsCompleted(this, new GetMailTipsCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapHttpClientTraceExtension]
		[SoapHeader("RequestServerVersionValue")]
		[SoapHeader("MailboxCulture")]
		[SoapHeader("ExchangeImpersonation")]
		[SoapDocumentMethod("http://schemas.microsoft.com/exchange/services/2006/messages/PlayOnPhone", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Bare)]
		[SoapHeader("ServerVersionInfoValue", Direction = SoapHeaderDirection.Out)]
		[return: XmlElement("PlayOnPhoneResponse", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public PlayOnPhoneResponseMessageType PlayOnPhone([XmlElement("PlayOnPhone", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")] PlayOnPhoneType PlayOnPhone1)
		{
			object[] array = this.Invoke("PlayOnPhone", new object[]
			{
				PlayOnPhone1
			});
			return (PlayOnPhoneResponseMessageType)array[0];
		}

		public IAsyncResult BeginPlayOnPhone(PlayOnPhoneType PlayOnPhone1, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("PlayOnPhone", new object[]
			{
				PlayOnPhone1
			}, callback, asyncState);
		}

		public PlayOnPhoneResponseMessageType EndPlayOnPhone(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (PlayOnPhoneResponseMessageType)array[0];
		}

		public void PlayOnPhoneAsync(PlayOnPhoneType PlayOnPhone1)
		{
			this.PlayOnPhoneAsync(PlayOnPhone1, null);
		}

		public void PlayOnPhoneAsync(PlayOnPhoneType PlayOnPhone1, object userState)
		{
			if (this.PlayOnPhoneOperationCompleted == null)
			{
				this.PlayOnPhoneOperationCompleted = new SendOrPostCallback(this.OnPlayOnPhoneOperationCompleted);
			}
			base.InvokeAsync("PlayOnPhone", new object[]
			{
				PlayOnPhone1
			}, this.PlayOnPhoneOperationCompleted, userState);
		}

		private void OnPlayOnPhoneOperationCompleted(object arg)
		{
			if (this.PlayOnPhoneCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.PlayOnPhoneCompleted(this, new PlayOnPhoneCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://schemas.microsoft.com/exchange/services/2006/messages/GetPhoneCallInformation", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Bare)]
		[SoapHeader("MailboxCulture")]
		[SoapHttpClientTraceExtension]
		[SoapHeader("ServerVersionInfoValue", Direction = SoapHeaderDirection.Out)]
		[SoapHeader("ExchangeImpersonation")]
		[SoapHeader("RequestServerVersionValue")]
		[return: XmlElement("GetPhoneCallInformationResponse", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public GetPhoneCallInformationResponseMessageType GetPhoneCallInformation([XmlElement("GetPhoneCallInformation", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")] GetPhoneCallInformationType GetPhoneCallInformation1)
		{
			object[] array = this.Invoke("GetPhoneCallInformation", new object[]
			{
				GetPhoneCallInformation1
			});
			return (GetPhoneCallInformationResponseMessageType)array[0];
		}

		public IAsyncResult BeginGetPhoneCallInformation(GetPhoneCallInformationType GetPhoneCallInformation1, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("GetPhoneCallInformation", new object[]
			{
				GetPhoneCallInformation1
			}, callback, asyncState);
		}

		public GetPhoneCallInformationResponseMessageType EndGetPhoneCallInformation(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (GetPhoneCallInformationResponseMessageType)array[0];
		}

		public void GetPhoneCallInformationAsync(GetPhoneCallInformationType GetPhoneCallInformation1)
		{
			this.GetPhoneCallInformationAsync(GetPhoneCallInformation1, null);
		}

		public void GetPhoneCallInformationAsync(GetPhoneCallInformationType GetPhoneCallInformation1, object userState)
		{
			if (this.GetPhoneCallInformationOperationCompleted == null)
			{
				this.GetPhoneCallInformationOperationCompleted = new SendOrPostCallback(this.OnGetPhoneCallInformationOperationCompleted);
			}
			base.InvokeAsync("GetPhoneCallInformation", new object[]
			{
				GetPhoneCallInformation1
			}, this.GetPhoneCallInformationOperationCompleted, userState);
		}

		private void OnGetPhoneCallInformationOperationCompleted(object arg)
		{
			if (this.GetPhoneCallInformationCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.GetPhoneCallInformationCompleted(this, new GetPhoneCallInformationCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapHeader("RequestServerVersionValue")]
		[SoapHeader("ServerVersionInfoValue", Direction = SoapHeaderDirection.Out)]
		[SoapHeader("ExchangeImpersonation")]
		[SoapHttpClientTraceExtension]
		[SoapDocumentMethod("http://schemas.microsoft.com/exchange/services/2006/messages/DisconnectPhoneCall", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Bare)]
		[SoapHeader("MailboxCulture")]
		[return: XmlElement("DisconnectPhoneCallResponse", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public DisconnectPhoneCallResponseMessageType DisconnectPhoneCall([XmlElement("DisconnectPhoneCall", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")] DisconnectPhoneCallType DisconnectPhoneCall1)
		{
			object[] array = this.Invoke("DisconnectPhoneCall", new object[]
			{
				DisconnectPhoneCall1
			});
			return (DisconnectPhoneCallResponseMessageType)array[0];
		}

		public IAsyncResult BeginDisconnectPhoneCall(DisconnectPhoneCallType DisconnectPhoneCall1, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("DisconnectPhoneCall", new object[]
			{
				DisconnectPhoneCall1
			}, callback, asyncState);
		}

		public DisconnectPhoneCallResponseMessageType EndDisconnectPhoneCall(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (DisconnectPhoneCallResponseMessageType)array[0];
		}

		public void DisconnectPhoneCallAsync(DisconnectPhoneCallType DisconnectPhoneCall1)
		{
			this.DisconnectPhoneCallAsync(DisconnectPhoneCall1, null);
		}

		public void DisconnectPhoneCallAsync(DisconnectPhoneCallType DisconnectPhoneCall1, object userState)
		{
			if (this.DisconnectPhoneCallOperationCompleted == null)
			{
				this.DisconnectPhoneCallOperationCompleted = new SendOrPostCallback(this.OnDisconnectPhoneCallOperationCompleted);
			}
			base.InvokeAsync("DisconnectPhoneCall", new object[]
			{
				DisconnectPhoneCall1
			}, this.DisconnectPhoneCallOperationCompleted, userState);
		}

		private void OnDisconnectPhoneCallOperationCompleted(object arg)
		{
			if (this.DisconnectPhoneCallCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.DisconnectPhoneCallCompleted(this, new DisconnectPhoneCallCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapHeader("ServerVersionInfoValue", Direction = SoapHeaderDirection.Out)]
		[SoapDocumentMethod("http://schemas.microsoft.com/exchange/services/2006/messages/GetSharingMetadata", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Bare)]
		[SoapHttpClientTraceExtension]
		[SoapHeader("RequestServerVersionValue")]
		[return: XmlElement("GetSharingMetadataResponse", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public GetSharingMetadataResponseMessageType GetSharingMetadata([XmlElement("GetSharingMetadata", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")] GetSharingMetadataType GetSharingMetadata1)
		{
			object[] array = this.Invoke("GetSharingMetadata", new object[]
			{
				GetSharingMetadata1
			});
			return (GetSharingMetadataResponseMessageType)array[0];
		}

		public IAsyncResult BeginGetSharingMetadata(GetSharingMetadataType GetSharingMetadata1, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("GetSharingMetadata", new object[]
			{
				GetSharingMetadata1
			}, callback, asyncState);
		}

		public GetSharingMetadataResponseMessageType EndGetSharingMetadata(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (GetSharingMetadataResponseMessageType)array[0];
		}

		public void GetSharingMetadataAsync(GetSharingMetadataType GetSharingMetadata1)
		{
			this.GetSharingMetadataAsync(GetSharingMetadata1, null);
		}

		public void GetSharingMetadataAsync(GetSharingMetadataType GetSharingMetadata1, object userState)
		{
			if (this.GetSharingMetadataOperationCompleted == null)
			{
				this.GetSharingMetadataOperationCompleted = new SendOrPostCallback(this.OnGetSharingMetadataOperationCompleted);
			}
			base.InvokeAsync("GetSharingMetadata", new object[]
			{
				GetSharingMetadata1
			}, this.GetSharingMetadataOperationCompleted, userState);
		}

		private void OnGetSharingMetadataOperationCompleted(object arg)
		{
			if (this.GetSharingMetadataCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.GetSharingMetadataCompleted(this, new GetSharingMetadataCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapHeader("ServerVersionInfoValue", Direction = SoapHeaderDirection.Out)]
		[SoapDocumentMethod("http://schemas.microsoft.com/exchange/services/2006/messages/RefreshSharingFolder", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Bare)]
		[SoapHeader("RequestServerVersionValue")]
		[SoapHttpClientTraceExtension]
		[return: XmlElement("RefreshSharingFolderResponse", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public RefreshSharingFolderResponseMessageType RefreshSharingFolder([XmlElement("RefreshSharingFolder", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")] RefreshSharingFolderType RefreshSharingFolder1)
		{
			object[] array = this.Invoke("RefreshSharingFolder", new object[]
			{
				RefreshSharingFolder1
			});
			return (RefreshSharingFolderResponseMessageType)array[0];
		}

		public IAsyncResult BeginRefreshSharingFolder(RefreshSharingFolderType RefreshSharingFolder1, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("RefreshSharingFolder", new object[]
			{
				RefreshSharingFolder1
			}, callback, asyncState);
		}

		public RefreshSharingFolderResponseMessageType EndRefreshSharingFolder(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (RefreshSharingFolderResponseMessageType)array[0];
		}

		public void RefreshSharingFolderAsync(RefreshSharingFolderType RefreshSharingFolder1)
		{
			this.RefreshSharingFolderAsync(RefreshSharingFolder1, null);
		}

		public void RefreshSharingFolderAsync(RefreshSharingFolderType RefreshSharingFolder1, object userState)
		{
			if (this.RefreshSharingFolderOperationCompleted == null)
			{
				this.RefreshSharingFolderOperationCompleted = new SendOrPostCallback(this.OnRefreshSharingFolderOperationCompleted);
			}
			base.InvokeAsync("RefreshSharingFolder", new object[]
			{
				RefreshSharingFolder1
			}, this.RefreshSharingFolderOperationCompleted, userState);
		}

		private void OnRefreshSharingFolderOperationCompleted(object arg)
		{
			if (this.RefreshSharingFolderCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.RefreshSharingFolderCompleted(this, new RefreshSharingFolderCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://schemas.microsoft.com/exchange/services/2006/messages/GetSharingFolder", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Bare)]
		[SoapHeader("RequestServerVersionValue")]
		[SoapHttpClientTraceExtension]
		[SoapHeader("ServerVersionInfoValue", Direction = SoapHeaderDirection.Out)]
		[return: XmlElement("GetSharingFolderResponse", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public GetSharingFolderResponseMessageType GetSharingFolder([XmlElement("GetSharingFolder", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")] GetSharingFolderType GetSharingFolder1)
		{
			object[] array = this.Invoke("GetSharingFolder", new object[]
			{
				GetSharingFolder1
			});
			return (GetSharingFolderResponseMessageType)array[0];
		}

		public IAsyncResult BeginGetSharingFolder(GetSharingFolderType GetSharingFolder1, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("GetSharingFolder", new object[]
			{
				GetSharingFolder1
			}, callback, asyncState);
		}

		public GetSharingFolderResponseMessageType EndGetSharingFolder(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (GetSharingFolderResponseMessageType)array[0];
		}

		public void GetSharingFolderAsync(GetSharingFolderType GetSharingFolder1)
		{
			this.GetSharingFolderAsync(GetSharingFolder1, null);
		}

		public void GetSharingFolderAsync(GetSharingFolderType GetSharingFolder1, object userState)
		{
			if (this.GetSharingFolderOperationCompleted == null)
			{
				this.GetSharingFolderOperationCompleted = new SendOrPostCallback(this.OnGetSharingFolderOperationCompleted);
			}
			base.InvokeAsync("GetSharingFolder", new object[]
			{
				GetSharingFolder1
			}, this.GetSharingFolderOperationCompleted, userState);
		}

		private void OnGetSharingFolderOperationCompleted(object arg)
		{
			if (this.GetSharingFolderCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.GetSharingFolderCompleted(this, new GetSharingFolderCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapHttpClientTraceExtension]
		[SoapHeader("ManagementRole")]
		[SoapDocumentMethod("http://schemas.microsoft.com/exchange/services/2006/messages/SetTeamMailbox", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Bare)]
		[SoapHeader("RequestServerVersionValue")]
		[SoapHeader("ServerVersionInfoValue", Direction = SoapHeaderDirection.Out)]
		[return: XmlElement("SetTeamMailboxResponse", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public SetTeamMailboxResponseMessageType SetTeamMailbox([XmlElement("SetTeamMailbox", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")] SetTeamMailboxRequestType SetTeamMailbox1)
		{
			object[] array = this.Invoke("SetTeamMailbox", new object[]
			{
				SetTeamMailbox1
			});
			return (SetTeamMailboxResponseMessageType)array[0];
		}

		public IAsyncResult BeginSetTeamMailbox(SetTeamMailboxRequestType SetTeamMailbox1, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("SetTeamMailbox", new object[]
			{
				SetTeamMailbox1
			}, callback, asyncState);
		}

		public SetTeamMailboxResponseMessageType EndSetTeamMailbox(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (SetTeamMailboxResponseMessageType)array[0];
		}

		public void SetTeamMailboxAsync(SetTeamMailboxRequestType SetTeamMailbox1)
		{
			this.SetTeamMailboxAsync(SetTeamMailbox1, null);
		}

		public void SetTeamMailboxAsync(SetTeamMailboxRequestType SetTeamMailbox1, object userState)
		{
			if (this.SetTeamMailboxOperationCompleted == null)
			{
				this.SetTeamMailboxOperationCompleted = new SendOrPostCallback(this.OnSetTeamMailboxOperationCompleted);
			}
			base.InvokeAsync("SetTeamMailbox", new object[]
			{
				SetTeamMailbox1
			}, this.SetTeamMailboxOperationCompleted, userState);
		}

		private void OnSetTeamMailboxOperationCompleted(object arg)
		{
			if (this.SetTeamMailboxCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.SetTeamMailboxCompleted(this, new SetTeamMailboxCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://schemas.microsoft.com/exchange/services/2006/messages/UnpinTeamMailbox", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Bare)]
		[SoapHttpClientTraceExtension]
		[SoapHeader("RequestServerVersionValue")]
		[SoapHeader("ServerVersionInfoValue", Direction = SoapHeaderDirection.Out)]
		[return: XmlElement("UnpinTeamMailboxResponse", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public UnpinTeamMailboxResponseMessageType UnpinTeamMailbox([XmlElement("UnpinTeamMailbox", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")] UnpinTeamMailboxRequestType UnpinTeamMailbox1)
		{
			object[] array = this.Invoke("UnpinTeamMailbox", new object[]
			{
				UnpinTeamMailbox1
			});
			return (UnpinTeamMailboxResponseMessageType)array[0];
		}

		public IAsyncResult BeginUnpinTeamMailbox(UnpinTeamMailboxRequestType UnpinTeamMailbox1, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("UnpinTeamMailbox", new object[]
			{
				UnpinTeamMailbox1
			}, callback, asyncState);
		}

		public UnpinTeamMailboxResponseMessageType EndUnpinTeamMailbox(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (UnpinTeamMailboxResponseMessageType)array[0];
		}

		public void UnpinTeamMailboxAsync(UnpinTeamMailboxRequestType UnpinTeamMailbox1)
		{
			this.UnpinTeamMailboxAsync(UnpinTeamMailbox1, null);
		}

		public void UnpinTeamMailboxAsync(UnpinTeamMailboxRequestType UnpinTeamMailbox1, object userState)
		{
			if (this.UnpinTeamMailboxOperationCompleted == null)
			{
				this.UnpinTeamMailboxOperationCompleted = new SendOrPostCallback(this.OnUnpinTeamMailboxOperationCompleted);
			}
			base.InvokeAsync("UnpinTeamMailbox", new object[]
			{
				UnpinTeamMailbox1
			}, this.UnpinTeamMailboxOperationCompleted, userState);
		}

		private void OnUnpinTeamMailboxOperationCompleted(object arg)
		{
			if (this.UnpinTeamMailboxCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.UnpinTeamMailboxCompleted(this, new UnpinTeamMailboxCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapHttpClientTraceExtension]
		[SoapHeader("RequestServerVersionValue")]
		[SoapHeader("MailboxCulture")]
		[SoapHeader("ServerVersionInfoValue", Direction = SoapHeaderDirection.Out)]
		[SoapHeader("ExchangeImpersonation")]
		[SoapDocumentMethod("http://schemas.microsoft.com/exchange/services/2006/messages/GetRoomLists", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Bare)]
		[return: XmlElement("GetRoomListsResponse", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public GetRoomListsResponseMessageType GetRoomLists([XmlElement("GetRoomLists", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")] GetRoomListsType GetRoomLists1)
		{
			object[] array = this.Invoke("GetRoomLists", new object[]
			{
				GetRoomLists1
			});
			return (GetRoomListsResponseMessageType)array[0];
		}

		public IAsyncResult BeginGetRoomLists(GetRoomListsType GetRoomLists1, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("GetRoomLists", new object[]
			{
				GetRoomLists1
			}, callback, asyncState);
		}

		public GetRoomListsResponseMessageType EndGetRoomLists(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (GetRoomListsResponseMessageType)array[0];
		}

		public void GetRoomListsAsync(GetRoomListsType GetRoomLists1)
		{
			this.GetRoomListsAsync(GetRoomLists1, null);
		}

		public void GetRoomListsAsync(GetRoomListsType GetRoomLists1, object userState)
		{
			if (this.GetRoomListsOperationCompleted == null)
			{
				this.GetRoomListsOperationCompleted = new SendOrPostCallback(this.OnGetRoomListsOperationCompleted);
			}
			base.InvokeAsync("GetRoomLists", new object[]
			{
				GetRoomLists1
			}, this.GetRoomListsOperationCompleted, userState);
		}

		private void OnGetRoomListsOperationCompleted(object arg)
		{
			if (this.GetRoomListsCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.GetRoomListsCompleted(this, new GetRoomListsCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapHeader("ExchangeImpersonation")]
		[SoapHttpClientTraceExtension]
		[SoapDocumentMethod("http://schemas.microsoft.com/exchange/services/2006/messages/GetRooms", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Bare)]
		[SoapHeader("RequestServerVersionValue")]
		[SoapHeader("MailboxCulture")]
		[SoapHeader("ServerVersionInfoValue", Direction = SoapHeaderDirection.Out)]
		[return: XmlElement("GetRoomsResponse", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public GetRoomsResponseMessageType GetRooms([XmlElement("GetRooms", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")] GetRoomsType GetRooms1)
		{
			object[] array = this.Invoke("GetRooms", new object[]
			{
				GetRooms1
			});
			return (GetRoomsResponseMessageType)array[0];
		}

		public IAsyncResult BeginGetRooms(GetRoomsType GetRooms1, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("GetRooms", new object[]
			{
				GetRooms1
			}, callback, asyncState);
		}

		public GetRoomsResponseMessageType EndGetRooms(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (GetRoomsResponseMessageType)array[0];
		}

		public void GetRoomsAsync(GetRoomsType GetRooms1)
		{
			this.GetRoomsAsync(GetRooms1, null);
		}

		public void GetRoomsAsync(GetRoomsType GetRooms1, object userState)
		{
			if (this.GetRoomsOperationCompleted == null)
			{
				this.GetRoomsOperationCompleted = new SendOrPostCallback(this.OnGetRoomsOperationCompleted);
			}
			base.InvokeAsync("GetRooms", new object[]
			{
				GetRooms1
			}, this.GetRoomsOperationCompleted, userState);
		}

		private void OnGetRoomsOperationCompleted(object arg)
		{
			if (this.GetRoomsCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.GetRoomsCompleted(this, new GetRoomsCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapHeader("RequestServerVersionValue")]
		[SoapDocumentMethod("http://schemas.microsoft.com/exchange/services/2006/messages/FindMessageTrackingReport", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Bare)]
		[SoapHeader("ServerVersionInfoValue", Direction = SoapHeaderDirection.Out)]
		[SoapHttpClientTraceExtension]
		[return: XmlElement("FindMessageTrackingReportResponse", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public FindMessageTrackingReportResponseMessageType FindMessageTrackingReport([XmlElement("FindMessageTrackingReport", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")] FindMessageTrackingReportRequestType FindMessageTrackingReport1)
		{
			object[] array = this.Invoke("FindMessageTrackingReport", new object[]
			{
				FindMessageTrackingReport1
			});
			return (FindMessageTrackingReportResponseMessageType)array[0];
		}

		public IAsyncResult BeginFindMessageTrackingReport(FindMessageTrackingReportRequestType FindMessageTrackingReport1, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("FindMessageTrackingReport", new object[]
			{
				FindMessageTrackingReport1
			}, callback, asyncState);
		}

		public FindMessageTrackingReportResponseMessageType EndFindMessageTrackingReport(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (FindMessageTrackingReportResponseMessageType)array[0];
		}

		public void FindMessageTrackingReportAsync(FindMessageTrackingReportRequestType FindMessageTrackingReport1)
		{
			this.FindMessageTrackingReportAsync(FindMessageTrackingReport1, null);
		}

		public void FindMessageTrackingReportAsync(FindMessageTrackingReportRequestType FindMessageTrackingReport1, object userState)
		{
			if (this.FindMessageTrackingReportOperationCompleted == null)
			{
				this.FindMessageTrackingReportOperationCompleted = new SendOrPostCallback(this.OnFindMessageTrackingReportOperationCompleted);
			}
			base.InvokeAsync("FindMessageTrackingReport", new object[]
			{
				FindMessageTrackingReport1
			}, this.FindMessageTrackingReportOperationCompleted, userState);
		}

		private void OnFindMessageTrackingReportOperationCompleted(object arg)
		{
			if (this.FindMessageTrackingReportCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.FindMessageTrackingReportCompleted(this, new FindMessageTrackingReportCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapHeader("ServerVersionInfoValue", Direction = SoapHeaderDirection.Out)]
		[SoapDocumentMethod("http://schemas.microsoft.com/exchange/services/2006/messages/GetMessageTrackingReport", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Bare)]
		[SoapHttpClientTraceExtension]
		[SoapHeader("RequestServerVersionValue")]
		[return: XmlElement("GetMessageTrackingReportResponse", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public GetMessageTrackingReportResponseMessageType GetMessageTrackingReport([XmlElement("GetMessageTrackingReport", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")] GetMessageTrackingReportRequestType GetMessageTrackingReport1)
		{
			object[] array = this.Invoke("GetMessageTrackingReport", new object[]
			{
				GetMessageTrackingReport1
			});
			return (GetMessageTrackingReportResponseMessageType)array[0];
		}

		public IAsyncResult BeginGetMessageTrackingReport(GetMessageTrackingReportRequestType GetMessageTrackingReport1, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("GetMessageTrackingReport", new object[]
			{
				GetMessageTrackingReport1
			}, callback, asyncState);
		}

		public GetMessageTrackingReportResponseMessageType EndGetMessageTrackingReport(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (GetMessageTrackingReportResponseMessageType)array[0];
		}

		public void GetMessageTrackingReportAsync(GetMessageTrackingReportRequestType GetMessageTrackingReport1)
		{
			this.GetMessageTrackingReportAsync(GetMessageTrackingReport1, null);
		}

		public void GetMessageTrackingReportAsync(GetMessageTrackingReportRequestType GetMessageTrackingReport1, object userState)
		{
			if (this.GetMessageTrackingReportOperationCompleted == null)
			{
				this.GetMessageTrackingReportOperationCompleted = new SendOrPostCallback(this.OnGetMessageTrackingReportOperationCompleted);
			}
			base.InvokeAsync("GetMessageTrackingReport", new object[]
			{
				GetMessageTrackingReport1
			}, this.GetMessageTrackingReportOperationCompleted, userState);
		}

		private void OnGetMessageTrackingReportOperationCompleted(object arg)
		{
			if (this.GetMessageTrackingReportCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.GetMessageTrackingReportCompleted(this, new GetMessageTrackingReportCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapHeader("ServerVersionInfoValue", Direction = SoapHeaderDirection.Out)]
		[SoapHeader("RequestServerVersionValue")]
		[SoapHttpClientTraceExtension]
		[SoapDocumentMethod("http://schemas.microsoft.com/exchange/services/2006/messages/FindConversation", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Bare)]
		[SoapHeader("ExchangeImpersonation")]
		[return: XmlElement("FindConversationResponse", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public FindConversationResponseMessageType FindConversation([XmlElement("FindConversation", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")] FindConversationType FindConversation1)
		{
			object[] array = this.Invoke("FindConversation", new object[]
			{
				FindConversation1
			});
			return (FindConversationResponseMessageType)array[0];
		}

		public IAsyncResult BeginFindConversation(FindConversationType FindConversation1, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("FindConversation", new object[]
			{
				FindConversation1
			}, callback, asyncState);
		}

		public FindConversationResponseMessageType EndFindConversation(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (FindConversationResponseMessageType)array[0];
		}

		public void FindConversationAsync(FindConversationType FindConversation1)
		{
			this.FindConversationAsync(FindConversation1, null);
		}

		public void FindConversationAsync(FindConversationType FindConversation1, object userState)
		{
			if (this.FindConversationOperationCompleted == null)
			{
				this.FindConversationOperationCompleted = new SendOrPostCallback(this.OnFindConversationOperationCompleted);
			}
			base.InvokeAsync("FindConversation", new object[]
			{
				FindConversation1
			}, this.FindConversationOperationCompleted, userState);
		}

		private void OnFindConversationOperationCompleted(object arg)
		{
			if (this.FindConversationCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.FindConversationCompleted(this, new FindConversationCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://schemas.microsoft.com/exchange/services/2006/messages/ApplyConversationAction", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Bare)]
		[SoapHeader("RequestServerVersionValue")]
		[SoapHeader("ExchangeImpersonation")]
		[SoapHttpClientTraceExtension]
		[SoapHeader("ServerVersionInfoValue", Direction = SoapHeaderDirection.Out)]
		[return: XmlElement("ApplyConversationActionResponse", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public ApplyConversationActionResponseType ApplyConversationAction([XmlElement("ApplyConversationAction", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")] ApplyConversationActionType ApplyConversationAction1)
		{
			object[] array = this.Invoke("ApplyConversationAction", new object[]
			{
				ApplyConversationAction1
			});
			return (ApplyConversationActionResponseType)array[0];
		}

		public IAsyncResult BeginApplyConversationAction(ApplyConversationActionType ApplyConversationAction1, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("ApplyConversationAction", new object[]
			{
				ApplyConversationAction1
			}, callback, asyncState);
		}

		public ApplyConversationActionResponseType EndApplyConversationAction(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (ApplyConversationActionResponseType)array[0];
		}

		public void ApplyConversationActionAsync(ApplyConversationActionType ApplyConversationAction1)
		{
			this.ApplyConversationActionAsync(ApplyConversationAction1, null);
		}

		public void ApplyConversationActionAsync(ApplyConversationActionType ApplyConversationAction1, object userState)
		{
			if (this.ApplyConversationActionOperationCompleted == null)
			{
				this.ApplyConversationActionOperationCompleted = new SendOrPostCallback(this.OnApplyConversationActionOperationCompleted);
			}
			base.InvokeAsync("ApplyConversationAction", new object[]
			{
				ApplyConversationAction1
			}, this.ApplyConversationActionOperationCompleted, userState);
		}

		private void OnApplyConversationActionOperationCompleted(object arg)
		{
			if (this.ApplyConversationActionCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.ApplyConversationActionCompleted(this, new ApplyConversationActionCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://schemas.microsoft.com/exchange/services/2006/messages/GetConversationItems", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Bare)]
		[SoapHeader("RequestServerVersionValue")]
		[SoapHeader("ExchangeImpersonation")]
		[SoapHttpClientTraceExtension]
		[SoapHeader("ServerVersionInfoValue", Direction = SoapHeaderDirection.Out)]
		[return: XmlElement("GetConversationItemsResponse", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public GetConversationItemsResponseType GetConversationItems([XmlElement("GetConversationItems", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")] GetConversationItemsType GetConversationItems1)
		{
			object[] array = this.Invoke("GetConversationItems", new object[]
			{
				GetConversationItems1
			});
			return (GetConversationItemsResponseType)array[0];
		}

		public IAsyncResult BeginGetConversationItems(GetConversationItemsType GetConversationItems1, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("GetConversationItems", new object[]
			{
				GetConversationItems1
			}, callback, asyncState);
		}

		public GetConversationItemsResponseType EndGetConversationItems(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (GetConversationItemsResponseType)array[0];
		}

		public void GetConversationItemsAsync(GetConversationItemsType GetConversationItems1)
		{
			this.GetConversationItemsAsync(GetConversationItems1, null);
		}

		public void GetConversationItemsAsync(GetConversationItemsType GetConversationItems1, object userState)
		{
			if (this.GetConversationItemsOperationCompleted == null)
			{
				this.GetConversationItemsOperationCompleted = new SendOrPostCallback(this.OnGetConversationItemsOperationCompleted);
			}
			base.InvokeAsync("GetConversationItems", new object[]
			{
				GetConversationItems1
			}, this.GetConversationItemsOperationCompleted, userState);
		}

		private void OnGetConversationItemsOperationCompleted(object arg)
		{
			if (this.GetConversationItemsCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.GetConversationItemsCompleted(this, new GetConversationItemsCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapHeader("ExchangeImpersonation")]
		[SoapHttpClientTraceExtension]
		[SoapDocumentMethod("http://schemas.microsoft.com/exchange/services/2006/messages/FindPeople", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Bare)]
		[SoapHeader("RequestServerVersionValue")]
		[SoapHeader("ServerVersionInfoValue", Direction = SoapHeaderDirection.Out)]
		[return: XmlElement("FindPeopleResponse", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public FindPeopleResponseMessageType FindPeople([XmlElement("FindPeople", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")] FindPeopleType FindPeople1)
		{
			object[] array = this.Invoke("FindPeople", new object[]
			{
				FindPeople1
			});
			return (FindPeopleResponseMessageType)array[0];
		}

		public IAsyncResult BeginFindPeople(FindPeopleType FindPeople1, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("FindPeople", new object[]
			{
				FindPeople1
			}, callback, asyncState);
		}

		public FindPeopleResponseMessageType EndFindPeople(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (FindPeopleResponseMessageType)array[0];
		}

		public void FindPeopleAsync(FindPeopleType FindPeople1)
		{
			this.FindPeopleAsync(FindPeople1, null);
		}

		public void FindPeopleAsync(FindPeopleType FindPeople1, object userState)
		{
			if (this.FindPeopleOperationCompleted == null)
			{
				this.FindPeopleOperationCompleted = new SendOrPostCallback(this.OnFindPeopleOperationCompleted);
			}
			base.InvokeAsync("FindPeople", new object[]
			{
				FindPeople1
			}, this.FindPeopleOperationCompleted, userState);
		}

		private void OnFindPeopleOperationCompleted(object arg)
		{
			if (this.FindPeopleCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.FindPeopleCompleted(this, new FindPeopleCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapHttpClientTraceExtension]
		[SoapHeader("ServerVersionInfoValue", Direction = SoapHeaderDirection.Out)]
		[SoapDocumentMethod("http://schemas.microsoft.com/exchange/services/2006/messages/GetPersona", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Bare)]
		[SoapHeader("ExchangeImpersonation")]
		[SoapHeader("RequestServerVersionValue")]
		[return: XmlElement("GetPersonaResponseMessage", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public GetPersonaResponseMessageType GetPersona([XmlElement("GetPersona", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")] GetPersonaType GetPersona1)
		{
			object[] array = this.Invoke("GetPersona", new object[]
			{
				GetPersona1
			});
			return (GetPersonaResponseMessageType)array[0];
		}

		public IAsyncResult BeginGetPersona(GetPersonaType GetPersona1, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("GetPersona", new object[]
			{
				GetPersona1
			}, callback, asyncState);
		}

		public GetPersonaResponseMessageType EndGetPersona(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (GetPersonaResponseMessageType)array[0];
		}

		public void GetPersonaAsync(GetPersonaType GetPersona1)
		{
			this.GetPersonaAsync(GetPersona1, null);
		}

		public void GetPersonaAsync(GetPersonaType GetPersona1, object userState)
		{
			if (this.GetPersonaOperationCompleted == null)
			{
				this.GetPersonaOperationCompleted = new SendOrPostCallback(this.OnGetPersonaOperationCompleted);
			}
			base.InvokeAsync("GetPersona", new object[]
			{
				GetPersona1
			}, this.GetPersonaOperationCompleted, userState);
		}

		private void OnGetPersonaOperationCompleted(object arg)
		{
			if (this.GetPersonaCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.GetPersonaCompleted(this, new GetPersonaCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapHeader("ServerVersionInfoValue", Direction = SoapHeaderDirection.Out)]
		[SoapHttpClientTraceExtension]
		[SoapDocumentMethod("http://schemas.microsoft.com/exchange/services/2006/messages/GetInboxRules", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Bare)]
		[SoapHeader("ExchangeImpersonation")]
		[SoapHeader("RequestServerVersionValue")]
		[SoapHeader("MailboxCulture")]
		[SoapHeader("TimeZoneContext")]
		[return: XmlElement("GetInboxRulesResponse", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public GetInboxRulesResponseType GetInboxRules([XmlElement("GetInboxRules", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")] GetInboxRulesRequestType GetInboxRules1)
		{
			object[] array = this.Invoke("GetInboxRules", new object[]
			{
				GetInboxRules1
			});
			return (GetInboxRulesResponseType)array[0];
		}

		public IAsyncResult BeginGetInboxRules(GetInboxRulesRequestType GetInboxRules1, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("GetInboxRules", new object[]
			{
				GetInboxRules1
			}, callback, asyncState);
		}

		public GetInboxRulesResponseType EndGetInboxRules(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (GetInboxRulesResponseType)array[0];
		}

		public void GetInboxRulesAsync(GetInboxRulesRequestType GetInboxRules1)
		{
			this.GetInboxRulesAsync(GetInboxRules1, null);
		}

		public void GetInboxRulesAsync(GetInboxRulesRequestType GetInboxRules1, object userState)
		{
			if (this.GetInboxRulesOperationCompleted == null)
			{
				this.GetInboxRulesOperationCompleted = new SendOrPostCallback(this.OnGetInboxRulesOperationCompleted);
			}
			base.InvokeAsync("GetInboxRules", new object[]
			{
				GetInboxRules1
			}, this.GetInboxRulesOperationCompleted, userState);
		}

		private void OnGetInboxRulesOperationCompleted(object arg)
		{
			if (this.GetInboxRulesCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.GetInboxRulesCompleted(this, new GetInboxRulesCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapHeader("ExchangeImpersonation")]
		[SoapDocumentMethod("http://schemas.microsoft.com/exchange/services/2006/messages/UpdateInboxRules", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Bare)]
		[SoapHeader("RequestServerVersionValue")]
		[SoapHttpClientTraceExtension]
		[SoapHeader("MailboxCulture")]
		[SoapHeader("TimeZoneContext")]
		[SoapHeader("ServerVersionInfoValue", Direction = SoapHeaderDirection.Out)]
		[return: XmlElement("UpdateInboxRulesResponse", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public UpdateInboxRulesResponseType UpdateInboxRules([XmlElement("UpdateInboxRules", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")] UpdateInboxRulesRequestType UpdateInboxRules1)
		{
			object[] array = this.Invoke("UpdateInboxRules", new object[]
			{
				UpdateInboxRules1
			});
			return (UpdateInboxRulesResponseType)array[0];
		}

		public IAsyncResult BeginUpdateInboxRules(UpdateInboxRulesRequestType UpdateInboxRules1, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("UpdateInboxRules", new object[]
			{
				UpdateInboxRules1
			}, callback, asyncState);
		}

		public UpdateInboxRulesResponseType EndUpdateInboxRules(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (UpdateInboxRulesResponseType)array[0];
		}

		public void UpdateInboxRulesAsync(UpdateInboxRulesRequestType UpdateInboxRules1)
		{
			this.UpdateInboxRulesAsync(UpdateInboxRules1, null);
		}

		public void UpdateInboxRulesAsync(UpdateInboxRulesRequestType UpdateInboxRules1, object userState)
		{
			if (this.UpdateInboxRulesOperationCompleted == null)
			{
				this.UpdateInboxRulesOperationCompleted = new SendOrPostCallback(this.OnUpdateInboxRulesOperationCompleted);
			}
			base.InvokeAsync("UpdateInboxRules", new object[]
			{
				UpdateInboxRules1
			}, this.UpdateInboxRulesOperationCompleted, userState);
		}

		private void OnUpdateInboxRulesOperationCompleted(object arg)
		{
			if (this.UpdateInboxRulesCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.UpdateInboxRulesCompleted(this, new UpdateInboxRulesCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapHeader("ServerVersionInfoValue", Direction = SoapHeaderDirection.Out)]
		[SoapHeader("RequestServerVersionValue")]
		[SoapDocumentMethod("http://schemas.microsoft.com/exchange/services/2006/messages/GetPasswordExpirationDate", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Bare)]
		[SoapHeader("MailboxCulture")]
		[SoapHttpClientTraceExtension]
		[return: XmlElement("GetPasswordExpirationDateResponse", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public GetPasswordExpirationDateResponseMessageType GetPasswordExpirationDate([XmlElement("GetPasswordExpirationDate", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")] GetPasswordExpirationDateType GetPasswordExpirationDate1)
		{
			object[] array = this.Invoke("GetPasswordExpirationDate", new object[]
			{
				GetPasswordExpirationDate1
			});
			return (GetPasswordExpirationDateResponseMessageType)array[0];
		}

		public IAsyncResult BeginGetPasswordExpirationDate(GetPasswordExpirationDateType GetPasswordExpirationDate1, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("GetPasswordExpirationDate", new object[]
			{
				GetPasswordExpirationDate1
			}, callback, asyncState);
		}

		public GetPasswordExpirationDateResponseMessageType EndGetPasswordExpirationDate(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (GetPasswordExpirationDateResponseMessageType)array[0];
		}

		public void GetPasswordExpirationDateAsync(GetPasswordExpirationDateType GetPasswordExpirationDate1)
		{
			this.GetPasswordExpirationDateAsync(GetPasswordExpirationDate1, null);
		}

		public void GetPasswordExpirationDateAsync(GetPasswordExpirationDateType GetPasswordExpirationDate1, object userState)
		{
			if (this.GetPasswordExpirationDateOperationCompleted == null)
			{
				this.GetPasswordExpirationDateOperationCompleted = new SendOrPostCallback(this.OnGetPasswordExpirationDateOperationCompleted);
			}
			base.InvokeAsync("GetPasswordExpirationDate", new object[]
			{
				GetPasswordExpirationDate1
			}, this.GetPasswordExpirationDateOperationCompleted, userState);
		}

		private void OnGetPasswordExpirationDateOperationCompleted(object arg)
		{
			if (this.GetPasswordExpirationDateCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.GetPasswordExpirationDateCompleted(this, new GetPasswordExpirationDateCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://schemas.microsoft.com/exchange/services/2006/messages/GetSearchableMailboxes", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Bare)]
		[SoapHeader("ServerVersionInfoValue", Direction = SoapHeaderDirection.Out)]
		[SoapHttpClientTraceExtension]
		[SoapHeader("RequestServerVersionValue")]
		[SoapHeader("ManagementRole")]
		[return: XmlElement("GetSearchableMailboxesResponse", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public GetSearchableMailboxesResponseMessageType GetSearchableMailboxes([XmlElement("GetSearchableMailboxes", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")] GetSearchableMailboxesType GetSearchableMailboxes1)
		{
			object[] array = this.Invoke("GetSearchableMailboxes", new object[]
			{
				GetSearchableMailboxes1
			});
			return (GetSearchableMailboxesResponseMessageType)array[0];
		}

		public IAsyncResult BeginGetSearchableMailboxes(GetSearchableMailboxesType GetSearchableMailboxes1, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("GetSearchableMailboxes", new object[]
			{
				GetSearchableMailboxes1
			}, callback, asyncState);
		}

		public GetSearchableMailboxesResponseMessageType EndGetSearchableMailboxes(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (GetSearchableMailboxesResponseMessageType)array[0];
		}

		public void GetSearchableMailboxesAsync(GetSearchableMailboxesType GetSearchableMailboxes1)
		{
			this.GetSearchableMailboxesAsync(GetSearchableMailboxes1, null);
		}

		public void GetSearchableMailboxesAsync(GetSearchableMailboxesType GetSearchableMailboxes1, object userState)
		{
			if (this.GetSearchableMailboxesOperationCompleted == null)
			{
				this.GetSearchableMailboxesOperationCompleted = new SendOrPostCallback(this.OnGetSearchableMailboxesOperationCompleted);
			}
			base.InvokeAsync("GetSearchableMailboxes", new object[]
			{
				GetSearchableMailboxes1
			}, this.GetSearchableMailboxesOperationCompleted, userState);
		}

		private void OnGetSearchableMailboxesOperationCompleted(object arg)
		{
			if (this.GetSearchableMailboxesCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.GetSearchableMailboxesCompleted(this, new GetSearchableMailboxesCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapHeader("ManagementRole")]
		[SoapHeader("RequestServerVersionValue")]
		[SoapDocumentMethod("http://schemas.microsoft.com/exchange/services/2006/messages/SearchMailboxes", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Bare)]
		[SoapHttpClientTraceExtension]
		[SoapHeader("ServerVersionInfoValue", Direction = SoapHeaderDirection.Out)]
		[return: XmlElement("SearchMailboxesResponse", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public SearchMailboxesResponseType SearchMailboxes([XmlElement("SearchMailboxes", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")] SearchMailboxesType SearchMailboxes1)
		{
			object[] array = this.Invoke("SearchMailboxes", new object[]
			{
				SearchMailboxes1
			});
			return (SearchMailboxesResponseType)array[0];
		}

		public IAsyncResult BeginSearchMailboxes(SearchMailboxesType SearchMailboxes1, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("SearchMailboxes", new object[]
			{
				SearchMailboxes1
			}, callback, asyncState);
		}

		public SearchMailboxesResponseType EndSearchMailboxes(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (SearchMailboxesResponseType)array[0];
		}

		public void SearchMailboxesAsync(SearchMailboxesType SearchMailboxes1)
		{
			this.SearchMailboxesAsync(SearchMailboxes1, null);
		}

		public void SearchMailboxesAsync(SearchMailboxesType SearchMailboxes1, object userState)
		{
			if (this.SearchMailboxesOperationCompleted == null)
			{
				this.SearchMailboxesOperationCompleted = new SendOrPostCallback(this.OnSearchMailboxesOperationCompleted);
			}
			base.InvokeAsync("SearchMailboxes", new object[]
			{
				SearchMailboxes1
			}, this.SearchMailboxesOperationCompleted, userState);
		}

		private void OnSearchMailboxesOperationCompleted(object arg)
		{
			if (this.SearchMailboxesCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.SearchMailboxesCompleted(this, new SearchMailboxesCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapHttpClientTraceExtension]
		[SoapHeader("RequestServerVersionValue")]
		[SoapHeader("ManagementRole")]
		[SoapHeader("ServerVersionInfoValue", Direction = SoapHeaderDirection.Out)]
		[SoapDocumentMethod("http://schemas.microsoft.com/exchange/services/2006/messages/GetDiscoverySearchConfiguration", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Bare)]
		[return: XmlElement("GetDiscoverySearchConfigurationResponse", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public GetDiscoverySearchConfigurationResponseMessageType GetDiscoverySearchConfiguration([XmlElement("GetDiscoverySearchConfiguration", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")] GetDiscoverySearchConfigurationType GetDiscoverySearchConfiguration1)
		{
			object[] array = this.Invoke("GetDiscoverySearchConfiguration", new object[]
			{
				GetDiscoverySearchConfiguration1
			});
			return (GetDiscoverySearchConfigurationResponseMessageType)array[0];
		}

		public IAsyncResult BeginGetDiscoverySearchConfiguration(GetDiscoverySearchConfigurationType GetDiscoverySearchConfiguration1, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("GetDiscoverySearchConfiguration", new object[]
			{
				GetDiscoverySearchConfiguration1
			}, callback, asyncState);
		}

		public GetDiscoverySearchConfigurationResponseMessageType EndGetDiscoverySearchConfiguration(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (GetDiscoverySearchConfigurationResponseMessageType)array[0];
		}

		public void GetDiscoverySearchConfigurationAsync(GetDiscoverySearchConfigurationType GetDiscoverySearchConfiguration1)
		{
			this.GetDiscoverySearchConfigurationAsync(GetDiscoverySearchConfiguration1, null);
		}

		public void GetDiscoverySearchConfigurationAsync(GetDiscoverySearchConfigurationType GetDiscoverySearchConfiguration1, object userState)
		{
			if (this.GetDiscoverySearchConfigurationOperationCompleted == null)
			{
				this.GetDiscoverySearchConfigurationOperationCompleted = new SendOrPostCallback(this.OnGetDiscoverySearchConfigurationOperationCompleted);
			}
			base.InvokeAsync("GetDiscoverySearchConfiguration", new object[]
			{
				GetDiscoverySearchConfiguration1
			}, this.GetDiscoverySearchConfigurationOperationCompleted, userState);
		}

		private void OnGetDiscoverySearchConfigurationOperationCompleted(object arg)
		{
			if (this.GetDiscoverySearchConfigurationCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.GetDiscoverySearchConfigurationCompleted(this, new GetDiscoverySearchConfigurationCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapHeader("ServerVersionInfoValue", Direction = SoapHeaderDirection.Out)]
		[SoapHttpClientTraceExtension]
		[SoapDocumentMethod("http://schemas.microsoft.com/exchange/services/2006/messages/GetHoldOnMailboxes", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Bare)]
		[SoapHeader("RequestServerVersionValue")]
		[SoapHeader("ManagementRole")]
		[return: XmlElement("GetHoldOnMailboxesResponse", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public GetHoldOnMailboxesResponseMessageType GetHoldOnMailboxes([XmlElement("GetHoldOnMailboxes", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")] GetHoldOnMailboxesType GetHoldOnMailboxes1)
		{
			object[] array = this.Invoke("GetHoldOnMailboxes", new object[]
			{
				GetHoldOnMailboxes1
			});
			return (GetHoldOnMailboxesResponseMessageType)array[0];
		}

		public IAsyncResult BeginGetHoldOnMailboxes(GetHoldOnMailboxesType GetHoldOnMailboxes1, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("GetHoldOnMailboxes", new object[]
			{
				GetHoldOnMailboxes1
			}, callback, asyncState);
		}

		public GetHoldOnMailboxesResponseMessageType EndGetHoldOnMailboxes(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (GetHoldOnMailboxesResponseMessageType)array[0];
		}

		public void GetHoldOnMailboxesAsync(GetHoldOnMailboxesType GetHoldOnMailboxes1)
		{
			this.GetHoldOnMailboxesAsync(GetHoldOnMailboxes1, null);
		}

		public void GetHoldOnMailboxesAsync(GetHoldOnMailboxesType GetHoldOnMailboxes1, object userState)
		{
			if (this.GetHoldOnMailboxesOperationCompleted == null)
			{
				this.GetHoldOnMailboxesOperationCompleted = new SendOrPostCallback(this.OnGetHoldOnMailboxesOperationCompleted);
			}
			base.InvokeAsync("GetHoldOnMailboxes", new object[]
			{
				GetHoldOnMailboxes1
			}, this.GetHoldOnMailboxesOperationCompleted, userState);
		}

		private void OnGetHoldOnMailboxesOperationCompleted(object arg)
		{
			if (this.GetHoldOnMailboxesCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.GetHoldOnMailboxesCompleted(this, new GetHoldOnMailboxesCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapHeader("RequestServerVersionValue")]
		[SoapHeader("ServerVersionInfoValue", Direction = SoapHeaderDirection.Out)]
		[SoapDocumentMethod("http://schemas.microsoft.com/exchange/services/2006/messages/SetHoldOnMailboxes", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Bare)]
		[SoapHttpClientTraceExtension]
		[SoapHeader("ManagementRole")]
		[return: XmlElement("SetHoldOnMailboxesResponse", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public SetHoldOnMailboxesResponseMessageType SetHoldOnMailboxes([XmlElement("SetHoldOnMailboxes", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")] SetHoldOnMailboxesType SetHoldOnMailboxes1)
		{
			object[] array = this.Invoke("SetHoldOnMailboxes", new object[]
			{
				SetHoldOnMailboxes1
			});
			return (SetHoldOnMailboxesResponseMessageType)array[0];
		}

		public IAsyncResult BeginSetHoldOnMailboxes(SetHoldOnMailboxesType SetHoldOnMailboxes1, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("SetHoldOnMailboxes", new object[]
			{
				SetHoldOnMailboxes1
			}, callback, asyncState);
		}

		public SetHoldOnMailboxesResponseMessageType EndSetHoldOnMailboxes(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (SetHoldOnMailboxesResponseMessageType)array[0];
		}

		public void SetHoldOnMailboxesAsync(SetHoldOnMailboxesType SetHoldOnMailboxes1)
		{
			this.SetHoldOnMailboxesAsync(SetHoldOnMailboxes1, null);
		}

		public void SetHoldOnMailboxesAsync(SetHoldOnMailboxesType SetHoldOnMailboxes1, object userState)
		{
			if (this.SetHoldOnMailboxesOperationCompleted == null)
			{
				this.SetHoldOnMailboxesOperationCompleted = new SendOrPostCallback(this.OnSetHoldOnMailboxesOperationCompleted);
			}
			base.InvokeAsync("SetHoldOnMailboxes", new object[]
			{
				SetHoldOnMailboxes1
			}, this.SetHoldOnMailboxesOperationCompleted, userState);
		}

		private void OnSetHoldOnMailboxesOperationCompleted(object arg)
		{
			if (this.SetHoldOnMailboxesCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.SetHoldOnMailboxesCompleted(this, new SetHoldOnMailboxesCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapHttpClientTraceExtension]
		[SoapHeader("RequestServerVersionValue")]
		[SoapHeader("ServerVersionInfoValue", Direction = SoapHeaderDirection.Out)]
		[SoapDocumentMethod("http://schemas.microsoft.com/exchange/services/2006/messages/GetNonIndexableItemStatistics", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Bare)]
		[SoapHeader("ManagementRole")]
		[return: XmlElement("GetNonIndexableItemStatisticsResponse", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public GetNonIndexableItemStatisticsResponseMessageType GetNonIndexableItemStatistics([XmlElement("GetNonIndexableItemStatistics", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")] GetNonIndexableItemStatisticsType GetNonIndexableItemStatistics1)
		{
			object[] array = this.Invoke("GetNonIndexableItemStatistics", new object[]
			{
				GetNonIndexableItemStatistics1
			});
			return (GetNonIndexableItemStatisticsResponseMessageType)array[0];
		}

		public IAsyncResult BeginGetNonIndexableItemStatistics(GetNonIndexableItemStatisticsType GetNonIndexableItemStatistics1, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("GetNonIndexableItemStatistics", new object[]
			{
				GetNonIndexableItemStatistics1
			}, callback, asyncState);
		}

		public GetNonIndexableItemStatisticsResponseMessageType EndGetNonIndexableItemStatistics(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (GetNonIndexableItemStatisticsResponseMessageType)array[0];
		}

		public void GetNonIndexableItemStatisticsAsync(GetNonIndexableItemStatisticsType GetNonIndexableItemStatistics1)
		{
			this.GetNonIndexableItemStatisticsAsync(GetNonIndexableItemStatistics1, null);
		}

		public void GetNonIndexableItemStatisticsAsync(GetNonIndexableItemStatisticsType GetNonIndexableItemStatistics1, object userState)
		{
			if (this.GetNonIndexableItemStatisticsOperationCompleted == null)
			{
				this.GetNonIndexableItemStatisticsOperationCompleted = new SendOrPostCallback(this.OnGetNonIndexableItemStatisticsOperationCompleted);
			}
			base.InvokeAsync("GetNonIndexableItemStatistics", new object[]
			{
				GetNonIndexableItemStatistics1
			}, this.GetNonIndexableItemStatisticsOperationCompleted, userState);
		}

		private void OnGetNonIndexableItemStatisticsOperationCompleted(object arg)
		{
			if (this.GetNonIndexableItemStatisticsCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.GetNonIndexableItemStatisticsCompleted(this, new GetNonIndexableItemStatisticsCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapHttpClientTraceExtension]
		[SoapHeader("RequestServerVersionValue")]
		[SoapHeader("ManagementRole")]
		[SoapHeader("ServerVersionInfoValue", Direction = SoapHeaderDirection.Out)]
		[SoapDocumentMethod("http://schemas.microsoft.com/exchange/services/2006/messages/GetNonIndexableItemDetails", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Bare)]
		[return: XmlElement("GetNonIndexableItemDetailsResponse", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public GetNonIndexableItemDetailsResponseMessageType GetNonIndexableItemDetails([XmlElement("GetNonIndexableItemDetails", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")] GetNonIndexableItemDetailsType GetNonIndexableItemDetails1)
		{
			object[] array = this.Invoke("GetNonIndexableItemDetails", new object[]
			{
				GetNonIndexableItemDetails1
			});
			return (GetNonIndexableItemDetailsResponseMessageType)array[0];
		}

		public IAsyncResult BeginGetNonIndexableItemDetails(GetNonIndexableItemDetailsType GetNonIndexableItemDetails1, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("GetNonIndexableItemDetails", new object[]
			{
				GetNonIndexableItemDetails1
			}, callback, asyncState);
		}

		public GetNonIndexableItemDetailsResponseMessageType EndGetNonIndexableItemDetails(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (GetNonIndexableItemDetailsResponseMessageType)array[0];
		}

		public void GetNonIndexableItemDetailsAsync(GetNonIndexableItemDetailsType GetNonIndexableItemDetails1)
		{
			this.GetNonIndexableItemDetailsAsync(GetNonIndexableItemDetails1, null);
		}

		public void GetNonIndexableItemDetailsAsync(GetNonIndexableItemDetailsType GetNonIndexableItemDetails1, object userState)
		{
			if (this.GetNonIndexableItemDetailsOperationCompleted == null)
			{
				this.GetNonIndexableItemDetailsOperationCompleted = new SendOrPostCallback(this.OnGetNonIndexableItemDetailsOperationCompleted);
			}
			base.InvokeAsync("GetNonIndexableItemDetails", new object[]
			{
				GetNonIndexableItemDetails1
			}, this.GetNonIndexableItemDetailsOperationCompleted, userState);
		}

		private void OnGetNonIndexableItemDetailsOperationCompleted(object arg)
		{
			if (this.GetNonIndexableItemDetailsCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.GetNonIndexableItemDetailsCompleted(this, new GetNonIndexableItemDetailsCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapHeader("RequestServerVersionValue")]
		[SoapHttpClientTraceExtension]
		[SoapDocumentMethod("http://schemas.microsoft.com/exchange/services/2006/messages/MarkAllItemsAsRead", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Bare)]
		[SoapHeader("MailboxCulture")]
		[SoapHeader("ServerVersionInfoValue", Direction = SoapHeaderDirection.Out)]
		[SoapHeader("ExchangeImpersonation")]
		[return: XmlElement("MarkAllItemsAsReadResponse", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public MarkAllItemsAsReadResponseType MarkAllItemsAsRead([XmlElement("MarkAllItemsAsRead", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")] MarkAllItemsAsReadType MarkAllItemsAsRead1)
		{
			object[] array = this.Invoke("MarkAllItemsAsRead", new object[]
			{
				MarkAllItemsAsRead1
			});
			return (MarkAllItemsAsReadResponseType)array[0];
		}

		public IAsyncResult BeginMarkAllItemsAsRead(MarkAllItemsAsReadType MarkAllItemsAsRead1, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("MarkAllItemsAsRead", new object[]
			{
				MarkAllItemsAsRead1
			}, callback, asyncState);
		}

		public MarkAllItemsAsReadResponseType EndMarkAllItemsAsRead(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (MarkAllItemsAsReadResponseType)array[0];
		}

		public void MarkAllItemsAsReadAsync(MarkAllItemsAsReadType MarkAllItemsAsRead1)
		{
			this.MarkAllItemsAsReadAsync(MarkAllItemsAsRead1, null);
		}

		public void MarkAllItemsAsReadAsync(MarkAllItemsAsReadType MarkAllItemsAsRead1, object userState)
		{
			if (this.MarkAllItemsAsReadOperationCompleted == null)
			{
				this.MarkAllItemsAsReadOperationCompleted = new SendOrPostCallback(this.OnMarkAllItemsAsReadOperationCompleted);
			}
			base.InvokeAsync("MarkAllItemsAsRead", new object[]
			{
				MarkAllItemsAsRead1
			}, this.MarkAllItemsAsReadOperationCompleted, userState);
		}

		private void OnMarkAllItemsAsReadOperationCompleted(object arg)
		{
			if (this.MarkAllItemsAsReadCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.MarkAllItemsAsReadCompleted(this, new MarkAllItemsAsReadCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapHeader("RequestServerVersionValue")]
		[SoapHeader("MailboxCulture")]
		[SoapHttpClientTraceExtension]
		[SoapHeader("ServerVersionInfoValue", Direction = SoapHeaderDirection.Out)]
		[SoapHeader("ExchangeImpersonation")]
		[SoapDocumentMethod("http://schemas.microsoft.com/exchange/services/2006/messages/MarkAsJunk", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Bare)]
		[return: XmlElement("MarkAsJunkResponse", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public MarkAsJunkResponseType MarkAsJunk([XmlElement("MarkAsJunk", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")] MarkAsJunkType MarkAsJunk1)
		{
			object[] array = this.Invoke("MarkAsJunk", new object[]
			{
				MarkAsJunk1
			});
			return (MarkAsJunkResponseType)array[0];
		}

		public IAsyncResult BeginMarkAsJunk(MarkAsJunkType MarkAsJunk1, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("MarkAsJunk", new object[]
			{
				MarkAsJunk1
			}, callback, asyncState);
		}

		public MarkAsJunkResponseType EndMarkAsJunk(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (MarkAsJunkResponseType)array[0];
		}

		public void MarkAsJunkAsync(MarkAsJunkType MarkAsJunk1)
		{
			this.MarkAsJunkAsync(MarkAsJunk1, null);
		}

		public void MarkAsJunkAsync(MarkAsJunkType MarkAsJunk1, object userState)
		{
			if (this.MarkAsJunkOperationCompleted == null)
			{
				this.MarkAsJunkOperationCompleted = new SendOrPostCallback(this.OnMarkAsJunkOperationCompleted);
			}
			base.InvokeAsync("MarkAsJunk", new object[]
			{
				MarkAsJunk1
			}, this.MarkAsJunkOperationCompleted, userState);
		}

		private void OnMarkAsJunkOperationCompleted(object arg)
		{
			if (this.MarkAsJunkCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.MarkAsJunkCompleted(this, new MarkAsJunkCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapHeader("RequestServerVersionValue")]
		[SoapDocumentMethod("http://schemas.microsoft.com/exchange/services/2006/messages/GetAppManifests", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Bare)]
		[SoapHttpClientTraceExtension]
		[SoapHeader("ServerVersionInfoValue", Direction = SoapHeaderDirection.Out)]
		[return: XmlElement("GetAppManifestsResponse", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public GetAppManifestsResponseType GetAppManifests([XmlElement("GetAppManifests", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")] GetAppManifestsType GetAppManifests1)
		{
			object[] array = this.Invoke("GetAppManifests", new object[]
			{
				GetAppManifests1
			});
			return (GetAppManifestsResponseType)array[0];
		}

		public IAsyncResult BeginGetAppManifests(GetAppManifestsType GetAppManifests1, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("GetAppManifests", new object[]
			{
				GetAppManifests1
			}, callback, asyncState);
		}

		public GetAppManifestsResponseType EndGetAppManifests(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (GetAppManifestsResponseType)array[0];
		}

		public void GetAppManifestsAsync(GetAppManifestsType GetAppManifests1)
		{
			this.GetAppManifestsAsync(GetAppManifests1, null);
		}

		public void GetAppManifestsAsync(GetAppManifestsType GetAppManifests1, object userState)
		{
			if (this.GetAppManifestsOperationCompleted == null)
			{
				this.GetAppManifestsOperationCompleted = new SendOrPostCallback(this.OnGetAppManifestsOperationCompleted);
			}
			base.InvokeAsync("GetAppManifests", new object[]
			{
				GetAppManifests1
			}, this.GetAppManifestsOperationCompleted, userState);
		}

		private void OnGetAppManifestsOperationCompleted(object arg)
		{
			if (this.GetAppManifestsCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.GetAppManifestsCompleted(this, new GetAppManifestsCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://schemas.microsoft.com/exchange/services/2006/messages/AddNewImContactToGroup", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Bare)]
		[SoapHttpClientTraceExtension]
		[SoapHeader("RequestServerVersionValue")]
		[SoapHeader("MailboxCulture")]
		[SoapHeader("ServerVersionInfoValue", Direction = SoapHeaderDirection.Out)]
		[SoapHeader("ExchangeImpersonation")]
		[return: XmlElement("AddNewImContactToGroupResponse", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public AddNewImContactToGroupResponseMessageType AddNewImContactToGroup([XmlElement("AddNewImContactToGroup", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")] AddNewImContactToGroupType AddNewImContactToGroup1)
		{
			object[] array = this.Invoke("AddNewImContactToGroup", new object[]
			{
				AddNewImContactToGroup1
			});
			return (AddNewImContactToGroupResponseMessageType)array[0];
		}

		public IAsyncResult BeginAddNewImContactToGroup(AddNewImContactToGroupType AddNewImContactToGroup1, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("AddNewImContactToGroup", new object[]
			{
				AddNewImContactToGroup1
			}, callback, asyncState);
		}

		public AddNewImContactToGroupResponseMessageType EndAddNewImContactToGroup(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (AddNewImContactToGroupResponseMessageType)array[0];
		}

		public void AddNewImContactToGroupAsync(AddNewImContactToGroupType AddNewImContactToGroup1)
		{
			this.AddNewImContactToGroupAsync(AddNewImContactToGroup1, null);
		}

		public void AddNewImContactToGroupAsync(AddNewImContactToGroupType AddNewImContactToGroup1, object userState)
		{
			if (this.AddNewImContactToGroupOperationCompleted == null)
			{
				this.AddNewImContactToGroupOperationCompleted = new SendOrPostCallback(this.OnAddNewImContactToGroupOperationCompleted);
			}
			base.InvokeAsync("AddNewImContactToGroup", new object[]
			{
				AddNewImContactToGroup1
			}, this.AddNewImContactToGroupOperationCompleted, userState);
		}

		private void OnAddNewImContactToGroupOperationCompleted(object arg)
		{
			if (this.AddNewImContactToGroupCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.AddNewImContactToGroupCompleted(this, new AddNewImContactToGroupCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapHeader("RequestServerVersionValue")]
		[SoapHttpClientTraceExtension]
		[SoapHeader("MailboxCulture")]
		[SoapHeader("ServerVersionInfoValue", Direction = SoapHeaderDirection.Out)]
		[SoapHeader("ExchangeImpersonation")]
		[SoapDocumentMethod("http://schemas.microsoft.com/exchange/services/2006/messages/AddNewTelUriContactToGroup", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Bare)]
		[return: XmlElement("AddNewTelUriContactToGroupResponse", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public AddNewTelUriContactToGroupResponseMessageType AddNewTelUriContactToGroup([XmlElement("AddNewTelUriContactToGroup", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")] AddNewTelUriContactToGroupType AddNewTelUriContactToGroup1)
		{
			object[] array = this.Invoke("AddNewTelUriContactToGroup", new object[]
			{
				AddNewTelUriContactToGroup1
			});
			return (AddNewTelUriContactToGroupResponseMessageType)array[0];
		}

		public IAsyncResult BeginAddNewTelUriContactToGroup(AddNewTelUriContactToGroupType AddNewTelUriContactToGroup1, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("AddNewTelUriContactToGroup", new object[]
			{
				AddNewTelUriContactToGroup1
			}, callback, asyncState);
		}

		public AddNewTelUriContactToGroupResponseMessageType EndAddNewTelUriContactToGroup(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (AddNewTelUriContactToGroupResponseMessageType)array[0];
		}

		public void AddNewTelUriContactToGroupAsync(AddNewTelUriContactToGroupType AddNewTelUriContactToGroup1)
		{
			this.AddNewTelUriContactToGroupAsync(AddNewTelUriContactToGroup1, null);
		}

		public void AddNewTelUriContactToGroupAsync(AddNewTelUriContactToGroupType AddNewTelUriContactToGroup1, object userState)
		{
			if (this.AddNewTelUriContactToGroupOperationCompleted == null)
			{
				this.AddNewTelUriContactToGroupOperationCompleted = new SendOrPostCallback(this.OnAddNewTelUriContactToGroupOperationCompleted);
			}
			base.InvokeAsync("AddNewTelUriContactToGroup", new object[]
			{
				AddNewTelUriContactToGroup1
			}, this.AddNewTelUriContactToGroupOperationCompleted, userState);
		}

		private void OnAddNewTelUriContactToGroupOperationCompleted(object arg)
		{
			if (this.AddNewTelUriContactToGroupCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.AddNewTelUriContactToGroupCompleted(this, new AddNewTelUriContactToGroupCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapHeader("MailboxCulture")]
		[SoapHttpClientTraceExtension]
		[SoapHeader("ExchangeImpersonation")]
		[SoapDocumentMethod("http://schemas.microsoft.com/exchange/services/2006/messages/AddImContactToGroup", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Bare)]
		[SoapHeader("RequestServerVersionValue")]
		[SoapHeader("ServerVersionInfoValue", Direction = SoapHeaderDirection.Out)]
		[return: XmlElement("AddImContactToGroupResponse", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public AddImContactToGroupResponseMessageType AddImContactToGroup([XmlElement("AddImContactToGroup", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")] AddImContactToGroupType AddImContactToGroup1)
		{
			object[] array = this.Invoke("AddImContactToGroup", new object[]
			{
				AddImContactToGroup1
			});
			return (AddImContactToGroupResponseMessageType)array[0];
		}

		public IAsyncResult BeginAddImContactToGroup(AddImContactToGroupType AddImContactToGroup1, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("AddImContactToGroup", new object[]
			{
				AddImContactToGroup1
			}, callback, asyncState);
		}

		public AddImContactToGroupResponseMessageType EndAddImContactToGroup(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (AddImContactToGroupResponseMessageType)array[0];
		}

		public void AddImContactToGroupAsync(AddImContactToGroupType AddImContactToGroup1)
		{
			this.AddImContactToGroupAsync(AddImContactToGroup1, null);
		}

		public void AddImContactToGroupAsync(AddImContactToGroupType AddImContactToGroup1, object userState)
		{
			if (this.AddImContactToGroupOperationCompleted == null)
			{
				this.AddImContactToGroupOperationCompleted = new SendOrPostCallback(this.OnAddImContactToGroupOperationCompleted);
			}
			base.InvokeAsync("AddImContactToGroup", new object[]
			{
				AddImContactToGroup1
			}, this.AddImContactToGroupOperationCompleted, userState);
		}

		private void OnAddImContactToGroupOperationCompleted(object arg)
		{
			if (this.AddImContactToGroupCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.AddImContactToGroupCompleted(this, new AddImContactToGroupCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapHeader("RequestServerVersionValue")]
		[SoapHeader("ServerVersionInfoValue", Direction = SoapHeaderDirection.Out)]
		[SoapHeader("MailboxCulture")]
		[SoapHeader("ExchangeImpersonation")]
		[SoapHttpClientTraceExtension]
		[SoapDocumentMethod("http://schemas.microsoft.com/exchange/services/2006/messages/RemoveImContactFromGroup", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Bare)]
		[return: XmlElement("RemoveImContactFromGroupResponse", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public RemoveImContactFromGroupResponseMessageType RemoveImContactFromGroup([XmlElement("RemoveImContactFromGroup", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")] RemoveImContactFromGroupType RemoveImContactFromGroup1)
		{
			object[] array = this.Invoke("RemoveImContactFromGroup", new object[]
			{
				RemoveImContactFromGroup1
			});
			return (RemoveImContactFromGroupResponseMessageType)array[0];
		}

		public IAsyncResult BeginRemoveImContactFromGroup(RemoveImContactFromGroupType RemoveImContactFromGroup1, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("RemoveImContactFromGroup", new object[]
			{
				RemoveImContactFromGroup1
			}, callback, asyncState);
		}

		public RemoveImContactFromGroupResponseMessageType EndRemoveImContactFromGroup(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (RemoveImContactFromGroupResponseMessageType)array[0];
		}

		public void RemoveImContactFromGroupAsync(RemoveImContactFromGroupType RemoveImContactFromGroup1)
		{
			this.RemoveImContactFromGroupAsync(RemoveImContactFromGroup1, null);
		}

		public void RemoveImContactFromGroupAsync(RemoveImContactFromGroupType RemoveImContactFromGroup1, object userState)
		{
			if (this.RemoveImContactFromGroupOperationCompleted == null)
			{
				this.RemoveImContactFromGroupOperationCompleted = new SendOrPostCallback(this.OnRemoveImContactFromGroupOperationCompleted);
			}
			base.InvokeAsync("RemoveImContactFromGroup", new object[]
			{
				RemoveImContactFromGroup1
			}, this.RemoveImContactFromGroupOperationCompleted, userState);
		}

		private void OnRemoveImContactFromGroupOperationCompleted(object arg)
		{
			if (this.RemoveImContactFromGroupCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.RemoveImContactFromGroupCompleted(this, new RemoveImContactFromGroupCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapHeader("MailboxCulture")]
		[SoapHeader("ServerVersionInfoValue", Direction = SoapHeaderDirection.Out)]
		[SoapHeader("RequestServerVersionValue")]
		[SoapHeader("ExchangeImpersonation")]
		[SoapDocumentMethod("http://schemas.microsoft.com/exchange/services/2006/messages/AddImGroup", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Bare)]
		[SoapHttpClientTraceExtension]
		[return: XmlElement("AddImGroupResponse", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public AddImGroupResponseMessageType AddImGroup([XmlElement("AddImGroup", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")] AddImGroupType AddImGroup1)
		{
			object[] array = this.Invoke("AddImGroup", new object[]
			{
				AddImGroup1
			});
			return (AddImGroupResponseMessageType)array[0];
		}

		public IAsyncResult BeginAddImGroup(AddImGroupType AddImGroup1, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("AddImGroup", new object[]
			{
				AddImGroup1
			}, callback, asyncState);
		}

		public AddImGroupResponseMessageType EndAddImGroup(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (AddImGroupResponseMessageType)array[0];
		}

		public void AddImGroupAsync(AddImGroupType AddImGroup1)
		{
			this.AddImGroupAsync(AddImGroup1, null);
		}

		public void AddImGroupAsync(AddImGroupType AddImGroup1, object userState)
		{
			if (this.AddImGroupOperationCompleted == null)
			{
				this.AddImGroupOperationCompleted = new SendOrPostCallback(this.OnAddImGroupOperationCompleted);
			}
			base.InvokeAsync("AddImGroup", new object[]
			{
				AddImGroup1
			}, this.AddImGroupOperationCompleted, userState);
		}

		private void OnAddImGroupOperationCompleted(object arg)
		{
			if (this.AddImGroupCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.AddImGroupCompleted(this, new AddImGroupCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapHttpClientTraceExtension]
		[SoapHeader("RequestServerVersionValue")]
		[SoapHeader("ExchangeImpersonation")]
		[SoapDocumentMethod("http://schemas.microsoft.com/exchange/services/2006/messages/AddDistributionGroupToImList", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Bare)]
		[SoapHeader("MailboxCulture")]
		[SoapHeader("ServerVersionInfoValue", Direction = SoapHeaderDirection.Out)]
		[return: XmlElement("AddDistributionGroupToImListResponse", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public AddDistributionGroupToImListResponseMessageType AddDistributionGroupToImList([XmlElement("AddDistributionGroupToImList", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")] AddDistributionGroupToImListType AddDistributionGroupToImList1)
		{
			object[] array = this.Invoke("AddDistributionGroupToImList", new object[]
			{
				AddDistributionGroupToImList1
			});
			return (AddDistributionGroupToImListResponseMessageType)array[0];
		}

		public IAsyncResult BeginAddDistributionGroupToImList(AddDistributionGroupToImListType AddDistributionGroupToImList1, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("AddDistributionGroupToImList", new object[]
			{
				AddDistributionGroupToImList1
			}, callback, asyncState);
		}

		public AddDistributionGroupToImListResponseMessageType EndAddDistributionGroupToImList(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (AddDistributionGroupToImListResponseMessageType)array[0];
		}

		public void AddDistributionGroupToImListAsync(AddDistributionGroupToImListType AddDistributionGroupToImList1)
		{
			this.AddDistributionGroupToImListAsync(AddDistributionGroupToImList1, null);
		}

		public void AddDistributionGroupToImListAsync(AddDistributionGroupToImListType AddDistributionGroupToImList1, object userState)
		{
			if (this.AddDistributionGroupToImListOperationCompleted == null)
			{
				this.AddDistributionGroupToImListOperationCompleted = new SendOrPostCallback(this.OnAddDistributionGroupToImListOperationCompleted);
			}
			base.InvokeAsync("AddDistributionGroupToImList", new object[]
			{
				AddDistributionGroupToImList1
			}, this.AddDistributionGroupToImListOperationCompleted, userState);
		}

		private void OnAddDistributionGroupToImListOperationCompleted(object arg)
		{
			if (this.AddDistributionGroupToImListCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.AddDistributionGroupToImListCompleted(this, new AddDistributionGroupToImListCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapHeader("RequestServerVersionValue")]
		[SoapHttpClientTraceExtension]
		[SoapHeader("ServerVersionInfoValue", Direction = SoapHeaderDirection.Out)]
		[SoapHeader("ExchangeImpersonation")]
		[SoapHeader("MailboxCulture")]
		[SoapDocumentMethod("http://schemas.microsoft.com/exchange/services/2006/messages/GetImItemList", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Bare)]
		[return: XmlElement("GetImItemListResponse", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public GetImItemListResponseMessageType GetImItemList([XmlElement("GetImItemList", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")] GetImItemListType GetImItemList1)
		{
			object[] array = this.Invoke("GetImItemList", new object[]
			{
				GetImItemList1
			});
			return (GetImItemListResponseMessageType)array[0];
		}

		public IAsyncResult BeginGetImItemList(GetImItemListType GetImItemList1, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("GetImItemList", new object[]
			{
				GetImItemList1
			}, callback, asyncState);
		}

		public GetImItemListResponseMessageType EndGetImItemList(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (GetImItemListResponseMessageType)array[0];
		}

		public void GetImItemListAsync(GetImItemListType GetImItemList1)
		{
			this.GetImItemListAsync(GetImItemList1, null);
		}

		public void GetImItemListAsync(GetImItemListType GetImItemList1, object userState)
		{
			if (this.GetImItemListOperationCompleted == null)
			{
				this.GetImItemListOperationCompleted = new SendOrPostCallback(this.OnGetImItemListOperationCompleted);
			}
			base.InvokeAsync("GetImItemList", new object[]
			{
				GetImItemList1
			}, this.GetImItemListOperationCompleted, userState);
		}

		private void OnGetImItemListOperationCompleted(object arg)
		{
			if (this.GetImItemListCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.GetImItemListCompleted(this, new GetImItemListCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapDocumentMethod("http://schemas.microsoft.com/exchange/services/2006/messages/GetImItems", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Bare)]
		[SoapHeader("ExchangeImpersonation")]
		[SoapHeader("MailboxCulture")]
		[SoapHttpClientTraceExtension]
		[SoapHeader("RequestServerVersionValue")]
		[SoapHeader("ServerVersionInfoValue", Direction = SoapHeaderDirection.Out)]
		[return: XmlElement("GetImItemsResponse", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public GetImItemsResponseMessageType GetImItems([XmlElement("GetImItems", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")] GetImItemsType GetImItems1)
		{
			object[] array = this.Invoke("GetImItems", new object[]
			{
				GetImItems1
			});
			return (GetImItemsResponseMessageType)array[0];
		}

		public IAsyncResult BeginGetImItems(GetImItemsType GetImItems1, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("GetImItems", new object[]
			{
				GetImItems1
			}, callback, asyncState);
		}

		public GetImItemsResponseMessageType EndGetImItems(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (GetImItemsResponseMessageType)array[0];
		}

		public void GetImItemsAsync(GetImItemsType GetImItems1)
		{
			this.GetImItemsAsync(GetImItems1, null);
		}

		public void GetImItemsAsync(GetImItemsType GetImItems1, object userState)
		{
			if (this.GetImItemsOperationCompleted == null)
			{
				this.GetImItemsOperationCompleted = new SendOrPostCallback(this.OnGetImItemsOperationCompleted);
			}
			base.InvokeAsync("GetImItems", new object[]
			{
				GetImItems1
			}, this.GetImItemsOperationCompleted, userState);
		}

		private void OnGetImItemsOperationCompleted(object arg)
		{
			if (this.GetImItemsCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.GetImItemsCompleted(this, new GetImItemsCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapHeader("MailboxCulture")]
		[SoapHttpClientTraceExtension]
		[SoapDocumentMethod("http://schemas.microsoft.com/exchange/services/2006/messages/RemoveContactFromImList", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Bare)]
		[SoapHeader("ServerVersionInfoValue", Direction = SoapHeaderDirection.Out)]
		[SoapHeader("ExchangeImpersonation")]
		[SoapHeader("RequestServerVersionValue")]
		[return: XmlElement("RemoveContactFromImListResponse", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public RemoveContactFromImListResponseMessageType RemoveContactFromImList([XmlElement("RemoveContactFromImList", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")] RemoveContactFromImListType RemoveContactFromImList1)
		{
			object[] array = this.Invoke("RemoveContactFromImList", new object[]
			{
				RemoveContactFromImList1
			});
			return (RemoveContactFromImListResponseMessageType)array[0];
		}

		public IAsyncResult BeginRemoveContactFromImList(RemoveContactFromImListType RemoveContactFromImList1, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("RemoveContactFromImList", new object[]
			{
				RemoveContactFromImList1
			}, callback, asyncState);
		}

		public RemoveContactFromImListResponseMessageType EndRemoveContactFromImList(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (RemoveContactFromImListResponseMessageType)array[0];
		}

		public void RemoveContactFromImListAsync(RemoveContactFromImListType RemoveContactFromImList1)
		{
			this.RemoveContactFromImListAsync(RemoveContactFromImList1, null);
		}

		public void RemoveContactFromImListAsync(RemoveContactFromImListType RemoveContactFromImList1, object userState)
		{
			if (this.RemoveContactFromImListOperationCompleted == null)
			{
				this.RemoveContactFromImListOperationCompleted = new SendOrPostCallback(this.OnRemoveContactFromImListOperationCompleted);
			}
			base.InvokeAsync("RemoveContactFromImList", new object[]
			{
				RemoveContactFromImList1
			}, this.RemoveContactFromImListOperationCompleted, userState);
		}

		private void OnRemoveContactFromImListOperationCompleted(object arg)
		{
			if (this.RemoveContactFromImListCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.RemoveContactFromImListCompleted(this, new RemoveContactFromImListCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapHeader("RequestServerVersionValue")]
		[SoapHeader("ServerVersionInfoValue", Direction = SoapHeaderDirection.Out)]
		[SoapHeader("MailboxCulture")]
		[SoapHeader("ExchangeImpersonation")]
		[SoapDocumentMethod("http://schemas.microsoft.com/exchange/services/2006/messages/RemoveDistributionGroupFromImList", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Bare)]
		[SoapHttpClientTraceExtension]
		[return: XmlElement("RemoveDistributionGroupFromImListResponse", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public RemoveDistributionGroupFromImListResponseMessageType RemoveDistributionGroupFromImList([XmlElement("RemoveDistributionGroupFromImList", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")] RemoveDistributionGroupFromImListType RemoveDistributionGroupFromImList1)
		{
			object[] array = this.Invoke("RemoveDistributionGroupFromImList", new object[]
			{
				RemoveDistributionGroupFromImList1
			});
			return (RemoveDistributionGroupFromImListResponseMessageType)array[0];
		}

		public IAsyncResult BeginRemoveDistributionGroupFromImList(RemoveDistributionGroupFromImListType RemoveDistributionGroupFromImList1, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("RemoveDistributionGroupFromImList", new object[]
			{
				RemoveDistributionGroupFromImList1
			}, callback, asyncState);
		}

		public RemoveDistributionGroupFromImListResponseMessageType EndRemoveDistributionGroupFromImList(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (RemoveDistributionGroupFromImListResponseMessageType)array[0];
		}

		public void RemoveDistributionGroupFromImListAsync(RemoveDistributionGroupFromImListType RemoveDistributionGroupFromImList1)
		{
			this.RemoveDistributionGroupFromImListAsync(RemoveDistributionGroupFromImList1, null);
		}

		public void RemoveDistributionGroupFromImListAsync(RemoveDistributionGroupFromImListType RemoveDistributionGroupFromImList1, object userState)
		{
			if (this.RemoveDistributionGroupFromImListOperationCompleted == null)
			{
				this.RemoveDistributionGroupFromImListOperationCompleted = new SendOrPostCallback(this.OnRemoveDistributionGroupFromImListOperationCompleted);
			}
			base.InvokeAsync("RemoveDistributionGroupFromImList", new object[]
			{
				RemoveDistributionGroupFromImList1
			}, this.RemoveDistributionGroupFromImListOperationCompleted, userState);
		}

		private void OnRemoveDistributionGroupFromImListOperationCompleted(object arg)
		{
			if (this.RemoveDistributionGroupFromImListCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.RemoveDistributionGroupFromImListCompleted(this, new RemoveDistributionGroupFromImListCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapHeader("RequestServerVersionValue")]
		[SoapHeader("ExchangeImpersonation")]
		[SoapHeader("ServerVersionInfoValue", Direction = SoapHeaderDirection.Out)]
		[SoapHttpClientTraceExtension]
		[SoapHeader("MailboxCulture")]
		[SoapDocumentMethod("http://schemas.microsoft.com/exchange/services/2006/messages/RemoveImGroup", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Bare)]
		[return: XmlElement("RemoveImGroupResponse", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public RemoveImGroupResponseMessageType RemoveImGroup([XmlElement("RemoveImGroup", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")] RemoveImGroupType RemoveImGroup1)
		{
			object[] array = this.Invoke("RemoveImGroup", new object[]
			{
				RemoveImGroup1
			});
			return (RemoveImGroupResponseMessageType)array[0];
		}

		public IAsyncResult BeginRemoveImGroup(RemoveImGroupType RemoveImGroup1, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("RemoveImGroup", new object[]
			{
				RemoveImGroup1
			}, callback, asyncState);
		}

		public RemoveImGroupResponseMessageType EndRemoveImGroup(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (RemoveImGroupResponseMessageType)array[0];
		}

		public void RemoveImGroupAsync(RemoveImGroupType RemoveImGroup1)
		{
			this.RemoveImGroupAsync(RemoveImGroup1, null);
		}

		public void RemoveImGroupAsync(RemoveImGroupType RemoveImGroup1, object userState)
		{
			if (this.RemoveImGroupOperationCompleted == null)
			{
				this.RemoveImGroupOperationCompleted = new SendOrPostCallback(this.OnRemoveImGroupOperationCompleted);
			}
			base.InvokeAsync("RemoveImGroup", new object[]
			{
				RemoveImGroup1
			}, this.RemoveImGroupOperationCompleted, userState);
		}

		private void OnRemoveImGroupOperationCompleted(object arg)
		{
			if (this.RemoveImGroupCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.RemoveImGroupCompleted(this, new RemoveImGroupCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapHeader("RequestServerVersionValue")]
		[SoapHeader("MailboxCulture")]
		[SoapHeader("ServerVersionInfoValue", Direction = SoapHeaderDirection.Out)]
		[SoapHeader("ExchangeImpersonation")]
		[SoapDocumentMethod("http://schemas.microsoft.com/exchange/services/2006/messages/SetImGroup", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Bare)]
		[SoapHttpClientTraceExtension]
		[return: XmlElement("SetImGroupResponse", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public SetImGroupResponseMessageType SetImGroup([XmlElement("SetImGroup", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")] SetImGroupType SetImGroup1)
		{
			object[] array = this.Invoke("SetImGroup", new object[]
			{
				SetImGroup1
			});
			return (SetImGroupResponseMessageType)array[0];
		}

		public IAsyncResult BeginSetImGroup(SetImGroupType SetImGroup1, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("SetImGroup", new object[]
			{
				SetImGroup1
			}, callback, asyncState);
		}

		public SetImGroupResponseMessageType EndSetImGroup(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (SetImGroupResponseMessageType)array[0];
		}

		public void SetImGroupAsync(SetImGroupType SetImGroup1)
		{
			this.SetImGroupAsync(SetImGroup1, null);
		}

		public void SetImGroupAsync(SetImGroupType SetImGroup1, object userState)
		{
			if (this.SetImGroupOperationCompleted == null)
			{
				this.SetImGroupOperationCompleted = new SendOrPostCallback(this.OnSetImGroupOperationCompleted);
			}
			base.InvokeAsync("SetImGroup", new object[]
			{
				SetImGroup1
			}, this.SetImGroupOperationCompleted, userState);
		}

		private void OnSetImGroupOperationCompleted(object arg)
		{
			if (this.SetImGroupCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.SetImGroupCompleted(this, new SetImGroupCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapHeader("RequestServerVersionValue")]
		[SoapHeader("ServerVersionInfoValue", Direction = SoapHeaderDirection.Out)]
		[SoapHeader("ExchangeImpersonation")]
		[SoapDocumentMethod("http://schemas.microsoft.com/exchange/services/2006/messages/SetImListMigrationCompleted", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Bare)]
		[SoapHeader("MailboxCulture")]
		[SoapHttpClientTraceExtension]
		[return: XmlElement("SetImListMigrationCompletedResponse", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public SetImListMigrationCompletedResponseMessageType SetImListMigrationCompleted([XmlElement("SetImListMigrationCompleted", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")] SetImListMigrationCompletedType SetImListMigrationCompleted1)
		{
			object[] array = this.Invoke("SetImListMigrationCompleted", new object[]
			{
				SetImListMigrationCompleted1
			});
			return (SetImListMigrationCompletedResponseMessageType)array[0];
		}

		public IAsyncResult BeginSetImListMigrationCompleted(SetImListMigrationCompletedType SetImListMigrationCompleted1, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("SetImListMigrationCompleted", new object[]
			{
				SetImListMigrationCompleted1
			}, callback, asyncState);
		}

		public SetImListMigrationCompletedResponseMessageType EndSetImListMigrationCompleted(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (SetImListMigrationCompletedResponseMessageType)array[0];
		}

		public void SetImListMigrationCompletedAsync(SetImListMigrationCompletedType SetImListMigrationCompleted1)
		{
			this.SetImListMigrationCompletedAsync(SetImListMigrationCompleted1, null);
		}

		public void SetImListMigrationCompletedAsync(SetImListMigrationCompletedType SetImListMigrationCompleted1, object userState)
		{
			if (this.SetImListMigrationCompletedOperationCompleted == null)
			{
				this.SetImListMigrationCompletedOperationCompleted = new SendOrPostCallback(this.OnSetImListMigrationCompletedOperationCompleted);
			}
			base.InvokeAsync("SetImListMigrationCompleted", new object[]
			{
				SetImListMigrationCompleted1
			}, this.SetImListMigrationCompletedOperationCompleted, userState);
		}

		private void OnSetImListMigrationCompletedOperationCompleted(object arg)
		{
			if (this.SetImListMigrationCompletedCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.SetImListMigrationCompletedCompleted(this, new SetImListMigrationCompletedCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapHeader("RequestServerVersionValue")]
		[SoapHeader("ServerVersionInfoValue", Direction = SoapHeaderDirection.Out)]
		[SoapDocumentMethod("http://schemas.microsoft.com/exchange/services/2006/messages/GetUserRetentionPolicyTags", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Bare)]
		[SoapHttpClientTraceExtension]
		[return: XmlElement("GetUserRetentionPolicyTagsResponse", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public GetUserRetentionPolicyTagsResponseMessageType GetUserRetentionPolicyTags([XmlElement("GetUserRetentionPolicyTags", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")] GetUserRetentionPolicyTagsType GetUserRetentionPolicyTags1)
		{
			object[] array = this.Invoke("GetUserRetentionPolicyTags", new object[]
			{
				GetUserRetentionPolicyTags1
			});
			return (GetUserRetentionPolicyTagsResponseMessageType)array[0];
		}

		public IAsyncResult BeginGetUserRetentionPolicyTags(GetUserRetentionPolicyTagsType GetUserRetentionPolicyTags1, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("GetUserRetentionPolicyTags", new object[]
			{
				GetUserRetentionPolicyTags1
			}, callback, asyncState);
		}

		public GetUserRetentionPolicyTagsResponseMessageType EndGetUserRetentionPolicyTags(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (GetUserRetentionPolicyTagsResponseMessageType)array[0];
		}

		public void GetUserRetentionPolicyTagsAsync(GetUserRetentionPolicyTagsType GetUserRetentionPolicyTags1)
		{
			this.GetUserRetentionPolicyTagsAsync(GetUserRetentionPolicyTags1, null);
		}

		public void GetUserRetentionPolicyTagsAsync(GetUserRetentionPolicyTagsType GetUserRetentionPolicyTags1, object userState)
		{
			if (this.GetUserRetentionPolicyTagsOperationCompleted == null)
			{
				this.GetUserRetentionPolicyTagsOperationCompleted = new SendOrPostCallback(this.OnGetUserRetentionPolicyTagsOperationCompleted);
			}
			base.InvokeAsync("GetUserRetentionPolicyTags", new object[]
			{
				GetUserRetentionPolicyTags1
			}, this.GetUserRetentionPolicyTagsOperationCompleted, userState);
		}

		private void OnGetUserRetentionPolicyTagsOperationCompleted(object arg)
		{
			if (this.GetUserRetentionPolicyTagsCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.GetUserRetentionPolicyTagsCompleted(this, new GetUserRetentionPolicyTagsCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapHeader("ServerVersionInfoValue", Direction = SoapHeaderDirection.Out)]
		[SoapHttpClientTraceExtension]
		[SoapDocumentMethod("http://schemas.microsoft.com/exchange/services/2006/messages/InstallApp", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Bare)]
		[SoapHeader("RequestServerVersionValue")]
		[return: XmlElement("InstallAppResponse", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public InstallAppResponseType InstallApp([XmlElement("InstallApp", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")] InstallAppType InstallApp1)
		{
			object[] array = this.Invoke("InstallApp", new object[]
			{
				InstallApp1
			});
			return (InstallAppResponseType)array[0];
		}

		public IAsyncResult BeginInstallApp(InstallAppType InstallApp1, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("InstallApp", new object[]
			{
				InstallApp1
			}, callback, asyncState);
		}

		public InstallAppResponseType EndInstallApp(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (InstallAppResponseType)array[0];
		}

		public void InstallAppAsync(InstallAppType InstallApp1)
		{
			this.InstallAppAsync(InstallApp1, null);
		}

		public void InstallAppAsync(InstallAppType InstallApp1, object userState)
		{
			if (this.InstallAppOperationCompleted == null)
			{
				this.InstallAppOperationCompleted = new SendOrPostCallback(this.OnInstallAppOperationCompleted);
			}
			base.InvokeAsync("InstallApp", new object[]
			{
				InstallApp1
			}, this.InstallAppOperationCompleted, userState);
		}

		private void OnInstallAppOperationCompleted(object arg)
		{
			if (this.InstallAppCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.InstallAppCompleted(this, new InstallAppCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapHeader("RequestServerVersionValue")]
		[SoapHeader("ServerVersionInfoValue", Direction = SoapHeaderDirection.Out)]
		[SoapDocumentMethod("http://schemas.microsoft.com/exchange/services/2006/messages/UninstallApp", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Bare)]
		[SoapHttpClientTraceExtension]
		[return: XmlElement("UninstallAppResponse", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public UninstallAppResponseType UninstallApp([XmlElement("UninstallApp", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")] UninstallAppType UninstallApp1)
		{
			object[] array = this.Invoke("UninstallApp", new object[]
			{
				UninstallApp1
			});
			return (UninstallAppResponseType)array[0];
		}

		public IAsyncResult BeginUninstallApp(UninstallAppType UninstallApp1, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("UninstallApp", new object[]
			{
				UninstallApp1
			}, callback, asyncState);
		}

		public UninstallAppResponseType EndUninstallApp(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (UninstallAppResponseType)array[0];
		}

		public void UninstallAppAsync(UninstallAppType UninstallApp1)
		{
			this.UninstallAppAsync(UninstallApp1, null);
		}

		public void UninstallAppAsync(UninstallAppType UninstallApp1, object userState)
		{
			if (this.UninstallAppOperationCompleted == null)
			{
				this.UninstallAppOperationCompleted = new SendOrPostCallback(this.OnUninstallAppOperationCompleted);
			}
			base.InvokeAsync("UninstallApp", new object[]
			{
				UninstallApp1
			}, this.UninstallAppOperationCompleted, userState);
		}

		private void OnUninstallAppOperationCompleted(object arg)
		{
			if (this.UninstallAppCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.UninstallAppCompleted(this, new UninstallAppCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapHttpClientTraceExtension]
		[SoapHeader("RequestServerVersionValue")]
		[SoapHeader("ServerVersionInfoValue", Direction = SoapHeaderDirection.Out)]
		[SoapDocumentMethod("http://schemas.microsoft.com/exchange/services/2006/messages/DisableApp", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Bare)]
		[return: XmlElement("DisableAppResponse", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public DisableAppResponseType DisableApp([XmlElement("DisableApp", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")] DisableAppType DisableApp1)
		{
			object[] array = this.Invoke("DisableApp", new object[]
			{
				DisableApp1
			});
			return (DisableAppResponseType)array[0];
		}

		public IAsyncResult BeginDisableApp(DisableAppType DisableApp1, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("DisableApp", new object[]
			{
				DisableApp1
			}, callback, asyncState);
		}

		public DisableAppResponseType EndDisableApp(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (DisableAppResponseType)array[0];
		}

		public void DisableAppAsync(DisableAppType DisableApp1)
		{
			this.DisableAppAsync(DisableApp1, null);
		}

		public void DisableAppAsync(DisableAppType DisableApp1, object userState)
		{
			if (this.DisableAppOperationCompleted == null)
			{
				this.DisableAppOperationCompleted = new SendOrPostCallback(this.OnDisableAppOperationCompleted);
			}
			base.InvokeAsync("DisableApp", new object[]
			{
				DisableApp1
			}, this.DisableAppOperationCompleted, userState);
		}

		private void OnDisableAppOperationCompleted(object arg)
		{
			if (this.DisableAppCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.DisableAppCompleted(this, new DisableAppCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapHeader("ServerVersionInfoValue", Direction = SoapHeaderDirection.Out)]
		[SoapHeader("RequestServerVersionValue")]
		[SoapDocumentMethod("http://schemas.microsoft.com/exchange/services/2006/messages/GetAppMarketplaceUrl", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Bare)]
		[SoapHttpClientTraceExtension]
		[return: XmlElement("GetAppMarketplaceUrlResponse", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public GetAppMarketplaceUrlResponseMessageType GetAppMarketplaceUrl([XmlElement("GetAppMarketplaceUrl", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")] GetAppMarketplaceUrlType GetAppMarketplaceUrl1)
		{
			object[] array = this.Invoke("GetAppMarketplaceUrl", new object[]
			{
				GetAppMarketplaceUrl1
			});
			return (GetAppMarketplaceUrlResponseMessageType)array[0];
		}

		public IAsyncResult BeginGetAppMarketplaceUrl(GetAppMarketplaceUrlType GetAppMarketplaceUrl1, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("GetAppMarketplaceUrl", new object[]
			{
				GetAppMarketplaceUrl1
			}, callback, asyncState);
		}

		public GetAppMarketplaceUrlResponseMessageType EndGetAppMarketplaceUrl(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (GetAppMarketplaceUrlResponseMessageType)array[0];
		}

		public void GetAppMarketplaceUrlAsync(GetAppMarketplaceUrlType GetAppMarketplaceUrl1)
		{
			this.GetAppMarketplaceUrlAsync(GetAppMarketplaceUrl1, null);
		}

		public void GetAppMarketplaceUrlAsync(GetAppMarketplaceUrlType GetAppMarketplaceUrl1, object userState)
		{
			if (this.GetAppMarketplaceUrlOperationCompleted == null)
			{
				this.GetAppMarketplaceUrlOperationCompleted = new SendOrPostCallback(this.OnGetAppMarketplaceUrlOperationCompleted);
			}
			base.InvokeAsync("GetAppMarketplaceUrl", new object[]
			{
				GetAppMarketplaceUrl1
			}, this.GetAppMarketplaceUrlOperationCompleted, userState);
		}

		private void OnGetAppMarketplaceUrlOperationCompleted(object arg)
		{
			if (this.GetAppMarketplaceUrlCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.GetAppMarketplaceUrlCompleted(this, new GetAppMarketplaceUrlCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		[SoapHeader("ServerVersionInfoValue", Direction = SoapHeaderDirection.Out)]
		[SoapHttpClientTraceExtension]
		[SoapDocumentMethod("http://schemas.microsoft.com/exchange/services/2006/messages/GetUserPhoto", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Bare)]
		[SoapHeader("RequestServerVersionValue")]
		[return: XmlElement("GetUserPhotoResponse", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
		public GetUserPhotoResponseMessageType GetUserPhoto([XmlElement("GetUserPhoto", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")] GetUserPhotoType GetUserPhoto1)
		{
			object[] array = this.Invoke("GetUserPhoto", new object[]
			{
				GetUserPhoto1
			});
			return (GetUserPhotoResponseMessageType)array[0];
		}

		public IAsyncResult BeginGetUserPhoto(GetUserPhotoType GetUserPhoto1, AsyncCallback callback, object asyncState)
		{
			return base.BeginInvoke("GetUserPhoto", new object[]
			{
				GetUserPhoto1
			}, callback, asyncState);
		}

		public GetUserPhotoResponseMessageType EndGetUserPhoto(IAsyncResult asyncResult)
		{
			object[] array = base.EndInvoke(asyncResult);
			return (GetUserPhotoResponseMessageType)array[0];
		}

		public void GetUserPhotoAsync(GetUserPhotoType GetUserPhoto1)
		{
			this.GetUserPhotoAsync(GetUserPhoto1, null);
		}

		public void GetUserPhotoAsync(GetUserPhotoType GetUserPhoto1, object userState)
		{
			if (this.GetUserPhotoOperationCompleted == null)
			{
				this.GetUserPhotoOperationCompleted = new SendOrPostCallback(this.OnGetUserPhotoOperationCompleted);
			}
			base.InvokeAsync("GetUserPhoto", new object[]
			{
				GetUserPhoto1
			}, this.GetUserPhotoOperationCompleted, userState);
		}

		private void OnGetUserPhotoOperationCompleted(object arg)
		{
			if (this.GetUserPhotoCompleted != null)
			{
				InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
				this.GetUserPhotoCompleted(this, new GetUserPhotoCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
			}
		}

		public new void CancelAsync(object userState)
		{
			base.CancelAsync(userState);
		}

		internal override XmlNamespaceDefinition[] PredefinedNamespaces
		{
			get
			{
				return Constants.EwsNamespaces;
			}
		}

		public ExchangeServiceBinding(string componentId, RemoteCertificateValidationCallback remoteCertificateValidationCallback) : base(remoteCertificateValidationCallback, true)
		{
		}

		public ExchangeServiceBinding(string componentId, RemoteCertificateValidationCallback remoteCertificateValidationCallback, bool normalization) : base(remoteCertificateValidationCallback, normalization)
		{
		}

		public ExchangeServiceBinding(RemoteCertificateValidationCallback remoteCertificateValidationCallback) : base(remoteCertificateValidationCallback, true)
		{
		}

		public ExchangeServiceBinding(RemoteCertificateValidationCallback remoteCertificateValidationCallback, bool normalization) : base(remoteCertificateValidationCallback, normalization)
		{
		}

		string IExchangeService.Url
		{
			get
			{
				return base.Url;
			}
			set
			{
				base.Url = value;
			}
		}

		int IExchangeService.Timeout
		{
			get
			{
				return base.Timeout;
			}
			set
			{
				base.Timeout = value;
			}
		}

		protected override WebRequest GetWebRequest(Uri uri)
		{
			HttpWebRequest httpWebRequest = (HttpWebRequest)base.GetWebRequest(uri);
			IActivityScope currentActivityScope = ActivityContext.GetCurrentActivityScope();
			if (currentActivityScope != null)
			{
				currentActivityScope.SerializeTo(httpWebRequest);
			}
			return httpWebRequest;
		}

		public ExchangeImpersonationType ExchangeImpersonation;

		public MailboxCultureType MailboxCulture;

		public RequestServerVersion RequestServerVersionValue;

		public ServerVersionInfo ServerVersionInfoValue;

		private SendOrPostCallback ResolveNamesOperationCompleted;

		private SendOrPostCallback ExpandDLOperationCompleted;

		private SendOrPostCallback GetServerTimeZonesOperationCompleted;

		public TimeZoneContextType TimeZoneContext;

		public ManagementRoleType ManagementRole;

		private SendOrPostCallback FindFolderOperationCompleted;

		public DateTimePrecisionType DateTimePrecision;

		private SendOrPostCallback FindItemOperationCompleted;

		private SendOrPostCallback GetFolderOperationCompleted;

		private SendOrPostCallback UploadItemsOperationCompleted;

		private SendOrPostCallback ExportItemsOperationCompleted;

		private SendOrPostCallback ConvertIdOperationCompleted;

		private SendOrPostCallback CreateFolderOperationCompleted;

		private SendOrPostCallback CreateFolderPathOperationCompleted;

		private SendOrPostCallback DeleteFolderOperationCompleted;

		private SendOrPostCallback EmptyFolderOperationCompleted;

		private SendOrPostCallback UpdateFolderOperationCompleted;

		private SendOrPostCallback MoveFolderOperationCompleted;

		private SendOrPostCallback CopyFolderOperationCompleted;

		private SendOrPostCallback SubscribeOperationCompleted;

		private SendOrPostCallback UnsubscribeOperationCompleted;

		private SendOrPostCallback GetEventsOperationCompleted;

		private SendOrPostCallback GetStreamingEventsOperationCompleted;

		private SendOrPostCallback SyncFolderHierarchyOperationCompleted;

		private SendOrPostCallback SyncFolderItemsOperationCompleted;

		private SendOrPostCallback CreateManagedFolderOperationCompleted;

		private SendOrPostCallback GetItemOperationCompleted;

		private SendOrPostCallback CreateItemOperationCompleted;

		private SendOrPostCallback DeleteItemOperationCompleted;

		private SendOrPostCallback UpdateItemOperationCompleted;

		private SendOrPostCallback UpdateItemInRecoverableItemsOperationCompleted;

		private SendOrPostCallback SendItemOperationCompleted;

		private SendOrPostCallback MoveItemOperationCompleted;

		private SendOrPostCallback CopyItemOperationCompleted;

		private SendOrPostCallback ArchiveItemOperationCompleted;

		private SendOrPostCallback CreateAttachmentOperationCompleted;

		private SendOrPostCallback DeleteAttachmentOperationCompleted;

		private SendOrPostCallback GetAttachmentOperationCompleted;

		private SendOrPostCallback GetClientAccessTokenOperationCompleted;

		private SendOrPostCallback GetDelegateOperationCompleted;

		private SendOrPostCallback AddDelegateOperationCompleted;

		private SendOrPostCallback RemoveDelegateOperationCompleted;

		private SendOrPostCallback UpdateDelegateOperationCompleted;

		private SendOrPostCallback CreateUserConfigurationOperationCompleted;

		private SendOrPostCallback DeleteUserConfigurationOperationCompleted;

		private SendOrPostCallback GetUserConfigurationOperationCompleted;

		private SendOrPostCallback UpdateUserConfigurationOperationCompleted;

		private SendOrPostCallback GetUserAvailabilityOperationCompleted;

		private SendOrPostCallback GetUserOofSettingsOperationCompleted;

		private SendOrPostCallback SetUserOofSettingsOperationCompleted;

		private SendOrPostCallback GetServiceConfigurationOperationCompleted;

		private SendOrPostCallback GetMailTipsOperationCompleted;

		private SendOrPostCallback PlayOnPhoneOperationCompleted;

		private SendOrPostCallback GetPhoneCallInformationOperationCompleted;

		private SendOrPostCallback DisconnectPhoneCallOperationCompleted;

		private SendOrPostCallback GetSharingMetadataOperationCompleted;

		private SendOrPostCallback RefreshSharingFolderOperationCompleted;

		private SendOrPostCallback GetSharingFolderOperationCompleted;

		private SendOrPostCallback SetTeamMailboxOperationCompleted;

		private SendOrPostCallback UnpinTeamMailboxOperationCompleted;

		private SendOrPostCallback GetRoomListsOperationCompleted;

		private SendOrPostCallback GetRoomsOperationCompleted;

		private SendOrPostCallback FindMessageTrackingReportOperationCompleted;

		private SendOrPostCallback GetMessageTrackingReportOperationCompleted;

		private SendOrPostCallback FindConversationOperationCompleted;

		private SendOrPostCallback ApplyConversationActionOperationCompleted;

		private SendOrPostCallback GetConversationItemsOperationCompleted;

		private SendOrPostCallback FindPeopleOperationCompleted;

		private SendOrPostCallback GetPersonaOperationCompleted;

		private SendOrPostCallback GetInboxRulesOperationCompleted;

		private SendOrPostCallback UpdateInboxRulesOperationCompleted;

		private SendOrPostCallback GetPasswordExpirationDateOperationCompleted;

		private SendOrPostCallback GetSearchableMailboxesOperationCompleted;

		private SendOrPostCallback SearchMailboxesOperationCompleted;

		private SendOrPostCallback GetDiscoverySearchConfigurationOperationCompleted;

		private SendOrPostCallback GetHoldOnMailboxesOperationCompleted;

		private SendOrPostCallback SetHoldOnMailboxesOperationCompleted;

		private SendOrPostCallback GetNonIndexableItemStatisticsOperationCompleted;

		private SendOrPostCallback GetNonIndexableItemDetailsOperationCompleted;

		private SendOrPostCallback MarkAllItemsAsReadOperationCompleted;

		private SendOrPostCallback MarkAsJunkOperationCompleted;

		private SendOrPostCallback GetAppManifestsOperationCompleted;

		private SendOrPostCallback AddNewImContactToGroupOperationCompleted;

		private SendOrPostCallback AddNewTelUriContactToGroupOperationCompleted;

		private SendOrPostCallback AddImContactToGroupOperationCompleted;

		private SendOrPostCallback RemoveImContactFromGroupOperationCompleted;

		private SendOrPostCallback AddImGroupOperationCompleted;

		private SendOrPostCallback AddDistributionGroupToImListOperationCompleted;

		private SendOrPostCallback GetImItemListOperationCompleted;

		private SendOrPostCallback GetImItemsOperationCompleted;

		private SendOrPostCallback RemoveContactFromImListOperationCompleted;

		private SendOrPostCallback RemoveDistributionGroupFromImListOperationCompleted;

		private SendOrPostCallback RemoveImGroupOperationCompleted;

		private SendOrPostCallback SetImGroupOperationCompleted;

		private SendOrPostCallback SetImListMigrationCompletedOperationCompleted;

		private SendOrPostCallback GetUserRetentionPolicyTagsOperationCompleted;

		private SendOrPostCallback InstallAppOperationCompleted;

		private SendOrPostCallback UninstallAppOperationCompleted;

		private SendOrPostCallback DisableAppOperationCompleted;

		private SendOrPostCallback GetAppMarketplaceUrlOperationCompleted;

		private SendOrPostCallback GetUserPhotoOperationCompleted;
	}
}
