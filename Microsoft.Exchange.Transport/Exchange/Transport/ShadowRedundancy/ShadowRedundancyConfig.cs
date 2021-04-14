using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Extensibility.Internal;

namespace Microsoft.Exchange.Transport.ShadowRedundancy
{
	internal sealed class ShadowRedundancyConfig : IShadowRedundancyConfigurationSource
	{
		public bool Enabled
		{
			get
			{
				return this.shadowRedundancyConfigData.Enabled;
			}
		}

		public ShadowRedundancyCompatibilityVersion CompatibilityVersion
		{
			get
			{
				return this.shadowRedundancyConfigData.CompatibilityVersion;
			}
		}

		public TimeSpan ShadowMessageAutoDiscardInterval
		{
			get
			{
				return this.shadowRedundancyConfigData.ShadowMessageAutoDiscardInterval;
			}
		}

		public TimeSpan DiscardEventExpireInterval
		{
			get
			{
				return this.shadowRedundancyConfigData.ShadowMessageAutoDiscardInterval;
			}
		}

		public TimeSpan QueueMaxIdleTimeInterval
		{
			get
			{
				return this.shadowRedundancyConfigData.QueueMaxIdleTimeInterval;
			}
		}

		public TimeSpan ShadowServerInfoMaxIdleTimeInterval
		{
			get
			{
				return this.shadowRedundancyConfigData.ShadowServerInfoMaxIdleTimeInterval;
			}
		}

		public TimeSpan ShadowQueueCheckExpiryInterval
		{
			get
			{
				return this.shadowRedundancyConfigData.ShadowQueueCheckExpiryInterval;
			}
		}

		public TimeSpan DelayedAckCheckExpiryInterval
		{
			get
			{
				return this.shadowRedundancyConfigData.DelayedAckCheckExpiryInterval;
			}
		}

		public bool DelayedAckSkippingEnabled
		{
			get
			{
				return this.shadowRedundancyConfigData.DelayedAckSkippingEnabled;
			}
		}

		public int DelayedAckSkippingQueueLength
		{
			get
			{
				return this.shadowRedundancyConfigData.DelayedAckSkippingQueueLength;
			}
		}

		public TimeSpan DiscardEventsCheckExpiryInterval
		{
			get
			{
				return this.shadowRedundancyConfigData.DiscardEventsCheckExpiryInterval;
			}
		}

		public TimeSpan StringPoolCleanupInterval
		{
			get
			{
				return this.shadowRedundancyConfigData.StringPoolCleanupInterval;
			}
		}

		public int PrimaryServerInfoHardCleanupThreshold
		{
			get
			{
				return this.shadowRedundancyConfigData.PrimaryServerInfoHardCleanupThreshold;
			}
		}

		public TimeSpan PrimaryServerInfoCleanupInterval
		{
			get
			{
				return this.shadowRedundancyConfigData.PrimaryServerInfoCleanupInterval;
			}
		}

		public TimeSpan HeartbeatFrequency
		{
			get
			{
				return this.shadowRedundancyConfigData.HeartbeatFrequency;
			}
		}

		public int HeartbeatRetryCount
		{
			get
			{
				return this.shadowRedundancyConfigData.HeartbeatRetryCount;
			}
		}

		public int MaxRemoteShadowAttempts
		{
			get
			{
				return this.shadowRedundancyConfigData.MaxRemoteShadowAttempts;
			}
		}

		public int MaxLocalShadowAttempts
		{
			get
			{
				return this.shadowRedundancyConfigData.MaxLocalShadowAttempts;
			}
		}

		public ShadowMessagePreference ShadowMessagePreference
		{
			get
			{
				return this.shadowRedundancyConfigData.ShadowMessagePreference;
			}
		}

		public bool RejectMessageOnShadowFailure
		{
			get
			{
				return this.shadowRedundancyConfigData.RejectMessageOnShadowFailure;
			}
		}

		public int MaxDiscardIdsPerSmtpCommand
		{
			get
			{
				return this.shadowRedundancyConfigData.MaxDiscardIdsPerSmtpCommand;
			}
		}

		public TimeSpan MaxPendingHeartbeatInterval
		{
			get
			{
				return this.shadowRedundancyConfigData.MaxPendingHeartbeatInterval;
			}
		}

		public void Load()
		{
			this.shadowRedundancyConfigData = ShadowRedundancyConfig.ReadTransportConfig(Components.Configuration.TransportSettings.TransportSettings);
			this.RegisterConfigurationChangeHandlers();
		}

		public void Unload()
		{
			Components.ConfigChanged -= this.ChangeNotificationConfigUpdate;
			Components.Configuration.TransportSettingsChanged -= this.TransportSettingsConfigUpdate;
		}

		public void SetShadowRedundancyConfigChangeNotification(ShadowRedundancyConfigChange shadowRedundancyConfigChange)
		{
			if (shadowRedundancyConfigChange == null)
			{
				throw new ArgumentNullException("shadowRedundancyConfigChange");
			}
			this.shadowRedundancyConfigChange = shadowRedundancyConfigChange;
		}

		private static ShadowRedundancyConfig.ShadowRedundancyConfigData ReadTransportConfig(TransportConfigContainer transportSettings)
		{
			return new ShadowRedundancyConfig.ShadowRedundancyConfigData
			{
				Enabled = (transportSettings.ShadowRedundancyEnabled && !Components.TransportAppConfig.ShadowRedundancy.ShadowRedundancyLocalDisabled),
				HeartbeatFrequency = transportSettings.ShadowHeartbeatFrequency,
				ShadowMessageAutoDiscardInterval = transportSettings.ShadowMessageAutoDiscardInterval,
				MaxRemoteShadowAttempts = transportSettings.MaxRetriesForRemoteSiteShadow,
				MaxLocalShadowAttempts = transportSettings.MaxRetriesForLocalSiteShadow,
				ShadowMessagePreference = transportSettings.ShadowMessagePreferenceSetting,
				RejectMessageOnShadowFailure = transportSettings.RejectMessageOnShadowFailure,
				HeartbeatRetryCount = (int)Math.Ceiling((double)transportSettings.ShadowResubmitTimeSpan.Ticks / (double)transportSettings.ShadowHeartbeatFrequency.Ticks)
			};
		}

		private void RegisterConfigurationChangeHandlers()
		{
			ADObjectId orgAdObjectId = null;
			IConfigurationSession adSession;
			ADOperationResult adoperationResult = ADNotificationAdapter.TryRunADOperation(delegate()
			{
				adSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 311, "RegisterConfigurationChangeHandlers", "f:\\15.00.1497\\sources\\dev\\Transport\\src\\ShadowRedundancy\\ShadowRedundancyConfig.cs");
				orgAdObjectId = adSession.GetOrgContainerId();
			});
			if (!adoperationResult.Succeeded)
			{
				throw new TransportComponentLoadFailedException(Strings.ReadOrgContainerFailed, adoperationResult.Exception);
			}
			Components.ConfigChanged += this.ChangeNotificationConfigUpdate;
			Components.Configuration.TransportSettingsChanged += this.TransportSettingsConfigUpdate;
		}

		private void ChangeNotificationConfigUpdate(object source, EventArgs args)
		{
			this.TransportSettingsConfigUpdate(Components.Configuration.TransportSettings);
		}

		private void TransportSettingsConfigUpdate(TransportSettingsConfiguration transportSettingsConfig)
		{
			ShadowRedundancyConfig.ShadowRedundancyConfigData shadowRedundancyConfigData = ShadowRedundancyConfig.ReadTransportConfig(transportSettingsConfig.TransportSettings);
			if (shadowRedundancyConfigData != null && !this.shadowRedundancyConfigData.Equals(shadowRedundancyConfigData))
			{
				ShadowRedundancyConfig.ShadowRedundancyConfigData oldConfiguration = this.shadowRedundancyConfigData;
				this.shadowRedundancyConfigData = shadowRedundancyConfigData;
				if (this.shadowRedundancyConfigChange != null)
				{
					this.shadowRedundancyConfigChange(oldConfiguration);
				}
			}
		}

		private ShadowRedundancyConfig.ShadowRedundancyConfigData shadowRedundancyConfigData = new ShadowRedundancyConfig.ShadowRedundancyConfigData();

		private ShadowRedundancyConfigChange shadowRedundancyConfigChange;

		private sealed class ShadowRedundancyConfigData : IShadowRedundancyConfigurationSource
		{
			public bool Enabled
			{
				get
				{
					return this.shadowRedundancyEnabled;
				}
				internal set
				{
					this.shadowRedundancyEnabled = value;
				}
			}

			public ShadowRedundancyCompatibilityVersion CompatibilityVersion
			{
				get
				{
					if (Components.Configuration.ProcessTransportRole != ProcessTransportRole.Edge)
					{
						return Components.TransportAppConfig.ShadowRedundancy.CompatibilityVersion;
					}
					return ShadowRedundancyCompatibilityVersion.E14;
				}
			}

			public TimeSpan ShadowMessageAutoDiscardInterval
			{
				get
				{
					return this.shadowMessageAutoDiscardInterval;
				}
				internal set
				{
					this.shadowMessageAutoDiscardInterval = value;
				}
			}

			public TimeSpan DiscardEventExpireInterval
			{
				get
				{
					return this.ShadowMessageAutoDiscardInterval;
				}
			}

			public TimeSpan QueueMaxIdleTimeInterval
			{
				get
				{
					return Components.Configuration.LocalServer.TransportServer.QueueMaxIdleTime;
				}
			}

			public TimeSpan ShadowServerInfoMaxIdleTimeInterval
			{
				get
				{
					return this.QueueMaxIdleTimeInterval;
				}
			}

			public TimeSpan ShadowQueueCheckExpiryInterval
			{
				get
				{
					return ShadowRedundancyConfig.ShadowRedundancyConfigData.shadowQueueCheckExpiryInterval;
				}
			}

			public TimeSpan DelayedAckCheckExpiryInterval
			{
				get
				{
					return ShadowRedundancyConfig.ShadowRedundancyConfigData.delayedAckCheckExpiryInterval;
				}
			}

			public bool DelayedAckSkippingEnabled
			{
				get
				{
					return Components.TransportAppConfig.ShadowRedundancy.DelayedAckSkippingEnabled;
				}
			}

			public int DelayedAckSkippingQueueLength
			{
				get
				{
					return Components.TransportAppConfig.ShadowRedundancy.DelayedAckSkippingQueueLength;
				}
			}

			public TimeSpan DiscardEventsCheckExpiryInterval
			{
				get
				{
					return ShadowRedundancyConfig.ShadowRedundancyConfigData.discardEventsCheckExpiryInterval;
				}
			}

			public TimeSpan StringPoolCleanupInterval
			{
				get
				{
					return ShadowRedundancyConfig.ShadowRedundancyConfigData.stringPoolCleanupInterval;
				}
			}

			public TimeSpan PrimaryServerInfoCleanupInterval
			{
				get
				{
					return ShadowRedundancyConfig.ShadowRedundancyConfigData.primaryServerInfoCleanupInterval;
				}
			}

			public int PrimaryServerInfoHardCleanupThreshold
			{
				get
				{
					return 10000;
				}
			}

			public TimeSpan HeartbeatFrequency
			{
				get
				{
					return this.heartbeatFrequency;
				}
				internal set
				{
					this.heartbeatFrequency = value;
				}
			}

			public int HeartbeatRetryCount
			{
				get
				{
					return this.heartbeatRetryCount;
				}
				internal set
				{
					this.heartbeatRetryCount = value;
				}
			}

			public int MaxRemoteShadowAttempts
			{
				get
				{
					return this.maxRemoteAttempts;
				}
				internal set
				{
					this.maxRemoteAttempts = value;
				}
			}

			public int MaxLocalShadowAttempts
			{
				get
				{
					return this.maxLocalAttempts;
				}
				internal set
				{
					this.maxLocalAttempts = value;
				}
			}

			public ShadowMessagePreference ShadowMessagePreference
			{
				get
				{
					return this.shadowMessagePreference;
				}
				internal set
				{
					this.shadowMessagePreference = value;
				}
			}

			public bool RejectMessageOnShadowFailure
			{
				get
				{
					return this.rejectMessageOnShadowFailure;
				}
				internal set
				{
					this.rejectMessageOnShadowFailure = value;
				}
			}

			public int MaxDiscardIdsPerSmtpCommand
			{
				get
				{
					return Components.TransportAppConfig.ShadowRedundancy.MaxDiscardIdsPerSmtpCommand;
				}
			}

			public TimeSpan MaxPendingHeartbeatInterval
			{
				get
				{
					return Components.TransportAppConfig.ShadowRedundancy.MaxPendingHeartbeatInterval;
				}
			}

			public void Load()
			{
				throw new NotSupportedException("ShadowRedundancyConfigData.Load() should never be called.");
			}

			public void Unload()
			{
				throw new NotSupportedException("ShadowRedundancyConfigData.Unload() should never be called.");
			}

			public void SetShadowRedundancyConfigChangeNotification(ShadowRedundancyConfigChange shadowRedundancyConfigChange)
			{
				throw new NotSupportedException("ShadowRedundancyConfigData.SetShadowRedundancyConfigChangeNotification() should never be called.");
			}

			public bool Equals(ShadowRedundancyConfig.ShadowRedundancyConfigData shadowRedundancyConfigData)
			{
				return shadowRedundancyConfigData != null && this.heartbeatRetryCount == shadowRedundancyConfigData.heartbeatRetryCount && this.heartbeatFrequency == shadowRedundancyConfigData.heartbeatFrequency && this.shadowMessageAutoDiscardInterval == shadowRedundancyConfigData.shadowMessageAutoDiscardInterval && this.maxRemoteAttempts == shadowRedundancyConfigData.maxRemoteAttempts && this.maxLocalAttempts == shadowRedundancyConfigData.maxLocalAttempts && this.rejectMessageOnShadowFailure == shadowRedundancyConfigData.rejectMessageOnShadowFailure && this.shadowRedundancyEnabled == shadowRedundancyConfigData.shadowRedundancyEnabled;
			}

			private const int DefaultPrimaryServerInfoHardCleanupThreshold = 10000;

			private static readonly TimeSpan shadowQueueCheckExpiryInterval = TimeSpan.FromMinutes(1.0);

			private static readonly TimeSpan delayedAckCheckExpiryInterval = TimeSpan.FromSeconds(5.0);

			private static readonly TimeSpan discardEventsCheckExpiryInterval = TimeSpan.FromMinutes(5.0);

			private static readonly TimeSpan stringPoolCleanupInterval = TimeSpan.FromHours(3.0);

			private static readonly TimeSpan primaryServerInfoCleanupInterval = TimeSpan.FromMinutes(5.0);

			private bool shadowRedundancyEnabled;

			private TimeSpan shadowMessageAutoDiscardInterval;

			private TimeSpan heartbeatFrequency;

			private int heartbeatRetryCount;

			private int maxRemoteAttempts;

			private int maxLocalAttempts;

			private ShadowMessagePreference shadowMessagePreference;

			private bool rejectMessageOnShadowFailure;
		}
	}
}
