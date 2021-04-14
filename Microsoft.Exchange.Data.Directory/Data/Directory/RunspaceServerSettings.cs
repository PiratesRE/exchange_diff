using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Directory;
using Microsoft.Exchange.Extensions;

namespace Microsoft.Exchange.Data.Directory
{
	[Serializable]
	internal class RunspaceServerSettings : ADServerSettings, ICloneable, IEquatable<RunspaceServerSettings>
	{
		protected RunspaceServerSettings()
		{
			this.ViewEntireForest = false;
		}

		private RunspaceServerSettings(string partitionFqdn, ADServerInfo serverInfo, string token) : this()
		{
			base.Token = token;
			this.partitionFqdn = partitionFqdn;
			if (serverInfo != null)
			{
				this.SetPreferredGlobalCatalog(partitionFqdn, serverInfo);
				this.SetConfigurationDomainController(partitionFqdn, serverInfo);
				this.AddPreferredDC(serverInfo);
			}
		}

		private RunspaceServerSettings(string partitionFqdn, ADServerInfo gc, ADServerInfo cdc) : this()
		{
			this.partitionFqdn = partitionFqdn;
			if (gc != null)
			{
				this.SetPreferredGlobalCatalog(partitionFqdn, gc);
			}
			if (cdc != null)
			{
				this.SetConfigurationDomainController(partitionFqdn, cdc);
			}
		}

		protected override bool EnforceIsUpdatableByADSession
		{
			get
			{
				return true;
			}
		}

		internal static string GetTokenForOrganization(OrganizationId organization)
		{
			return organization.OrganizationalUnit.Name.ToLowerInvariant();
		}

		internal static string GetTokenForUser(string userId, OrganizationId organization)
		{
			return string.Format("{0}@{1}", userId, organization.OrganizationalUnit.Name).ToLowerInvariant();
		}

		internal static RunspaceServerSettings CreateGcOnlyRunspaceServerSettings(string token, bool forestWideAffinityRequested = false)
		{
			return RunspaceServerSettings.CreateGcOnlyRunspaceServerSettings(token, TopologyProvider.LocalForestFqdn, forestWideAffinityRequested);
		}

		internal static RunspaceServerSettings CreateGcOnlyRunspaceServerSettings(string token, string partitionFqdn, bool forestWideAffinityRequested = false)
		{
			if (string.IsNullOrEmpty(token))
			{
				throw new ArgumentNullException("token");
			}
			if (string.IsNullOrEmpty(partitionFqdn))
			{
				throw new ArgumentNullException("partitionFqdn");
			}
			ADRunspaceServerSettingsProvider instance = ADRunspaceServerSettingsProvider.GetInstance();
			ExTraceGlobals.ServerSettingsProviderTracer.TraceDebug<string>((long)instance.GetHashCode(), "GetGcOnlyRunspaceServerSettings for token {0}", token);
			bool flag;
			ADServerInfo gcFromToken = instance.GetGcFromToken(partitionFqdn, token, out flag, forestWideAffinityRequested);
			return new RunspaceServerSettings(partitionFqdn, gcFromToken, token)
			{
				ViewEntireForest = true
			};
		}

		internal static RunspaceServerSettings CreateRunspaceServerSettings(bool forestWideAffinityRequested = false)
		{
			return RunspaceServerSettings.CreateRunspaceServerSettings(TopologyProvider.LocalForestFqdn, forestWideAffinityRequested);
		}

		internal static RunspaceServerSettings CreateRunspaceServerSettings(string partitionFqdn, bool forestWideAffinityRequested = false)
		{
			ADRunspaceServerSettingsProvider instance = ADRunspaceServerSettingsProvider.GetInstance();
			bool flag;
			ADServerInfo gcFromToken = instance.GetGcFromToken(partitionFqdn, null, out flag, forestWideAffinityRequested);
			RunspaceServerSettings runspaceServerSettings = new RunspaceServerSettings(partitionFqdn, gcFromToken, instance.GetConfigDc(partitionFqdn));
			if (flag)
			{
				runspaceServerSettings.AddPreferredDC(gcFromToken);
			}
			return runspaceServerSettings;
		}

		internal override Fqdn PreferredGlobalCatalog(string partitionFqdn)
		{
			Fqdn fqdn = this.userPreferredGlobalCatalog;
			if (fqdn != null && ADServerSettings.IsServerNamePartitionSameAsPartitionId(fqdn, partitionFqdn))
			{
				return fqdn;
			}
			return base.PreferredGlobalCatalog(partitionFqdn);
		}

		internal Fqdn GetSingleDefaultGlobalCatalog()
		{
			if (!string.IsNullOrEmpty(this.partitionFqdn))
			{
				return this.PreferredGlobalCatalog(this.partitionFqdn);
			}
			return this.PreferredGlobalCatalog(TopologyProvider.LocalForestFqdn);
		}

		internal Fqdn GetSingleDefaultConfigurationDomainController()
		{
			if (!string.IsNullOrEmpty(this.partitionFqdn))
			{
				return this.ConfigurationDomainController(this.partitionFqdn);
			}
			return this.ConfigurationDomainController(TopologyProvider.LocalForestFqdn);
		}

		internal Fqdn UserPreferredGlobalCatalog
		{
			get
			{
				return this.userPreferredGlobalCatalog;
			}
		}

		internal void SetUserPreferredGlobalCatalog(Fqdn fqdn)
		{
			ADServerSettings.GetServerInfoFromFqdn(fqdn, ConnectionType.GlobalCatalog);
			this.userPreferredGlobalCatalog = fqdn;
		}

		internal void ClearUserPreferredGlobalCatalog()
		{
			this.userPreferredGlobalCatalog = null;
		}

		internal override Fqdn ConfigurationDomainController(string partitionFqdn)
		{
			Fqdn fqdn = this.userConfigurationDomainController;
			if (fqdn != null && ADServerSettings.IsServerNamePartitionSameAsPartitionId(fqdn, partitionFqdn))
			{
				return fqdn;
			}
			return base.ConfigurationDomainController(partitionFqdn);
		}

		internal Fqdn UserConfigurationDomainController
		{
			get
			{
				return this.userConfigurationDomainController;
			}
		}

		internal void SetUserConfigurationDomainController(Fqdn fqdn)
		{
			ADServerSettings.GetServerInfoFromFqdn(fqdn, ConnectionType.DomainController);
			this.userConfigurationDomainController = fqdn;
		}

		internal void ClearUserConfigurationDomainController()
		{
			this.userConfigurationDomainController = null;
		}

		internal MultiValuedProperty<Fqdn> UserPreferredDomainControllers
		{
			get
			{
				return this.cachedUserPreferredDCList;
			}
		}

		internal override string GetPreferredDC(ADObjectId domain)
		{
			Fqdn fqdn;
			if (this.userServerPerDomain.TryGetValue(domain, out fqdn))
			{
				return fqdn;
			}
			return base.GetPreferredDC(domain);
		}

		internal override void AddPreferredDC(ADServerInfo serverInfo)
		{
			ADObjectId key = new ADObjectId(serverInfo.WritableNC);
			if (this.userServerPerDomain.ContainsKey(key))
			{
				return;
			}
			base.AddPreferredDC(serverInfo);
		}

		internal override bool IsUpdatableByADSession
		{
			get
			{
				return true;
			}
		}

		internal void AddOrReplaceUserPreferredDC(Fqdn fqdn, out ADObjectId domain, out Fqdn replacedDc)
		{
			ADServerInfo serverInfoFromFqdn = ADServerSettings.GetServerInfoFromFqdn(fqdn, ConnectionType.DomainController);
			domain = new ADObjectId(serverInfoFromFqdn.WritableNC);
			replacedDc = null;
			Fqdn fqdn2;
			if (this.userServerPerDomain.TryGetValue(domain, out fqdn2) && string.Equals(fqdn2, serverInfoFromFqdn.Fqdn))
			{
				return;
			}
			lock (this.dictLock)
			{
				Dictionary<ADObjectId, Fqdn> dictionary = new Dictionary<ADObjectId, Fqdn>(this.userServerPerDomain);
				this.userServerPerDomain.TryGetValue(domain, out replacedDc);
				dictionary[domain] = fqdn;
				this.userServerPerDomain = dictionary;
				this.cachedUserPreferredDCList = new MultiValuedProperty<Fqdn>(this.userServerPerDomain.Values);
			}
		}

		internal void ClearAllUserPreferredDCs()
		{
			if (this.UserPreferredDomainControllers.Count == 0)
			{
				return;
			}
			lock (this.dictLock)
			{
				if (this.UserPreferredDomainControllers.Count > 0)
				{
					this.userServerPerDomain = new Dictionary<ADObjectId, Fqdn>();
					this.cachedUserPreferredDCList = new MultiValuedProperty<Fqdn>(this.userServerPerDomain.Values);
				}
			}
		}

		internal Dictionary<ADObjectId, Fqdn> UserServerPerDomain
		{
			get
			{
				return this.userServerPerDomain;
			}
		}

		public override object Clone()
		{
			RunspaceServerSettings runspaceServerSettings = new RunspaceServerSettings();
			this.CopyTo(runspaceServerSettings);
			return runspaceServerSettings;
		}

		public bool Equals(RunspaceServerSettings other)
		{
			return other != null && (((this.userPreferredGlobalCatalog != null) ? this.userPreferredGlobalCatalog.Equals(other.userPreferredGlobalCatalog) : (other.userPreferredGlobalCatalog == null)) && ((this.userConfigurationDomainController != null) ? this.userConfigurationDomainController.Equals(other.userConfigurationDomainController) : (other.userConfigurationDomainController == null)) && base.Equals(other)) && this.userServerPerDomain.Equals(other.userServerPerDomain);
		}

		protected override void CopyTo(object targetObj)
		{
			ArgumentValidator.ThrowIfNull("targetObj", targetObj);
			ArgumentValidator.ThrowIfTypeInvalid<RunspaceServerSettings>("targetObj", targetObj);
			RunspaceServerSettings runspaceServerSettings = (RunspaceServerSettings)targetObj;
			base.CopyTo(targetObj);
			runspaceServerSettings.userPreferredGlobalCatalog = this.userPreferredGlobalCatalog;
			runspaceServerSettings.userConfigurationDomainController = this.userConfigurationDomainController;
			runspaceServerSettings.userServerPerDomain = new Dictionary<ADObjectId, Fqdn>(this.userServerPerDomain);
			runspaceServerSettings.cachedUserPreferredDCList = new MultiValuedProperty<Fqdn>(runspaceServerSettings.userServerPerDomain.Values);
			runspaceServerSettings.partitionFqdn = this.partitionFqdn;
		}

		private string partitionFqdn;

		private Fqdn userPreferredGlobalCatalog;

		private Fqdn userConfigurationDomainController;

		private Dictionary<ADObjectId, Fqdn> userServerPerDomain = new Dictionary<ADObjectId, Fqdn>();

		private object dictLock = new object();

		private MultiValuedProperty<Fqdn> cachedUserPreferredDCList = new MultiValuedProperty<Fqdn>();
	}
}
