using System;
using System.Collections.Generic;
using System.Security.Principal;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Win32;

namespace Microsoft.Exchange.Configuration.Authorization
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ExchangeExpiringRunspaceConfiguration : ExchangeRunspaceConfiguration, IExpiringRunspaceConfiguration
	{
		public ExchangeExpiringRunspaceConfiguration(IIdentity identity, ExchangeRunspaceConfigurationSettings settings) : this(identity, settings, false)
		{
		}

		public ExchangeExpiringRunspaceConfiguration(IIdentity identity, ExchangeRunspaceConfigurationSettings settings, bool isPowerShellWebService) : base(identity, null, settings, null, null, null, false, isPowerShellWebService, false, SnapinSet.Default)
		{
			this.SetMaxAgeLimit(ExpirationLimit.RunspaceRefresh);
			this.SetMaxAgeLimit(ExpirationLimit.ExternalAccountRunspaceTermination);
		}

		private bool IsMaximumAgeLimitExceeded(ExpirationLimit limit)
		{
			return DateTime.UtcNow >= this.maxAgeLimits[(int)limit];
		}

		public static TimeSpan GetMaximumAgeLimit(ExpirationLimit limit)
		{
			ExchangeExpiringRunspaceConfiguration.RefreshLimitsFromRegistryIfNeeded();
			return ExchangeExpiringRunspaceConfiguration.ExpiryPeriods[(int)limit];
		}

		private static void RefreshLimitsFromRegistryIfNeeded()
		{
			if (DateTime.UtcNow >= ExchangeExpiringRunspaceConfiguration.ExpiryRefreshTime)
			{
				lock (ExchangeExpiringRunspaceConfiguration.syncRoot)
				{
					if (DateTime.UtcNow >= ExchangeExpiringRunspaceConfiguration.ExpiryRefreshTime)
					{
						ExchangeExpiringRunspaceConfiguration.ExpiryPeriods[0] = ExchangeExpiringRunspaceConfiguration.GetMaximumAgeLimitFromRegistry(ExpirationLimit.RunspaceRefresh);
						ExchangeExpiringRunspaceConfiguration.ExpiryPeriods[1] = ExchangeExpiringRunspaceConfiguration.GetMaximumAgeLimitFromRegistry(ExpirationLimit.ExternalAccountRunspaceTermination);
						ExchangeExpiringRunspaceConfiguration.ExpiryRefreshTime = DateTime.UtcNow.AddMinutes(5.0);
					}
				}
			}
		}

		internal override ScopeSet CalculateScopeSetForExchangeCmdlet(string exchangeCmdletName, IList<string> parameters, OrganizationId organizationId, Task.ErrorLoggerDelegate writeError)
		{
			if (this.IsMaximumAgeLimitExceeded(ExpirationLimit.RunspaceRefresh))
			{
				base.LoadRoleCmdletInfo();
				this.SetMaxAgeLimit(ExpirationLimit.RunspaceRefresh);
				base.RefreshProvisioningBroker();
			}
			return base.CalculateScopeSetForExchangeCmdlet(exchangeCmdletName, parameters, organizationId, writeError);
		}

		public bool ShouldCloseDueToExpiration()
		{
			return (base.HasLinkedRoleGroups || base.DelegatedPrincipal != null) && this.IsMaximumAgeLimitExceeded(ExpirationLimit.ExternalAccountRunspaceTermination);
		}

		private static TimeSpan GetMaximumAgeLimitFromRegistry(ExpirationLimit limit)
		{
			TimeSpan result;
			using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Services\\MSExchange RBAC", false))
			{
				int num;
				string name;
				switch (limit)
				{
				case ExpirationLimit.RunspaceRefresh:
					num = 60;
					name = "MaximumAge";
					break;
				case ExpirationLimit.ExternalAccountRunspaceTermination:
					num = 1440;
					name = "MaximumAgeExternalAccount";
					break;
				default:
					throw new ArgumentOutOfRangeException("limit");
				}
				if (registryKey != null)
				{
					object value = registryKey.GetValue(name);
					if (value != null && value is int)
					{
						int num2 = (int)value;
						if (num2 > 0)
						{
							num = num2;
						}
					}
				}
				result = TimeSpan.FromMinutes((double)num);
			}
			return result;
		}

		private void SetMaxAgeLimit(ExpirationLimit limit)
		{
			this.maxAgeLimits[(int)limit] = DateTime.UtcNow.Add(ExchangeExpiringRunspaceConfiguration.GetMaximumAgeLimit(limit));
		}

		private const string RbacConfigurationKey = "SYSTEM\\CurrentControlSet\\Services\\MSExchange RBAC";

		private const string MaximumAgeKey = "MaximumAge";

		private const string MaximumAgeExternalAccountKey = "MaximumAgeExternalAccount";

		private const int DefaultMaximumAge = 60;

		private const int DefaultMaximumAgeExternalAccount = 1440;

		private static TimeSpan[] ExpiryPeriods = new TimeSpan[2];

		private static DateTime ExpiryRefreshTime = DateTime.UtcNow;

		private static object syncRoot = new object();

		private DateTime[] maxAgeLimits = new DateTime[2];
	}
}
