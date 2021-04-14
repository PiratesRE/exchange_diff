using System;
using System.Collections;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Serializable]
	public class ExchangeWebAppVirtualDirectory : ExchangeVirtualDirectory
	{
		internal override ADObjectSchema Schema
		{
			get
			{
				return ExchangeWebAppVirtualDirectory.schema;
			}
		}

		public new string Name
		{
			get
			{
				return base.Name;
			}
			internal set
			{
				base.Name = value;
			}
		}

		public new MultiValuedProperty<AuthenticationMethod> InternalAuthenticationMethods
		{
			get
			{
				return base.InternalAuthenticationMethods;
			}
		}

		public new string MetabasePath
		{
			get
			{
				return base.MetabasePath;
			}
		}

		public bool BasicAuthentication
		{
			get
			{
				return (bool)this[ExchangeWebAppVirtualDirectorySchema.BasicAuthentication];
			}
			set
			{
				this[ExchangeWebAppVirtualDirectorySchema.BasicAuthentication] = value;
				ExchangeWebAppVirtualDirectory.SetAuthenticationMethodHelper(value, this, AuthenticationMethod.Basic);
			}
		}

		public bool WindowsAuthentication
		{
			get
			{
				return (bool)this[ExchangeWebAppVirtualDirectorySchema.WindowsAuthentication];
			}
			set
			{
				this[ExchangeWebAppVirtualDirectorySchema.WindowsAuthentication] = value;
				ExchangeWebAppVirtualDirectory.SetAuthenticationMethodHelper(value, this, AuthenticationMethod.WindowsIntegrated);
			}
		}

		public bool DigestAuthentication
		{
			get
			{
				return (bool)this[ExchangeWebAppVirtualDirectorySchema.DigestAuthentication];
			}
			set
			{
				this[ExchangeWebAppVirtualDirectorySchema.DigestAuthentication] = value;
				ExchangeWebAppVirtualDirectory.SetAuthenticationMethodHelper(value, this, AuthenticationMethod.Digest);
			}
		}

		public bool FormsAuthentication
		{
			get
			{
				return (bool)this[ExchangeWebAppVirtualDirectorySchema.FormsAuthentication];
			}
			set
			{
				this[ExchangeWebAppVirtualDirectorySchema.FormsAuthentication] = value;
				ExchangeWebAppVirtualDirectory.SetAuthenticationMethodHelper(value, this, AuthenticationMethod.Fba);
			}
		}

		public bool LiveIdAuthentication
		{
			get
			{
				return (bool)this[ExchangeWebAppVirtualDirectorySchema.LiveIdAuthentication];
			}
			set
			{
				this[ExchangeWebAppVirtualDirectorySchema.LiveIdAuthentication] = value;
				ExchangeWebAppVirtualDirectory.SetAuthenticationMethodHelper(value, this, AuthenticationMethod.LiveIdFba);
			}
		}

		public bool AdfsAuthentication
		{
			get
			{
				return (bool)this[ExchangeWebAppVirtualDirectorySchema.AdfsAuthentication];
			}
			set
			{
				this[ExchangeWebAppVirtualDirectorySchema.AdfsAuthentication] = value;
				ExchangeWebAppVirtualDirectory.SetAuthenticationMethodHelper(value, this, AuthenticationMethod.Adfs);
			}
		}

		public bool OAuthAuthentication
		{
			get
			{
				return (bool)this[ExchangeWebAppVirtualDirectorySchema.OAuthAuthentication];
			}
			set
			{
				this[ExchangeWebAppVirtualDirectorySchema.OAuthAuthentication] = value;
				ExchangeWebAppVirtualDirectory.SetAuthenticationMethodHelper(value, this, AuthenticationMethod.OAuth);
			}
		}

		public string DefaultDomain
		{
			get
			{
				return (string)this[ExchangeWebAppVirtualDirectorySchema.DefaultDomain];
			}
			set
			{
				this[ExchangeWebAppVirtualDirectorySchema.DefaultDomain] = value;
			}
		}

		public GzipLevel GzipLevel
		{
			get
			{
				return (GzipLevel)this[ExchangeWebAppVirtualDirectorySchema.ADGzipLevel];
			}
			set
			{
				this[ExchangeWebAppVirtualDirectorySchema.ADGzipLevel] = value;
			}
		}

		public string WebSite
		{
			get
			{
				return (string)this[ExchangeWebAppVirtualDirectorySchema.WebSite];
			}
			internal set
			{
				this[ExchangeWebAppVirtualDirectorySchema.WebSite] = value;
			}
		}

		public string DisplayName
		{
			get
			{
				return (string)this[ExchangeWebAppVirtualDirectorySchema.DisplayName];
			}
			internal set
			{
				this[ExchangeWebAppVirtualDirectorySchema.DisplayName] = value;
			}
		}

		internal static object LiveIdAuthenticationGetter(IPropertyBag propertyBag)
		{
			return ExchangeWebAppVirtualDirectory.GetAuthenticationMethodHelper(propertyBag, AuthenticationMethod.LiveIdFba);
		}

		internal static void LiveIdAuthenticationSetter(object value, IPropertyBag propertyBag)
		{
			ExchangeWebAppVirtualDirectory.SetAuthenticationMethodHelper((bool)value, propertyBag, AuthenticationMethod.LiveIdFba);
		}

		internal static object AdfsAuthenticationGetter(IPropertyBag propertyBag)
		{
			return ExchangeWebAppVirtualDirectory.GetAuthenticationMethodHelper(propertyBag, AuthenticationMethod.Adfs);
		}

		internal static void AdfsAuthenticationSetter(object value, IPropertyBag propertyBag)
		{
			ExchangeWebAppVirtualDirectory.SetAuthenticationMethodHelper((bool)value, propertyBag, AuthenticationMethod.Adfs);
		}

		internal static object OAuthAuthenticationGetter(IPropertyBag propertyBag)
		{
			return ExchangeWebAppVirtualDirectory.GetAuthenticationMethodHelper(propertyBag, AuthenticationMethod.OAuth);
		}

		internal static void OAuthAuthenticationSetter(object value, IPropertyBag propertyBag)
		{
			ExchangeWebAppVirtualDirectory.SetAuthenticationMethodHelper((bool)value, propertyBag, AuthenticationMethod.OAuth);
		}

		internal static void SetAuthenticationMethodHelper(bool value, IPropertyBag propertyBag, AuthenticationMethod method)
		{
			bool flag = ExchangeWebAppVirtualDirectory.GetAuthenticationMethodHelper(propertyBag, AuthenticationMethod.Basic);
			bool flag2 = ExchangeWebAppVirtualDirectory.GetAuthenticationMethodHelper(propertyBag, AuthenticationMethod.Digest);
			bool flag3 = ExchangeWebAppVirtualDirectory.GetAuthenticationMethodHelper(propertyBag, AuthenticationMethod.WindowsIntegrated);
			bool flag4 = ExchangeWebAppVirtualDirectory.GetAuthenticationMethodHelper(propertyBag, AuthenticationMethod.Fba);
			bool flag5 = ExchangeWebAppVirtualDirectory.GetAuthenticationMethodHelper(propertyBag, AuthenticationMethod.LiveIdFba);
			bool flag6 = ExchangeWebAppVirtualDirectory.GetAuthenticationMethodHelper(propertyBag, AuthenticationMethod.Adfs);
			bool flag7 = ExchangeWebAppVirtualDirectory.GetAuthenticationMethodHelper(propertyBag, AuthenticationMethod.OAuth);
			switch (method)
			{
			case AuthenticationMethod.Basic:
				flag = value;
				if (!flag)
				{
					flag4 = false;
				}
				else
				{
					flag5 = false;
				}
				break;
			case AuthenticationMethod.Digest:
				flag2 = value;
				if (flag2)
				{
					flag4 = false;
					flag5 = false;
				}
				break;
			case AuthenticationMethod.Fba:
				flag4 = value;
				if (!flag4)
				{
					flag = false;
				}
				else
				{
					flag = true;
					flag2 = false;
					flag3 = false;
					flag5 = false;
				}
				break;
			case AuthenticationMethod.WindowsIntegrated:
				flag3 = value;
				if (flag3)
				{
					flag4 = false;
					flag5 = false;
				}
				break;
			case AuthenticationMethod.LiveIdFba:
				flag5 = value;
				if (flag5)
				{
					flag = false;
					flag2 = false;
					flag3 = false;
					flag4 = false;
					flag6 = false;
				}
				break;
			case AuthenticationMethod.OAuth:
				flag7 = value;
				break;
			case AuthenticationMethod.Adfs:
				flag6 = value;
				if (flag6)
				{
					flag5 = false;
				}
				break;
			}
			propertyBag[ExchangeWebAppVirtualDirectorySchema.WindowsAuthentication] = flag3;
			propertyBag[ExchangeWebAppVirtualDirectorySchema.BasicAuthentication] = flag;
			propertyBag[ExchangeWebAppVirtualDirectorySchema.DigestAuthentication] = flag2;
			propertyBag[ExchangeWebAppVirtualDirectorySchema.FormsAuthentication] = flag4;
			ArrayList arrayList = new ArrayList();
			if (flag)
			{
				arrayList.Add(AuthenticationMethod.Basic);
			}
			if (flag2)
			{
				arrayList.Add(AuthenticationMethod.Digest);
			}
			if (flag3)
			{
				arrayList.Add(AuthenticationMethod.WindowsIntegrated);
				arrayList.Add(AuthenticationMethod.Ntlm);
			}
			if (flag4)
			{
				arrayList.Add(AuthenticationMethod.Fba);
			}
			if (flag5)
			{
				arrayList.Add(AuthenticationMethod.LiveIdFba);
			}
			if (flag6)
			{
				arrayList.Add(AuthenticationMethod.Adfs);
			}
			if (flag7)
			{
				arrayList.Add(AuthenticationMethod.OAuth);
			}
			MultiValuedProperty<AuthenticationMethod> value2 = new MultiValuedProperty<AuthenticationMethod>(arrayList);
			propertyBag[ADVirtualDirectorySchema.InternalAuthenticationMethods] = value2;
		}

		internal static bool GetAuthenticationMethodHelper(IPropertyBag propertyBag, AuthenticationMethod method)
		{
			MultiValuedProperty<AuthenticationMethod> multiValuedProperty = (MultiValuedProperty<AuthenticationMethod>)propertyBag[ADVirtualDirectorySchema.InternalAuthenticationMethods];
			return multiValuedProperty.Contains(method);
		}

		private static readonly ExchangeWebAppVirtualDirectorySchema schema = ObjectSchema.GetInstance<ExchangeWebAppVirtualDirectorySchema>();
	}
}
