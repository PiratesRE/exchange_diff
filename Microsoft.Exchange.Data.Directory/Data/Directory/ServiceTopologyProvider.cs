using System;
using System.Collections.Generic;
using System.Net;
using System.ServiceModel;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.EventLog;
using Microsoft.Exchange.Data.Directory.TopologyDiscovery;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Directory;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Data.Directory
{
	internal class ServiceTopologyProvider : TopologyProvider, IDisposeTrackable, IDisposable
	{
		internal ServiceTopologyProvider() : this("localhost")
		{
		}

		internal ServiceTopologyProvider(string machineName)
		{
			ArgumentValidator.ThrowIfNullOrEmpty("machineName", machineName);
			ExTraceGlobals.TopologyProviderTracer.TraceDebug<string>((long)this.GetHashCode(), "Creating new Service Topology provider instance to server {0}", machineName);
			this.machineName = machineName;
			this.InitializeServiceProxyPool();
			this.disposeTracker = this.GetDisposeTracker();
		}

		internal static bool IsAdTopologyServiceInstalled()
		{
			return ServiceControllerUtils.IsInstalled("MSExchangeADTopology");
		}

		public DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<ServiceTopologyProvider>(this);
		}

		public void SuppressDisposeTracker()
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Suppress();
			}
		}

		internal override TopologyMode TopologyMode
		{
			get
			{
				return TopologyMode.ADTopologyService;
			}
		}

		public override IList<TopologyVersion> GetAllTopologyVersions()
		{
			ExTraceGlobals.TopologyProviderTracer.TraceFunction(this.GetHashCode(), (long)this.GetHashCode(), "Enter ServiceTopologyprovider.GetAllTopologyVersions().");
			IList<TopologyVersion> topologyVersions = null;
			this.serviceProxyPool.CallServiceWithRetry(delegate(IPooledServiceProxy<ITopologyClient> proxy)
			{
				topologyVersions = proxy.Client.GetAllTopologyVersions();
			}, "GetAllTopologyVersions", 3);
			if (topologyVersions == null)
			{
				topologyVersions = new List<TopologyVersion>(0);
			}
			ExTraceGlobals.TopologyProviderTracer.TraceFunction(this.GetHashCode(), (long)this.GetHashCode(), "Exit ServiceTopologyprovider.GetAllTopologyVersions().");
			if (ExTraceGlobals.TopologyProviderTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				string arg = string.Join<TopologyVersion>(",", topologyVersions);
				ExTraceGlobals.TopologyProviderTracer.TraceDebug<int, string>((long)this.GetHashCode(), "ServiceTopologyprovider.GetAllTopologyVersions() returned {0} versions. Values {1}", topologyVersions.Count, arg);
			}
			return topologyVersions;
		}

		public override IList<TopologyVersion> GetTopologyVersions(IList<string> partitionFqdns)
		{
			ExTraceGlobals.TopologyProviderTracer.TraceFunction(this.GetHashCode(), (long)this.GetHashCode(), "Enter ServiceTopologyprovider.GetTopologyVersions().");
			if (ExTraceGlobals.TopologyProviderTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				string arg = (partitionFqdns == null) ? string.Empty : string.Join(",", partitionFqdns);
				ExTraceGlobals.TopologyProviderTracer.TraceDebug<string>((long)this.GetHashCode(), "ServiceTopologyprovider.GetTopologyVersions() for {0}", arg);
			}
			List<TopologyVersion> topologyVersions = null;
			this.serviceProxyPool.CallServiceWithRetry(delegate(IPooledServiceProxy<ITopologyClient> proxy)
			{
				topologyVersions = proxy.Client.GetTopologyVersions((List<string>)partitionFqdns);
			}, "GetTopologyVersions", 3);
			ExTraceGlobals.TopologyProviderTracer.TraceFunction(this.GetHashCode(), (long)this.GetHashCode(), "Exit ServiceTopologyprovider.GetTopologyVersions().");
			if (topologyVersions == null)
			{
				topologyVersions = new List<TopologyVersion>(0);
			}
			if (ExTraceGlobals.TopologyProviderTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				string arg2 = string.Join<TopologyVersion>(",", topologyVersions);
				ExTraceGlobals.TopologyProviderTracer.TraceDebug<int, string>((long)this.GetHashCode(), "ServiceTopologyprovider.GetAllTopologyVersions() returned {0} versions. Values {1}", topologyVersions.Count, arg2);
			}
			return topologyVersions;
		}

		public override ADServerInfo GetServerFromDomainDN(string distinguishedName, NetworkCredential credential)
		{
			ExTraceGlobals.TopologyProviderTracer.TraceDebug<string, string>((long)this.GetHashCode(), "ServiceTopologyprovider - Need server from domain {0}. Credentials are {1}NULL", distinguishedName, (credential == null) ? string.Empty : "NOT ");
			ADServerInfo serverInfo = null;
			this.serviceProxyPool.CallServiceWithRetry(delegate(IPooledServiceProxy<ITopologyClient> proxy)
			{
				ServerInfo serverFromDomainDN = proxy.Client.GetServerFromDomainDN(distinguishedName);
				if (serverFromDomainDN != null)
				{
					serverInfo = serverFromDomainDN.ToADServerInfo();
				}
			}, string.Format("GetServerFromDomainDN {0}", distinguishedName), 3);
			ExTraceGlobals.TopologyProviderTracer.TraceDebug<string>((long)this.GetHashCode(), "GetServerFromDomainDN returning {0}", (serverInfo != null) ? serverInfo.FqdnPlusPort : "<NULL>");
			return serverInfo;
		}

		public override ADServerInfo GetRemoteServerFromDomainFqdn(string domainFqdn, NetworkCredential credential)
		{
			ExTraceGlobals.TopologyProviderTracer.TraceDebug<string, string>((long)this.GetHashCode(), "Need server from remote domain {0} {1} credentials.", domainFqdn, (credential == null) ? "without" : "with");
			if (credential == null)
			{
				return this.GetServerFromDomainDN(NativeHelpers.DistinguishedNameFromCanonicalName(domainFqdn), credential);
			}
			LdapTopologyProvider ldapTopologyProvider = new LdapTopologyProvider();
			return ldapTopologyProvider.GetRemoteServerFromDomainFqdn(domainFqdn, credential);
		}

		protected override ADServerInfo GetDefaultServerInfo(string partitionFqdn)
		{
			if (Globals.IsDatacenter)
			{
				return this.InternalServiceProviderGetServersForRole(partitionFqdn, new List<string>(), ADServerRole.DomainController, 1, true, false)[0];
			}
			return base.GetDefaultServerInfo(partitionFqdn);
		}

		public override void SetConfigDC(string partitionFqdn, string serverName, int port)
		{
			base.SetConfigDC(partitionFqdn, serverName, port);
			ExTraceGlobals.TopologyProviderTracer.TraceDebug<string, int>((long)this.GetHashCode(), "ServiceTopologyprovider - Setting Config DC to {0}:{1}", serverName, port);
			this.serviceProxyPool.CallServiceWithRetry(delegate(IPooledServiceProxy<ITopologyClient> proxy)
			{
				proxy.Client.SetConfigDC(partitionFqdn, serverName);
			}, string.Format("Set Config DC {0}", partitionFqdn), 3);
		}

		public override void ReportServerDown(string partitionFqdn, string serverName, ADServerRole role)
		{
			if (string.IsNullOrEmpty(serverName) || string.IsNullOrEmpty(partitionFqdn))
			{
				return;
			}
			ExTraceGlobals.TopologyProviderTracer.TraceWarning<string, ADServerRole>((long)this.GetHashCode(), "ServiceTopologyprovider - {0} is down for role {1}", serverName, role);
			this.serviceProxyPool.CallServiceWithRetry(delegate(IPooledServiceProxy<ITopologyClient> proxy)
			{
				proxy.Client.ReportServerDown(partitionFqdn, serverName, role);
			}, string.Format("Report Server Down {0}", partitionFqdn), 1);
		}

		public void Dispose()
		{
			ExTraceGlobals.TopologyProviderTracer.TraceDebug<int>((long)this.GetHashCode(), "Disposing of SAFM topo provider instance {0}", this.GetHashCode());
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Dispose();
			}
			this.serviceProxyPool.Dispose();
			GC.SuppressFinalize(this);
		}

		internal string GetConfigDC(bool throwOnFailure)
		{
			ADServerInfo configDCInfo = this.GetConfigDCInfo(TopologyProvider.LocalForestFqdn, throwOnFailure);
			string text = (configDCInfo == null) ? null : configDCInfo.Fqdn;
			ExTraceGlobals.TopologyProviderTracer.TraceDebug<string>((long)this.GetHashCode(), "Topology Service reported Config DC: {0}", text ?? "<null>");
			return text;
		}

		internal override ADServerInfo GetConfigDCInfo(string partitionFqdn, bool throwOnFailure)
		{
			Exception ex = null;
			ADServerInfo result = null;
			try
			{
				IList<ADServerInfo> list = this.InternalServiceProviderGetServersForRole(partitionFqdn, new List<string>(), ADServerRole.ConfigurationDomainController, 1, throwOnFailure, false);
				if (list.Count == 0)
				{
					result = null;
				}
				else
				{
					result = list[0];
				}
			}
			catch (ADTransientException ex2)
			{
				ex = ex2;
			}
			catch (ADTopologyPermanentException ex3)
			{
				ex = ex3;
			}
			catch (ADTopologyUnexpectedException ex4)
			{
				ex = ex4;
			}
			finally
			{
				if (ex != null)
				{
					ExTraceGlobals.TopologyProviderTracer.TraceError<string>((long)this.GetHashCode(), "ServiceTopologyprovider - Could not get Config DC: {0}", ex.Message);
					if (throwOnFailure)
					{
						throw ex;
					}
				}
			}
			return result;
		}

		internal string[] GetCurrentDCs(string partitionFqdn)
		{
			IList<ADServerInfo> serversForRole = base.GetServersForRole(partitionFqdn, new List<string>(), ADServerRole.DomainController, int.MaxValue, false);
			List<string> list = new List<string>(serversForRole.Count);
			foreach (ADServerInfo adserverInfo in serversForRole)
			{
				list.Add(adserverInfo.Fqdn);
			}
			ExTraceGlobals.TopologyProviderTracer.TraceDebug<int>((long)this.GetHashCode(), "ServiceTopologyprovider - AD Topology Service reported {0} DCs", list.Count);
			return list.ToArray();
		}

		internal string[] GetCurrentGCs(string partitionFqdn)
		{
			List<ADServerInfo> list = (List<ADServerInfo>)base.GetServersForRole(partitionFqdn, new List<string>(), ADServerRole.GlobalCatalog, int.MaxValue, false);
			List<string> list2 = new List<string>();
			foreach (ADServerInfo adserverInfo in list)
			{
				list2.Add(adserverInfo.Fqdn);
			}
			ExTraceGlobals.TopologyProviderTracer.TraceDebug<int>((long)this.GetHashCode(), "ServiceTopologyprovider - AD Topology Service reported {0} GCs", list2.Count);
			return list2.ToArray();
		}

		public override int TopoRecheckIntervalMsec
		{
			get
			{
				return 10000;
			}
		}

		private static Exception GetTransientWrappedException(Exception wcfException, string targetInfo)
		{
			if (wcfException is TimeoutException)
			{
				return new ADTransientException(DirectoryStrings.ExceptionADTopologyTimeoutCall(targetInfo, wcfException.Message), wcfException);
			}
			if (wcfException is EndpointNotFoundException)
			{
				return new ADTopologyEndpointNotFoundException(targetInfo);
			}
			if (wcfException is FaultException<TopologyServiceFault>)
			{
				return new ADTransientException(DirectoryStrings.ServerSideADTopologyServiceCallError(targetInfo, ((FaultException<TopologyServiceFault>)wcfException).Detail.Message), wcfException);
			}
			if (wcfException is FaultException<ExceptionDetail> || wcfException is FaultException)
			{
				return new ADTransientException(DirectoryStrings.ServerSideADTopologyServiceCallError(targetInfo, wcfException.Message), wcfException);
			}
			return new ADTransientException(DirectoryStrings.ExceptionADTopologyServiceCallError(targetInfo, wcfException.Message), wcfException);
		}

		private static Exception GetPermanentWrappedException(Exception wcfException, string targetInfo)
		{
			if (wcfException is FaultException<InvalidOperationException>)
			{
				FaultException<InvalidOperationException> faultException = (FaultException<InvalidOperationException>)wcfException;
				if (faultException.Detail == null)
				{
					return wcfException;
				}
				return faultException.Detail;
			}
			else if (wcfException is FaultException<ArgumentNullException>)
			{
				FaultException<ArgumentNullException> faultException2 = (FaultException<ArgumentNullException>)wcfException;
				if (faultException2.Detail == null)
				{
					return wcfException;
				}
				return faultException2.Detail;
			}
			else if (wcfException is FaultException<ArgumentException>)
			{
				FaultException<ArgumentException> faultException3 = (FaultException<ArgumentException>)wcfException;
				if (faultException3.Detail == null)
				{
					return wcfException;
				}
				return faultException3.Detail;
			}
			else
			{
				if (wcfException is FaultException<TopologyServiceFault>)
				{
					return new ADTopologyPermanentException(DirectoryStrings.ServerSideADTopologyServiceCallError(targetInfo, ((FaultException<TopologyServiceFault>)wcfException).Detail.Message), wcfException);
				}
				if (wcfException is FaultException<ExceptionDetail> || wcfException is FaultException)
				{
					return new ADTopologyPermanentException(DirectoryStrings.ServerSideADTopologyServiceCallError(targetInfo, wcfException.Message), wcfException);
				}
				return new ADTopologyUnexpectedException(DirectoryStrings.ExceptionADTopologyUnexpectedError(targetInfo, wcfException.Message), wcfException);
			}
		}

		protected override IList<ADServerInfo> InternalGetServersForRole(string partitionFqdn, IList<string> currentlyUsedServers, ADServerRole role, int serversRequested, bool forestWideAffinityRequested = false)
		{
			return this.InternalServiceProviderGetServersForRole(partitionFqdn, currentlyUsedServers, role, serversRequested, false, forestWideAffinityRequested);
		}

		private IList<ADServerInfo> InternalServiceProviderGetServersForRole(string partitionFqdn, IList<string> currentlyUsedServers, ADServerRole role, int serversRequested, bool throwOnServersNotFound = false, bool forestWideAffinityRequested = false)
		{
			if (ExTraceGlobals.TopologyProviderTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				string text = string.Join(",", currentlyUsedServers);
				ExTraceGlobals.TopologyProviderTracer.TraceDebug((long)this.GetHashCode(), "{0} ServiceTopologyprovider.GetServersForRole {1}, {2} current: [{3}], need {4} servers, forestWideAffinityRequested {5}", new object[]
				{
					partitionFqdn,
					role,
					currentlyUsedServers.Count,
					text,
					serversRequested,
					forestWideAffinityRequested
				});
			}
			IList<ADServerInfo> suitableServers = null;
			this.serviceProxyPool.CallServiceWithRetry(delegate(IPooledServiceProxy<ITopologyClient> proxy)
			{
				List<ServerInfo> serversForRole = proxy.Client.GetServersForRole(partitionFqdn, (List<string>)currentlyUsedServers, role, serversRequested, forestWideAffinityRequested);
				suitableServers = new List<ADServerInfo>(serversForRole.Count);
				foreach (ServerInfo serverInfo in serversForRole)
				{
					suitableServers.Add(serverInfo.ToADServerInfo());
				}
			}, string.Format("Get Servers for {0}", partitionFqdn), 3);
			if (suitableServers == null)
			{
				suitableServers = new List<ADServerInfo>(0);
			}
			if (suitableServers.Count == 0 && throwOnServersNotFound)
			{
				LocalizedString message;
				switch (role)
				{
				case ADServerRole.GlobalCatalog:
					message = DirectoryStrings.ErrorNoSuitableGC(this.machineName, partitionFqdn);
					goto IL_166;
				case ADServerRole.DomainController:
				case ADServerRole.ConfigurationDomainController:
					message = DirectoryStrings.ErrorNoSuitableDC(this.machineName, partitionFqdn);
					goto IL_166;
				}
				throw new ArgumentException("role");
				IL_166:
				throw new NoSuitableServerFoundException(message);
			}
			if (ExTraceGlobals.TopologyProviderTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ExTraceGlobals.TopologyProviderTracer.TraceDebug<int, string>((long)this.GetHashCode(), "ServiceTopologyprovider.GetServerForRole returning {0} servers: {1}", suitableServers.Count, string.Join<ADServerInfo>(" ", suitableServers));
			}
			foreach (ADServerInfo adserverInfo in suitableServers)
			{
				adserverInfo.Mapping = ((List<string>)currentlyUsedServers).IndexOf(adserverInfo.Fqdn);
			}
			return suitableServers;
		}

		private void InitializeServiceProxyPool()
		{
			ExTraceGlobals.TopologyProviderTracer.TraceDebug((long)this.GetHashCode(), "ServiceTopologyprovider - Initializing Service proxy pool");
			NetTcpBinding defaultBinding = TopologyServiceClient.CreateAndConfigureTopologyServiceBinding(this.machineName);
			EndpointAddress endpointAddress = TopologyServiceClient.CreateAndConfigureTopologyServiceEndpoint(this.machineName);
			this.serviceProxyPool = DirectoryServiceProxyPool<ITopologyClient>.CreateDirectoryServiceProxyPool("TopologyClientTcpEndpoint", endpointAddress, ExTraceGlobals.TopologyProviderTracer, 3, defaultBinding, new GetWrappedExceptionDelegate(ServiceTopologyProvider.GetTransientWrappedException), new GetWrappedExceptionDelegate(ServiceTopologyProvider.GetPermanentWrappedException), DirectoryEventLogConstants.Tuple_DSC_EVENT_CANNOT_CONTACT_AD_TOPOLOGY_SERVICE, true);
		}

		private const string ADTopologyServiceRegkey = "SYSTEM\\CurrentControlSet\\Services\\MSExchangeADTopology";

		private const string TopologyClientTcpEndpoint = "TopologyClientTcpEndpoint";

		private const string ServiceShortName = "MSExchangeADTopology";

		private const int TotalRetries = 2;

		private const int MaxNumberOfClientProxies = 3;

		private readonly string machineName;

		private DisposeTracker disposeTracker;

		private DirectoryServiceProxyPool<ITopologyClient> serviceProxyPool;
	}
}
