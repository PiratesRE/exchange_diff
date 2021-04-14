using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.DirectoryServices.Protocols;
using System.Linq;
using System.Net;
using System.Threading;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.EventLog;
using Microsoft.Exchange.Data.Directory.TopologyDiscovery;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Directory;

namespace Microsoft.Exchange.Data.Directory
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class LdapTopologyProvider : TopologyProvider
	{
		internal LdapTopologyProvider()
		{
			this.localServerFqdn = NativeHelpers.GetLocalComputerFqdn(false);
			this.topologies = new ConcurrentDictionary<string, LdapTopologyProvider.MiniTopology>(StringComparer.OrdinalIgnoreCase);
		}

		internal override TopologyMode TopologyMode
		{
			get
			{
				return TopologyMode.Ldap;
			}
		}

		public override IList<TopologyVersion> GetAllTopologyVersions()
		{
			throw new NotSupportedException();
		}

		public override IList<TopologyVersion> GetTopologyVersions(IList<string> partitionFqdns)
		{
			if (partitionFqdns == null)
			{
				throw new ArgumentNullException("partitionFqdns");
			}
			ExTraceGlobals.TopologyProviderTracer.Information<int>((long)this.GetHashCode(), "GetTopologyVersions. Partitions {0}", partitionFqdns.Count);
			TopologyVersion[] array = new TopologyVersion[partitionFqdns.Count];
			LdapTopologyProvider.MiniTopology miniTopology = null;
			for (int i = 0; i < partitionFqdns.Count; i++)
			{
				TopologyProvider.EnforceNonEmptyPartition(partitionFqdns[i]);
				int version = 1;
				if (this.topologies.TryGetValue(partitionFqdns[i], out miniTopology))
				{
					version = miniTopology.Version;
				}
				array[i] = new TopologyVersion(partitionFqdns[i], version);
			}
			if (ExTraceGlobals.TopologyProviderTracer.IsTraceEnabled(TraceType.InfoTrace))
			{
				ExTraceGlobals.TopologyProviderTracer.Information<string>((long)this.GetHashCode(), "GetTopologyVersions. Partitions {0}", string.Join(",", (object[])array));
			}
			return array;
		}

		protected override IList<ADServerInfo> InternalGetServersForRole(string partitionFqdn, IList<string> currentlyUsedServers, ADServerRole role, int serversRequested, bool forestWideAffinityRequested = false)
		{
			LdapTopologyProvider.MiniTopology miniTopology = new LdapTopologyProvider.MiniTopology(partitionFqdn);
			miniTopology = this.topologies.GetOrAdd(partitionFqdn, miniTopology);
			if (ExTraceGlobals.TopologyProviderTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				ExTraceGlobals.TopologyProviderTracer.TraceDebug((long)this.GetHashCode(), "PartitionFqdn {0}. GetServersForRole {1}, {2} current: [{3}], need {4} servers", new object[]
				{
					partitionFqdn,
					role,
					currentlyUsedServers.Count,
					string.Join(",", currentlyUsedServers ?? Enumerable.Empty<string>()),
					serversRequested
				});
			}
			ADServerInfo adserverInfo = null;
			if (miniTopology.DCInfo == null)
			{
				adserverInfo = this.GetDirectoryServer(partitionFqdn, ADRole.DomainController);
				miniTopology.SetServerInfo(adserverInfo, ADServerRole.DomainController);
				miniTopology.IncrementTopologyVersion();
				adserverInfo = null;
			}
			switch (role)
			{
			case ADServerRole.GlobalCatalog:
				adserverInfo = miniTopology.GCInfo;
				if (adserverInfo == null)
				{
					adserverInfo = this.GetDirectoryServer(partitionFqdn, ADRole.GlobalCatalog);
					miniTopology.SetServerInfo(adserverInfo, role);
					miniTopology.IncrementTopologyVersion();
				}
				break;
			case ADServerRole.DomainController:
			case ADServerRole.ConfigurationDomainController:
				adserverInfo = ((ADServerRole.DomainController == role) ? miniTopology.DCInfo : miniTopology.CDCInfo);
				if (adserverInfo == null)
				{
					adserverInfo = this.GetDirectoryServer(partitionFqdn, ADRole.DomainController);
					miniTopology.SetServerInfo(adserverInfo, role);
					miniTopology.IncrementTopologyVersion();
				}
				break;
			}
			ExTraceGlobals.TopologyProviderTracer.TraceDebug<string, string>((long)this.GetHashCode(), "PartitionFqdn {0}. GetServerForRole returning 1 server {1}", partitionFqdn, adserverInfo.FqdnPlusPort);
			ADProviderPerf.AddDCInstance(adserverInfo.Fqdn);
			ADServerInfo adserverInfo2 = (ADServerInfo)adserverInfo.Clone();
			adserverInfo2.Mapping = (adserverInfo2.Fqdn.Equals((currentlyUsedServers == null || currentlyUsedServers.Count == 0) ? string.Empty : currentlyUsedServers[0], StringComparison.OrdinalIgnoreCase) ? 0 : -1);
			return new List<ADServerInfo>
			{
				adserverInfo2
			};
		}

		public override ADServerInfo GetServerFromDomainDN(string distinguishedName, NetworkCredential credential)
		{
			ExTraceGlobals.TopologyProviderTracer.TraceDebug<string, string>((long)this.GetHashCode(), "Need server from domain {0}. Credentials {1} NULL", distinguishedName, (credential == null) ? "are" : "are NOT");
			ADServerInfo remoteServerFromDomainFqdn = this.GetRemoteServerFromDomainFqdn(NativeHelpers.CanonicalNameFromDistinguishedName(distinguishedName), credential);
			ExTraceGlobals.TopologyProviderTracer.TraceDebug<string>((long)this.GetHashCode(), "GetServerFromDomainDN returning {0}", remoteServerFromDomainFqdn.FqdnPlusPort);
			return remoteServerFromDomainFqdn;
		}

		public override ADServerInfo GetRemoteServerFromDomainFqdn(string domainFqdn, NetworkCredential credential)
		{
			ExTraceGlobals.TopologyProviderTracer.TraceDebug<string, string>((long)this.GetHashCode(), "Need server from remote domain {0} {1} credentials.", domainFqdn, (credential == null) ? "without" : "with");
			return LdapTopologyProvider.FindDirectoryServerForForestOrDomain(domainFqdn, credential, false);
		}

		public override void SetConfigDC(string partitionFqdn, string serverName, int port)
		{
			base.SetConfigDC(partitionFqdn, serverName, port);
			ExTraceGlobals.TopologyProviderTracer.TraceDebug<string, string, int>((long)this.GetHashCode(), "PartitionFqdn {0} setting Config DC to {1}:{2}", partitionFqdn, serverName, port);
			LdapTopologyProvider.MiniTopology miniTopology = new LdapTopologyProvider.MiniTopology(partitionFqdn);
			miniTopology = this.topologies.GetOrAdd(partitionFqdn, miniTopology);
			miniTopology.SetServerInfo(new ADServerInfo(serverName, partitionFqdn, port, null, 100, AuthType.Kerberos, true), ADServerRole.ConfigurationDomainController);
			miniTopology.IncrementTopologyVersion();
			ADProviderPerf.AddDCInstance(serverName);
		}

		internal override ADServerInfo GetConfigDCInfo(string partitionFqdn, bool throwOnFailure)
		{
			ExTraceGlobals.TopologyProviderTracer.TraceDebug<string>((long)this.GetHashCode(), "PartitionFqdn {0}. Get Config DC.", partitionFqdn);
			try
			{
				IList<ADServerInfo> serversForRole = base.GetServersForRole(partitionFqdn, new List<string>(), ADServerRole.ConfigurationDomainController, 1, false);
				if (serversForRole.Count == 0)
				{
					throw new NoSuitableServerFoundException(DirectoryStrings.ErrorNoSuitableDC(this.localServerFqdn, partitionFqdn));
				}
				return serversForRole[0];
			}
			catch (ADTransientException ex)
			{
				ExTraceGlobals.TopologyProviderTracer.TraceError<string, string>((long)this.GetHashCode(), "PartitionFqdn {0}. Could not get Config DC: {1}", partitionFqdn, ex.Message);
				if (throwOnFailure)
				{
					throw;
				}
			}
			return null;
		}

		public override void ReportServerDown(string partitionFqdn, string serverName, ADServerRole role)
		{
			if (string.IsNullOrEmpty(serverName))
			{
				ExTraceGlobals.TopologyProviderTracer.TraceWarning<string>((long)this.GetHashCode(), "PartitionFqdn {0}. ServerName is null or empty", partitionFqdn);
				return;
			}
			ArgumentValidator.ThrowIfNullOrEmpty("partitionFqdn", partitionFqdn);
			ArgumentValidator.ThrowIfNullOrEmpty("serverName", serverName);
			ExTraceGlobals.TopologyProviderTracer.TraceWarning<string, string, ADServerRole>((long)this.GetHashCode(), "PartitionFqdn {0}. {1} is down for role {2}. If this server is part of topology we will bump topology version.", partitionFqdn, serverName, role);
			LdapTopologyProvider.MiniTopology miniTopology = null;
			if (!this.topologies.TryGetValue(partitionFqdn, out miniTopology))
			{
				ExTraceGlobals.TopologyProviderTracer.TraceWarning<string>((long)this.GetHashCode(), "PartitionFqdn {0} NOT FOUND.", partitionFqdn);
				return;
			}
			ADServerInfo dcinfo = miniTopology.DCInfo;
			ADServerInfo cdcinfo = miniTopology.CDCInfo;
			ADServerInfo gcinfo = miniTopology.GCInfo;
			ADServerRole adserverRole = ADServerRole.None;
			if (dcinfo != null && serverName.Equals(dcinfo.Fqdn, StringComparison.OrdinalIgnoreCase))
			{
				miniTopology.ResetServerInfoForRole(ADServerRole.DomainController);
				adserverRole |= ADServerRole.DomainController;
			}
			if (cdcinfo != null && serverName.Equals(cdcinfo.Fqdn, StringComparison.OrdinalIgnoreCase))
			{
				miniTopology.ResetServerInfoForRole(ADServerRole.ConfigurationDomainController);
				adserverRole |= ADServerRole.ConfigurationDomainController;
			}
			if (gcinfo != null && gcinfo.Fqdn.Equals(serverName, StringComparison.OrdinalIgnoreCase))
			{
				miniTopology.ResetServerInfoForRole(ADServerRole.GlobalCatalog);
				adserverRole |= ADServerRole.GlobalCatalog;
			}
			if (adserverRole != ADServerRole.None)
			{
				miniTopology.IncrementTopologyVersion();
			}
			ExTraceGlobals.TopologyProviderTracer.TraceDebug<string, ADServerRole, int>((long)this.GetHashCode(), "PartitionFqdn {0}. Role(s) {1} were reported as down. Topology version change {2}.", partitionFqdn, adserverRole, miniTopology.Version);
		}

		public override int TopoRecheckIntervalMsec
		{
			get
			{
				if (!ExEnvironment.IsTest)
				{
					return 1000;
				}
				return 0;
			}
		}

		protected override ADServerInfo GetDefaultServerInfo(string partitionFqdn)
		{
			ExTraceGlobals.TopologyProviderTracer.TraceDebug<string>((long)this.GetHashCode(), "PartitionFqdn {0}. GetDefaultServerInfo", partitionFqdn);
			ADServerInfo result;
			try
			{
				IList<ADServerInfo> serversForRole = base.GetServersForRole(partitionFqdn, new List<string>(), ADServerRole.DomainController, 1, false);
				if (serversForRole.Count == 0)
				{
					throw new NoSuitableServerFoundException(DirectoryStrings.ErrorNoSuitableDC(this.localServerFqdn, partitionFqdn));
				}
				result = serversForRole[0];
			}
			catch (ADTransientException ex)
			{
				ExTraceGlobals.TopologyProviderTracer.TraceError<string, string>((long)this.GetHashCode(), "PartitionFqdn {0}. Could not get default server info: {1}", partitionFqdn, ex.Message);
				throw;
			}
			return result;
		}

		private static ADServerInfo FindDirectoryServerForForestOrDomain(string domainOrForestFqdn, NetworkCredential credential, bool requireGCs = false)
		{
			StringCollection stringCollection = requireGCs ? NativeHelpers.FindAllGlobalCatalogs(domainOrForestFqdn, null) : NativeHelpers.FindAllDomainControllers(domainOrForestFqdn, null);
			LocalizedString value = LocalizedString.Empty;
			LocalizedString empty = LocalizedString.Empty;
			string writableNC = null;
			string siteName = null;
			foreach (string text in stringCollection)
			{
				if (SuitabilityVerifier.IsServerSuitableIgnoreExceptions(text, requireGCs, credential, out writableNC, out siteName, out empty))
				{
					if (!requireGCs)
					{
						Globals.LogEvent(DirectoryEventLogConstants.Tuple_DSC_EVENT_GET_DC_FROM_DOMAIN, domainOrForestFqdn, new object[]
						{
							domainOrForestFqdn,
							text
						});
					}
					return new ADServerInfo(text, domainOrForestFqdn, requireGCs ? 3268 : 389, writableNC, 100, AuthType.Kerberos, true)
					{
						SiteName = siteName
					};
				}
				ExTraceGlobals.TopologyProviderTracer.TraceError(0L, "{0} {1} {2} was found not suitable. Will try to find another {1} in the forest\\domain. Error message: {3}", new object[]
				{
					domainOrForestFqdn,
					requireGCs ? "Global Catalog" : "Domain Controller",
					text,
					empty
				});
				value = DirectoryStrings.AppendLocalizedStrings(value, empty);
			}
			if (requireGCs)
			{
				Globals.LogEvent(DirectoryEventLogConstants.Tuple_DSC_EVENT_ALL_GC_DOWN, string.Empty, new object[]
				{
					domainOrForestFqdn,
					string.Empty
				});
				throw new NoSuitableServerFoundException(DirectoryStrings.ErrorNoSuitableGCInForest(domainOrForestFqdn, value));
			}
			Globals.LogEvent(DirectoryEventLogConstants.Tuple_DSC_EVENT_GET_DC_FROM_DOMAIN_FAILED, domainOrForestFqdn, new object[]
			{
				domainOrForestFqdn
			});
			throw new NoSuitableServerFoundException(DirectoryStrings.ErrorNoSuitableDCInDomain(domainOrForestFqdn, value));
		}

		private ADServerInfo GetDirectoryServer(string partitionFqdn, ADRole role)
		{
			ExTraceGlobals.TopologyProviderTracer.TraceDebug<string, ADRole>((long)this.GetHashCode(), "GetDirectoryServer PartitionFqdn {0}. Role {1}", partitionFqdn, role);
			LocatorFlags locatorFlags = LocatorFlags.ForceRediscovery | LocatorFlags.DirectoryServicesRequired | LocatorFlags.ReturnDnsName;
			string text = partitionFqdn;
			if (ADRole.GlobalCatalog == role)
			{
				ADObjectId rootDomainNamingContext = base.GetRootDomainNamingContext(partitionFqdn);
				ADObjectId domainNamingContext = base.GetDomainNamingContext(partitionFqdn);
				if (!rootDomainNamingContext.DistinguishedName.Equals(domainNamingContext.DistinguishedName, StringComparison.OrdinalIgnoreCase))
				{
					text = NativeHelpers.CanonicalNameFromDistinguishedName(rootDomainNamingContext.DistinguishedName);
				}
				locatorFlags |= LocatorFlags.GCRequired;
			}
			ExTraceGlobals.TopologyProviderTracer.TraceDebug<string, string, LocatorFlags>((long)this.GetHashCode(), "GetDirectoryServer. Partition Fqdn {0} Parent Domain {1}. Flags {2}", partitionFqdn, text, locatorFlags);
			ADServerInfo serverInfo = new ADServerInfo(null, text, (ADRole.GlobalCatalog == role) ? 3268 : 389, null, 100, AuthType.Kerberos, true);
			PooledLdapConnection pooledLdapConnection = null;
			ADServerInfo adserverInfo = null;
			try
			{
				pooledLdapConnection = LdapConnectionPool.CreateOneTimeConnection(null, serverInfo, locatorFlags);
				if (!string.IsNullOrEmpty(pooledLdapConnection.SessionOptions.HostName))
				{
					adserverInfo = pooledLdapConnection.ADServerInfo.CloneWithServerNameResolved(pooledLdapConnection.SessionOptions.HostName);
				}
				ExTraceGlobals.TopologyProviderTracer.TraceDebug<string, string>((long)this.GetHashCode(), "GetDirectoryServer. Partition Fqdn {0}. Server {1}", partitionFqdn, pooledLdapConnection.SessionOptions.HostName ?? string.Empty);
			}
			finally
			{
				if (pooledLdapConnection != null)
				{
					pooledLdapConnection.ReturnToPool();
				}
			}
			string text2;
			LocalizedString localizedString;
			if (adserverInfo != null && SuitabilityVerifier.IsServerSuitableIgnoreExceptions(adserverInfo.Fqdn, ADRole.GlobalCatalog == role, null, out text2, out localizedString))
			{
				return adserverInfo;
			}
			return LdapTopologyProvider.FindDirectoryServerForForestOrDomain(text, null, ADRole.GlobalCatalog == role);
		}

		private readonly string localServerFqdn;

		private ConcurrentDictionary<string, LdapTopologyProvider.MiniTopology> topologies;

		internal class MiniTopology
		{
			public MiniTopology(string partitionFqdn)
			{
				this.PartitionFqdn = partitionFqdn;
				this.version = 0;
			}

			public string PartitionFqdn { get; private set; }

			public int Version
			{
				get
				{
					return this.version;
				}
			}

			public ADServerInfo DCInfo { get; private set; }

			public ADServerInfo GCInfo { get; private set; }

			public ADServerInfo CDCInfo { get; private set; }

			public void IncrementTopologyVersion()
			{
				Interlocked.Increment(ref this.version);
			}

			public void ResetServerInfoForRole(ADServerRole role)
			{
				if (role == ADServerRole.None)
				{
					throw new ArgumentException("Invalid ADServerRole value. None");
				}
				switch (role)
				{
				case ADServerRole.GlobalCatalog:
					this.GCInfo = null;
					return;
				case ADServerRole.DomainController:
					this.DCInfo = null;
					return;
				case ADServerRole.ConfigurationDomainController:
					this.CDCInfo = null;
					return;
				}
				throw new NotSupportedException("Role Not Supported");
			}

			public void SetServerInfo(ADServerInfo serverInfo, ADServerRole role)
			{
				ArgumentValidator.ThrowIfNull("serverInfo", serverInfo);
				if (role == ADServerRole.None)
				{
					throw new ArgumentException("Invalid ADServerRole value. None");
				}
				switch (role)
				{
				case ADServerRole.GlobalCatalog:
				{
					this.GCInfo = serverInfo;
					if (!Globals.IsDatacenter || (this.DCInfo != null && this.CDCInfo != null))
					{
						return;
					}
					ExTraceGlobals.TopologyProviderTracer.TraceDebug<string, string>((long)this.GetHashCode(), "{0}. GC {1} will be set as DC or CDC", this.PartitionFqdn, serverInfo.Fqdn);
					ADServerInfo adserverInfo = serverInfo.CloneAsRole(ADRole.DomainController);
					if (this.DCInfo == null)
					{
						this.DCInfo = adserverInfo;
					}
					if (this.CDCInfo == null)
					{
						this.CDCInfo = adserverInfo;
						return;
					}
					return;
				}
				case ADServerRole.DomainController:
				case ADServerRole.ConfigurationDomainController:
				{
					if (this.CDCInfo == null)
					{
						this.CDCInfo = serverInfo;
					}
					if (this.DCInfo == null)
					{
						this.DCInfo = serverInfo;
					}
					if (!Globals.IsDatacenter || this.GCInfo != null)
					{
						return;
					}
					ExTraceGlobals.TopologyProviderTracer.TraceDebug<string, string>((long)this.GetHashCode(), "{0}. DC {1} will be set as GC", this.PartitionFqdn, serverInfo.Fqdn);
					ADServerInfo gcinfo = serverInfo.CloneAsRole(ADRole.GlobalCatalog);
					if (this.GCInfo == null)
					{
						this.GCInfo = gcinfo;
						return;
					}
					return;
				}
				}
				throw new NotSupportedException("Role Not Supported");
			}

			private int version;
		}
	}
}
