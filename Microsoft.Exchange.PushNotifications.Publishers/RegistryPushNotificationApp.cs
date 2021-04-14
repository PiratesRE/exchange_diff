using System;
using System.Text;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.PushNotifications.Extensions;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	[Serializable]
	public sealed class RegistryPushNotificationApp : RegistryObject, IPushNotificationRawSettings, IEquatable<IPushNotificationRawSettings>, IEquatable<RegistryPushNotificationApp>
	{
		public RegistryPushNotificationApp()
		{
		}

		public RegistryPushNotificationApp(string name)
		{
			ArgumentValidator.ThrowIfNullOrEmpty("name", name);
			this.propertyBag[SimpleProviderObjectSchema.Identity] = new RegistryObjectId("SYSTEM\\CurrentControlSet\\Services\\MSExchange PushNotifications\\Applications", name);
			base.ResetChangeTracking(true);
		}

		public string Name
		{
			get
			{
				return ((RegistryObjectId)this.Identity).Name;
			}
		}

		public PushNotificationPlatform Platform
		{
			get
			{
				return (PushNotificationPlatform)this[RegistryPushNotificationApp.Schema.Platform];
			}
			set
			{
				this[RegistryPushNotificationApp.Schema.Platform] = value;
			}
		}

		public bool? Enabled
		{
			get
			{
				return (bool?)this[RegistryPushNotificationApp.Schema.Enabled];
			}
			set
			{
				this[RegistryPushNotificationApp.Schema.Enabled] = value;
			}
		}

		public Version ExchangeMinimumVersion
		{
			get
			{
				return (Version)this[RegistryPushNotificationApp.Schema.ExchangeMinimumVersion];
			}
			set
			{
				this[RegistryPushNotificationApp.Schema.ExchangeMinimumVersion] = value;
			}
		}

		public Version ExchangeMaximumVersion
		{
			get
			{
				return (Version)this[RegistryPushNotificationApp.Schema.ExchangeMaximumVersion];
			}
			set
			{
				this[RegistryPushNotificationApp.Schema.ExchangeMaximumVersion] = value;
			}
		}

		public int? QueueSize
		{
			get
			{
				return (int?)this[RegistryPushNotificationApp.Schema.QueueSize];
			}
			set
			{
				this[RegistryPushNotificationApp.Schema.QueueSize] = value;
			}
		}

		public int? NumberOfChannels
		{
			get
			{
				return (int?)this[RegistryPushNotificationApp.Schema.NumberOfChannels];
			}
			set
			{
				this[RegistryPushNotificationApp.Schema.NumberOfChannels] = value;
			}
		}

		public int? BackOffTimeInSeconds
		{
			get
			{
				return (int?)this[RegistryPushNotificationApp.Schema.BackOffTimeInSeconds];
			}
			set
			{
				this[RegistryPushNotificationApp.Schema.BackOffTimeInSeconds] = value;
			}
		}

		public string AuthenticationId
		{
			get
			{
				return (string)this[RegistryPushNotificationApp.Schema.AuthenticationId];
			}
			set
			{
				this[RegistryPushNotificationApp.Schema.AuthenticationId] = value;
			}
		}

		public string AuthenticationKey
		{
			get
			{
				return (string)this[RegistryPushNotificationApp.Schema.AuthenticationKey];
			}
			set
			{
				this[RegistryPushNotificationApp.Schema.AuthenticationKey] = value;
			}
		}

		public string AuthenticationKeyFallback
		{
			get
			{
				return (string)this[RegistryPushNotificationApp.Schema.AuthenticationKeyFallback];
			}
			set
			{
				this[RegistryPushNotificationApp.Schema.AuthenticationKeyFallback] = value;
			}
		}

		public bool? IsAuthenticationKeyEncrypted
		{
			get
			{
				return (bool?)this[RegistryPushNotificationApp.Schema.IsAuthenticationKeyEncrypted];
			}
			set
			{
				this[RegistryPushNotificationApp.Schema.IsAuthenticationKeyEncrypted] = value;
			}
		}

		public string Url
		{
			get
			{
				return (string)this[RegistryPushNotificationApp.Schema.Url];
			}
			set
			{
				this[RegistryPushNotificationApp.Schema.Url] = value;
			}
		}

		public int? Port
		{
			get
			{
				return (int?)this[RegistryPushNotificationApp.Schema.Port];
			}
			set
			{
				this[RegistryPushNotificationApp.Schema.Port] = value;
			}
		}

		public string SecondaryUrl
		{
			get
			{
				return (string)this[RegistryPushNotificationApp.Schema.SecondaryUrl];
			}
			set
			{
				this[RegistryPushNotificationApp.Schema.SecondaryUrl] = value;
			}
		}

		public int? SecondaryPort
		{
			get
			{
				return (int?)this[RegistryPushNotificationApp.Schema.SecondaryPort];
			}
			set
			{
				this[RegistryPushNotificationApp.Schema.SecondaryPort] = value;
			}
		}

		public int? AddTimeout
		{
			get
			{
				return (int?)this[RegistryPushNotificationApp.Schema.AddTimeout];
			}
			set
			{
				this[RegistryPushNotificationApp.Schema.AddTimeout] = value;
			}
		}

		public int? ConnectStepTimeout
		{
			get
			{
				return (int?)this[RegistryPushNotificationApp.Schema.ConnectStepTimeout];
			}
			set
			{
				this[RegistryPushNotificationApp.Schema.ConnectStepTimeout] = value;
			}
		}

		public int? ConnectTotalTimeout
		{
			get
			{
				return (int?)this[RegistryPushNotificationApp.Schema.ConnectTotalTimeout];
			}
			set
			{
				this[RegistryPushNotificationApp.Schema.ConnectTotalTimeout] = value;
			}
		}

		public int? ConnectRetryDelay
		{
			get
			{
				return (int?)this[RegistryPushNotificationApp.Schema.ConnectRetryDelay];
			}
			set
			{
				this[RegistryPushNotificationApp.Schema.ConnectRetryDelay] = value;
			}
		}

		public int? AuthenticateRetryMax
		{
			get
			{
				return (int?)this[RegistryPushNotificationApp.Schema.AuthenticateRetryMax];
			}
			set
			{
				this[RegistryPushNotificationApp.Schema.AuthenticateRetryMax] = value;
			}
		}

		public int? ConnectRetryMax
		{
			get
			{
				return (int?)this[RegistryPushNotificationApp.Schema.ConnectRetryMax];
			}
			set
			{
				this[RegistryPushNotificationApp.Schema.ConnectRetryMax] = value;
			}
		}

		public int? ReadTimeout
		{
			get
			{
				return (int?)this[RegistryPushNotificationApp.Schema.ReadTimeout];
			}
			set
			{
				this[RegistryPushNotificationApp.Schema.ReadTimeout] = value;
			}
		}

		public int? WriteTimeout
		{
			get
			{
				return (int?)this[RegistryPushNotificationApp.Schema.WriteTimeout];
			}
			set
			{
				this[RegistryPushNotificationApp.Schema.WriteTimeout] = value;
			}
		}

		public bool? IgnoreCertificateErrors
		{
			get
			{
				return (bool?)this[RegistryPushNotificationApp.Schema.IgnoreCertificateErrors];
			}
			set
			{
				this[RegistryPushNotificationApp.Schema.IgnoreCertificateErrors] = value;
			}
		}

		public string UriTemplate
		{
			get
			{
				return (string)this[RegistryPushNotificationApp.Schema.UriTemplate];
			}
			set
			{
				this[RegistryPushNotificationApp.Schema.UriTemplate] = value;
			}
		}

		public int? MaximumCacheSize
		{
			get
			{
				return (int?)this[RegistryPushNotificationApp.Schema.MaximumCacheSize];
			}
			set
			{
				this[RegistryPushNotificationApp.Schema.MaximumCacheSize] = value;
			}
		}

		public string RegistrationTemplate
		{
			get
			{
				return (string)this[RegistryPushNotificationApp.Schema.RegistrationTemplate];
			}
			set
			{
				this[RegistryPushNotificationApp.Schema.RegistrationTemplate] = value;
			}
		}

		public bool? RegistrationEnabled
		{
			get
			{
				return (bool?)this[RegistryPushNotificationApp.Schema.RegistrationEnabled];
			}
			set
			{
				this[RegistryPushNotificationApp.Schema.RegistrationEnabled] = value;
			}
		}

		public bool? MultifactorRegistrationEnabled
		{
			get
			{
				return (bool?)this[RegistryPushNotificationApp.Schema.MultifactorRegistrationEnabled];
			}
			set
			{
				this[RegistryPushNotificationApp.Schema.MultifactorRegistrationEnabled] = value;
			}
		}

		public string PartitionName
		{
			get
			{
				return (string)this[RegistryPushNotificationApp.Schema.PartitionName];
			}
			set
			{
				this[RegistryPushNotificationApp.Schema.PartitionName] = value;
			}
		}

		public bool? IsDefaultPartitionName
		{
			get
			{
				return (bool?)this[RegistryPushNotificationApp.Schema.IsDefaultPartitionName];
			}
			set
			{
				this[RegistryPushNotificationApp.Schema.IsDefaultPartitionName] = value;
			}
		}

		internal override RegistryObjectSchema RegistrySchema
		{
			get
			{
				return RegistryPushNotificationApp.SchemaInstance;
			}
		}

		public string ToFullString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (PropertyDefinition propertyDefinition in this.RegistrySchema.AllProperties)
			{
				if (stringBuilder.Length > 0)
				{
					stringBuilder.Append("; ");
				}
				stringBuilder.AppendFormat("{0}:{1}", propertyDefinition.Name, this[propertyDefinition].ToNullableString(null));
			}
			return stringBuilder.ToString();
		}

		public override bool Equals(object obj)
		{
			return ConfigurableObject.AreEqual(this, obj as RegistryPushNotificationApp);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public bool Equals(RegistryPushNotificationApp other)
		{
			return ConfigurableObject.AreEqual(this, other);
		}

		public bool Equals(IPushNotificationRawSettings other)
		{
			return ConfigurableObject.AreEqual(this, other as RegistryPushNotificationApp);
		}

		private static readonly RegistryPushNotificationApp.Schema SchemaInstance = ObjectSchema.GetInstance<RegistryPushNotificationApp.Schema>();

		internal class Schema : RegistryObjectSchema
		{
			public override string DefaultRegistryKeyPath
			{
				get
				{
					return "SYSTEM\\CurrentControlSet\\Services\\MSExchange PushNotifications\\Applications";
				}
			}

			public override string DefaultName
			{
				get
				{
					return null;
				}
			}

			public const string RegistryRootName = "Applications";

			public const string RegistryRoot = "SYSTEM\\CurrentControlSet\\Services\\MSExchange PushNotifications\\Applications";

			public static readonly RegistryObjectId RootFolder = new RegistryObjectId("SYSTEM\\CurrentControlSet\\Services\\MSExchange PushNotifications\\Applications");

			public static readonly RegistryPropertyDefinition Platform = new RegistryPropertyDefinition("Platform", typeof(PushNotificationPlatform), PushNotificationPlatform.None);

			public static readonly RegistryPropertyDefinition Enabled = new RegistryPropertyDefinition("Enabled", typeof(bool?), null);

			public static readonly RegistryPropertyDefinition ExchangeMaximumVersion = new RegistryPropertyDefinition("ExchangeMaximumVersion", typeof(Version), null);

			public static readonly RegistryPropertyDefinition ExchangeMinimumVersion = new RegistryPropertyDefinition("ExchangeMinimumVersion", typeof(Version), null);

			public static readonly RegistryPropertyDefinition QueueSize = new RegistryPropertyDefinition("QueueSize", typeof(int?), null);

			public static readonly RegistryPropertyDefinition NumberOfChannels = new RegistryPropertyDefinition("NumberOfChannels", typeof(int?), null);

			public static readonly RegistryPropertyDefinition BackOffTimeInSeconds = new RegistryPropertyDefinition("BackOffTimeInSeconds", typeof(int?), null);

			public static readonly RegistryPropertyDefinition AuthenticationId = new RegistryPropertyDefinition("AuthenticationId", typeof(string), null);

			public static readonly RegistryPropertyDefinition AuthenticationKey = new RegistryPropertyDefinition("AuthenticationKey", typeof(string), null);

			public static readonly RegistryPropertyDefinition AuthenticationKeyFallback = new RegistryPropertyDefinition("AuthenticationKeyFallback", typeof(string), null);

			public static readonly RegistryPropertyDefinition IsAuthenticationKeyEncrypted = new RegistryPropertyDefinition("IsAuthenticationKeyEncrypted", typeof(bool?), null);

			public static readonly RegistryPropertyDefinition Url = new RegistryPropertyDefinition("Url", typeof(string), null);

			public static readonly RegistryPropertyDefinition Port = new RegistryPropertyDefinition("Port", typeof(int?), null);

			public static readonly RegistryPropertyDefinition SecondaryUrl = new RegistryPropertyDefinition("SecondaryUrl", typeof(string), null);

			public static readonly RegistryPropertyDefinition SecondaryPort = new RegistryPropertyDefinition("SecondaryPort", typeof(int?), null);

			public static readonly RegistryPropertyDefinition AddTimeout = new RegistryPropertyDefinition("AddTimeout", typeof(int?), null);

			public static readonly RegistryPropertyDefinition ConnectStepTimeout = new RegistryPropertyDefinition("ConnectStepTimeout", typeof(int?), null);

			public static readonly RegistryPropertyDefinition ConnectTotalTimeout = new RegistryPropertyDefinition("ConnectTotalTimeout", typeof(int?), null);

			public static readonly RegistryPropertyDefinition ConnectRetryDelay = new RegistryPropertyDefinition("ConnectRetryDelay", typeof(int?), null);

			public static readonly RegistryPropertyDefinition AuthenticateRetryMax = new RegistryPropertyDefinition("AuthenticateRetryMax", typeof(int?), null);

			public static readonly RegistryPropertyDefinition ConnectRetryMax = new RegistryPropertyDefinition("ConnectRetryMax", typeof(int?), null);

			public static readonly RegistryPropertyDefinition ReadTimeout = new RegistryPropertyDefinition("ReadTimeout", typeof(int?), null);

			public static readonly RegistryPropertyDefinition WriteTimeout = new RegistryPropertyDefinition("WriteTimeout", typeof(int?), null);

			public static readonly RegistryPropertyDefinition IgnoreCertificateErrors = new RegistryPropertyDefinition("IgnoreCertificateErrors", typeof(bool?), null);

			public static readonly RegistryPropertyDefinition UriTemplate = new RegistryPropertyDefinition("UriTemplate", typeof(string), null);

			public static readonly RegistryPropertyDefinition MaximumCacheSize = new RegistryPropertyDefinition("MaximumCacheSize", typeof(int?), null);

			public static readonly RegistryPropertyDefinition RegistrationTemplate = new RegistryPropertyDefinition("RegistrationTemplate", typeof(string), null);

			public static readonly RegistryPropertyDefinition RegistrationEnabled = new RegistryPropertyDefinition("RegistrationEnabled", typeof(bool?), null);

			public static readonly RegistryPropertyDefinition MultifactorRegistrationEnabled = new RegistryPropertyDefinition("MultifactorRegistrationEnabled", typeof(bool?), null);

			public static readonly RegistryPropertyDefinition PartitionName = new RegistryPropertyDefinition("PartitionName", typeof(string), null);

			public static readonly RegistryPropertyDefinition IsDefaultPartitionName = new RegistryPropertyDefinition("IsDefaultPartitionName", typeof(bool?), null);
		}
	}
}
