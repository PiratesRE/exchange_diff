using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[ObjectScope(new ConfigScopes[]
	{
		ConfigScopes.TenantLocal,
		ConfigScopes.TenantSubTree
	})]
	[Serializable]
	public sealed class AuthConfig : ADConfigurationObject
	{
		public string CurrentCertificateThumbprint
		{
			get
			{
				return (string)this[AuthConfigSchema.CurrentCertificateThumbprint];
			}
			set
			{
				this[AuthConfigSchema.CurrentCertificateThumbprint] = value;
			}
		}

		public string PreviousCertificateThumbprint
		{
			get
			{
				return (string)this[AuthConfigSchema.PreviousCertificateThumbprint];
			}
			set
			{
				this[AuthConfigSchema.PreviousCertificateThumbprint] = value;
			}
		}

		public string NextCertificateThumbprint
		{
			get
			{
				return (string)this[AuthConfigSchema.NextCertificateThumbprint];
			}
			set
			{
				this[AuthConfigSchema.NextCertificateThumbprint] = value;
			}
		}

		public DateTime? NextCertificateEffectiveDate
		{
			get
			{
				return (DateTime?)this[AuthConfigSchema.NextCertificateEffectiveDate];
			}
			set
			{
				this[AuthConfigSchema.NextCertificateEffectiveDate] = value;
			}
		}

		public string ServiceName
		{
			get
			{
				return (string)this[AuthConfigSchema.ServiceName];
			}
			set
			{
				this[AuthConfigSchema.ServiceName] = value;
			}
		}

		public string Realm
		{
			get
			{
				return (string)this[AuthConfigSchema.Realm];
			}
			set
			{
				this[AuthConfigSchema.Realm] = value;
			}
		}

		internal override ADObjectSchema Schema
		{
			get
			{
				return AuthConfig.SchemaObject;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return AuthConfig.ObjectClassName;
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2012;
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

		internal static ADObjectId GetContainerId(IConfigurationSession configSession)
		{
			if (configSession == null)
			{
				throw new ArgumentNullException("configSession");
			}
			return configSession.GetOrgContainerId().GetChildId(AuthConfig.ContainerName);
		}

		internal static AuthConfig Read(IConfigurationSession configSession)
		{
			if (configSession == null)
			{
				throw new ArgumentNullException("configSession");
			}
			return configSession.Read<AuthConfig>(AuthConfig.GetContainerId(configSession));
		}

		public static readonly string DefaultServiceNameValue = WellknownPartnerApplicationIdentifiers.Exchange;

		internal static readonly string ContainerName = "Auth Configuration";

		internal static readonly string ObjectClassName = "msExchAuthAuthConfig";

		private static readonly AuthConfigSchema SchemaObject = ObjectSchema.GetInstance<AuthConfigSchema>();
	}
}
