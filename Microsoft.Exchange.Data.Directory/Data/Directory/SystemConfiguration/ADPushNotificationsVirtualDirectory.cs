using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Serializable]
	public class ADPushNotificationsVirtualDirectory : ADExchangeServiceVirtualDirectory
	{
		internal override string MostDerivedObjectClass
		{
			get
			{
				return ADPushNotificationsVirtualDirectory.MostDerivedClassName;
			}
		}

		public bool LiveIdAuthentication
		{
			get
			{
				return (bool)this[ADPushNotificationsVirtualDirectorySchema.LiveIdAuthentication];
			}
			set
			{
				this[ADPushNotificationsVirtualDirectorySchema.LiveIdAuthentication] = value;
			}
		}

		internal static object LiveIdAuthenticationGetter(IPropertyBag propertyBag)
		{
			MultiValuedProperty<AuthenticationMethod> multiValuedProperty = (MultiValuedProperty<AuthenticationMethod>)propertyBag[ADVirtualDirectorySchema.InternalAuthenticationMethods];
			return multiValuedProperty.Contains(AuthenticationMethod.LiveIdFba);
		}

		internal static void LiveIdAuthenticationSetter(object value, IPropertyBag propertyBag)
		{
			List<AuthenticationMethod> list = new List<AuthenticationMethod>();
			MultiValuedProperty<AuthenticationMethod> multiValuedProperty = (MultiValuedProperty<AuthenticationMethod>)propertyBag[ADVirtualDirectorySchema.InternalAuthenticationMethods];
			if (multiValuedProperty != null)
			{
				list.AddRange(multiValuedProperty);
			}
			ADExchangeServiceVirtualDirectory.AddOrRemoveAuthenticationMethod(list, new bool?((bool)value), new AuthenticationMethod[]
			{
				AuthenticationMethod.LiveIdFba
			});
			MultiValuedProperty<AuthenticationMethod> value2 = new MultiValuedProperty<AuthenticationMethod>(list);
			propertyBag[ADVirtualDirectorySchema.InternalAuthenticationMethods] = value2;
			propertyBag[ADVirtualDirectorySchema.ExternalAuthenticationMethods] = value2;
		}

		internal const string VDirName = "PushNotifications";

		internal const string FrontEndWebSiteName = "Default Web Site";

		internal const string BackEndWebSiteName = "Exchange Back End";

		private static readonly string MostDerivedClassName = "msExchPushNotificationsVirtualDirectory";
	}
}
