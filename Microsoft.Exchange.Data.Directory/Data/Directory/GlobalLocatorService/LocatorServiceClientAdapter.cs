using System;
using System.Net;
using System.Net.Sockets;
using System.ServiceModel;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Net;
using schemas.microsoft.com.O365Filtering.GlobalLocatorService.Data;

namespace Microsoft.Exchange.Data.Directory.GlobalLocatorService
{
	internal abstract class LocatorServiceClientAdapter
	{
		internal string ResolveEndpointToIpAddress(bool flushCache)
		{
			DnsResult dnsResult = null;
			DnsQuery query = new DnsQuery(DnsRecordType.A, this.endpointHostName);
			if (flushCache || LocatorServiceClientAdapter.lastCacheUpdate + 300000 < Environment.TickCount)
			{
				LocatorServiceClientAdapter.dnsCache.FlushCache();
				LocatorServiceClientAdapter.lastCacheUpdate = Environment.TickCount;
			}
			dnsResult = LocatorServiceClientAdapter.dnsCache.Find(query);
			if (dnsResult == null)
			{
				try
				{
					IPHostEntry hostEntry = Dns.GetHostEntry(this.endpointHostName);
					IPAddress[] addressList = hostEntry.AddressList;
					if (addressList.Length > 0)
					{
						dnsResult = new DnsResult(DnsStatus.Success, addressList[0], TimeSpan.FromMinutes(1.0));
						LocatorServiceClientAdapter.dnsCache.Add(query, dnsResult);
					}
				}
				catch (SocketException)
				{
				}
			}
			if (dnsResult != null)
			{
				return dnsResult.Server.ToString();
			}
			return string.Empty;
		}

		internal LocatorServiceClientAdapter(GlsCallerId glsCallerId)
		{
			this.requestIdentity = new RequestIdentity
			{
				CallerId = glsCallerId.CallerIdString,
				TrackingGuid = glsCallerId.TrackingGuid
			};
			WSHttpBinding wshttpBinding = new WSHttpBinding(SecurityMode.Transport)
			{
				ReceiveTimeout = TimeSpan.FromSeconds(20.0),
				SendTimeout = TimeSpan.FromSeconds(15.0),
				MaxBufferPoolSize = 524288L,
				MaxReceivedMessageSize = 65536L
			};
			wshttpBinding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Certificate;
			ServiceEndpoint serviceEndpoint = LocatorServiceClientAdapter.LoadServiceEndpoint();
			this.serviceProxyPool = new ServiceProxyPool<LocatorService>(wshttpBinding, serviceEndpoint);
			this.endpointHostName = serviceEndpoint.Uri.Host;
		}

		internal LocatorServiceClientAdapter(GlsCallerId glsCallerId, LocatorService serviceProxy)
		{
			this.requestIdentity = new RequestIdentity
			{
				CallerId = glsCallerId.CallerIdString,
				TrackingGuid = glsCallerId.TrackingGuid
			};
			this.serviceProxyPool = new SingletonServiceProxyPool<LocatorService>(serviceProxy);
		}

		protected static void OnWebServiceRequestCompleted(IAsyncResult internalAR)
		{
			GlsAsyncState glsAsyncState = (GlsAsyncState)internalAR.AsyncState;
			AsyncCallback clientCallback = glsAsyncState.ClientCallback;
			object clientAsyncState = glsAsyncState.ClientAsyncState;
			LocatorService serviceProxy = glsAsyncState.ServiceProxy;
			if (clientCallback != null)
			{
				IAsyncResult ar = new GlsAsyncResult(clientCallback, clientAsyncState, serviceProxy, internalAR);
				clientCallback(ar);
			}
		}

		protected static void ThrowIfNull(object argument, string parameterName)
		{
			if (argument == null)
			{
				throw new ArgumentException(parameterName);
			}
		}

		protected static void ThrowIfEmptyGuid(Guid argument, string parameterName)
		{
			if (argument == Guid.Empty)
			{
				throw new ArgumentException(parameterName);
			}
		}

		protected static void ThrowIfInvalidNamespace(Namespace ns)
		{
			if (ns == Namespace.Invalid)
			{
				throw new ArgumentException("namespace");
			}
		}

		protected static void ThrowIfInvalidNamespace(Namespace[] ns)
		{
			foreach (Namespace ns2 in ns)
			{
				LocatorServiceClientAdapter.ThrowIfInvalidNamespace(ns2);
			}
		}

		protected virtual LocatorService AcquireServiceProxy()
		{
			return this.serviceProxyPool.Acquire();
		}

		protected void ReleaseServiceProxy(LocatorService serviceProxy)
		{
			this.serviceProxyPool.Release(serviceProxy);
		}

		private static ServiceEndpoint LoadServiceEndpoint()
		{
			ServiceEndpoint endpoint = LocatorServiceClientConfiguration.Instance.Endpoint;
			if (endpoint != null)
			{
				return endpoint;
			}
			ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 163, "LoadServiceEndpoint", "f:\\15.00.1497\\sources\\dev\\data\\src\\directory\\GlobalLocatorService\\LocatorServiceClientAdapter.cs");
			ServiceEndpointContainer endpointContainer = topologyConfigurationSession.GetEndpointContainer();
			return endpointContainer.GetEndpoint("GlobalLocatorService");
		}

		private const string ServiceEndpointName = "GlobalLocatorService";

		private const int DnsCacheTTLMsec = 300000;

		private readonly string endpointHostName;

		private static DnsCache dnsCache = new DnsCache(5);

		private static int lastCacheUpdate = 0;

		protected RequestIdentity requestIdentity;

		private IServiceProxyPool<LocatorService> serviceProxyPool;
	}
}
