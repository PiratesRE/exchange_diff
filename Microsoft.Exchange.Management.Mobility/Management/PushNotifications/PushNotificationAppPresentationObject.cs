using System;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.PushNotifications
{
	[Serializable]
	public sealed class PushNotificationAppPresentationObject : ADPresentationObject
	{
		internal PushNotificationAppPresentationObject(PushNotificationApp pushNotificationApp) : base(pushNotificationApp)
		{
			switch (pushNotificationApp.Platform)
			{
			case PushNotificationPlatform.APNS:
				this.defaultValues = PushNotificationAppPresentationObject.ApnsDefaults;
				return;
			case PushNotificationPlatform.PendingGet:
				this.defaultValues = PushNotificationAppPresentationObject.PendingGetDefaults;
				return;
			case PushNotificationPlatform.WNS:
				this.defaultValues = PushNotificationAppPresentationObject.WnsDefaults;
				return;
			case PushNotificationPlatform.Proxy:
				this.defaultValues = PushNotificationAppPresentationObject.ProxyDefaults;
				return;
			case PushNotificationPlatform.GCM:
				this.defaultValues = PushNotificationAppPresentationObject.GcmDefaults;
				return;
			case PushNotificationPlatform.WebApp:
				this.defaultValues = PushNotificationAppPresentationObject.WebAppDefaults;
				return;
			case PushNotificationPlatform.Azure:
				this.defaultValues = PushNotificationAppPresentationObject.AzureDefaults;
				return;
			case PushNotificationPlatform.AzureHubCreation:
				this.defaultValues = PushNotificationAppPresentationObject.AzureHubCreationDefaults;
				return;
			case PushNotificationPlatform.AzureChallengeRequest:
				this.defaultValues = PushNotificationAppPresentationObject.AzureChallengeRequestDefaults;
				return;
			case PushNotificationPlatform.AzureDeviceRegistration:
				this.defaultValues = PushNotificationAppPresentationObject.AzureAzureDeviceRegistrationDefaults;
				return;
			default:
				throw new NotSupportedException("Unsupported PushNotificationPlatform: " + pushNotificationApp.Platform.ToString());
			}
		}

		public PushNotificationAppPresentationObject()
		{
		}

		public string DisplayName
		{
			get
			{
				return (string)this[PushNotificationAppPresentationSchema.DisplayName];
			}
		}

		public PushNotificationPlatform Platform
		{
			get
			{
				return (PushNotificationPlatform)this[PushNotificationAppPresentationSchema.Platform];
			}
		}

		public bool? Enabled
		{
			get
			{
				return (bool?)(this[PushNotificationAppPresentationSchema.Enabled] ?? this.defaultValues.Enabled);
			}
		}

		public Version ExchangeMinimumVersion
		{
			get
			{
				return (Version)(this[PushNotificationAppPresentationSchema.ExchangeMinimumVersion] ?? this.defaultValues.ExchangeMinimumVersion);
			}
		}

		public Version ExchangeMaximumVersion
		{
			get
			{
				return (Version)(this[PushNotificationAppPresentationSchema.ExchangeMaximumVersion] ?? this.defaultValues.ExchangeMaximumVersion);
			}
		}

		public int? QueueSize
		{
			get
			{
				return (int?)(this[PushNotificationAppPresentationSchema.QueueSize] ?? this.defaultValues.QueueSize);
			}
		}

		public int? NumberOfChannels
		{
			get
			{
				return (int?)(this[PushNotificationAppPresentationSchema.NumberOfChannels] ?? this.defaultValues.NumberOfChannels);
			}
		}

		public int? BackOffTimeInSeconds
		{
			get
			{
				return (int?)(this[PushNotificationAppPresentationSchema.BackOffTimeInSeconds] ?? this.defaultValues.BackOffTimeInSeconds);
			}
		}

		public string AuthenticationId
		{
			get
			{
				return (string)(this[PushNotificationAppPresentationSchema.AuthenticationId] ?? this.defaultValues.AuthenticationId);
			}
		}

		public string AuthenticationKey
		{
			get
			{
				return (string)(this[PushNotificationAppPresentationSchema.AuthenticationKey] ?? this.defaultValues.AuthenticationKey);
			}
		}

		public string AuthenticationKeyFallback
		{
			get
			{
				return (string)(this[PushNotificationAppPresentationSchema.AuthenticationKeyFallback] ?? this.defaultValues.AuthenticationKeyFallback);
			}
		}

		internal bool? IsAuthenticationKeyEncrypted
		{
			get
			{
				return (bool?)(this[PushNotificationAppPresentationSchema.IsAuthenticationKeyEncrypted] ?? this.defaultValues.IsAuthenticationKeyEncrypted);
			}
		}

		public string UriTemplate
		{
			get
			{
				return (string)(this[PushNotificationAppPresentationSchema.UriTemplate] ?? this.defaultValues.UriTemplate);
			}
		}

		public string Url
		{
			get
			{
				return (string)(this[PushNotificationAppPresentationSchema.Url] ?? this.defaultValues.Url);
			}
		}

		public int? Port
		{
			get
			{
				return (int?)(this[PushNotificationAppPresentationSchema.Port] ?? this.defaultValues.Port);
			}
		}

		public bool? RegistrationEnabled
		{
			get
			{
				return (bool?)(this[PushNotificationAppPresentationSchema.RegitrationEnabled] ?? this.defaultValues.RegistrationEnabled);
			}
		}

		public string RegistrationTemplate
		{
			get
			{
				return (string)(this[PushNotificationAppPresentationSchema.RegistrationTemplate] ?? this.defaultValues.RegistrationTemplate);
			}
		}

		public bool? MultifactorRegistrationEnabled
		{
			get
			{
				return (bool?)(this[PushNotificationAppPresentationSchema.MultifactorRegistrationEnabled] ?? this.defaultValues.MultifactorRegistrationEnabled);
			}
		}

		public string PartitionName
		{
			get
			{
				return (string)(this[PushNotificationAppPresentationSchema.PartitionName] ?? this.defaultValues.RegistrationTemplate);
			}
		}

		public bool? IsDefaultPartitionName
		{
			get
			{
				return (bool?)(this[PushNotificationAppPresentationSchema.IsDefaultPartitionName] ?? this.defaultValues.IsDefaultPartitionName);
			}
		}

		public string SecondaryUrl
		{
			get
			{
				return (string)(this[PushNotificationAppPresentationSchema.SecondaryUrl] ?? this.defaultValues.SecondaryUrl);
			}
		}

		public int? SecondaryPort
		{
			get
			{
				return (int?)(this[PushNotificationAppPresentationSchema.SecondaryPort] ?? this.defaultValues.SecondaryPort);
			}
		}

		internal override ADPresentationSchema PresentationSchema
		{
			get
			{
				return PushNotificationAppPresentationObject.SchemaInstance;
			}
		}

		private static PushNotificationApp BuildApnsDefaults()
		{
			return new PushNotificationApp
			{
				Enabled = new bool?(true),
				ExchangeMinimumVersion = null,
				ExchangeMaximumVersion = null,
				QueueSize = new int?(10000),
				NumberOfChannels = new int?(1),
				Url = "gateway.push.apple.com",
				Port = new int?(2195),
				SecondaryUrl = "feedback.push.apple.com",
				SecondaryPort = new int?(2196),
				BackOffTimeInSeconds = new int?(600)
			};
		}

		private static PushNotificationApp BuildWnsDefaults()
		{
			return new PushNotificationApp
			{
				Enabled = new bool?(true),
				ExchangeMinimumVersion = null,
				ExchangeMaximumVersion = null,
				QueueSize = new int?(10000),
				NumberOfChannels = new int?(1),
				IsAuthenticationKeyEncrypted = new bool?(true),
				SecondaryUrl = "https://login.live.com/accesstoken.srf",
				BackOffTimeInSeconds = new int?(600)
			};
		}

		private static PushNotificationApp BuildGcmDefaults()
		{
			return new PushNotificationApp
			{
				Enabled = new bool?(true),
				ExchangeMinimumVersion = null,
				ExchangeMaximumVersion = null,
				QueueSize = new int?(10000),
				NumberOfChannels = new int?(1),
				IsAuthenticationKeyEncrypted = new bool?(true),
				Url = "https://android.googleapis.com/gcm/send",
				BackOffTimeInSeconds = new int?(600)
			};
		}

		private static PushNotificationApp BuildAzureDefaults()
		{
			return new PushNotificationApp
			{
				Enabled = new bool?(true),
				ExchangeMinimumVersion = null,
				ExchangeMaximumVersion = null,
				QueueSize = new int?(10000),
				NumberOfChannels = new int?(1),
				MultifactorRegistrationEnabled = new bool?(false),
				IsDefaultPartitionName = new bool?(false),
				IsAuthenticationKeyEncrypted = new bool?(true),
				UriTemplate = "https://{0}-{1}.servicebus.windows.net/exo/{2}/{3}",
				RegistrationEnabled = new bool?(false),
				BackOffTimeInSeconds = new int?(600)
			};
		}

		private static PushNotificationApp BuildAzureHubCreationDefaults()
		{
			return new PushNotificationApp
			{
				Enabled = new bool?(true),
				ExchangeMinimumVersion = null,
				ExchangeMaximumVersion = null,
				QueueSize = new int?(10000),
				NumberOfChannels = new int?(1),
				IsAuthenticationKeyEncrypted = new bool?(true),
				UriTemplate = "https://{0}-{1}.servicebus.windows.net/exo/{2}{3}",
				Url = "https://{0}-{1}-sb.accesscontrol.windows.net/",
				SecondaryUrl = "http://{0}-{1}.servicebus.windows.net/exo/",
				BackOffTimeInSeconds = new int?(600)
			};
		}

		private static PushNotificationApp BuildAzureChallengeRequestDefaults()
		{
			return new PushNotificationApp
			{
				Enabled = new bool?(true),
				ExchangeMinimumVersion = null,
				ExchangeMaximumVersion = null,
				QueueSize = new int?(10000),
				NumberOfChannels = new int?(1),
				BackOffTimeInSeconds = new int?(600)
			};
		}

		private static PushNotificationApp BuildAzureDeviceRegistrationDefaults()
		{
			return new PushNotificationApp
			{
				Enabled = new bool?(true),
				ExchangeMinimumVersion = null,
				ExchangeMaximumVersion = null,
				QueueSize = new int?(10000),
				NumberOfChannels = new int?(1),
				BackOffTimeInSeconds = new int?(600)
			};
		}

		private static PushNotificationApp BuildWebAppDefaults()
		{
			return new PushNotificationApp
			{
				Enabled = new bool?(true),
				ExchangeMinimumVersion = null,
				ExchangeMaximumVersion = null,
				QueueSize = new int?(10000),
				NumberOfChannels = new int?(1)
			};
		}

		private static PushNotificationApp BuildPendingGetDefaults()
		{
			return new PushNotificationApp
			{
				Enabled = new bool?(true),
				ExchangeMinimumVersion = null,
				ExchangeMaximumVersion = null,
				QueueSize = new int?(10000),
				NumberOfChannels = new int?(1)
			};
		}

		private static PushNotificationApp BuildProxyDefaults()
		{
			return new PushNotificationApp
			{
				Enabled = new bool?(false),
				ExchangeMinimumVersion = null,
				ExchangeMaximumVersion = null,
				QueueSize = new int?(10000),
				NumberOfChannels = new int?(1)
			};
		}

		private static readonly PushNotificationAppPresentationSchema SchemaInstance = ObjectSchema.GetInstance<PushNotificationAppPresentationSchema>();

		private static readonly PushNotificationApp ApnsDefaults = PushNotificationAppPresentationObject.BuildApnsDefaults();

		private static readonly PushNotificationApp WnsDefaults = PushNotificationAppPresentationObject.BuildWnsDefaults();

		private static readonly PushNotificationApp GcmDefaults = PushNotificationAppPresentationObject.BuildGcmDefaults();

		private static readonly PushNotificationApp WebAppDefaults = PushNotificationAppPresentationObject.BuildWebAppDefaults();

		private static readonly PushNotificationApp PendingGetDefaults = PushNotificationAppPresentationObject.BuildPendingGetDefaults();

		private static readonly PushNotificationApp ProxyDefaults = PushNotificationAppPresentationObject.BuildProxyDefaults();

		private static readonly PushNotificationApp AzureDefaults = PushNotificationAppPresentationObject.BuildAzureDefaults();

		private static readonly PushNotificationApp AzureHubCreationDefaults = PushNotificationAppPresentationObject.BuildAzureHubCreationDefaults();

		private static readonly PushNotificationApp AzureChallengeRequestDefaults = PushNotificationAppPresentationObject.BuildAzureChallengeRequestDefaults();

		private static readonly PushNotificationApp AzureAzureDeviceRegistrationDefaults = PushNotificationAppPresentationObject.BuildAzureDeviceRegistrationDefaults();

		private PushNotificationApp defaultValues;
	}
}
