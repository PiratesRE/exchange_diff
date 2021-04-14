using System;
using System.Security;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Directory;
using Microsoft.Win32;

namespace Microsoft.Exchange.Data.Directory.ResourceHealth
{
	internal abstract class ResourceHealthMonitorConfiguration<T> where T : ResourceHealthMonitorConfigurationSetting, new()
	{
		protected ResourceHealthMonitorConfiguration()
		{
			this.ConfigSettings = Activator.CreateInstance<T>();
			this.refreshInterval = this.ConfigSettings.DefaultRefreshInterval;
			using (RegistryKey registryKey = ResourceHealthMonitorConfiguration<T>.OpenConfigurationKey())
			{
				if (registryKey != null)
				{
					this.enabled = ((int)registryKey.GetValue(this.ConfigSettings.DisabledRegistryValueName, 0) == 0);
					ResourceHealthMonitorConfiguration<T>.Tracer.TraceDebug<bool>((long)this.GetHashCode(), "[ResourceHealthMonitorConfiguration::ctor] Enabled = '{0}'.", this.enabled);
					this.refreshInterval = ResourceHealthMonitorConfiguration<T>.ReadTimeSpan(registryKey, this.ConfigSettings.RefreshIntervalRegistryValueName, this.ConfigSettings.DefaultRefreshInterval);
					ResourceHealthMonitorConfiguration<T>.Tracer.TraceDebug<TimeSpan>((long)this.GetHashCode(), "[ResourceHealthMonitorConfiguration::ctor] RefreshInterval = '{0}'.", this.refreshInterval);
					this.overrideMetricValue = (registryKey.GetValue(this.ConfigSettings.OverrideMetricValueRegistryValueName) as int?);
					ResourceHealthMonitorConfiguration<T>.Tracer.TraceDebug<string>((long)this.GetHashCode(), "[ResourceHealthMonitorConfiguration::ctor] OverrideMetricValue = '{0}'.", (this.overrideMetricValue != null) ? this.overrideMetricValue.Value.ToString() : "<null>");
				}
			}
		}

		protected static RegistryKey OpenConfigurationKey()
		{
			try
			{
				return Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Services\\MSExchange ResourceHealth", false);
			}
			catch (SecurityException arg)
			{
				ResourceHealthMonitorConfiguration<T>.Tracer.TraceError<SecurityException>(0L, "[ResourceHealthMonitorConfiguration::OpenConfigurationKey] Security exception encountered while reading Resource Health Monitor configuration: {0}", arg);
			}
			catch (UnauthorizedAccessException arg2)
			{
				ResourceHealthMonitorConfiguration<T>.Tracer.TraceError<UnauthorizedAccessException>(0L, "[ResourceHealthMonitorConfiguration::OpenConfigurationKey] Security exception encountered while reading Resource Health Monitor configuration: {0}", arg2);
			}
			return null;
		}

		protected static TimeSpan ReadTimeSpan(RegistryKey key, string valueName, TimeSpan defaultValue)
		{
			TimeSpan result = defaultValue;
			object value = key.GetValue(valueName);
			if (value != null)
			{
				if (key.GetValueKind(valueName) != RegistryValueKind.DWord)
				{
					ResourceHealthMonitorConfiguration<T>.Tracer.TraceError<string>(0L, "[ResourceHealthMonitorConfiguration::ReadTimeSpan] {0} should be of type DWORD", valueName);
				}
				else
				{
					result = TimeSpan.FromSeconds((double)((int)value));
				}
			}
			return result;
		}

		private const int DefaultDisabledRegistryValue = 0;

		protected readonly T ConfigSettings;

		protected bool enabled = true;

		protected TimeSpan refreshInterval;

		protected int? overrideMetricValue = null;

		private static readonly Trace Tracer = ExTraceGlobals.ResourceHealthManagerTracer;
	}
}
