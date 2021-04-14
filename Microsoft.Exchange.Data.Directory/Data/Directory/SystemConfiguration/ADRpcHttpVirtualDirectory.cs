using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Serializable]
	public class ADRpcHttpVirtualDirectory : ExchangeVirtualDirectory
	{
		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2012;
			}
		}

		internal override ADObjectSchema Schema
		{
			get
			{
				return ADRpcHttpVirtualDirectory.schema;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return ADRpcHttpVirtualDirectory.MostDerivedClass;
			}
		}

		internal override QueryFilter ImplicitFilter
		{
			get
			{
				return new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.ObjectCategory, this.MostDerivedObjectClass);
			}
		}

		public string ServerName
		{
			get
			{
				return (string)this[ADVirtualDirectorySchema.ServerName];
			}
		}

		public bool SSLOffloading
		{
			get
			{
				return (bool)this[ADRpcHttpVirtualDirectorySchema.SSLOffloading];
			}
			set
			{
				this[ADRpcHttpVirtualDirectorySchema.SSLOffloading] = value;
			}
		}

		public Hostname ExternalHostname
		{
			get
			{
				return (Hostname)this[ADRpcHttpVirtualDirectorySchema.ExternalHostname];
			}
			set
			{
				this[ADRpcHttpVirtualDirectorySchema.ExternalHostname] = value;
			}
		}

		public Hostname InternalHostname
		{
			get
			{
				return (Hostname)this[ADRpcHttpVirtualDirectorySchema.InternalHostname];
			}
			set
			{
				this[ADRpcHttpVirtualDirectorySchema.InternalHostname] = value;
			}
		}

		public AuthenticationMethod ExternalClientAuthenticationMethod
		{
			get
			{
				return (AuthenticationMethod)this[ADRpcHttpVirtualDirectorySchema.ExternalClientAuthenticationMethod];
			}
			set
			{
				this[ADRpcHttpVirtualDirectorySchema.ExternalClientAuthenticationMethod] = value;
			}
		}

		public AuthenticationMethod InternalClientAuthenticationMethod
		{
			get
			{
				return (AuthenticationMethod)this[ADRpcHttpVirtualDirectorySchema.InternalClientAuthenticationMethod];
			}
			set
			{
				this[ADRpcHttpVirtualDirectorySchema.InternalClientAuthenticationMethod] = value;
			}
		}

		public MultiValuedProperty<AuthenticationMethod> IISAuthenticationMethods
		{
			get
			{
				return (MultiValuedProperty<AuthenticationMethod>)this[ADRpcHttpVirtualDirectorySchema.IISAuthenticationMethods];
			}
			set
			{
				this[ADRpcHttpVirtualDirectorySchema.IISAuthenticationMethods] = value;
			}
		}

		public Uri XropUrl
		{
			get
			{
				MultiValuedProperty<Uri> multiValuedProperty = (MultiValuedProperty<Uri>)this[ADRpcHttpVirtualDirectorySchema.XropUrl];
				if (multiValuedProperty != null && multiValuedProperty.Count != 0)
				{
					return multiValuedProperty[0];
				}
				return null;
			}
			set
			{
				this[ADRpcHttpVirtualDirectorySchema.XropUrl] = new MultiValuedProperty<Uri>(value);
			}
		}

		public bool ExternalClientsRequireSsl
		{
			get
			{
				return (bool)this[ADRpcHttpVirtualDirectorySchema.ExternalClientsRequireSsl];
			}
			set
			{
				this[ADRpcHttpVirtualDirectorySchema.ExternalClientsRequireSsl] = value;
			}
		}

		public bool InternalClientsRequireSsl
		{
			get
			{
				return (bool)this[ADRpcHttpVirtualDirectorySchema.InternalClientsRequireSsl];
			}
			set
			{
				this[ADRpcHttpVirtualDirectorySchema.InternalClientsRequireSsl] = value;
			}
		}

		internal static object GetClientsRequireSsl(IPropertyBag propertyBag, ADPropertyDefinition adPropertyDefinition)
		{
			Uri uri = (Uri)propertyBag[adPropertyDefinition];
			return uri != null && uri.Scheme == Uri.UriSchemeHttps;
		}

		internal static object GetExternalClientsRequireSsl(IPropertyBag propertyBag)
		{
			return ADRpcHttpVirtualDirectory.GetClientsRequireSsl(propertyBag, ADVirtualDirectorySchema.ExternalUrl);
		}

		internal static object GetInternalClientsRequireSsl(IPropertyBag propertyBag)
		{
			return ADRpcHttpVirtualDirectory.GetClientsRequireSsl(propertyBag, ADVirtualDirectorySchema.InternalUrl);
		}

		internal static Uri CreateRpcUri(string uriScheme, string hostNameText)
		{
			return new Uri(uriScheme + "://" + hostNameText + "/rpc");
		}

		internal static void SetClientsRequireSsl(object value, IPropertyBag propertyBag, ADPropertyDefinition adPropertyDefinition)
		{
			Uri uri = (Uri)propertyBag[adPropertyDefinition];
			if (uri == null)
			{
				return;
			}
			Hostname hostname = null;
			if (Hostname.TryParse(uri.DnsSafeHost, out hostname))
			{
				propertyBag[adPropertyDefinition] = ADRpcHttpVirtualDirectory.CreateRpcUri(((bool)value) ? Uri.UriSchemeHttps : Uri.UriSchemeHttp, hostname.ToString());
			}
		}

		internal static void SetExternalClientsRequireSsl(object value, IPropertyBag propertyBag)
		{
			ADRpcHttpVirtualDirectory.SetClientsRequireSsl(value, propertyBag, ADVirtualDirectorySchema.ExternalUrl);
		}

		internal static void SetInternalClientsRequireSsl(object value, IPropertyBag propertyBag)
		{
			ADRpcHttpVirtualDirectory.SetClientsRequireSsl(value, propertyBag, ADVirtualDirectorySchema.InternalUrl);
		}

		internal static void SetHostname(object value, IPropertyBag propertyBag, ADPropertyDefinition adPropertyDefinition)
		{
			if (value != null)
			{
				propertyBag[adPropertyDefinition] = ADRpcHttpVirtualDirectory.CreateRpcUri(((bool)ADRpcHttpVirtualDirectory.GetClientsRequireSsl(propertyBag, adPropertyDefinition)) ? Uri.UriSchemeHttps : Uri.UriSchemeHttp, value.ToString());
				return;
			}
			propertyBag[adPropertyDefinition] = null;
		}

		internal static void SetExternalHostname(object value, IPropertyBag propertyBag)
		{
			ADRpcHttpVirtualDirectory.SetHostname(value, propertyBag, ADVirtualDirectorySchema.ExternalUrl);
		}

		internal static void SetInternalHostname(object value, IPropertyBag propertyBag)
		{
			ADRpcHttpVirtualDirectory.SetHostname(value, propertyBag, ADVirtualDirectorySchema.InternalUrl);
		}

		internal static object GetHostname(IPropertyBag propertyBag, ADPropertyDefinition adPropertyDefinition)
		{
			Uri uri = (Uri)propertyBag[adPropertyDefinition];
			Hostname result;
			if (uri != null && Hostname.TryParse(uri.DnsSafeHost, out result))
			{
				return result;
			}
			return null;
		}

		internal static object GetExternalHostname(IPropertyBag propertyBag)
		{
			return ADRpcHttpVirtualDirectory.GetHostname(propertyBag, ADVirtualDirectorySchema.ExternalUrl);
		}

		internal static object GetInternalHostname(IPropertyBag propertyBag)
		{
			return ADRpcHttpVirtualDirectory.GetHostname(propertyBag, ADVirtualDirectorySchema.InternalUrl);
		}

		internal static void SetClientAuthenticationMethod(object value, IPropertyBag propertyBag, ADPropertyDefinition adPropertyDefinition)
		{
			AuthenticationMethod authMethod = (AuthenticationMethod)value;
			AuthenticationMethodFlags authenticationMethodFlags = ADRpcHttpVirtualDirectory.ClientAuthenticationMethodToFlags(authMethod);
			propertyBag[adPropertyDefinition] = authenticationMethodFlags;
		}

		internal static AuthenticationMethodFlags ClientAuthenticationMethodToFlags(AuthenticationMethod authMethod)
		{
			switch (authMethod)
			{
			case AuthenticationMethod.Basic:
				return AuthenticationMethodFlags.Basic;
			case AuthenticationMethod.Digest:
				break;
			case AuthenticationMethod.Ntlm:
				return AuthenticationMethodFlags.Ntlm;
			default:
				if (authMethod == AuthenticationMethod.NegoEx)
				{
					return AuthenticationMethodFlags.NegoEx;
				}
				if (authMethod == AuthenticationMethod.Negotiate)
				{
					return AuthenticationMethodFlags.Negotiate;
				}
				break;
			}
			return AuthenticationMethodFlags.None;
		}

		internal static void SetExternalClientAuthenticationMethod(object value, IPropertyBag propertyBag)
		{
			ADRpcHttpVirtualDirectory.SetClientAuthenticationMethod(value, propertyBag, ADVirtualDirectorySchema.ExternalAuthenticationMethodFlags);
		}

		internal static object GetClientAuthenticationMethod(IPropertyBag propertyBag, ADPropertyDefinition adPropertyDefinition)
		{
			AuthenticationMethodFlags authenticationMethodFlags = (AuthenticationMethodFlags)propertyBag[adPropertyDefinition];
			AuthenticationMethod authenticationMethod = ADRpcHttpVirtualDirectory.ClientAuthenticationMethodFromFlags(authenticationMethodFlags);
			return authenticationMethod;
		}

		internal static AuthenticationMethod ClientAuthenticationMethodFromFlags(AuthenticationMethodFlags authenticationMethodFlags)
		{
			switch (authenticationMethodFlags)
			{
			case AuthenticationMethodFlags.Basic:
				return AuthenticationMethod.Basic;
			case AuthenticationMethodFlags.Ntlm:
				return AuthenticationMethod.Ntlm;
			default:
				if (authenticationMethodFlags == AuthenticationMethodFlags.NegoEx)
				{
					return AuthenticationMethod.NegoEx;
				}
				if (authenticationMethodFlags != AuthenticationMethodFlags.Negotiate)
				{
					return AuthenticationMethod.Misconfigured;
				}
				return AuthenticationMethod.Negotiate;
			}
		}

		internal static object GetExternalClientAuthenticationMethod(IPropertyBag propertyBag)
		{
			return ADRpcHttpVirtualDirectory.GetClientAuthenticationMethod(propertyBag, ADVirtualDirectorySchema.ExternalAuthenticationMethodFlags);
		}

		internal static object GetIISAuthenticationMethods(IPropertyBag propertyBag)
		{
			AuthenticationMethodFlags authenticationMethodFlags = (AuthenticationMethodFlags)propertyBag[ADVirtualDirectorySchema.InternalAuthenticationMethodFlags];
			if (authenticationMethodFlags == AuthenticationMethodFlags.None)
			{
				authenticationMethodFlags = (AuthenticationMethodFlags.Basic | AuthenticationMethodFlags.Ntlm | AuthenticationMethodFlags.Negotiate);
			}
			return ADVirtualDirectory.AuthenticationMethodFlagsToAuthenticationMethodPropertyValue(authenticationMethodFlags);
		}

		private new Uri InternalUrl
		{
			set
			{
			}
		}

		private new Uri ExternalUrl
		{
			set
			{
			}
		}

		private new MultiValuedProperty<AuthenticationMethod> InternalAuthenticationMethods
		{
			set
			{
			}
		}

		private new MultiValuedProperty<AuthenticationMethod> ExternalAuthenticationMethods
		{
			set
			{
			}
		}

		private static readonly ADRpcHttpVirtualDirectorySchema schema = ObjectSchema.GetInstance<ADRpcHttpVirtualDirectorySchema>();

		public static readonly string MostDerivedClass = "msExchRpcHttpVirtualDirectory";
	}
}
