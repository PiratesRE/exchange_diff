using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.DirectoryServices.Protocols;
using System.Net;
using Microsoft.Exchange.Data.Directory.EventLog;
using Microsoft.Exchange.Data.Directory.TopologyDiscovery;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Directory;

namespace Microsoft.Exchange.Data.Directory
{
	internal abstract class TopologyProvider
	{
		internal static TopologyMode CurrentTopologyMode
		{
			get
			{
				if (TopologyProvider.staticInstance != null)
				{
					return TopologyProvider.GetInstance().TopologyMode;
				}
				int num;
				return TopologyProvider.SelectTopologyMode(out num);
			}
		}

		internal static string LocalForestFqdn
		{
			get
			{
				if (string.IsNullOrEmpty(TopologyProvider.localForestFqdn))
				{
					if (!ADSessionSettings.SessionSettingsFactory.Default.InDomain())
					{
						TopologyProvider.localForestFqdn = "localhost";
					}
					else
					{
						TopologyProvider.localForestFqdn = NativeHelpers.GetForestName();
					}
				}
				return TopologyProvider.localForestFqdn;
			}
		}

		internal static bool IsAdminMode
		{
			get
			{
				return TopologyMode.Ldap == TopologyProvider.CurrentTopologyMode && !TopologyProvider.isRunningOnTopologyService;
			}
		}

		internal static bool IsAdamTopology()
		{
			return TopologyProvider.CurrentTopologyMode == TopologyMode.Adam;
		}

		internal static void SetProcessTopologyMode(bool isAdminMode, bool publicMethodCheck)
		{
			ExTraceGlobals.TopologyProviderTracer.TraceDebug<string, bool>(0L, "{0} method sets admin mode to {1}", publicMethodCheck ? "Public" : "Internal", isAdminMode);
			int num = 0;
			if (!publicMethodCheck || (TopologyProvider.staticInstance != null && TopologyProvider.CurrentTopologyMode == TopologyMode.Ldap && AdamTopologyProvider.CheckIfAdamConfigured(out num)) || TopologyProvider.staticInstance == null)
			{
				TopologyProvider.SetProcessTopologyMode(isAdminMode ? TopologyMode.Ldap : TopologyMode.ADTopologyService);
			}
		}

		internal static void SetProcessTopologyMode(TopologyMode mode)
		{
			if (TopologyMode.Adam == mode)
			{
				throw new ArgumentException("mode. Adam topology mode can't be specified");
			}
			if (TopologyMode.Ldap == mode && TopologyProvider.IsTopologyServiceProcess())
			{
				TopologyProvider.isRunningOnTopologyService = true;
			}
			ExTraceGlobals.TopologyProviderTracer.TraceDebug<TopologyMode, TopologyMode>(0L, "User set topology mode from {0} to {1}", (TopologyProvider.userSetTopologyMode != null) ? TopologyProvider.userSetTopologyMode.Value : TopologyProvider.CurrentTopologyMode, mode);
			TopologyProvider.userSetTopologyMode = new TopologyMode?(mode);
			int num;
			TopologyMode topologyMode = TopologyProvider.SelectTopologyMode(out num);
			if (TopologyProvider.staticInstance != null && (TopologyProvider.userSetTopologyMode.Value != TopologyProvider.staticInstance.TopologyMode || TopologyProvider.userSetTopologyMode.Value != topologyMode))
			{
				IDisposable disposable = TopologyProvider.staticInstance as IDisposable;
				TopologyProvider topologyProvider = TopologyProvider.InitializeInstance();
				topologyProvider.PopulateConfigNamingContextsForLocalForest();
				topologyProvider.PopulateDomainNamingContextsForLocalForest();
				ConnectionPoolManager.Reset();
				if (disposable != null)
				{
					disposable.Dispose();
				}
			}
		}

		private static TopologyMode SelectTopologyMode(out int adamPort)
		{
			adamPort = 0;
			if (AdamTopologyProvider.CheckIfAdamConfigured(out adamPort))
			{
				return TopologyMode.Adam;
			}
			if (TopologyProvider.userSetTopologyMode != null)
			{
				return TopologyProvider.userSetTopologyMode.Value;
			}
			if (ServiceTopologyProvider.IsAdTopologyServiceInstalled())
			{
				return TopologyMode.ADTopologyService;
			}
			return TopologyMode.Ldap;
		}

		private static TopologyProvider InitializeInstance()
		{
			ExTraceGlobals.TopologyProviderTracer.TraceDebug(0L, "Starting InitializeInstance and waiting for the lock");
			TopologyProvider result;
			lock (TopologyProvider.instanceLockRoot)
			{
				int adamPort;
				switch (TopologyProvider.SelectTopologyMode(out adamPort))
				{
				case TopologyMode.ADTopologyService:
				{
					ServiceTopologyProvider serviceTopologyProvider = new ServiceTopologyProvider();
					serviceTopologyProvider.SuppressDisposeTracker();
					TopologyProvider.staticInstance = serviceTopologyProvider;
					break;
				}
				case TopologyMode.Adam:
					TopologyProvider.staticInstance = new AdamTopologyProvider(adamPort);
					break;
				case TopologyMode.Ldap:
					TopologyProvider.staticInstance = new LdapTopologyProvider();
					break;
				}
				ExTraceGlobals.TopologyProviderTracer.TraceDebug<int>(0L, "TopologyProvider::InitializeInstance created {0}", (int)TopologyProvider.CurrentTopologyMode);
				result = TopologyProvider.staticInstance;
			}
			return result;
		}

		[Conditional("DEBUG")]
		private static void PopulateTopologyModeStackSwitch()
		{
			new StackTrace(true);
		}

		internal static TopologyProvider GetInstance()
		{
			TopologyProvider topologyProvider = TopologyProvider.staticInstance;
			if (topologyProvider == null)
			{
				topologyProvider = TopologyProvider.InitializeInstance();
			}
			return topologyProvider;
		}

		protected void PopulateConfigNamingContextsForLocalForest()
		{
			this.PopulateConfigNamingContexts(TopologyProvider.LocalForestFqdn);
		}

		protected void PopulateConfigNamingContexts(string partitionFqdn)
		{
			TopologyProvider.EnforceNonEmptyPartition(partitionFqdn);
			ADServerInfo configDCInfo = this.GetConfigDCInfo(partitionFqdn, true);
			if (string.IsNullOrEmpty(configDCInfo.ConfigNC) || string.IsNullOrEmpty(configDCInfo.SchemaNC))
			{
				PooledLdapConnection pooledLdapConnection = LdapConnectionPool.CreateOneTimeConnection(null, configDCInfo, LocatorFlags.None);
				try
				{
					if (string.IsNullOrEmpty(pooledLdapConnection.ADServerInfo.ConfigNC))
					{
						this.LogRootDSEReadFailureAndThrow("configurationNamingContext", configDCInfo.FqdnPlusPort);
					}
					this.configNCs[partitionFqdn] = ADObjectId.ParseExtendedDN(pooledLdapConnection.ADServerInfo.ConfigNC);
					if (string.IsNullOrEmpty(pooledLdapConnection.ADServerInfo.SchemaNC))
					{
						this.LogRootDSEReadFailureAndThrow("schemaNamingContext", configDCInfo.FqdnPlusPort);
					}
					this.schemaNCs[partitionFqdn] = ADObjectId.ParseExtendedDN(pooledLdapConnection.ADServerInfo.SchemaNC);
					return;
				}
				finally
				{
					pooledLdapConnection.ReturnToPool();
				}
			}
			this.configNCs[partitionFqdn] = ADObjectId.ParseExtendedDN(configDCInfo.ConfigNC);
			this.schemaNCs[partitionFqdn] = ADObjectId.ParseExtendedDN(configDCInfo.SchemaNC);
		}

		protected void ClearConfigNamingContexts(string partitionFqdn)
		{
			TopologyProvider.EnforceNonEmptyPartition(partitionFqdn);
			ADObjectId adobjectId;
			this.configNCs.TryRemove(partitionFqdn, out adobjectId);
			this.schemaNCs.TryRemove(partitionFqdn, out adobjectId);
		}

		protected void PopulateDomainNamingContextsForLocalForest()
		{
			this.PopulateDomainNamingContexts(TopologyProvider.LocalForestFqdn);
		}

		protected void PopulateDomainNamingContexts(string partitionFqdn)
		{
			TopologyProvider.EnforceNonEmptyPartition(partitionFqdn);
			ADServerInfo defaultServerInfo = this.GetDefaultServerInfo(partitionFqdn);
			if (string.IsNullOrEmpty(defaultServerInfo.WritableNC) || string.IsNullOrEmpty(defaultServerInfo.RootDomainNC))
			{
				PooledLdapConnection pooledLdapConnection = LdapConnectionPool.CreateOneTimeConnection(null, defaultServerInfo, LocatorFlags.None);
				try
				{
					if (string.IsNullOrEmpty(pooledLdapConnection.ADServerInfo.WritableNC) && !TopologyProvider.IsAdamTopology())
					{
						this.LogRootDSEReadFailureAndThrow("domainNamingContext", defaultServerInfo.FqdnPlusPort);
					}
					this.domainNCs[partitionFqdn] = ADObjectId.ParseExtendedDN(pooledLdapConnection.ADServerInfo.WritableNC);
					if (string.IsNullOrEmpty(pooledLdapConnection.ADServerInfo.RootDomainNC) && !TopologyProvider.IsAdamTopology())
					{
						this.LogRootDSEReadFailureAndThrow("rootDomainNamingContext", defaultServerInfo.FqdnPlusPort);
					}
					this.rootDomainNCs[partitionFqdn] = ADObjectId.ParseExtendedDN(pooledLdapConnection.ADServerInfo.RootDomainNC);
					return;
				}
				finally
				{
					pooledLdapConnection.ReturnToPool();
				}
			}
			this.domainNCs[partitionFqdn] = ADObjectId.ParseExtendedDN(defaultServerInfo.WritableNC);
			this.rootDomainNCs[partitionFqdn] = ADObjectId.ParseExtendedDN(defaultServerInfo.RootDomainNC);
		}

		protected void ClearDomainNamingContextsForLocalForest()
		{
			this.ClearDomainNamingContexts(TopologyProvider.LocalForestFqdn);
		}

		protected void ClearDomainNamingContexts(string partitionFqdn)
		{
			TopologyProvider.EnforceNonEmptyPartition(partitionFqdn);
			ADObjectId adobjectId;
			this.domainNCs.TryRemove(partitionFqdn, out adobjectId);
			this.rootDomainNCs.TryRemove(partitionFqdn, out adobjectId);
		}

		private void LogRootDSEReadFailureAndThrow(string attribute, string server)
		{
			ExTraceGlobals.ADTopologyTracer.TraceError<string, string>(0L, "Could not read {0} from root DSE of {1}", attribute, server);
			Globals.LogEvent(DirectoryEventLogConstants.Tuple_DSC_EVENT_ROOTDSE_READ_FAILED, attribute, new object[]
			{
				attribute,
				server
			});
			throw new ADTransientException(DirectoryStrings.ExceptionRootDSE(attribute, server));
		}

		internal ADObjectId GetConfigurationNamingContextForLocalForest()
		{
			return this.GetConfigurationNamingContext(TopologyProvider.LocalForestFqdn);
		}

		internal ADObjectId GetConfigurationNamingContext(string partitionFqdn)
		{
			TopologyProvider.EnforceNonEmptyPartition(partitionFqdn);
			ADObjectId result;
			if (!this.configNCs.TryGetValue(partitionFqdn, out result))
			{
				this.PopulateConfigNamingContexts(partitionFqdn);
				this.configNCs.TryGetValue(partitionFqdn, out result);
			}
			return result;
		}

		internal ADObjectId GetSchemaNamingContextForLocalForest()
		{
			return this.GetSchemaNamingContext(TopologyProvider.LocalForestFqdn);
		}

		internal ADObjectId GetSchemaNamingContext(string partitionFqdn)
		{
			TopologyProvider.EnforceNonEmptyPartition(partitionFqdn);
			ADObjectId result;
			if (!this.schemaNCs.TryGetValue(partitionFqdn, out result))
			{
				this.PopulateConfigNamingContexts(partitionFqdn);
				this.schemaNCs.TryGetValue(partitionFqdn, out result);
			}
			return result;
		}

		internal ADObjectId GetDomainNamingContextForLocalForest()
		{
			return this.GetDomainNamingContext(TopologyProvider.LocalForestFqdn);
		}

		internal ADObjectId GetDomainNamingContext(string partitionFqdn)
		{
			TopologyProvider.EnforceNonEmptyPartition(partitionFqdn);
			ADObjectId result;
			if (!this.domainNCs.TryGetValue(partitionFqdn, out result))
			{
				this.PopulateDomainNamingContexts(partitionFqdn);
				this.domainNCs.TryGetValue(partitionFqdn, out result);
			}
			return result;
		}

		internal ADObjectId GetRootDomainNamingContextForLocalForest()
		{
			return this.GetRootDomainNamingContext(TopologyProvider.LocalForestFqdn);
		}

		internal ADObjectId GetRootDomainNamingContext(string partitionFqdn)
		{
			TopologyProvider.EnforceNonEmptyPartition(partitionFqdn);
			ADObjectId result;
			if (!this.rootDomainNCs.TryGetValue(partitionFqdn, out result))
			{
				this.PopulateDomainNamingContexts(partitionFqdn);
				this.rootDomainNCs.TryGetValue(partitionFqdn, out result);
			}
			return result;
		}

		internal abstract TopologyMode TopologyMode { get; }

		public abstract IList<TopologyVersion> GetAllTopologyVersions();

		public abstract IList<TopologyVersion> GetTopologyVersions(IList<string> partitionFqdns);

		public int GetTopologyVersion(string partitionFqdn)
		{
			TopologyProvider.EnforceNonEmptyPartition(partitionFqdn);
			IList<TopologyVersion> topologyVersions = this.GetTopologyVersions(new List<string>
			{
				partitionFqdn
			});
			if (topologyVersions.Count > 0)
			{
				return topologyVersions[0].Version;
			}
			return -1;
		}

		public IList<ADServerInfo> GetServersForRole(string partitionFqdn, IList<string> currentlyUsedServers, ADServerRole role, int serversRequested, bool forestWideAffinityRequested = false)
		{
			ArgumentValidator.ThrowIfNullOrEmpty("partitionFqdn", partitionFqdn);
			return this.InternalGetServersForRole(partitionFqdn, currentlyUsedServers, role, serversRequested, forestWideAffinityRequested);
		}

		public abstract ADServerInfo GetServerFromDomainDN(string distinguishedName, NetworkCredential credential);

		public abstract ADServerInfo GetRemoteServerFromDomainFqdn(string domainFqdn, NetworkCredential credential);

		public virtual void SetConfigDC(string partitionFqdn, string serverName, int port)
		{
			if (string.IsNullOrEmpty(partitionFqdn))
			{
				throw new ArgumentException("partitionFqdn");
			}
			if (string.IsNullOrEmpty(serverName))
			{
				throw new ArgumentException("serverName");
			}
			this.ClearConfigNamingContexts(partitionFqdn);
		}

		public string GetConfigDCForLocalForest()
		{
			return this.GetConfigDC(TopologyProvider.LocalForestFqdn);
		}

		public string GetConfigDC(string partitionFqdn)
		{
			TopologyProvider.EnforceNonEmptyPartition(partitionFqdn);
			return this.GetConfigDC(partitionFqdn, true);
		}

		public string GetConfigDC(string partitionFqdn, bool throwOnFailure)
		{
			ADServerInfo configDCInfo = this.GetConfigDCInfo(partitionFqdn, throwOnFailure);
			if (configDCInfo != null)
			{
				return configDCInfo.Fqdn;
			}
			return null;
		}

		internal abstract ADServerInfo GetConfigDCInfo(string partitionFqdn, bool throwOnFailure);

		public abstract void ReportServerDown(string partitionFqdn, string serverName, ADServerRole role);

		protected virtual ADServerInfo GetDefaultServerInfo(string partitionFqdn)
		{
			string serverFqdn = null;
			if (partitionFqdn.Equals(TopologyProvider.LocalForestFqdn, StringComparison.OrdinalIgnoreCase) && NativeHelpers.LocalMachineRoleIsDomainController())
			{
				serverFqdn = NativeHelpers.GetLocalComputerFqdn(false);
			}
			return new ADServerInfo(serverFqdn, partitionFqdn, this.DefaultDCPort, null, 100, AuthType.Kerberos, true);
		}

		protected abstract IList<ADServerInfo> InternalGetServersForRole(string partitionFqdn, IList<string> currentlyUsedServers, ADServerRole role, int serversRequested, bool forestWideAffinityRequested = false);

		public virtual int TopoRecheckIntervalMsec
		{
			get
			{
				return 0;
			}
		}

		internal virtual int DefaultDCPort
		{
			get
			{
				return 389;
			}
		}

		internal virtual int DefaultGCPort
		{
			get
			{
				return 3268;
			}
		}

		protected static void EnforceLocalForestPartition(string partitionFqdn)
		{
			if (partitionFqdn != TopologyProvider.localForestFqdn)
			{
				throw new ArgumentException("partitionFqdn");
			}
		}

		internal static void EnforceNonEmptyPartition(string partitionFqdn)
		{
			if (string.IsNullOrEmpty(partitionFqdn))
			{
				throw new ArgumentException("partitionFqdn");
			}
		}

		protected static bool IsTopologyServiceProcess()
		{
			return Globals.ProcessName.Equals("Microsoft.Exchange.Directory.TopologyService.exe", StringComparison.OrdinalIgnoreCase) || Globals.ProcessName.Equals("Internal.Exchange.TopologyDiscovery.exe", StringComparison.OrdinalIgnoreCase);
		}

		private static TopologyMode? userSetTopologyMode;

		private static bool isRunningOnTopologyService = false;

		private static object instanceLockRoot = new object();

		private static TopologyProvider staticInstance;

		private static string localForestFqdn;

		private ConcurrentDictionary<string, ADObjectId> configNCs = new ConcurrentDictionary<string, ADObjectId>(StringComparer.OrdinalIgnoreCase);

		private ConcurrentDictionary<string, ADObjectId> schemaNCs = new ConcurrentDictionary<string, ADObjectId>(StringComparer.OrdinalIgnoreCase);

		private ConcurrentDictionary<string, ADObjectId> domainNCs = new ConcurrentDictionary<string, ADObjectId>(StringComparer.OrdinalIgnoreCase);

		private ConcurrentDictionary<string, ADObjectId> rootDomainNCs = new ConcurrentDictionary<string, ADObjectId>(StringComparer.OrdinalIgnoreCase);
	}
}
