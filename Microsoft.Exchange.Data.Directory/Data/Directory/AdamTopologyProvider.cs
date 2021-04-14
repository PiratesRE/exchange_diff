using System;
using System.Collections.Generic;
using System.DirectoryServices.Protocols;
using System.Net;
using System.Threading;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.TopologyDiscovery;
using Microsoft.Exchange.Diagnostics.Components.Data.Directory;
using Microsoft.Win32;

namespace Microsoft.Exchange.Data.Directory
{
	internal class AdamTopologyProvider : TopologyProvider
	{
		internal AdamTopologyProvider(int adamPort)
		{
			ExTraceGlobals.TopologyProviderTracer.TraceDebug<int, int>((long)this.GetHashCode(), "Creating new ADAM topo provider instance {0}, port {1}", this.GetHashCode(), adamPort);
			this.adamPort = adamPort;
			this.topologyVersion = 1;
			ADProviderPerf.AddDCInstance(Environment.MachineName);
		}

		internal static bool CheckIfAdamConfigured(out int portNumber)
		{
			bool flag = false;
			RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\ExchangeServer\\v15\\EdgeTransportRole\\AdamSettings\\MSExchange\\");
			if (registryKey != null)
			{
				try
				{
					portNumber = (int)registryKey.GetValue("LdapPort", 389, RegistryValueOptions.DoNotExpandEnvironmentNames);
					flag = true;
					goto IL_40;
				}
				finally
				{
					registryKey.Close();
				}
			}
			portNumber = 0;
			IL_40:
			ExTraceGlobals.TopologyProviderTracer.TraceDebug<bool, int>(0L, "CheckIfAdamIsConfigured returns {0}, port {1}", flag, portNumber);
			return flag;
		}

		internal override TopologyMode TopologyMode
		{
			get
			{
				return TopologyMode.Adam;
			}
		}

		public override IList<TopologyVersion> GetAllTopologyVersions()
		{
			throw new NotSupportedException();
		}

		public int GetTopologyVersion()
		{
			return this.topologyVersion;
		}

		public override IList<TopologyVersion> GetTopologyVersions(IList<string> partitionFqdns)
		{
			if (partitionFqdns == null)
			{
				throw new ArgumentException("partitionFqdns");
			}
			TopologyProvider.EnforceLocalForestPartition(partitionFqdns[0]);
			return new List<TopologyVersion>
			{
				new TopologyVersion(partitionFqdns[0], this.topologyVersion)
			};
		}

		protected override IList<ADServerInfo> InternalGetServersForRole(string partitionFqdn, IList<string> currentlyUsedServers, ADServerRole role, int serversRequested, bool forestWideAffinityRequested = false)
		{
			TopologyProvider.EnforceLocalForestPartition(partitionFqdn);
			return new List<ADServerInfo>
			{
				this.GetDefaultServerInfo(partitionFqdn)
			};
		}

		public override ADServerInfo GetServerFromDomainDN(string distinguishedName, NetworkCredential credential)
		{
			throw new NotSupportedException("ADAM topology provider works only with local objects");
		}

		public override ADServerInfo GetRemoteServerFromDomainFqdn(string domainFqdn, NetworkCredential credential)
		{
			throw new NotSupportedException("ADAM topology provider works only with local objects");
		}

		internal override ADServerInfo GetConfigDCInfo(string partitionFqdn, bool throwOnFailure)
		{
			return this.GetDefaultServerInfo(partitionFqdn);
		}

		public override void ReportServerDown(string partitionFqdn, string serverName, ADServerRole role)
		{
			int arg = Interlocked.Increment(ref this.topologyVersion);
			ExTraceGlobals.TopologyProviderTracer.TraceDebug<int>((long)this.GetHashCode(), "New topo version is {0}", arg);
		}

		protected override ADServerInfo GetDefaultServerInfo(string partitionFqdn)
		{
			LocalizedString empty = LocalizedString.Empty;
			try
			{
				if (!SuitabilityVerifier.IsAdamServerSuitable("localhost", this.adamPort, null, ref empty))
				{
					throw new NoSuitableServerFoundException(empty);
				}
			}
			catch (ADTransientException ex)
			{
				throw new NoSuitableServerFoundException(new LocalizedString(ex.Message), ex);
			}
			return new ADServerInfo("localhost", this.adamPort, "OU=MsExchangeGateway", 100, AuthType.Negotiate);
		}

		internal override int DefaultDCPort
		{
			get
			{
				return this.adamPort;
			}
		}

		internal override int DefaultGCPort
		{
			get
			{
				return this.adamPort;
			}
		}

		public const string AdamNamingContext = "OU=MsExchangeGateway";

		private int adamPort;

		private int topologyVersion;
	}
}
