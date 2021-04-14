using System;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using Microsoft.Office.CompliancePolicy.PolicyConfiguration;

namespace Microsoft.Office.CompliancePolicy.PolicySync
{
	public class PolicySyncProxy : IPolicySyncWebserviceClient, IDisposable
	{
		private PolicySyncProxy(EndpointAddress endpointAddress, X509Certificate2 certificate, string partnerName, ExecutionLog logProvider)
		{
			this.identity = new ServiceProxyIdentity(endpointAddress, certificate, partnerName);
			this.proxyPool = ServiceProxyPoolsManager<IPolicySyncWebserviceClient>.Instance.GetOrCreateProxyPool(this.identity, logProvider);
		}

		private PolicySyncProxy(EndpointAddress endpointAddress, ICredentials credentials, string partnerName, ExecutionLog logProvider)
		{
			this.identity = new ServiceProxyIdentity(endpointAddress, credentials, partnerName);
			this.proxyPool = ServiceProxyPoolsManager<IPolicySyncWebserviceClient>.Instance.GetOrCreateProxyPool(this.identity, logProvider);
		}

		public static PolicySyncProxy GetOrCreate(EndpointAddress endpointAddress, X509Certificate2 certificate, string partnerName, ExecutionLog logProvider)
		{
			ArgumentValidator.ThrowIfNull("endpointAddress", endpointAddress);
			ArgumentValidator.ThrowIfNull("certificate", certificate);
			ArgumentValidator.ThrowIfNull("logProvider", logProvider);
			ArgumentValidator.ThrowIfNullOrEmpty("partnerName", partnerName);
			return new PolicySyncProxy(endpointAddress, certificate, partnerName, logProvider);
		}

		public static PolicySyncProxy GetOrCreate(EndpointAddress endpointAddress, ICredentials credentials, string partnerName, ExecutionLog logProvider)
		{
			ArgumentValidator.ThrowIfNull("endpointAddress", endpointAddress);
			ArgumentValidator.ThrowIfNull("credentials", credentials);
			ArgumentValidator.ThrowIfNull("logProvider", logProvider);
			ArgumentValidator.ThrowIfNullOrEmpty("partnerName", partnerName);
			return new PolicySyncProxy(endpointAddress, credentials, partnerName, logProvider);
		}

		public PolicyChange GetSingleTenantChanges(TenantCookie tenantCookie)
		{
			ArgumentValidator.ThrowIfNull("tenantCookie", tenantCookie);
			PolicyChangeBatch policyChangeBatch = null;
			TenantCookieCollection tenantCookies = new TenantCookieCollection(tenantCookie.Workload, tenantCookie.ObjectType);
			tenantCookies[tenantCookie.TenantId] = tenantCookie;
			this.proxyPool.CallServiceWithRetry(delegate(IPooledServiceProxy<IPolicySyncWebserviceClient> proxy)
			{
				policyChangeBatch = proxy.Client.GetChanges(new GetChangesRequest
				{
					CallerContext = SyncCallerContext.Create(this.identity.PartnerName),
					TenantCookies = tenantCookies
				});
			}, string.Format("sync GetSingleTenantChanges - tenantId: {0}.", tenantCookie.TenantId), 3);
			return PolicySyncProxy.GetPolicyChangeFromBatch(tenantCookie, policyChangeBatch);
		}

		public IAsyncResult BeginGetSingleTenantChanges(TenantCookie tenantCookie, AsyncCallback userCallback, object stateObject)
		{
			ArgumentValidator.ThrowIfNull("tenantCookie", tenantCookie);
			TenantCookieCollection tenantCookies = new TenantCookieCollection(tenantCookie.Workload, tenantCookie.ObjectType);
			tenantCookies[tenantCookie.TenantId] = tenantCookie;
			IAsyncResult result = null;
			this.proxyPool.CallServiceWithRetryAsyncBegin(delegate(IPooledServiceProxy<IPolicySyncWebserviceClient> proxy)
			{
				AsyncCallStateObject stateObject2 = new AsyncCallStateObject(stateObject, proxy, tenantCookie);
				result = proxy.Client.BeginGetChanges(new GetChangesRequest
				{
					CallerContext = SyncCallerContext.Create(this.identity.PartnerName),
					TenantCookies = tenantCookies
				}, userCallback, stateObject2);
			}, string.Format("async BeginGetSingleTenantChanges - tenantId: {0}.", tenantCookie.TenantId), 3);
			return result;
		}

		public PolicyChange EndGetSingleTenantChanges(IAsyncResult asyncResult)
		{
			ArgumentValidator.ThrowIfNull("asyncResult", asyncResult);
			PolicyChangeBatch policyChangeBatch = null;
			AsyncCallStateObject asyncCallStateObject = (AsyncCallStateObject)asyncResult.AsyncState;
			this.proxyPool.CallServiceWithRetryAsyncEnd(asyncCallStateObject.ProxyToUse, delegate(IPooledServiceProxy<IPolicySyncWebserviceClient> proxy)
			{
				policyChangeBatch = proxy.Client.EndGetChanges(asyncResult);
			}, "async EndGetSingleTenantChanges");
			return PolicySyncProxy.GetPolicyChangeFromBatch(asyncCallStateObject.TenantCookie, policyChangeBatch);
		}

		public PolicyChangeBatch GetChanges(GetChangesRequest request)
		{
			ArgumentValidator.ThrowIfNull("request", request);
			PolicyChangeBatch policyChangeBatch = null;
			this.proxyPool.CallServiceWithRetry(delegate(IPooledServiceProxy<IPolicySyncWebserviceClient> proxy)
			{
				policyChangeBatch = proxy.Client.GetChanges(request);
			}, "sync GetChanges", 3);
			return policyChangeBatch;
		}

		public IAsyncResult BeginGetChanges(GetChangesRequest request, AsyncCallback userCallback, object stateObject)
		{
			ArgumentValidator.ThrowIfNull("request", request);
			IAsyncResult result = null;
			this.proxyPool.CallServiceWithRetryAsyncBegin(delegate(IPooledServiceProxy<IPolicySyncWebserviceClient> proxy)
			{
				AsyncCallStateObject stateObject2 = new AsyncCallStateObject(stateObject, proxy, null);
				result = proxy.Client.BeginGetChanges(request, userCallback, stateObject2);
			}, "async BeginGetChanges", 3);
			return result;
		}

		public PolicyChangeBatch EndGetChanges(IAsyncResult asyncResult)
		{
			PolicyChangeBatch policyChangeBatch = null;
			AsyncCallStateObject asyncCallStateObject = (AsyncCallStateObject)asyncResult.AsyncState;
			this.proxyPool.CallServiceWithRetryAsyncEnd(asyncCallStateObject.ProxyToUse, delegate(IPooledServiceProxy<IPolicySyncWebserviceClient> proxy)
			{
				policyChangeBatch = proxy.Client.EndGetChanges(asyncResult);
			}, "async EndGetChanges");
			return policyChangeBatch;
		}

		public PolicyConfigurationBase GetObject(SyncCallerContext callerContext, Guid tenantId, ConfigurationObjectType objectType, Guid objectId, bool includeDeletedObjects)
		{
			ArgumentValidator.ThrowIfNull("callerContext", callerContext);
			ArgumentValidator.ThrowIfNull("tenantId", tenantId);
			ArgumentValidator.ThrowIfNull("objectType", objectType);
			ArgumentValidator.ThrowIfNull("objectId", objectId);
			PolicyConfigurationBase policyConfigurationBase = null;
			this.proxyPool.CallServiceWithRetry(delegate(IPooledServiceProxy<IPolicySyncWebserviceClient> proxy)
			{
				policyConfigurationBase = proxy.Client.GetObject(callerContext, tenantId, objectType, objectId, includeDeletedObjects);
			}, string.Format("sync GetObject - partnerName: {0}, tenantId: {1}, objectType: {2}, objectId: {3}, includeDeletedObjects: {4}.", new object[]
			{
				callerContext.PartnerName,
				tenantId,
				objectType,
				objectId,
				includeDeletedObjects
			}), 3);
			return policyConfigurationBase;
		}

		public IAsyncResult BeginGetObject(SyncCallerContext callerContext, Guid tenantId, ConfigurationObjectType objectType, Guid objectId, bool includeDeletedObjects, AsyncCallback userCallback, object stateObject)
		{
			ArgumentValidator.ThrowIfNull("callerContext", callerContext);
			ArgumentValidator.ThrowIfNull("tenantId", tenantId);
			ArgumentValidator.ThrowIfNull("objectType", objectType);
			ArgumentValidator.ThrowIfNull("objectId", objectId);
			IAsyncResult result = null;
			this.proxyPool.CallServiceWithRetryAsyncBegin(delegate(IPooledServiceProxy<IPolicySyncWebserviceClient> proxy)
			{
				AsyncCallStateObject stateObject2 = new AsyncCallStateObject(stateObject, proxy, null);
				result = proxy.Client.BeginGetObject(callerContext, tenantId, objectType, objectId, includeDeletedObjects, userCallback, stateObject2);
			}, string.Format("async BeginGetObject - partnerName: {0}, tenantId: {1}, objectType: {2}, objectId: {3}, includeDeletedObjects: {4}.", new object[]
			{
				callerContext.PartnerName,
				tenantId,
				objectType,
				objectId,
				includeDeletedObjects
			}), 3);
			return result;
		}

		public PolicyConfigurationBase EndGetObject(IAsyncResult asyncResult)
		{
			PolicyConfigurationBase policyConfigurationBase = null;
			AsyncCallStateObject asyncCallStateObject = (AsyncCallStateObject)asyncResult.AsyncState;
			this.proxyPool.CallServiceWithRetryAsyncEnd(asyncCallStateObject.ProxyToUse, delegate(IPooledServiceProxy<IPolicySyncWebserviceClient> proxy)
			{
				policyConfigurationBase = proxy.Client.EndGetObject(asyncResult);
			}, "async EndGetObject");
			return policyConfigurationBase;
		}

		public void PublishStatus(PublishStatusRequest request)
		{
			ArgumentValidator.ThrowIfNull("request", request);
			this.proxyPool.CallServiceWithRetry(delegate(IPooledServiceProxy<IPolicySyncWebserviceClient> proxy)
			{
				proxy.Client.PublishStatus(request);
			}, "sync PublishStatus", 3);
		}

		public IAsyncResult BeginPublishStatus(PublishStatusRequest request, AsyncCallback userCallback, object stateObject)
		{
			ArgumentValidator.ThrowIfNull("request", request);
			IAsyncResult result = null;
			this.proxyPool.CallServiceWithRetryAsyncBegin(delegate(IPooledServiceProxy<IPolicySyncWebserviceClient> proxy)
			{
				AsyncCallStateObject stateObject2 = new AsyncCallStateObject(stateObject, proxy, null);
				result = proxy.Client.BeginPublishStatus(request, userCallback, stateObject2);
			}, "async BeginPublishStatus", 3);
			return result;
		}

		public void EndPublishStatus(IAsyncResult asyncResult)
		{
			AsyncCallStateObject asyncCallStateObject = (AsyncCallStateObject)asyncResult.AsyncState;
			this.proxyPool.CallServiceWithRetryAsyncEnd(asyncCallStateObject.ProxyToUse, delegate(IPooledServiceProxy<IPolicySyncWebserviceClient> proxy)
			{
				proxy.Client.EndPublishStatus(asyncResult);
			}, "async EndGetObject");
		}

		public void SetMaxNumberOfProxiesInPool(uint maxNumberOfProxiesInPool)
		{
			ArgumentValidator.ThrowIfZero("maxNumberOfProxiesInPool", maxNumberOfProxiesInPool);
			this.proxyPool.MaxNumberOfClientProxies = maxNumberOfProxiesInPool;
		}

		public void Dispose()
		{
			GC.SuppressFinalize(this);
		}

		private static PolicyChange GetPolicyChangeFromBatch(TenantCookie tenantCookie, PolicyChangeBatch policyChangeBatch)
		{
			PolicyChange policyChange = null;
			if (policyChangeBatch != null && tenantCookie != null)
			{
				policyChange = new PolicyChange();
				policyChange.Changes = policyChangeBatch.Changes;
				TenantCookie newCookie = null;
				if (policyChangeBatch.NewCookies != null)
				{
					policyChangeBatch.NewCookies.TryGetCookie(tenantCookie.TenantId, out newCookie);
				}
				policyChange.NewCookie = newCookie;
			}
			return policyChange;
		}

		private readonly ServiceProxyPool<IPolicySyncWebserviceClient> proxyPool;

		private readonly ServiceProxyIdentity identity;
	}
}
