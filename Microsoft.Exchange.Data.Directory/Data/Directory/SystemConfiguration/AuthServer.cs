using System;
using System.Management.Automation;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[ObjectScope(new ConfigScopes[]
	{
		ConfigScopes.TenantLocal,
		ConfigScopes.TenantSubTree
	})]
	[Serializable]
	public sealed class AuthServer : ADConfigurationObject
	{
		internal override ADObjectSchema Schema
		{
			get
			{
				return AuthServer.SchemaObject;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return AuthServer.ObjectClassName;
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2012;
			}
		}

		public string IssuerIdentifier
		{
			get
			{
				return (string)this[AuthServerSchema.IssuerIdentifier];
			}
			set
			{
				this[AuthServerSchema.IssuerIdentifier] = value;
			}
		}

		internal MultiValuedProperty<byte[]> CertificateBytes
		{
			get
			{
				return (MultiValuedProperty<byte[]>)this[AuthServerSchema.CertificateDataRaw];
			}
			set
			{
				this[AuthServerSchema.CertificateDataRaw] = value;
			}
		}

		public MultiValuedProperty<string> CertificateStrings
		{
			get
			{
				return (MultiValuedProperty<string>)this[AuthServerSchema.CertificateDataString];
			}
		}

		public string CurrentEncryptedAppSecret
		{
			get
			{
				return (string)this[AuthServerSchema.CurrentEncryptedAppSecret];
			}
			set
			{
				this[AuthServerSchema.CurrentEncryptedAppSecret] = value;
			}
		}

		public string PreviousEncryptedAppSecret
		{
			get
			{
				return (string)this[AuthServerSchema.PreviousEncryptedAppSecret];
			}
			set
			{
				this[AuthServerSchema.PreviousEncryptedAppSecret] = value;
			}
		}

		public string TokenIssuingEndpoint
		{
			get
			{
				return (string)this[AuthServerSchema.TokenIssuingEndpoint];
			}
			set
			{
				this[AuthServerSchema.TokenIssuingEndpoint] = value;
			}
		}

		public string AuthorizationEndpoint
		{
			get
			{
				return (string)this[AuthServerSchema.AuthorizationEndpoint];
			}
			set
			{
				this[AuthServerSchema.AuthorizationEndpoint] = value;
			}
		}

		public string ApplicationIdentifier
		{
			get
			{
				return (string)this[AuthServerSchema.ApplicationIdentifier];
			}
			set
			{
				this[AuthServerSchema.ApplicationIdentifier] = value;
			}
		}

		public string AuthMetadataUrl
		{
			get
			{
				return (string)this[AuthServerSchema.AuthMetadataUrl];
			}
			set
			{
				this[AuthServerSchema.AuthMetadataUrl] = value;
			}
		}

		public string Realm
		{
			get
			{
				return (string)this[AuthServerSchema.Realm];
			}
			set
			{
				this[AuthServerSchema.Realm] = value;
			}
		}

		public AuthServerType Type
		{
			get
			{
				return (AuthServerType)this[AuthServerSchema.Type];
			}
			set
			{
				this[AuthServerSchema.Type] = value;
			}
		}

		[Parameter]
		public bool Enabled
		{
			get
			{
				return (bool)this[AuthServerSchema.Enabled];
			}
			set
			{
				this[AuthServerSchema.Enabled] = value;
			}
		}

		public bool IsDefaultAuthorizationEndpoint
		{
			get
			{
				return (bool)this[AuthServerSchema.IsDefaultAuthorizationEndpoint];
			}
			set
			{
				this[AuthServerSchema.IsDefaultAuthorizationEndpoint] = value;
			}
		}

		internal static ADObjectId GetContainerId(IConfigurationSession configSession)
		{
			return configSession.GetOrgContainerId().GetChildId(AuthConfig.ContainerName).GetChildId(AuthServer.ParentContainerName);
		}

		internal static readonly string ParentContainerName = "Auth Servers";

		internal static readonly string ObjectClassName = "msExchAuthAuthServer";

		private static readonly AuthServerSchema SchemaObject = ObjectSchema.GetInstance<AuthServerSchema>();
	}
}
