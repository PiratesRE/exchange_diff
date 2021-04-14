using System;
using System.Diagnostics;
using System.Threading;
using Microsoft.Exchange.Diagnostics.Components.Security;
using Microsoft.Exchange.Threading;

namespace Microsoft.Exchange.Security.OAuth
{
	internal sealed class ConfigProvider
	{
		public DateTime LastRefreshDateTime
		{
			get
			{
				return this.lastRefreshDateTime;
			}
		}

		private ConfigProvider()
		{
		}

		public static ConfigProvider Instance
		{
			get
			{
				if (ConfigProvider.instance == null)
				{
					lock (ConfigProvider.staticLockObj)
					{
						if (ConfigProvider.instance == null)
						{
							ConfigProvider.instance = new ConfigProvider();
						}
					}
				}
				return ConfigProvider.instance;
			}
		}

		public bool AutoRefresh
		{
			get
			{
				return this.autoRefreshEnabled;
			}
			set
			{
				if (!value)
				{
					this.DisableAutoRefresh();
				}
				this.autoRefreshEnabled = value;
			}
		}

		public bool LoadTrustedIssuers
		{
			get
			{
				return this.loadTrustedIssuers;
			}
			set
			{
				this.loadTrustedIssuers = value;
			}
		}

		public LocalConfiguration Configuration
		{
			get
			{
				if (this.autoRefreshEnabled)
				{
					this.EnableAutoRefresh();
				}
				if (this.refreshTimer != null && this.underlyingConfiguration != null)
				{
					ExTraceGlobals.OAuthTracer.TraceDebug(0L, "[ConfigProvider::get_Configuration] returning auto-refreshed instance");
				}
				else
				{
					this.ManualLoadIfNecessary();
					ExTraceGlobals.OAuthTracer.TraceDebug(0L, "[ConfigProvider::get_Configuration] returning manual-loaded instance");
				}
				return this.underlyingConfiguration;
			}
		}

		private void EnableAutoRefresh()
		{
			if (this.refreshTimer == null)
			{
				lock (this.instanceLockObj)
				{
					if (this.refreshTimer == null)
					{
						ExTraceGlobals.OAuthTracer.TraceDebug(0L, "[ConfigProvider::EnableAutoRefresh] entering");
						TimeSpan startDelay = this.GetStartDelay();
						ExTraceGlobals.OAuthTracer.TraceDebug<DateTime>(0L, "[ConfigProvider::EnableAutoRefresh] refresh timer set, start at ~{0}", DateTime.UtcNow.Add(startDelay));
						this.refreshTimer = new GuardedTimer(new TimerCallback(this.AutoRefreshCallback), null, startDelay, ConfigProvider.refreshInterval);
					}
				}
			}
		}

		private TimeSpan GetStartDelay()
		{
			TimeSpan result;
			using (Process currentProcess = Process.GetCurrentProcess())
			{
				int id = currentProcess.Id;
				Random random = new Random(id);
				result = TimeSpan.FromSeconds((double)random.Next(0, (int)ConfigProvider.refreshInterval.TotalSeconds));
			}
			return result;
		}

		private void DisableAutoRefresh()
		{
			if (this.refreshTimer != null)
			{
				lock (this.instanceLockObj)
				{
					if (this.refreshTimer != null)
					{
						ExTraceGlobals.OAuthTracer.TraceDebug(0L, "[ConfigProvider::DisableAutoRefresh] clearing the refresh timer.");
						this.refreshTimer.Dispose(true);
						this.refreshTimer = null;
					}
				}
			}
		}

		private void AutoRefreshCallback(object state)
		{
			ExTraceGlobals.OAuthTracer.TraceDebug(0L, "[ConfigProvider::AutoRefresh] starting refreshing");
			Exception exception = null;
			LocalConfiguration localConfiguration = LocalConfiguration.InternalLoadHelper(null, this.loadTrustedIssuers, out exception);
			if (localConfiguration != null)
			{
				ExTraceGlobals.OAuthTracer.TraceDebug(0L, "[ConfigProvider::AutoRefresh] configuration was updated successfully");
				this.underlyingConfiguration = localConfiguration;
				this.lastRefreshDateTime = DateTime.UtcNow;
			}
			else
			{
				ExTraceGlobals.OAuthTracer.TraceWarning(0L, "[ConfigProvider::AutoRefresh] configuration was not updated.");
				this.LogEventIfNecessary(exception);
			}
			ExTraceGlobals.OAuthTracer.TraceDebug<DateTime>(0L, "[ConfigProvider::AutoRefresh] finishing refresh, next refresh at {0}", DateTime.UtcNow.Add(ConfigProvider.refreshInterval));
		}

		private void ManualLoadIfNecessary()
		{
			if (DateTime.UtcNow - this.lastRefreshDateTime > ConfigProvider.refreshInterval)
			{
				lock (this.instanceLockObj)
				{
					if (DateTime.UtcNow - this.lastRefreshDateTime > ConfigProvider.refreshInterval)
					{
						ExTraceGlobals.OAuthTracer.TraceDebug(0L, "[ConfigProvider::ManualLoadIfNecessary] loading the configuration");
						Exception exception = null;
						LocalConfiguration localConfiguration = LocalConfiguration.InternalLoadHelper(null, this.loadTrustedIssuers, out exception);
						if (localConfiguration != null)
						{
							ExTraceGlobals.OAuthTracer.TraceDebug(0L, "[ConfigProvider::ManualLoadIfNecessary] configuration was updated successfully");
							this.underlyingConfiguration = localConfiguration;
							this.lastRefreshDateTime = DateTime.UtcNow;
						}
						else
						{
							this.LogEventIfNecessary(exception);
						}
					}
				}
			}
		}

		private void LogEventIfNecessary(Exception exception)
		{
			ExTraceGlobals.OAuthTracer.TraceDebug<DateTime>((long)this.GetHashCode(), "[ConfigProvider::LogEventIfNecessary] unable to load the configuration, last successfuly load was at {0}", this.lastRefreshDateTime);
			if (DateTime.UtcNow - this.lastRefreshDateTime > ConfigProvider.alertInterval)
			{
				OAuthCommon.EventLogger.LogEvent(SecurityEventLogConstants.Tuple_OAuthFailToLoadLocalConfiguration, string.Empty, new object[]
				{
					this.lastRefreshDateTime,
					exception
				});
			}
		}

		public void EnforceReload()
		{
			this.lastRefreshDateTime = DateTime.MinValue;
		}

		public void ManuallyReload()
		{
			this.EnforceReload();
			this.ManualLoadIfNecessary();
		}

		private static readonly object staticLockObj = new object();

		internal static readonly TimeSpan refreshInterval = TimeSpan.FromMinutes(30.0);

		private static readonly TimeSpan alertInterval = TimeSpan.FromMinutes(ConfigProvider.refreshInterval.TotalMinutes * 3.0);

		private static ConfigProvider instance;

		private readonly object instanceLockObj = new object();

		private GuardedTimer refreshTimer;

		private DateTime lastRefreshDateTime = DateTime.MinValue;

		private LocalConfiguration underlyingConfiguration;

		private bool autoRefreshEnabled;

		private bool loadTrustedIssuers;
	}
}
