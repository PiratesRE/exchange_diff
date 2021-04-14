using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.PushNotifications
{
	internal sealed class PushNotificationAppPresentationSchema : ADPresentationSchema
	{
		internal override ADObjectSchema GetParentSchema()
		{
			return ObjectSchema.GetInstance<PushNotificationAppSchema>();
		}

		public static readonly ADPropertyDefinition DisplayName = PushNotificationAppSchema.DisplayName;

		public static readonly ADPropertyDefinition Platform = PushNotificationAppSchema.Platform;

		public static readonly ADPropertyDefinition Enabled = PushNotificationAppSchema.Enabled;

		public static readonly ADPropertyDefinition ExchangeMaximumVersion = PushNotificationAppSchema.ExchangeMaximumVersion;

		public static readonly ADPropertyDefinition ExchangeMinimumVersion = PushNotificationAppSchema.ExchangeMinimumVersion;

		public static readonly ADPropertyDefinition QueueSize = PushNotificationAppSchema.QueueSize;

		public static readonly ADPropertyDefinition NumberOfChannels = PushNotificationAppSchema.NumberOfChannels;

		public static readonly ADPropertyDefinition BackOffTimeInSeconds = PushNotificationAppSchema.BackOffTimeInSeconds;

		public static readonly ADPropertyDefinition AuthenticationId = PushNotificationAppSchema.AuthenticationId;

		public static readonly ADPropertyDefinition AuthenticationKey = PushNotificationAppSchema.AuthenticationKey;

		public static readonly ADPropertyDefinition IsAuthenticationKeyEncrypted = PushNotificationAppSchema.IsAuthenticationKeyEncrypted;

		public static readonly ADPropertyDefinition AuthenticationKeyFallback = PushNotificationAppSchema.AuthenticationKeyFallback;

		public static readonly ADPropertyDefinition UriTemplate = PushNotificationAppSchema.UriTemplate;

		public static readonly ADPropertyDefinition Url = PushNotificationAppSchema.Url;

		public static readonly ADPropertyDefinition Port = PushNotificationAppSchema.Port;

		public static readonly ADPropertyDefinition RegitrationEnabled = PushNotificationAppSchema.RegistrationEnabled;

		public static readonly ADPropertyDefinition MultifactorRegistrationEnabled = PushNotificationAppSchema.MultifactorRegistrationEnabled;

		public static readonly ADPropertyDefinition RegistrationTemplate = PushNotificationAppSchema.RegistrationTemplate;

		public static readonly ADPropertyDefinition PartitionName = PushNotificationAppSchema.PartitionName;

		public static readonly ADPropertyDefinition IsDefaultPartitionName = PushNotificationAppSchema.IsDefaultPartitionName;

		public static readonly ADPropertyDefinition SecondaryUrl = PushNotificationAppSchema.SecondaryUrl;

		public static readonly ADPropertyDefinition SecondaryPort = PushNotificationAppSchema.SecondaryPort;
	}
}
