using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.PushNotifications
{
	[Serializable]
	public sealed class PushNotificationProxyPresentationObject : ADPresentationObject
	{
		internal PushNotificationProxyPresentationObject(PushNotificationApp pushNotificationApp) : base(pushNotificationApp)
		{
		}

		public PushNotificationProxyPresentationObject()
		{
		}

		public string DisplayName
		{
			get
			{
				return (string)this[PushNotificationProxyPresentationSchema.DisplayName];
			}
		}

		public bool? Enabled
		{
			get
			{
				return new bool?((bool)(this[PushNotificationProxyPresentationSchema.Enabled] ?? PushNotificationProxyPresentationObject.ProxyDefaults.Enabled));
			}
		}

		public string Organization
		{
			get
			{
				return (string)(this[PushNotificationProxyPresentationSchema.Organization] ?? PushNotificationProxyPresentationObject.ProxyDefaults.AuthenticationKey);
			}
		}

		public string Uri
		{
			get
			{
				return (string)(this[PushNotificationProxyPresentationSchema.Uri] ?? PushNotificationProxyPresentationObject.ProxyDefaults.Url);
			}
		}

		internal override ADPresentationSchema PresentationSchema
		{
			get
			{
				return PushNotificationProxyPresentationObject.SchemaInstance;
			}
		}

		private static PushNotificationApp BuildProxyDefaults()
		{
			return new PushNotificationApp
			{
				Enabled = new bool?(false)
			};
		}

		private static readonly PushNotificationProxyPresentationSchema SchemaInstance = ObjectSchema.GetInstance<PushNotificationProxyPresentationSchema>();

		private static readonly PushNotificationApp ProxyDefaults = PushNotificationProxyPresentationObject.BuildProxyDefaults();
	}
}
