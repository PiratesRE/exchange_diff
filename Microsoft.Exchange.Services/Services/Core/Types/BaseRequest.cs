using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Security.Principal;
using System.Web;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.ResourceHealth;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.Net.Protocols;
using Microsoft.Exchange.Services.Core.DataConverter;
using Microsoft.Exchange.Services.Wcf;
using Microsoft.Exchange.Services.Wcf.Types;
using Microsoft.Exchange.WorkloadManagement;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlInclude(typeof(DeleteFolderRequest))]
	[XmlInclude(typeof(FindItemRequest))]
	[XmlInclude(typeof(FindFolderRequest))]
	[XmlInclude(typeof(CreateFolderRequest))]
	[XmlInclude(typeof(SendItemRequest))]
	[XmlInclude(typeof(DeleteItemRequest))]
	[XmlInclude(typeof(CopyItemRequest))]
	[XmlInclude(typeof(MoveItemRequest))]
	[XmlInclude(typeof(GetItemRequest))]
	[XmlInclude(typeof(UpdateItemRequest))]
	[XmlInclude(typeof(CreateItemRequest))]
	[XmlInclude(typeof(CreateAttachmentRequest))]
	[XmlInclude(typeof(DeleteAttachmentRequest))]
	[XmlInclude(typeof(GetAttachmentRequest))]
	[XmlInclude(typeof(ConvertIdRequest))]
	[XmlInclude(typeof(GetInboxRulesRequest))]
	[XmlInclude(typeof(FindPeopleRequest))]
	[XmlInclude(typeof(GetPersonaRequest))]
	[XmlInclude(typeof(MarkAllItemsAsReadRequest))]
	[XmlInclude(typeof(AddDistributionGroupToImListRequest))]
	[XmlInclude(typeof(AddImContactToGroupRequest))]
	[XmlInclude(typeof(RemoveImContactFromGroupRequest))]
	[XmlInclude(typeof(AddImGroupRequest))]
	[XmlInclude(typeof(AddNewImContactToGroupRequest))]
	[XmlInclude(typeof(AddNewTelUriContactToGroupRequest))]
	[XmlInclude(typeof(GetImItemListRequest))]
	[XmlInclude(typeof(GetImItemsRequest))]
	[XmlInclude(typeof(PerformInstantSearchRequest))]
	[XmlInclude(typeof(EndInstantSearchSessionRequest))]
	[XmlInclude(typeof(RemoveContactFromImListRequest))]
	[XmlInclude(typeof(RemoveDistributionGroupFromImListRequest))]
	[XmlInclude(typeof(RemoveImGroupRequest))]
	[XmlInclude(typeof(SetImGroupRequest))]
	[XmlInclude(typeof(GetUserUnifiedGroupsRequest))]
	[XmlInclude(typeof(GetClutterStateRequest))]
	[XmlInclude(typeof(SetClutterStateRequest))]
	[XmlType("BaseRequestType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
	[DataContract(Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlInclude(typeof(CreateManagedFolderRequest))]
	[XmlInclude(typeof(CreateUserConfigurationRequest))]
	[XmlInclude(typeof(SubscribeRequest))]
	[XmlInclude(typeof(UnsubscribeRequest))]
	[XmlInclude(typeof(GetEventsRequest))]
	[XmlInclude(typeof(CopyFolderRequest))]
	[XmlInclude(typeof(AddDelegateRequest))]
	[XmlInclude(typeof(RemoveDelegateRequest))]
	[XmlInclude(typeof(UpdateDelegateRequest))]
	[XmlInclude(typeof(UpdateInboxRulesRequest))]
	[XmlInclude(typeof(GetSharingMetadataRequest))]
	[XmlInclude(typeof(RefreshSharingFolderRequest))]
	[XmlInclude(typeof(SetImListMigrationCompletedRequest))]
	[XmlInclude(typeof(GetUserPhotoRequest))]
	[XmlInclude(typeof(GetDelegateRequest))]
	[XmlInclude(typeof(UpdateFolderRequest))]
	[XmlInclude(typeof(EmptyFolderRequest))]
	[XmlInclude(typeof(MoveFolderRequest))]
	public abstract class BaseRequest
	{
		internal virtual bool IsHierarchicalOperation
		{
			get
			{
				return false;
			}
		}

		internal virtual bool RequireMailboxAccess
		{
			get
			{
				return true;
			}
		}

		private static void EvaluateMailboxMetrics(List<ServiceObjectId> ids, bool isHierarchicalOperation)
		{
			if (ids == null || ids.Count == 0)
			{
				BaseRequest.RecordMailboxMetrics(CallContext.Current, 0, 0, 0, 0, 0);
				return;
			}
			HashSet<Guid> hashSet = new HashSet<Guid>();
			HashSet<Guid> hashSet2 = new HashSet<Guid>();
			HashSet<string> hashSet3 = new HashSet<string>();
			int num = 0;
			int num2 = 0;
			bool flag = false;
			foreach (ServiceObjectId serviceObjectId in ids)
			{
				try
				{
					BaseServerIdInfo serverInfo = serviceObjectId.GetServerInfo(isHierarchicalOperation);
					MailboxIdServerInfo mailboxIdServerInfo = serverInfo as MailboxIdServerInfo;
					if (mailboxIdServerInfo != null && mailboxIdServerInfo.MailboxGuid != Guid.Empty)
					{
						if (serverInfo.IsLocalServer)
						{
							hashSet.Add(mailboxIdServerInfo.MailboxGuid);
							num++;
						}
						else
						{
							if (serverInfo.ServerVersion < Server.E14MinVersion && !serverInfo.IsFromDifferentResourceForest)
							{
								flag = true;
							}
							hashSet2.Add(mailboxIdServerInfo.MailboxGuid);
							hashSet3.Add(serverInfo.ServerFQDN);
							num2++;
						}
					}
				}
				catch (ServicePermanentException)
				{
					BaseRequest.RecordMailboxMetrics(CallContext.Current, 0, 0, 0, 0, 0);
					return;
				}
			}
			BaseRequest.RecordMailboxMetrics(CallContext.Current, hashSet3.Count, hashSet.Count, hashSet2.Count, num, num2);
			if (flag && hashSet.Count + hashSet2.Count > 1)
			{
				throw FaultExceptionUtilities.CreateFault(new MultiLegacyMailboxAccessException(), FaultParty.Sender);
			}
		}

		private static void RecordMailboxMetrics(CallContext callContext, int nRemoteServers, int nLocalMbx, int nRemoteMbx, int nLocalIds, int nRemoteIds)
		{
			RequestDetailsLoggerBase<RequestDetailsLogger>.SafeSetLogger(callContext.ProtocolLog, EwsMetadata.RemoteBackendCount, nRemoteServers);
			RequestDetailsLoggerBase<RequestDetailsLogger>.SafeSetLogger(callContext.ProtocolLog, EwsMetadata.LocalMailboxCount, nLocalMbx);
			RequestDetailsLoggerBase<RequestDetailsLogger>.SafeSetLogger(callContext.ProtocolLog, EwsMetadata.RemoteMailboxCount, nRemoteMbx);
			RequestDetailsLoggerBase<RequestDetailsLogger>.SafeSetLogger(callContext.ProtocolLog, EwsMetadata.LocalIdCount, nLocalIds);
			RequestDetailsLoggerBase<RequestDetailsLogger>.SafeSetLogger(callContext.ProtocolLog, EwsMetadata.RemoteIdCount, nRemoteIds);
		}

		internal ServiceCommandBase ServiceCommand
		{
			get
			{
				return this.serviceCommand;
			}
		}

		protected virtual List<ServiceObjectId> GetAllIds()
		{
			return null;
		}

		internal static BaseServerIdInfo GetServerInfoForItemId(CallContext callContext, BaseItemId itemId)
		{
			if (callContext == null)
			{
				throw new ArgumentNullException("callContext");
			}
			if (itemId == null)
			{
				throw new ArgumentNullException("itemid");
			}
			BaseServerIdInfo result;
			try
			{
				result = IdConverter.GetServerInfoForItemId(callContext, itemId);
			}
			catch (LocalizedException ex)
			{
				ExTraceGlobals.ProxyEvaluatorTracer.TraceError<string, string>(0L, "[BaseRequest::GetServerInfoForItemId] Encounter exception parsing id.  Exception class: {0}, Message: {1}", ex.GetType().FullName, ex.Message);
				result = null;
			}
			return result;
		}

		internal static BaseServerIdInfo GetServerInfoForItemIdList(CallContext callContext, IEnumerable<BaseItemId> itemIds)
		{
			return BaseRequest.GetServerInfoForItemId(callContext, itemIds.First<BaseItemId>());
		}

		internal static BaseServerIdInfo GetServerInfoForAttachmentId(CallContext callContext, AttachmentIdType attachmentId)
		{
			if (callContext == null)
			{
				throw new ArgumentNullException("callContext");
			}
			if (attachmentId == null)
			{
				throw new ArgumentNullException("itemid");
			}
			BaseServerIdInfo result;
			try
			{
				result = IdConverter.GetServerInfoForItemId(callContext, attachmentId);
			}
			catch (LocalizedException ex)
			{
				ExTraceGlobals.ProxyEvaluatorTracer.TraceError<string, string>(0L, "[BaseRequest::GetServerInfoForAttachmentId] Encounter exception parsing id.  Exception class: {0}, Message: {1}", ex.GetType().FullName, ex.Message);
				result = null;
			}
			return result;
		}

		internal static BaseServerIdInfo GetServerInfoForAttachmentIdList(CallContext callContext, IEnumerable<AttachmentIdType> attachmentIds)
		{
			return BaseRequest.GetServerInfoForAttachmentId(callContext, attachmentIds.First<AttachmentIdType>());
		}

		internal static BaseServerIdInfo GetServerInfoForFolderId(CallContext callContext, BaseFolderId folderId)
		{
			if (callContext == null)
			{
				throw new ArgumentNullException("callContext");
			}
			if (folderId == null)
			{
				throw new ArgumentNullException("folderId");
			}
			BaseServerIdInfo result;
			try
			{
				result = IdConverter.GetServerInfoForFolderId(callContext, folderId, false);
			}
			catch (LocalizedException ex)
			{
				ExTraceGlobals.ProxyEvaluatorTracer.TraceError<string, string>(0L, "[BaseRequest::GetServerInfoForFolderId] Encounter exception parsing id.  Exception class: {0}, Message: {1}", ex.GetType().FullName, ex.Message);
				result = null;
			}
			return result;
		}

		internal static BaseServerIdInfo GetServerInfoForFolderIdHierarchyOperations(CallContext callContext, BaseFolderId folderId)
		{
			if (callContext == null)
			{
				throw new ArgumentNullException("callContext");
			}
			if (folderId == null)
			{
				throw new ArgumentNullException("folderId");
			}
			BaseServerIdInfo result;
			try
			{
				result = IdConverter.GetServerInfoForFolderId(callContext, folderId, true);
			}
			catch (LocalizedException ex)
			{
				ExTraceGlobals.ProxyEvaluatorTracer.TraceError<string, string>(0L, "[BaseRequest::GetServerInfoForFolderId] Encounter exception parsing id.  Exception class: {0}, Message: {1}", ex.GetType().FullName, ex.Message);
				result = null;
			}
			return result;
		}

		internal static BaseServerIdInfo GetServerInfoForFolderIdList(CallContext callContext, IEnumerable<BaseFolderId> folderIds)
		{
			if (callContext == null)
			{
				throw new ArgumentNullException("callContext");
			}
			if (folderIds == null)
			{
				throw new ArgumentNullException("folderIds");
			}
			return BaseRequest.GetServerInfoForFolderId(callContext, folderIds.First<BaseFolderId>());
		}

		internal static BaseServerIdInfo GetServerInfoForFolderIdListHierarchyOperations(CallContext callContext, IEnumerable<BaseFolderId> folderIds)
		{
			if (callContext == null)
			{
				throw new ArgumentNullException("callContext");
			}
			if (folderIds == null)
			{
				throw new ArgumentNullException("folderIds");
			}
			return BaseRequest.GetServerInfoForFolderIdHierarchyOperations(callContext, folderIds.First<BaseFolderId>());
		}

		internal static ResourceKey[] ServerInfoToResourceKeys(bool writeOperation, BaseServerIdInfo serverInfo)
		{
			if (serverInfo == null)
			{
				return null;
			}
			return serverInfo.ToResourceKey(writeOperation);
		}

		internal static ResourceKey[] ServerInfosToResourceKeys(bool writeOperation, params BaseServerIdInfo[] serverInfos)
		{
			if (serverInfos == null)
			{
				return null;
			}
			Dictionary<ResourceKey, object> dictionary = null;
			List<ResourceKey> list = null;
			foreach (BaseServerIdInfo baseServerIdInfo in serverInfos)
			{
				if (baseServerIdInfo != null)
				{
					if (dictionary == null)
					{
						dictionary = new Dictionary<ResourceKey, object>();
						list = new List<ResourceKey>();
					}
					ResourceKey[] array = baseServerIdInfo.ToResourceKey(writeOperation);
					if (array != null)
					{
						foreach (ResourceKey resourceKey in array)
						{
							if (!dictionary.ContainsKey(resourceKey))
							{
								list.Add(resourceKey);
								dictionary.Add(resourceKey, null);
							}
						}
					}
				}
			}
			if (list == null || list.Count <= 0)
			{
				return null;
			}
			return list.ToArray();
		}

		internal ResourceKey[] GetResourceKeysFromProxyInfo(bool writeOperation, CallContext callContext)
		{
			BaseServerIdInfo proxyInfo = this.GetProxyInfo(callContext);
			return BaseRequest.ServerInfoToResourceKeys(writeOperation, proxyInfo);
		}

		internal ResourceKey[] GetResourceKeysForFolderId(bool writeOperation, CallContext callContext, BaseFolderId folderId)
		{
			BaseServerIdInfo serverInfoForFolderId = BaseRequest.GetServerInfoForFolderId(callContext, folderId);
			return BaseRequest.ServerInfoToResourceKeys(writeOperation, serverInfoForFolderId);
		}

		internal ResourceKey[] GetResourceKeysForFolderIdHierarchyOperations(bool writeOperation, CallContext callContext, BaseFolderId folderId)
		{
			BaseServerIdInfo serverInfoForFolderIdHierarchyOperations = BaseRequest.GetServerInfoForFolderIdHierarchyOperations(callContext, folderId);
			return BaseRequest.ServerInfoToResourceKeys(writeOperation, serverInfoForFolderIdHierarchyOperations);
		}

		internal ResourceKey[] GetResourceKeysForItemId(bool writeOperation, CallContext callContext, BaseItemId itemId)
		{
			BaseServerIdInfo serverInfoForItemId = BaseRequest.GetServerInfoForItemId(callContext, itemId);
			return BaseRequest.ServerInfoToResourceKeys(writeOperation, serverInfoForItemId);
		}

		internal ResourceKey[] GetResourceKeysForAttachmentId(bool writeOperation, CallContext callContext, AttachmentIdType attachmentId)
		{
			BaseServerIdInfo serverInfoForAttachmentId = BaseRequest.GetServerInfoForAttachmentId(callContext, attachmentId);
			return BaseRequest.ServerInfoToResourceKeys(writeOperation, serverInfoForAttachmentId);
		}

		internal void InitializeServiceCommand(CallContext callContext)
		{
			this.serviceCommand = this.GetServiceCommand(callContext);
			if (this.ServiceCommand.IsDelayExecuted)
			{
				callContext.HttpContext.Items["WS_ServiceCommandKey"] = this.ServiceCommand;
				callContext.HttpContext.Items["WS_ServiceProviderRequestIdKey"] = Trace.TraceCasStart(CasTraceEventType.Ews);
				return;
			}
			if (this.ServiceCommand is IDisposable)
			{
				callContext.HttpContext.Items["WS_ServiceCommandKey"] = this.ServiceCommand;
			}
		}

		internal abstract ResourceKey[] GetResources(CallContext callContext, int taskStep);

		internal virtual ServiceCommandBase GetServiceCommand(CallContext callContext)
		{
			return null;
		}

		internal virtual ITask CreateServiceTask<T>(ServiceAsyncResult<T> serviceAsyncResult)
		{
			return new ServiceTask<T>(this, CallContext.Current, serviceAsyncResult);
		}

		protected virtual bool SupportsExternalUsers
		{
			get
			{
				return false;
			}
		}

		internal abstract BaseServerIdInfo GetProxyInfo(CallContext callContext);

		internal virtual void UpdatePerformanceCounters(ServiceCommandBase serviceCommand, IExchangeWebMethodResponse response)
		{
			PerformanceMonitor.UpdateResponseCounters(response, serviceCommand.ObjectsChanged);
		}

		internal bool AllowCommandOptimization(string methodName)
		{
			return Global.AllowCommandOptimization(methodName);
		}

		internal virtual bool CanOptimizeCommandExecution(CallContext callContext)
		{
			return false;
		}

		internal virtual void Validate()
		{
		}

		internal static BaseServerIdInfo GetServerInfoForFirstChild(CallContext callContext, XmlNode parentNode)
		{
			if (callContext == null)
			{
				throw new ArgumentNullException("callContext");
			}
			if (parentNode == null)
			{
				throw new ArgumentNullException("parentNode");
			}
			if (parentNode.ChildNodes.Count == 0)
			{
				return null;
			}
			return BaseRequest.GetServerInfoForSingleId(callContext, parentNode.FirstChild);
		}

		internal static BaseServerIdInfo GetServerInfoForIdList(CallContext callContext, XmlNodeList idNodes)
		{
			if (callContext == null)
			{
				throw new ArgumentNullException("callContext");
			}
			if (idNodes == null)
			{
				throw new ArgumentNullException("idNodes");
			}
			if (idNodes.Count == 0)
			{
				return null;
			}
			return BaseRequest.GetServerInfoForSingleId(callContext, idNodes[0]);
		}

		internal static BaseServerIdInfo GetServerInfoForIdList(CallContext callContext, List<XmlNode> idNodes)
		{
			if (callContext == null)
			{
				throw new ArgumentNullException("callContext");
			}
			if (idNodes == null)
			{
				throw new ArgumentNullException("idNodes");
			}
			if (idNodes.Count == 0)
			{
				return null;
			}
			return BaseRequest.GetServerInfoForSingleId(callContext, idNodes[0]);
		}

		internal static BaseServerIdInfo GetServerInfoForSingleId(CallContext callContext, XmlNode idNode)
		{
			if (callContext == null)
			{
				throw new ArgumentNullException("callContext");
			}
			if (idNode == null)
			{
				throw new ArgumentNullException("idNode");
			}
			BaseServerIdInfo result;
			try
			{
				result = IdConverter.GetServerInfoForId(callContext, (XmlElement)idNode);
			}
			catch (LocalizedException ex)
			{
				ExTraceGlobals.ProxyEvaluatorTracer.TraceError<string, string>(0L, "[BaseRequest::GetServerInfoForSingleId] Encounter exception parsing id.  Exception class: {0}, Message: {1}", ex.GetType().FullName, ex.Message);
				result = null;
			}
			return result;
		}

		internal static BaseServerIdInfo GetServerInfoFromMailboxElement(CallContext callContext, XmlElement mailboxElement)
		{
			if (callContext == null)
			{
				throw new ArgumentNullException("callContext");
			}
			if (mailboxElement == null)
			{
				return callContext.GetServerInfoForEffectiveCaller();
			}
			XmlElement xmlElement = mailboxElement["EmailAddress", "http://schemas.microsoft.com/exchange/services/2006/types"];
			if (xmlElement == null)
			{
				return null;
			}
			string xmlTextNodeValue = ServiceXml.GetXmlTextNodeValue(xmlElement);
			return MailboxIdServerInfo.Create(xmlTextNodeValue);
		}

		internal ResourceKey[] GetResourceKeysForXmlNode(bool writeOperation, CallContext callContext, XmlNode node)
		{
			BaseServerIdInfo serverInfoForSingleId = BaseRequest.GetServerInfoForSingleId(callContext, node);
			return BaseRequest.ServerInfoToResourceKeys(writeOperation, serverInfoForSingleId);
		}

		internal IAsyncResult ValidateAndSubmit<T>(AsyncCallback hostCallback, object hostState)
		{
			this.Validate();
			return this.Submit<T>(hostCallback, hostState);
		}

		internal virtual IAsyncResult Submit<T>(AsyncCallback hostCallback, object hostState)
		{
			if (this.RequireMailboxAccess)
			{
				BaseRequest.EvaluateMailboxMetrics(this.GetAllIds(), this.IsHierarchicalOperation);
			}
			bool flag = false;
			if (CallContext.Current.AccessingPrincipal != null && ExUserTracingAdaptor.Instance.IsTracingEnabledUser(CallContext.Current.AccessingPrincipal.LegacyDn))
			{
				flag = true;
				BaseTrace.CurrentThreadSettings.EnableTracing();
			}
			WindowsImpersonationContext windowsImpersonationContext = null;
			bool flag2 = false;
			IAsyncResult result;
			try
			{
				using (WindowsIdentity current = WindowsIdentity.GetCurrent())
				{
					if (current.ImpersonationLevel != TokenImpersonationLevel.None)
					{
						windowsImpersonationContext = WindowsIdentity.Impersonate(IntPtr.Zero);
					}
				}
				Guid relatedActivityId;
				if (CallContext.Current.ProtocolLog != null && CallContext.Current.ProtocolLog.ActivityScope != null)
				{
					relatedActivityId = CallContext.Current.ProtocolLog.ActivityScope.ActivityId;
				}
				else
				{
					relatedActivityId = Guid.NewGuid();
				}
				using (ExPerfTrace.RelatedActivity(relatedActivityId))
				{
					ServiceAsyncResult<T> serviceAsyncResult = new ServiceAsyncResult<T>();
					serviceAsyncResult.AsyncCallback = hostCallback;
					serviceAsyncResult.AsyncState = hostState;
					ITask task = this.CreateTask<T>(serviceAsyncResult);
					task.State = null;
					task.Description = this.ToString();
					if (Global.ChargePreExecuteToBudgetEnabled)
					{
						CallContext.Current.Budget.EndLocal();
						flag2 = true;
					}
					if (CallContext.Current.ProtocolLog != null && CallContext.Current.ProtocolLog.ActivityScope != null)
					{
						RequestDetailsLogger protocolLog = CallContext.Current.ProtocolLog;
						RequestDetailsLoggerBase<RequestDetailsLogger>.SafeSetLogger(protocolLog, ServiceLatencyMetadata.PreExecutionLatency, protocolLog.ActivityScope.TotalMilliseconds);
					}
					if (!CallContext.Current.WorkloadManager.TrySubmitNewTask(task))
					{
						BailOut.SetHTTPStatusAndClose(HttpStatusCode.ServiceUnavailable);
					}
					result = serviceAsyncResult;
				}
			}
			finally
			{
				if (Global.ChargePreExecuteToBudgetEnabled && !flag2)
				{
					CallContext.Current.Budget.EndLocal();
				}
				if (windowsImpersonationContext != null)
				{
					windowsImpersonationContext.Undo();
					windowsImpersonationContext.Dispose();
				}
				if (flag)
				{
					BaseTrace.CurrentThreadSettings.DisableTracing();
				}
			}
			return result;
		}

		private ITask CreateTask<T>(ServiceAsyncResult<T> serviceAsyncResult)
		{
			ProxyServiceTask<T> proxyServiceTask = this.GetProxyServiceTask<T>(serviceAsyncResult);
			ITask result;
			TypeOfTask typeOfTask;
			if (proxyServiceTask != null)
			{
				result = proxyServiceTask;
				typeOfTask = (proxyServiceTask.ProxyToCafe ? TypeOfTask.ProxyToCafeTask : TypeOfTask.ProxyTask);
			}
			else
			{
				result = this.CreateServiceTask<T>(serviceAsyncResult);
				typeOfTask = TypeOfTask.LocalTask;
			}
			RequestDetailsLoggerBase<RequestDetailsLogger>.SafeSetLogger(CallContext.Current.ProtocolLog, EwsMetadata.PrimaryOrProxyServer, this.proxyNature);
			RequestDetailsLoggerBase<RequestDetailsLogger>.SafeSetLogger(CallContext.Current.ProtocolLog, EwsMetadata.TaskType, typeOfTask);
			return result;
		}

		private ProxyServiceTask<T> GetProxyServiceTask<T>(ServiceAsyncResult<T> serviceAsyncResult)
		{
			CallContext callContext = CallContext.Current;
			if (callContext.ProxyCASStatus == ProxyCASStatus.DestinationCAS || callContext.WorkloadType == WorkloadType.Owa || callContext.WorkloadType == WorkloadType.OwaVoice || callContext.ProxyCASStatus == ProxyCASStatus.DestinationCASFromCAFE)
			{
				this.proxyNature = ((callContext.ProxyCASStatus == ProxyCASStatus.DestinationCASFromCAFE) ? PrimaryOrProxyServer.ProxiedToServerFromCafe : PrimaryOrProxyServer.ProxiedToServer);
			}
			else
			{
				if (callContext.IsExternalUser)
				{
					if (!callContext.IsExternalUser || !this.SupportsExternalUsers)
					{
						goto IL_AD;
					}
				}
				try
				{
					WebServicesInfo[] array = this.PerformServiceDiscovery(callContext);
					if (array != null)
					{
						return this.CreateProxyServiceTask<T>(serviceAsyncResult, callContext, array);
					}
					return null;
				}
				catch (ServicePermanentException arg)
				{
					ExTraceGlobals.ProxyEvaluatorTracer.TraceDebug<ServicePermanentException>((long)this.GetHashCode(), "[BaseRequest::ShouldProxy] Caught exception while evaluating request for proxying. Error message: '{0}'", arg);
					goto IL_D5;
				}
				catch (ADTransientException arg2)
				{
					ExTraceGlobals.ProxyEvaluatorTracer.TraceDebug<ADTransientException>((long)this.GetHashCode(), "[BaseRequest::ShouldProxy] Caught exception while evaluating request for proxying. Error message: '{0}'", arg2);
					goto IL_D5;
				}
				IL_AD:
				ExTraceGlobals.ProxyEvaluatorTracer.TraceDebug<bool, bool, bool>((long)this.GetHashCode(), "[BaseRequest::ShouldProxy] Web method will not be proxied and will be processed locally. IsExternalUser: {0}, IsWSSecurityUser: {1}, SupportsExternalUsers: {2}.", callContext.IsExternalUser, callContext.IsWSSecurityUser, this.SupportsExternalUsers);
			}
			IL_D5:
			return null;
		}

		internal virtual ProxyServiceTask<T> CreateProxyServiceTask<T>(ServiceAsyncResult<T> serviceAsyncResult, CallContext callContext, WebServicesInfo[] services)
		{
			return new ProxyServiceTask<T>(this, callContext, serviceAsyncResult, services);
		}

		internal virtual WebServicesInfo[] PerformServiceDiscovery(CallContext callContext)
		{
			string text = string.Empty;
			if (callContext.HttpContext != null)
			{
				text = callContext.HttpContext.Request.HttpMethod;
				if (!string.Equals(text, "POST", StringComparison.OrdinalIgnoreCase))
				{
					ExTraceGlobals.ProxyEvaluatorTracer.TraceDebug<string>((long)this.GetHashCode(), "[BaseRequest::PerformServiceDiscovery] EWS only supports POST request proxy. Current http method is {0}", text);
					return null;
				}
			}
			WebServicesInfo proxyToSelfCASIfNeeded = this.GetProxyToSelfCASIfNeeded();
			if (proxyToSelfCASIfNeeded != null)
			{
				return new WebServicesInfo[]
				{
					proxyToSelfCASIfNeeded
				};
			}
			BaseServerIdInfo proxyInfo = this.GetProxyInfo(callContext);
			if (proxyInfo == null)
			{
				return null;
			}
			WebServicesInfo[] array = null;
			SingleProxyDeterministicCASBoxScoring.SiteSearchResult bestEwsBoxesForRequest = SingleProxyDeterministicCASBoxScoring.GetBestEwsBoxesForRequest(proxyInfo, out array);
			WebServicesInfo[] result;
			try
			{
				switch (bestEwsBoxesForRequest)
				{
				case SingleProxyDeterministicCASBoxScoring.SiteSearchResult.ServiceLocally:
					result = null;
					break;
				case SingleProxyDeterministicCASBoxScoring.SiteSearchResult.NeedToProxy:
				case SingleProxyDeterministicCASBoxScoring.SiteSearchResult.NeedToProxyToCafe:
				{
					MailboxIdServerInfo mailboxIdServerInfo = proxyInfo as MailboxIdServerInfo;
					if (this.ShouldReturnXDBMountedOn() && mailboxIdServerInfo != null && callContext.AccessingPrincipal != null && callContext.MailboxAccessType == MailboxAccessType.Normal && callContext.ProxyCASStatus == ProxyCASStatus.InitialCASOrNoProxy && callContext.AccessingPrincipal.MailboxInfo.MailboxGuid.Equals(mailboxIdServerInfo.MailboxGuid))
					{
						string str = "Standard";
						if (callContext.HttpContext != null && callContext.HttpContext.Request != null)
						{
							string text2 = callContext.HttpContext.Request.Headers[WellKnownHeader.AnchorMailbox];
							if (!string.IsNullOrEmpty(text2) && SmtpAddress.IsValidSmtpAddress(text2))
							{
								str = "AnchorMailbox";
							}
							string text3 = callContext.HttpContext.Request.Headers["X-PreferServerAffinity"];
							if (!string.IsNullOrEmpty(text3) && text3.Equals(bool.TrueString, StringComparison.OrdinalIgnoreCase))
							{
								HttpCookie httpCookie = callContext.HttpContext.Request.Cookies["X-BackEndOverrideCookie"];
								string value = (httpCookie == null) ? null : httpCookie.Value;
								if (!string.IsNullOrEmpty(value))
								{
									str = "CookieBased";
								}
							}
						}
						RequestDetailsLoggerBase<RequestDetailsLogger>.SafeAppendGenericInfo(callContext.ProtocolLog, "MbxProxyEvalCase", "WrongBE-" + str);
					}
					result = array;
					break;
				}
				case SingleProxyDeterministicCASBoxScoring.SiteSearchResult.NoGoodCASFound:
					RequestDetailsLoggerBase<RequestDetailsLogger>.SafeAppendGenericInfo(callContext.ProtocolLog, "ProxyNoGoodCASServer", proxyInfo.ServerFQDN);
					throw FaultExceptionUtilities.CreateFault(new NoApplicableProxyCASServersAvailableException(), FaultParty.Receiver);
				case SingleProxyDeterministicCASBoxScoring.SiteSearchResult.CASInBadCache:
					RequestDetailsLoggerBase<RequestDetailsLogger>.SafeAppendGenericInfo(callContext.ProtocolLog, "ProxyBadCASCacheServer", proxyInfo.ServerFQDN);
					throw FaultExceptionUtilities.CreateFault(new NoRespondingCASInDestinationSiteException(), FaultParty.Receiver);
				default:
					result = null;
					break;
				}
			}
			finally
			{
				if (bestEwsBoxesForRequest != SingleProxyDeterministicCASBoxScoring.SiteSearchResult.ServiceLocally && proxyInfo is MailboxIdServerInfo)
				{
					RequestDetailsLoggerBase<RequestDetailsLogger>.SafeAppendGenericInfo(callContext.ProtocolLog, "MbxGuidForProxyEval", ((MailboxIdServerInfo)proxyInfo).MailboxGuid);
				}
			}
			return result;
		}

		internal virtual bool ShouldReturnXDBMountedOn()
		{
			return true;
		}

		internal WebServicesInfo GetProxyToSelfCASIfNeeded()
		{
			if (!Global.ProxyToSelf)
			{
				return null;
			}
			ExTraceGlobals.ProxyEvaluatorTracer.TraceDebug((long)this.GetHashCode(), "[BaseRequest::GetProxyToSelfCASIfNeeded] Proxying to self based on config file setting.");
			WebServicesInfo casserviceForServer = SingleProxyDeterministicCASBoxScoring.GetCASServiceForServer(LocalServer.GetServer().Fqdn);
			if (casserviceForServer == null)
			{
				ExTraceGlobals.ProxyEvaluatorTracer.TraceError((long)this.GetHashCode(), "[BaseRequest::GetProxyToSelfCASIfNeeded] Could not generate service instance for ProxyToSelf CAS");
				throw FaultExceptionUtilities.CreateFault(new ProxyCallFailedException(), FaultParty.Receiver);
			}
			if (casserviceForServer.Url == null)
			{
				ExTraceGlobals.ProxyEvaluatorTracer.TraceDebug((long)this.GetHashCode(), "[BaseRequest::GetProxyToSelfCASIfNeeded] The local server's InternalNLBBypassUrl is null.  Will not self proxy.");
				return null;
			}
			return casserviceForServer;
		}

		internal bool IsMonitoringRequest(CallContext callContext)
		{
			return !string.IsNullOrEmpty(callContext.UserAgent) && (callContext.UserAgent.IndexOf("AMProbe", StringComparison.InvariantCultureIgnoreCase) >= 0 || callContext.UserAgent.IndexOf("ACTIVEMONITORING", StringComparison.OrdinalIgnoreCase) >= 0 || callContext.UserAgent.IndexOf("MSEXCHMON", StringComparison.InvariantCultureIgnoreCase) >= 0);
		}

		internal void HandleStaleCache()
		{
			if (this.ShouldReturnXDBMountedOn())
			{
				MailboxIdServerInfo mailboxIdServerInfo = this.GetProxyInfo(CallContext.Current) as MailboxIdServerInfo;
				if (mailboxIdServerInfo != null && !mailboxIdServerInfo.IsFromDifferentResourceForest)
				{
					ExchangePrincipal exchangePrincipal = null;
					if (!string.IsNullOrEmpty(mailboxIdServerInfo.PrimarySmtpAddress))
					{
						exchangePrincipal = ExchangePrincipalCache.GetFromCache(mailboxIdServerInfo.PrimarySmtpAddress, CallContext.Current.ADRecipientSessionContext);
					}
					else if (mailboxIdServerInfo.MailboxId != null)
					{
						exchangePrincipal = ExchangePrincipalCache.GetFromCache(mailboxIdServerInfo.MailboxId, CallContext.Current.ADRecipientSessionContext);
					}
					if (exchangePrincipal != null && ExchangePrincipalCache.TryRemoveStale(exchangePrincipal) && exchangePrincipal.Sid != null)
					{
						ADIdentityInformationCache.Singleton.RemoveUserIdentity(exchangePrincipal.Sid, CallContext.Current.ADRecipientSessionContext.OrganizationPrefix);
					}
				}
			}
		}

		private const string WlmQueueSubmitKey = "WlmQueueSubmitTime";

		private ServiceCommandBase serviceCommand;

		private PrimaryOrProxyServer proxyNature;
	}
}
