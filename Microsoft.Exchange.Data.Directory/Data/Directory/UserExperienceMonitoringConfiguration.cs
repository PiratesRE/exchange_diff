using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.Data.Directory;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Data.Directory
{
	internal class UserExperienceMonitoringConfiguration
	{
		public UserExperienceMonitoringConfiguration()
		{
			this.registryWatcher = new RegistryWatcher(UserExperienceMonitoringConfiguration.RegistryPath, false);
			this.LoadConfiguration();
		}

		public bool Enabled
		{
			get
			{
				bool config = TenantRelocationConfigImpl.GetConfig<bool>("IsUserExperienceTestEnabled");
				ExTraceGlobals.TenantRelocationTracer.TraceDebug<bool>((long)this.GetHashCode(), "Global Config: UserExperienceMonitoringConfiguration::Enabled: {0}.", config);
				return config;
			}
		}

		public int MonitorAccountExpiredDays
		{
			get
			{
				int config = TenantRelocationConfigImpl.GetConfig<int>("UXMonitorAccountExpiredDays");
				ExTraceGlobals.TenantRelocationTracer.TraceDebug<int>((long)this.GetHashCode(), "Global Config: UserExperienceMonitoringConfiguration::MonitorAccountExpiredDays: {0}.", config);
				return config;
			}
		}

		public int ReplicationTimeoutForRemoveMonitoringAccount
		{
			get
			{
				int config = TenantRelocationConfigImpl.GetConfig<int>("RemoveUXMonitorAccountWaitReplicationMinutes");
				ExTraceGlobals.TenantRelocationTracer.TraceDebug<int>((long)this.GetHashCode(), "Global Config: UserExperienceMonitoringConfiguration::ReplicationTimeoutForRemoveMonitoringAccount: {0}.", config);
				return config;
			}
		}

		public int WaitProbeFailureReadableTimeout
		{
			get
			{
				int config = TenantRelocationConfigImpl.GetConfig<int>("WaitUXFailureResultSeconds");
				ExTraceGlobals.TenantRelocationTracer.TraceDebug<int>((long)this.GetHashCode(), "Global Config: UserExperienceMonitoringConfiguration::WaitProbeFailureReadableTimeout: {0}.", config);
				return config;
			}
		}

		public bool IsLockdownOnly
		{
			get
			{
				this.DetechChangeAndReloadConfiguration();
				return this.isLockdownOnly;
			}
		}

		public IList<UserExperienceMonitoringSupportedComponent> DisabledComponent
		{
			get
			{
				return UserExperienceMonitoringConfiguration.GetDisabledComponent(TenantRelocationConfigImpl.GetConfig<string>("DisabledUXProbes"));
			}
		}

		public int SupportedComponentCount
		{
			get
			{
				return Enum.GetValues(typeof(UserExperienceMonitoringSupportedComponent)).Length - this.DisabledComponent.Count;
			}
		}

		public static IList<UserExperienceMonitoringSupportedComponent> GetTenantUserExperienceTargetComponents(string tenant, string servicePlan)
		{
			IList<UserExperienceMonitoringSupportedComponent> supportedComponentInTenant = UserExperienceMonitoringConfiguration.GetSupportedComponentInTenant(tenant, servicePlan);
			if (supportedComponentInTenant == null)
			{
				return null;
			}
			if (UserExperienceMonitoringConfiguration.Config.Value.DisabledComponent == null)
			{
				return supportedComponentInTenant;
			}
			foreach (UserExperienceMonitoringSupportedComponent item in UserExperienceMonitoringConfiguration.Config.Value.DisabledComponent)
			{
				if (supportedComponentInTenant.Contains(item))
				{
					supportedComponentInTenant.Remove(item);
				}
			}
			return supportedComponentInTenant;
		}

		private static IList<UserExperienceMonitoringSupportedComponent> GetSupportedComponentInTenant(string tenant, string servicePlan)
		{
			List<UserExperienceMonitoringSupportedComponent> list = new List<UserExperienceMonitoringSupportedComponent>();
			list.Add(UserExperienceMonitoringSupportedComponent.Owa);
			list.Add(UserExperienceMonitoringSupportedComponent.Outlook);
			list.Add(UserExperienceMonitoringSupportedComponent.ActiveSync);
			list.Add(UserExperienceMonitoringSupportedComponent.AutoDiscover);
			list.Add(UserExperienceMonitoringSupportedComponent.Transport);
			if (servicePlan.StartsWith("BPOS_Basic_E"))
			{
				ExTraceGlobals.TenantRelocationTracer.TraceDebug(0L, "Skip components {0}, {1} and {2} for BPOS_Basic tenants {3}.", new object[]
				{
					UserExperienceMonitoringSupportedComponent.Outlook,
					UserExperienceMonitoringSupportedComponent.ActiveSync,
					UserExperienceMonitoringSupportedComponent.AutoDiscover,
					tenant
				});
				list.Remove(UserExperienceMonitoringSupportedComponent.Outlook);
				list.Remove(UserExperienceMonitoringSupportedComponent.ActiveSync);
				list.Remove(UserExperienceMonitoringSupportedComponent.AutoDiscover);
			}
			return list;
		}

		private void DetechChangeAndReloadConfiguration()
		{
			if (this.registryWatcher.IsChanged())
			{
				this.LoadConfiguration();
			}
		}

		private void LoadConfiguration()
		{
			this.isLockdownOnly = (Globals.GetIntValueFromRegistry(UserExperienceMonitoringConfiguration.RegistryPath, UserExperienceMonitoringConfiguration.UserExperienceMonitoringLockdownOnlyRegistryName, 0, 0) > 0);
		}

		private static IList<UserExperienceMonitoringSupportedComponent> GetDisabledComponent(string globalConfigValue)
		{
			IList<UserExperienceMonitoringSupportedComponent> list = new List<UserExperienceMonitoringSupportedComponent>();
			if (globalConfigValue != null)
			{
				ExTraceGlobals.TenantRelocationTracer.TraceDebug<string>(0L, "Global Config: UserExperienceMonitoringConfiguration::GetDisabledComponent: {0}.", globalConfigValue);
				string[] array = globalConfigValue.Split(new char[]
				{
					';'
				});
				foreach (string text in array)
				{
					UserExperienceMonitoringSupportedComponent item;
					if (Enum.TryParse<UserExperienceMonitoringSupportedComponent>(text, out item))
					{
						list.Add(item);
					}
					else
					{
						ExTraceGlobals.TenantRelocationTracer.TraceWarning((long)default(Guid).GetHashCode(), string.Format("Failed to parse the configured disabled component {0}, which will be ignored.", text));
					}
				}
			}
			return list;
		}

		private static readonly string RegistryPath = "SOFTWARE\\Microsoft\\ExchangeLabs";

		private static readonly string UserExperienceMonitoringLockdownOnlyRegistryName = "TenantRelocationUserExperienceLockdownOnly";

		private bool isLockdownOnly;

		private RegistryWatcher registryWatcher;

		public static readonly Lazy<UserExperienceMonitoringConfiguration> Config = new Lazy<UserExperienceMonitoringConfiguration>(() => new UserExperienceMonitoringConfiguration());
	}
}
