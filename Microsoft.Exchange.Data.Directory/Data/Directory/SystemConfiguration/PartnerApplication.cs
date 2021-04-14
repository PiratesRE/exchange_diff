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
	public sealed class PartnerApplication : ADConfigurationObject
	{
		internal override ADObjectSchema Schema
		{
			get
			{
				return PartnerApplication.SchemaObject;
			}
		}

		internal override string MostDerivedObjectClass
		{
			get
			{
				return PartnerApplication.ObjectClassName;
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2012;
			}
		}

		[Parameter]
		public bool Enabled
		{
			get
			{
				return (bool)this[PartnerApplicationSchema.Enabled];
			}
			set
			{
				this[PartnerApplicationSchema.Enabled] = value;
			}
		}

		public string ApplicationIdentifier
		{
			get
			{
				return (string)this[PartnerApplicationSchema.ApplicationIdentifier];
			}
			set
			{
				this[PartnerApplicationSchema.ApplicationIdentifier] = value;
			}
		}

		internal MultiValuedProperty<byte[]> CertificateBytes
		{
			get
			{
				return (MultiValuedProperty<byte[]>)this[PartnerApplicationSchema.CertificateDataRaw];
			}
			set
			{
				this[PartnerApplicationSchema.CertificateDataRaw] = value;
			}
		}

		public MultiValuedProperty<string> CertificateStrings
		{
			get
			{
				return (MultiValuedProperty<string>)this[PartnerApplicationSchema.CertificateDataString];
			}
		}

		public string AuthMetadataUrl
		{
			get
			{
				return (string)this[PartnerApplicationSchema.AuthMetadataUrl];
			}
			set
			{
				this[PartnerApplicationSchema.AuthMetadataUrl] = value;
			}
		}

		public string Realm
		{
			get
			{
				return (string)this[PartnerApplicationSchema.Realm];
			}
			set
			{
				this[PartnerApplicationSchema.Realm] = value;
			}
		}

		public bool UseAuthServer
		{
			get
			{
				return (bool)this[PartnerApplicationSchema.UseAuthServer];
			}
			set
			{
				this[PartnerApplicationSchema.UseAuthServer] = value;
			}
		}

		[Parameter]
		public bool AcceptSecurityIdentifierInformation
		{
			get
			{
				return (bool)this[PartnerApplicationSchema.AcceptSecurityIdentifierInformation];
			}
			set
			{
				this[PartnerApplicationSchema.AcceptSecurityIdentifierInformation] = value;
			}
		}

		public ADObjectId LinkedAccount
		{
			get
			{
				return (ADObjectId)this[PartnerApplicationSchema.LinkedAccount];
			}
			set
			{
				this[PartnerApplicationSchema.LinkedAccount] = value;
			}
		}

		public string IssuerIdentifier
		{
			get
			{
				return (string)this[PartnerApplicationSchema.IssuerIdentifier];
			}
			set
			{
				this[PartnerApplicationSchema.IssuerIdentifier] = value;
			}
		}

		public string[] AppOnlyPermissions
		{
			get
			{
				return (string[])this[PartnerApplicationSchema.AppOnlyPermissions];
			}
			set
			{
				this[PartnerApplicationSchema.AppOnlyPermissions] = value;
			}
		}

		public string[] ActAsPermissions
		{
			get
			{
				return (string[])this[PartnerApplicationSchema.ActAsPermissions];
			}
			set
			{
				this[PartnerApplicationSchema.ActAsPermissions] = value;
			}
		}

		internal static ADObjectId GetContainerId(IConfigurationSession configSession)
		{
			return configSession.GetOrgContainerId().GetChildId(AuthConfig.ContainerName).GetChildId(PartnerApplication.ParentContainerName);
		}

		internal static readonly string ParentContainerName = "Partner Applications";

		internal static readonly string ObjectClassName = "msExchAuthPartnerApplication";

		private static readonly PartnerApplicationSchema SchemaObject = ObjectSchema.GetInstance<PartnerApplicationSchema>();
	}
}
