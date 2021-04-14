using System;
using System.Collections.Generic;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.PushNotifications;
using Microsoft.Exchange.PushNotifications.CrimsonEvents;
using Microsoft.Exchange.PushNotifications.Extensions;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	internal class PushNotificationPublisherConfiguration : IEquatable<PushNotificationPublisherConfiguration>
	{
		public PushNotificationPublisherConfiguration(bool ignoreErrors = true, List<IPushNotificationPublisherConfigurationProvider> providers = null)
		{
			this.ignoreErrors = ignoreErrors;
			this.rawSettings = new Dictionary<string, IPushNotificationRawSettings>();
			this.publisherSettings = new Dictionary<string, PushNotificationPublisherSettings>();
			this.unsuitablePublishers = new Dictionary<string, string>();
			this.settingsFactory = new PushNotificationPublisherSettingsFactory();
			this.hashCode = new Lazy<int>(() => Guid.NewGuid().ToString().GetHashCode());
			this.apnsPublisherSettings = new Lazy<List<ApnsPublisherSettings>>(() => this.FilterByPlatform<ApnsPublisherSettings>(PushNotificationPlatform.APNS));
			this.wnsPublisherSettings = new Lazy<List<WnsPublisherSettings>>(() => this.FilterByPlatform<WnsPublisherSettings>(PushNotificationPlatform.WNS));
			this.gcmPublisherSettings = new Lazy<List<GcmPublisherSettings>>(() => this.FilterByPlatform<GcmPublisherSettings>(PushNotificationPlatform.GCM));
			this.webAppPublisherSettings = new Lazy<List<WebAppPublisherSettings>>(() => this.FilterByPlatform<WebAppPublisherSettings>(PushNotificationPlatform.WebApp));
			this.azurePublisherSettings = new Lazy<List<AzurePublisherSettings>>(() => this.FilterByPlatform<AzurePublisherSettings>(PushNotificationPlatform.Azure));
			this.azureHubCreationPublisherSettings = new Lazy<List<AzureHubCreationPublisherSettings>>(() => this.FilterByPlatform<AzureHubCreationPublisherSettings>(PushNotificationPlatform.AzureHubCreation));
			this.azureChallengeRequestPublisherSettings = new Lazy<List<AzureChallengeRequestPublisherSettings>>(() => this.FilterByPlatform<AzureChallengeRequestPublisherSettings>(PushNotificationPlatform.AzureChallengeRequest));
			this.azureDeviceRegistrationPublisherSettings = new Lazy<List<AzureDeviceRegistrationPublisherSettings>>(() => this.FilterByPlatform<AzureDeviceRegistrationPublisherSettings>(PushNotificationPlatform.AzureDeviceRegistration));
			this.proxyPublisherSettings = new Lazy<ProxyPublisherSettings>(() => this.FilterByName<ProxyPublisherSettings>(PushNotificationCannedApp.OnPremProxy.Name));
			this.azureSendSettings = new Lazy<Dictionary<string, AzurePublisherSettings>>(delegate()
			{
				Dictionary<string, AzurePublisherSettings> dictionary = new Dictionary<string, AzurePublisherSettings>();
				foreach (AzurePublisherSettings azurePublisherSettings in this.AzurePublisherSettings)
				{
					dictionary.Add(azurePublisherSettings.AppId, azurePublisherSettings);
				}
				return dictionary;
			});
			List<IPushNotificationPublisherConfigurationProvider> list = providers;
			if (list == null)
			{
				list = new List<IPushNotificationPublisherConfigurationProvider>(2)
				{
					new RegistryConfigurationProvider(),
					new ADConfigurationProvider()
				};
			}
			foreach (IPushNotificationPublisherConfigurationProvider provider in list)
			{
				this.LoadPublisherSettings(provider);
			}
		}

		public bool HasEnabledPublisherSettings
		{
			get
			{
				return this.publisherSettings.Count > 0;
			}
		}

		public virtual IEnumerable<PushNotificationPublisherSettings> PublisherSettings
		{
			get
			{
				return this.publisherSettings.Values;
			}
		}

		public virtual IEnumerable<string> UnsuitablePublishers
		{
			get
			{
				return this.unsuitablePublishers.Values;
			}
		}

		public IEnumerable<ApnsPublisherSettings> ApnsPublisherSettings
		{
			get
			{
				return this.apnsPublisherSettings.Value;
			}
		}

		public IEnumerable<WnsPublisherSettings> WnsPublisherSettings
		{
			get
			{
				return this.wnsPublisherSettings.Value;
			}
		}

		public IEnumerable<GcmPublisherSettings> GcmPublisherSettings
		{
			get
			{
				return this.gcmPublisherSettings.Value;
			}
		}

		public IEnumerable<WebAppPublisherSettings> WebAppPublisherSettings
		{
			get
			{
				return this.webAppPublisherSettings.Value;
			}
		}

		public IEnumerable<AzurePublisherSettings> AzurePublisherSettings
		{
			get
			{
				return this.azurePublisherSettings.Value;
			}
		}

		public IEnumerable<AzureHubCreationPublisherSettings> AzureHubCreationPublisherSettings
		{
			get
			{
				return this.azureHubCreationPublisherSettings.Value;
			}
		}

		public IEnumerable<AzureChallengeRequestPublisherSettings> AzureChallengeRequestPublisherSettings
		{
			get
			{
				return this.azureChallengeRequestPublisherSettings.Value;
			}
		}

		public IEnumerable<AzureDeviceRegistrationPublisherSettings> AzureDeviceRegistrationPublisherSettings
		{
			get
			{
				return this.azureDeviceRegistrationPublisherSettings.Value;
			}
		}

		public ProxyPublisherSettings ProxyPublisherSettings
		{
			get
			{
				return this.proxyPublisherSettings.Value;
			}
		}

		public Dictionary<string, AzurePublisherSettings> AzureSendPublisherSettings
		{
			get
			{
				return this.azureSendSettings.Value;
			}
		}

		public PushNotificationPublisherManager CreatePublisherManager(Dictionary<PushNotificationPlatform, PushNotificationPublisherFactory> configFactories)
		{
			ArgumentValidator.ThrowIfNull("configFactories", configFactories);
			PushNotificationPublisherManager pushNotificationPublisherManager = new PushNotificationPublisherManager(null);
			foreach (IPushNotificationRawSettings pushNotificationRawSettings in this.rawSettings.Values)
			{
				PushNotificationPublisherFactory pushNotificationPublisherFactory;
				if (configFactories.TryGetValue(pushNotificationRawSettings.Platform, out pushNotificationPublisherFactory))
				{
					pushNotificationPublisherManager.RegisterPublisher(pushNotificationPublisherFactory.CreatePublisher(this.publisherSettings[pushNotificationRawSettings.Name]));
				}
				else
				{
					PushNotificationsCrimsonEvents.UnsupportedPlatform.Log<string, PushNotificationPlatform>(pushNotificationRawSettings.Name, pushNotificationRawSettings.Platform);
					ExTraceGlobals.PushNotificationServiceTracer.TraceWarning<string, PushNotificationPlatform>((long)this.GetHashCode(), "[PushNotificationPublisherManager:Create] App with AppId '{0}' defines a platform '{1}' that is currently not supported by this server.", pushNotificationRawSettings.Name, pushNotificationRawSettings.Platform);
				}
			}
			foreach (string publisherName in this.UnsuitablePublishers)
			{
				pushNotificationPublisherManager.RegisterUnsuitablePublisher(publisherName);
			}
			PushNotificationsCrimsonEvents.PushNotificationKnownPublishersCreated.Log<PushNotificationPublisherManager>(pushNotificationPublisherManager);
			return pushNotificationPublisherManager;
		}

		public MonitoringMailboxNotificationFactory CreateMonitoringNotificationFactory(Dictionary<PushNotificationPlatform, IMonitoringMailboxNotificationRecipientFactory> recipientFactories)
		{
			MonitoringMailboxNotificationFactory monitoringMailboxNotificationFactory = new MonitoringMailboxNotificationFactory();
			foreach (IPushNotificationRawSettings pushNotificationRawSettings in this.rawSettings.Values)
			{
				IMonitoringMailboxNotificationRecipientFactory recipientFactory;
				if (recipientFactories.TryGetValue(pushNotificationRawSettings.Platform, out recipientFactory))
				{
					monitoringMailboxNotificationFactory.RegisterAppToMonitor(pushNotificationRawSettings.Name, recipientFactory);
				}
			}
			return monitoringMailboxNotificationFactory;
		}

		public override int GetHashCode()
		{
			return this.hashCode.Value;
		}

		public override bool Equals(object obj)
		{
			PushNotificationPublisherConfiguration pushNotificationPublisherConfiguration = obj as PushNotificationPublisherConfiguration;
			return pushNotificationPublisherConfiguration != null && this.Equals(pushNotificationPublisherConfiguration);
		}

		public bool Equals(PushNotificationPublisherConfiguration that)
		{
			if (that == null)
			{
				return false;
			}
			if (object.ReferenceEquals(this, that))
			{
				return true;
			}
			if (this.publisherSettings.Count != that.publisherSettings.Count || this.unsuitablePublishers.Count != that.unsuitablePublishers.Count)
			{
				return false;
			}
			foreach (IPushNotificationRawSettings pushNotificationRawSettings in this.rawSettings.Values)
			{
				IPushNotificationRawSettings other;
				if (!that.rawSettings.TryGetValue(pushNotificationRawSettings.Name, out other) || !pushNotificationRawSettings.Equals(other))
				{
					return false;
				}
			}
			foreach (string key in this.unsuitablePublishers.Keys)
			{
				if (!that.unsuitablePublishers.ContainsKey(key))
				{
					return false;
				}
			}
			return true;
		}

		public string ToFullString()
		{
			return string.Format("{0}; Unsuitable:[0]", this.rawSettings.ToNullableString(null, (IPushNotificationRawSettings provider) => provider.ToFullString()), this.unsuitablePublishers.Keys.ToNullableString(null));
		}

		private List<T> FilterByPlatform<T>(PushNotificationPlatform platform) where T : PushNotificationPublisherSettings
		{
			List<T> list = new List<T>();
			foreach (IPushNotificationRawSettings pushNotificationRawSettings in this.rawSettings.Values)
			{
				if (pushNotificationRawSettings.Platform == platform)
				{
					list.Add((T)((object)this.publisherSettings[pushNotificationRawSettings.Name]));
				}
			}
			return list;
		}

		private T FilterByName<T>(string name) where T : PushNotificationPublisherSettings
		{
			PushNotificationPublisherSettings pushNotificationPublisherSettings;
			if (this.publisherSettings.TryGetValue(name, out pushNotificationPublisherSettings))
			{
				return (T)((object)pushNotificationPublisherSettings);
			}
			return default(T);
		}

		private void LoadPublisherSettings(IPushNotificationPublisherConfigurationProvider provider)
		{
			foreach (IPushNotificationRawSettings pushNotificationRawSettings in provider.LoadSettings(this.ignoreErrors))
			{
				if (!this.rawSettings.ContainsKey(pushNotificationRawSettings.Name))
				{
					if (this.unsuitablePublishers.ContainsKey(pushNotificationRawSettings.Name))
					{
						PushNotificationsCrimsonEvents.UnsuitableConfigurationOverride.Log<string>(pushNotificationRawSettings.Name);
						ExTraceGlobals.PushNotificationServiceTracer.TraceWarning<string>((long)this.GetHashCode(), "App'{0}' settings ignored because unsuitable settings had precedence.", pushNotificationRawSettings.Name);
					}
					else
					{
						PushNotificationPublisherSettings pushNotificationPublisherSettings = this.settingsFactory.Create(pushNotificationRawSettings);
						if (pushNotificationPublisherSettings.IsSuitable)
						{
							this.rawSettings[pushNotificationRawSettings.Name] = pushNotificationRawSettings;
							this.publisherSettings[pushNotificationPublisherSettings.AppId] = pushNotificationPublisherSettings;
						}
						else
						{
							this.unsuitablePublishers[pushNotificationRawSettings.Name] = pushNotificationRawSettings.Name;
						}
					}
				}
			}
		}

		private readonly bool ignoreErrors;

		private readonly Dictionary<string, IPushNotificationRawSettings> rawSettings;

		private readonly Dictionary<string, PushNotificationPublisherSettings> publisherSettings;

		private readonly Dictionary<string, string> unsuitablePublishers;

		private readonly Lazy<List<ApnsPublisherSettings>> apnsPublisherSettings;

		private readonly Lazy<List<WnsPublisherSettings>> wnsPublisherSettings;

		private readonly Lazy<List<GcmPublisherSettings>> gcmPublisherSettings;

		private readonly Lazy<List<WebAppPublisherSettings>> webAppPublisherSettings;

		private readonly Lazy<List<AzurePublisherSettings>> azurePublisherSettings;

		private readonly Lazy<List<AzureHubCreationPublisherSettings>> azureHubCreationPublisherSettings;

		private readonly Lazy<List<AzureChallengeRequestPublisherSettings>> azureChallengeRequestPublisherSettings;

		private readonly Lazy<List<AzureDeviceRegistrationPublisherSettings>> azureDeviceRegistrationPublisherSettings;

		private readonly Lazy<ProxyPublisherSettings> proxyPublisherSettings;

		private readonly Lazy<Dictionary<string, AzurePublisherSettings>> azureSendSettings;

		private readonly PushNotificationPublisherSettingsFactory settingsFactory;

		private readonly Lazy<int> hashCode;
	}
}
