using System;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using Microsoft.Office.CompliancePolicy.PolicyConfiguration;

namespace Microsoft.Office.CompliancePolicy.PolicySync
{
	public sealed class BasicSyncProxy : IPolicySyncWebserviceClient, IDisposable
	{
		internal BasicSyncProxy(IPolicySyncWebservice webserviceProxy, ChannelFactory<IPolicySyncWebservice> channelFactory, string partnerName)
		{
			if (webserviceProxy == null)
			{
				throw new ArgumentNullException("webserviceProxy");
			}
			if (channelFactory == null)
			{
				throw new ArgumentNullException("channelFactory");
			}
			if (string.IsNullOrWhiteSpace(partnerName))
			{
				throw new ArgumentNullException("partnerName");
			}
			this.webserviceProxy = webserviceProxy;
			this.channelFactory = channelFactory;
			this.partnerName = partnerName;
		}

		public static BasicSyncProxy Create(EndpointAddress endpoint, X509Certificate2 certificate, string partnerName)
		{
			if (endpoint == null)
			{
				throw new ArgumentNullException("endpoint");
			}
			if (certificate == null)
			{
				throw new ArgumentNullException("certificate");
			}
			if (string.IsNullOrWhiteSpace(partnerName))
			{
				throw new ArgumentNullException("partnerName");
			}
			ChannelFactory<IPolicySyncWebservice> channelFactory = new ChannelFactory<IPolicySyncWebservice>(new WSHttpBinding(SecurityMode.Transport)
			{
				MaxReceivedMessageSize = 26214400L,
				Security = 
				{
					Transport = 
					{
						ClientCredentialType = HttpClientCredentialType.Certificate
					}
				}
			}, endpoint);
			channelFactory.Credentials.ClientCertificate.Certificate = certificate;
			IPolicySyncWebservice policySyncWebservice = channelFactory.CreateChannel(endpoint);
			return new BasicSyncProxy(policySyncWebservice, channelFactory, partnerName);
		}

		public PolicyChange GetSingleTenantChanges(TenantCookie tenantCookie)
		{
			PolicyChange policyChange = null;
			TenantCookieCollection tenantCookieCollection = new TenantCookieCollection(tenantCookie.Workload, tenantCookie.ObjectType);
			tenantCookieCollection[tenantCookie.TenantId] = tenantCookie;
			PolicyChangeBatch changes = this.webserviceProxy.GetChanges(new GetChangesRequest
			{
				CallerContext = SyncCallerContext.Create(this.partnerName),
				TenantCookies = tenantCookieCollection
			});
			if (changes != null)
			{
				policyChange = new PolicyChange();
				policyChange.Changes = changes.Changes;
				TenantCookie newCookie = null;
				if (changes.NewCookies != null)
				{
					changes.NewCookies.TryGetCookie(tenantCookie.TenantId, out newCookie);
				}
				policyChange.NewCookie = newCookie;
			}
			return policyChange;
		}

		public IAsyncResult BeginGetSingleTenantChanges(TenantCookie tenantCookie, AsyncCallback userCallback, object stateObject)
		{
			throw new NotImplementedException();
		}

		public PolicyChange EndGetSingleTenantChanges(IAsyncResult asyncResult)
		{
			throw new NotImplementedException();
		}

		public PolicyChangeBatch GetChanges(GetChangesRequest request)
		{
			return this.webserviceProxy.GetChanges(request);
		}

		public IAsyncResult BeginGetChanges(GetChangesRequest request, AsyncCallback userCallback, object stateObject)
		{
			throw new NotImplementedException();
		}

		public PolicyChangeBatch EndGetChanges(IAsyncResult asyncResult)
		{
			throw new NotImplementedException();
		}

		public PolicyConfigurationBase GetObject(SyncCallerContext callerContext, Guid tenantId, ConfigurationObjectType objectType, Guid objectId, bool includeDeletedObjects)
		{
			return this.webserviceProxy.GetObject(callerContext, tenantId, objectType, objectId, includeDeletedObjects);
		}

		public IAsyncResult BeginGetObject(SyncCallerContext callerContext, Guid tenantId, ConfigurationObjectType objectType, Guid objectId, bool includeDeletedObjects, AsyncCallback userCallback, object stateObject)
		{
			throw new NotImplementedException();
		}

		public PolicyConfigurationBase EndGetObject(IAsyncResult asyncResult)
		{
			throw new NotImplementedException();
		}

		public void PublishStatus(PublishStatusRequest request)
		{
			this.webserviceProxy.PublishStatus(request);
		}

		public IAsyncResult BeginPublishStatus(PublishStatusRequest request, AsyncCallback userCallback, object stateObject)
		{
			throw new NotImplementedException();
		}

		public void EndPublishStatus(IAsyncResult asyncResult)
		{
			throw new NotImplementedException();
		}

		public void Dispose()
		{
			if (this.channelFactory != null)
			{
				this.channelFactory.Close();
				this.channelFactory = null;
			}
			GC.SuppressFinalize(this);
		}

		private readonly IPolicySyncWebservice webserviceProxy;

		private readonly string partnerName;

		private ChannelFactory<IPolicySyncWebservice> channelFactory;
	}
}
