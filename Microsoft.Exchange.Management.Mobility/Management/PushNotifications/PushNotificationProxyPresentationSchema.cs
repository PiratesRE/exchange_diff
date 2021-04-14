using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.PushNotifications
{
	internal sealed class PushNotificationProxyPresentationSchema : ADPresentationSchema
	{
		internal override ADObjectSchema GetParentSchema()
		{
			return ObjectSchema.GetInstance<PushNotificationAppSchema>();
		}

		public static readonly ADPropertyDefinition DisplayName = PushNotificationAppSchema.DisplayName;

		public static readonly ADPropertyDefinition Enabled = PushNotificationAppSchema.Enabled;

		public static readonly ADPropertyDefinition Organization = PushNotificationAppSchema.AuthenticationKey;

		public static readonly ADPropertyDefinition Uri = PushNotificationAppSchema.Url;
	}
}
