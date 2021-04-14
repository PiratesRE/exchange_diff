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
	public sealed class SyncOrganization : ADLegacyVersionableObject
	{
		internal override string MostDerivedObjectClass
		{
			get
			{
				return ExchangeConfigurationUnit.MostDerivedClass;
			}
		}

		internal override ADObjectSchema Schema
		{
			get
			{
				return SyncOrganization.schema;
			}
		}

		public new string Name
		{
			get
			{
				return base.Name;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Managed")]
		public bool DisableWindowsLiveID
		{
			get
			{
				return (bool)this[SyncOrganizationSchema.DisableWindowsLiveID];
			}
			set
			{
				this[SyncOrganizationSchema.DisableWindowsLiveID] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Federated")]
		[ValidateNotNullOrEmpty]
		public string FederatedIdentitySourceADAttribute
		{
			get
			{
				return (string)this[SyncOrganizationSchema.FederatedIdentitySourceADAttribute];
			}
			set
			{
				this[SyncOrganizationSchema.FederatedIdentitySourceADAttribute] = value;
			}
		}

		[Parameter]
		public bool WlidUseSMTPPrimary
		{
			get
			{
				return (bool)this[SyncOrganizationSchema.WlidUseSMTPPrimary];
			}
			set
			{
				this[SyncOrganizationSchema.WlidUseSMTPPrimary] = value;
			}
		}

		[ValidateNotNullOrEmpty]
		[Parameter(Mandatory = false, ParameterSetName = "Managed")]
		public string PasswordFilePath
		{
			get
			{
				return (string)this[SyncOrganizationSchema.PasswordFilePath];
			}
			set
			{
				this[SyncOrganizationSchema.PasswordFilePath] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Managed")]
		public bool ResetPasswordOnNextLogon
		{
			get
			{
				return (bool)this[SyncOrganizationSchema.ResetPasswordOnNextLogon];
			}
			set
			{
				this[SyncOrganizationSchema.ResetPasswordOnNextLogon] = value;
			}
		}

		[ValidateNotNullOrEmpty]
		[Parameter]
		public SmtpDomainWithSubdomains ProvisioningDomain
		{
			get
			{
				return (SmtpDomainWithSubdomains)this[SyncOrganizationSchema.ProvisioningDomain];
			}
			set
			{
				this[SyncOrganizationSchema.ProvisioningDomain] = value;
			}
		}

		[Parameter]
		public EnterpriseExchangeVersionEnum EnterpriseExchangeVersion
		{
			get
			{
				return (EnterpriseExchangeVersionEnum)this[SyncOrganizationSchema.EnterpriseExchangeVersion];
			}
			set
			{
				this[SyncOrganizationSchema.EnterpriseExchangeVersion] = value;
			}
		}

		public bool FederatedTenant
		{
			get
			{
				return (bool)this[SyncOrganizationSchema.FederatedTenant];
			}
		}

		private static SyncOrganizationSchema schema = ObjectSchema.GetInstance<SyncOrganizationSchema>();
	}
}
