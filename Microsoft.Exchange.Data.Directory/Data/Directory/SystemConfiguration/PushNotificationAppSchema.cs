using System;
using Microsoft.Exchange.Common;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal class PushNotificationAppSchema : ADConfigurationObjectSchema
	{
		private static ADPropertyDefinition CreateXmlProperty<T>(string name, Func<PushNotificationAppConfigXml, T?> getter, Action<PushNotificationAppConfigXml, T?> setter) where T : struct
		{
			return XMLSerializableBase.ConfigXmlProperty<PushNotificationAppConfigXml, T?>(name, ExchangeObjectVersion.Exchange2012, PushNotificationAppSchema.AppSettings, null, getter, setter, null, null);
		}

		private static ADPropertyDefinition CreateXmlProperty<T>(string name, Func<PushNotificationAppConfigXml, T> getter, Action<PushNotificationAppConfigXml, T> setter) where T : class
		{
			return XMLSerializableBase.ConfigXmlProperty<PushNotificationAppConfigXml, T>(name, ExchangeObjectVersion.Exchange2012, PushNotificationAppSchema.AppSettings, default(T), getter, setter, null, null);
		}

		public static readonly ADPropertyDefinition DisplayName = SharedPropertyDefinitions.OptionalDisplayName;

		public static readonly ADPropertyDefinition Platform = new ADPropertyDefinition("Platform", ExchangeObjectVersion.Exchange2012, typeof(PushNotificationPlatform), "msExchProvisioningFlags", ADPropertyDefinitionFlags.None, PushNotificationPlatform.None, PropertyDefinitionConstraint.None, PropertyDefinitionConstraint.None, null, null);

		public static readonly ADPropertyDefinition RawAppSettingsXml = XMLSerializableBase.ConfigurationXmlRawProperty();

		public static readonly ADPropertyDefinition AppSettings = XMLSerializableBase.ConfigurationXmlProperty<PushNotificationAppConfigXml>(PushNotificationAppSchema.RawAppSettingsXml);

		public static readonly ADPropertyDefinition Enabled = PushNotificationAppSchema.CreateXmlProperty<bool>("Enabled", (PushNotificationAppConfigXml configXml) => configXml.Enabled, delegate(PushNotificationAppConfigXml configXml, bool? value)
		{
			configXml.Enabled = value;
		});

		public static readonly ADPropertyDefinition ExchangeMaximumVersion = PushNotificationAppSchema.CreateXmlProperty<Version>("ExchangeMaximumVersion", delegate(PushNotificationAppConfigXml configXml)
		{
			if (configXml.ExchangeMaximumVersion != null)
			{
				return new Version(configXml.ExchangeMaximumVersion);
			}
			return null;
		}, delegate(PushNotificationAppConfigXml configXml, Version value)
		{
			configXml.ExchangeMaximumVersion = ((value == null) ? null : value.ToString());
		});

		public static readonly ADPropertyDefinition ExchangeMinimumVersion = PushNotificationAppSchema.CreateXmlProperty<Version>("ExchangeMinimumVersion", delegate(PushNotificationAppConfigXml configXml)
		{
			if (configXml.ExchangeMinimumVersion != null)
			{
				return new Version(configXml.ExchangeMinimumVersion);
			}
			return null;
		}, delegate(PushNotificationAppConfigXml configXml, Version value)
		{
			configXml.ExchangeMinimumVersion = ((value == null) ? null : value.ToString());
		});

		public static readonly ADPropertyDefinition QueueSize = PushNotificationAppSchema.CreateXmlProperty<int>("QueueSize", (PushNotificationAppConfigXml configXml) => configXml.QueueSize, delegate(PushNotificationAppConfigXml configXml, int? value)
		{
			configXml.QueueSize = value;
		});

		public static readonly ADPropertyDefinition NumberOfChannels = PushNotificationAppSchema.CreateXmlProperty<int>("NumberOfChannels", (PushNotificationAppConfigXml configXml) => configXml.NumberOfChannels, delegate(PushNotificationAppConfigXml configXml, int? value)
		{
			configXml.NumberOfChannels = value;
		});

		public static readonly ADPropertyDefinition BackOffTimeInSeconds = PushNotificationAppSchema.CreateXmlProperty<int>("BackOffTimeInSeconds", (PushNotificationAppConfigXml configXml) => configXml.BackOffTimeInSeconds, delegate(PushNotificationAppConfigXml configXml, int? value)
		{
			configXml.BackOffTimeInSeconds = value;
		});

		public static readonly ADPropertyDefinition AuthenticationId = PushNotificationAppSchema.CreateXmlProperty<string>("AuthenticationId", (PushNotificationAppConfigXml configXml) => configXml.AuthId, delegate(PushNotificationAppConfigXml configXml, string value)
		{
			configXml.AuthId = value;
		});

		public static readonly ADPropertyDefinition AuthenticationKey = PushNotificationAppSchema.CreateXmlProperty<string>("AuthenticationKey", (PushNotificationAppConfigXml configXml) => configXml.AuthKey, delegate(PushNotificationAppConfigXml configXml, string value)
		{
			configXml.AuthKey = value;
		});

		public static readonly ADPropertyDefinition AuthenticationKeyFallback = PushNotificationAppSchema.CreateXmlProperty<string>("AuthenticationKey", (PushNotificationAppConfigXml configXml) => configXml.AuthKeyFallback, delegate(PushNotificationAppConfigXml configXml, string value)
		{
			configXml.AuthKeyFallback = value;
		});

		public static readonly ADPropertyDefinition IsAuthenticationKeyEncrypted = PushNotificationAppSchema.CreateXmlProperty<bool>("IsAuthenticationKeyEncrypted", (PushNotificationAppConfigXml configXml) => configXml.IsAuthKeyEncrypted, delegate(PushNotificationAppConfigXml configXml, bool? value)
		{
			configXml.IsAuthKeyEncrypted = value;
		});

		public static readonly ADPropertyDefinition Url = PushNotificationAppSchema.CreateXmlProperty<string>("Url", (PushNotificationAppConfigXml configXml) => configXml.Url, delegate(PushNotificationAppConfigXml configXml, string value)
		{
			configXml.Url = value;
		});

		public static readonly ADPropertyDefinition Port = PushNotificationAppSchema.CreateXmlProperty<int>("Port", (PushNotificationAppConfigXml configXml) => configXml.Port, delegate(PushNotificationAppConfigXml configXml, int? value)
		{
			configXml.Port = value;
		});

		public static readonly ADPropertyDefinition SecondaryUrl = PushNotificationAppSchema.CreateXmlProperty<string>("SecondaryUrl", (PushNotificationAppConfigXml configXml) => configXml.SecondaryUrl, delegate(PushNotificationAppConfigXml configXml, string value)
		{
			configXml.SecondaryUrl = value;
		});

		public static readonly ADPropertyDefinition SecondaryPort = PushNotificationAppSchema.CreateXmlProperty<int>("SecondaryPort", (PushNotificationAppConfigXml configXml) => configXml.SecondaryPort, delegate(PushNotificationAppConfigXml configXml, int? value)
		{
			configXml.SecondaryPort = value;
		});

		public static readonly ADPropertyDefinition UriTemplate = PushNotificationAppSchema.CreateXmlProperty<string>("UriTemplate", (PushNotificationAppConfigXml configXml) => configXml.UriTemplate, delegate(PushNotificationAppConfigXml configXml, string value)
		{
			configXml.UriTemplate = value;
		});

		public static readonly ADPropertyDefinition RegistrationTemplate = PushNotificationAppSchema.CreateXmlProperty<string>("RegistrationTemplate", (PushNotificationAppConfigXml configXml) => configXml.RegistrationTemplate, delegate(PushNotificationAppConfigXml configXml, string value)
		{
			configXml.RegistrationTemplate = value;
		});

		public static readonly ADPropertyDefinition RegistrationEnabled = PushNotificationAppSchema.CreateXmlProperty<bool>("RegistrationEnabled", (PushNotificationAppConfigXml configXml) => configXml.RegistrationEnabled, delegate(PushNotificationAppConfigXml configXml, bool? value)
		{
			configXml.RegistrationEnabled = value;
		});

		public static readonly ADPropertyDefinition MultifactorRegistrationEnabled = PushNotificationAppSchema.CreateXmlProperty<bool>("MultifactorRegistrationEnabled", (PushNotificationAppConfigXml configXml) => configXml.MultifactorRegistrationEnabled, delegate(PushNotificationAppConfigXml configXml, bool? value)
		{
			configXml.MultifactorRegistrationEnabled = value;
		});

		public static readonly ADPropertyDefinition PartitionName = PushNotificationAppSchema.CreateXmlProperty<string>("PartitionName", (PushNotificationAppConfigXml configXml) => configXml.PartitionName, delegate(PushNotificationAppConfigXml configXml, string value)
		{
			configXml.PartitionName = value;
		});

		public static readonly ADPropertyDefinition IsDefaultPartitionName = PushNotificationAppSchema.CreateXmlProperty<bool>("IsDefaultPartitionName", (PushNotificationAppConfigXml configXml) => configXml.IsDefaultPartitionName, delegate(PushNotificationAppConfigXml configXml, bool? value)
		{
			configXml.IsDefaultPartitionName = value;
		});

		public static readonly ADPropertyDefinition LastUpdateTimeUtc = PushNotificationAppSchema.CreateXmlProperty<DateTime>("LastUpdateTimeUtc", (PushNotificationAppConfigXml configXml) => configXml.LastUpdateTimeUtc, delegate(PushNotificationAppConfigXml configXml, DateTime? value)
		{
			configXml.LastUpdateTimeUtc = value;
		});
	}
}
