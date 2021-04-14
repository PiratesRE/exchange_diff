using System;
using System.Diagnostics;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.Directory.TopologyService;

namespace Microsoft.Exchange.Data.Directory.TopologyDiscovery
{
	[DebuggerDisplay("{DnsName}-{suitableRoles}")]
	internal class DirectoryServer
	{
		public DirectoryServer(ADServer server, NtdsDsa ntdsdsa)
		{
			if (server == null)
			{
				throw new ArgumentNullException("server");
			}
			if (ntdsdsa == null)
			{
				throw new ArgumentNullException("ntdsdsa");
			}
			if (!ntdsdsa.Id.Parent.Equals(server.Id))
			{
				throw new ArgumentException("ntdsdsa mismatch with server");
			}
			if (string.IsNullOrEmpty(server.DnsHostName))
			{
				throw new ArgumentException("server.DnsHostName null or empty");
			}
			this.server = server;
			this.isGC = (NtdsdsaOptions.IsGC == ntdsdsa.Options);
			this.SuitabilityResult = new SuitabilityCheckResult();
			this.suitableRoles = (ADServerRole.DomainController | ADServerRole.ConfigurationDomainController);
			if (this.IsGC)
			{
				this.suitableRoles |= ADServerRole.GlobalCatalog;
			}
			this.SuitabilityResult.IsEnabled = true;
			ADObjectId adobjectId = ntdsdsa.MasterNCs.Find((ADObjectId x) => x.DescendantDN(0).Equals(x));
			this.writableDomainNC = adobjectId;
		}

		public string DnsName
		{
			get
			{
				return this.server.DnsHostName;
			}
		}

		public bool IsGC
		{
			get
			{
				return this.isGC;
			}
		}

		public ADObjectId WritableDomainNC
		{
			get
			{
				return this.writableDomainNC;
			}
		}

		public string Site
		{
			get
			{
				return this.server.Site.Name;
			}
		}

		public SuitabilityCheckResult SuitabilityResult { get; private set; }

		public bool IsSuitableForRole(ADServerRole role)
		{
			bool flag = (this.suitableRoles & role) == role && this.SuitabilityResult.IsSuitable(role);
			ExTraceGlobals.TopologyTracer.TraceInformation<string, ADServerRole, bool>(this.GetHashCode(), (long)this.GetHashCode(), "{0} - IsSuitableForRole {1} returns {2}", this.DnsName, role, flag);
			return flag;
		}

		public bool TryGetServerInfoForRole(ADServerRole role, out ServerInfo serverInfo, bool forestWideAffinityRequested = false)
		{
			serverInfo = null;
			bool flag = this.IsSuitableForRole(role);
			if (!forestWideAffinityRequested && !flag)
			{
				return false;
			}
			serverInfo = new ServerInfo(this.server.DnsHostName, this.server.Id.GetPartitionId().ForestFQDN, (role == ADServerRole.GlobalCatalog) ? 3268 : 389)
			{
				WritableNC = this.writableDomainNC.DistinguishedName,
				SiteName = this.server.Site.Name,
				ConfigNC = this.SuitabilityResult.ConfigNC,
				RootDomainNC = this.SuitabilityResult.RootNC,
				SchemaNC = this.SuitabilityResult.SchemaNC,
				IsServerSuitable = flag
			};
			return true;
		}

		public void SetSuitabilityForRole(ADServerRole role, bool isSuitable)
		{
			ExTraceGlobals.TopologyTracer.TraceInformation<string, ADServerRole, bool>(this.GetHashCode(), (long)this.GetHashCode(), "{0} - SetSuitabilityForRole {1} {2}", this.DnsName, role, isSuitable);
			ADServerRole adserverRole;
			switch (role)
			{
			case ADServerRole.GlobalCatalog:
				if (!this.IsGC && isSuitable)
				{
					throw new NotSupportedException("Directory Server is not a GC, unable to set suitability role for GC.");
				}
				adserverRole = (isSuitable ? ADServerRole.GlobalCatalog : (ADServerRole.DomainController | ADServerRole.ConfigurationDomainController));
				goto IL_77;
			case ADServerRole.DomainController:
			case ADServerRole.ConfigurationDomainController:
				adserverRole = (isSuitable ? (ADServerRole.DomainController | ADServerRole.ConfigurationDomainController) : ADServerRole.GlobalCatalog);
				goto IL_77;
			}
			throw new NotSupportedException("Invalid Role Type");
			IL_77:
			if (isSuitable)
			{
				this.suitableRoles |= adserverRole;
				this.SuitabilityResult.IsReachableByTCPConnection |= adserverRole;
				return;
			}
			this.suitableRoles &= adserverRole;
			this.SuitabilityResult.IsReachableByTCPConnection &= adserverRole;
		}

		public bool HasAnySuitableRole()
		{
			return this.IsSuitableForRole(ADServerRole.DomainController) || this.IsSuitableForRole(ADServerRole.ConfigurationDomainController) || this.IsSuitableForRole(ADServerRole.GlobalCatalog);
		}

		public override string ToString()
		{
			string arg = string.Format("{0}{1}{2}", ((this.suitableRoles & ADServerRole.ConfigurationDomainController) == ADServerRole.ConfigurationDomainController) ? "C" : "-", ((this.suitableRoles & ADServerRole.DomainController) == ADServerRole.DomainController) ? "D" : "-", ((this.suitableRoles & ADServerRole.GlobalCatalog) == ADServerRole.GlobalCatalog) ? "G" : "-");
			return string.Format("{0}\t{1} {2}", this.DnsName, arg, this.SuitabilityResult.ToString());
		}

		internal void RefreshCounters(bool isDCInLocalSite)
		{
			ADProviderPerf.AddDCInstance(this.DnsName);
			ADProviderPerf.UpdateDCCounter(this.DnsName, Counter.DCLocalSite, UpdateType.Update, (uint)Convert.ToUInt16(isDCInLocalSite));
			ADProviderPerf.UpdateDCCounter(this.DnsName, Counter.DCStateReachability, UpdateType.Update, (uint)Convert.ToUInt16(this.SuitabilityResult.IsReachableByTCPConnection));
			ADProviderPerf.UpdateDCCounter(this.DnsName, Counter.DCStateSynchronized, UpdateType.Update, (uint)Convert.ToUInt16(this.SuitabilityResult.IsSynchronized));
			ADProviderPerf.UpdateDCCounter(this.DnsName, Counter.DCStateGCCapable, UpdateType.Update, (uint)Convert.ToUInt16(this.IsGC));
			ADProviderPerf.UpdateDCCounter(this.DnsName, Counter.DCStateIsPdc, UpdateType.Update, (uint)Convert.ToUInt16(this.SuitabilityResult.IsPDC));
			ADProviderPerf.UpdateDCCounter(this.DnsName, Counter.DCStateSaclRight, UpdateType.Update, (uint)Convert.ToUInt16(this.SuitabilityResult.IsSACLRightAvailable));
			ADProviderPerf.UpdateDCCounter(this.DnsName, Counter.DCStateCriticalData, UpdateType.Update, (uint)Convert.ToUInt16(this.SuitabilityResult.IsCriticalDataAvailable));
			ADProviderPerf.UpdateDCCounter(this.DnsName, Counter.DCStateNetlogon, UpdateType.Update, (uint)Convert.ToUInt16(this.SuitabilityResult.IsNetlogonAllowed));
			ADProviderPerf.UpdateDCCounter(this.DnsName, Counter.DCStateOsversion, UpdateType.Update, (uint)Convert.ToUInt16(this.SuitabilityResult.IsOSVersionSuitable));
		}

		[Conditional("DEBUG")]
		private void CheckProcess()
		{
			string processName = Globals.ProcessName;
			if (!processName.Equals("Microsoft.Exchange.Directory.TopologyService.exe", StringComparison.OrdinalIgnoreCase) && !processName.Equals("PerseusStudio.exe", StringComparison.OrdinalIgnoreCase) && !processName.Equals("Internal.Exchange.TopologyDiscovery.exe", StringComparison.OrdinalIgnoreCase))
			{
				processName.Equals("PerseusHarnessRuntime.exe", StringComparison.OrdinalIgnoreCase);
			}
		}

		private readonly bool isGC;

		private readonly ADObjectId writableDomainNC;

		private ADServer server;

		private ADServerRole suitableRoles;
	}
}
