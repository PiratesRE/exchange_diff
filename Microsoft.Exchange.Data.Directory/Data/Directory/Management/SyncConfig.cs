using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Data.Directory.Management
{
	[Serializable]
	public sealed class SyncConfig : ADPresentationObject
	{
		internal override ADPresentationSchema PresentationSchema
		{
			get
			{
				return SyncConfig.schema;
			}
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2010;
			}
		}

		public SyncConfig(SyncOrganization cu, SmtpDomain federatedNamespace, SmtpDomainWithSubdomains provisioningDomain) : base(cu)
		{
			if (cu == null)
			{
				throw new ArgumentNullException("The value of parameter cu is null!");
			}
			if (cu.FederatedTenant && federatedNamespace == null)
			{
				throw new ArgumentNullException("The value of parameter federatedNamespace should not be null for a federated organization!");
			}
			if (cu.ProvisioningDomain == null && provisioningDomain == null)
			{
				throw new ArgumentNullException("The value of parameter provisioningDomain is null!");
			}
			if (cu.FederatedTenant)
			{
				this.FederatedNamespace = federatedNamespace;
				if (string.IsNullOrEmpty(this.FederatedIdentitySourceADAttribute))
				{
					this[SyncConfigSchema.FederatedIdentitySourceADAttribute] = "objectGuid";
				}
			}
			else if (string.IsNullOrEmpty(this.PasswordFilePath))
			{
				this[SyncConfigSchema.PasswordFilePath] = "password.xml";
			}
			if (cu.ProvisioningDomain == null)
			{
				this[SyncConfigSchema.ProvisioningDomain] = provisioningDomain;
			}
		}

		public bool FederatedTenant
		{
			get
			{
				return (bool)this[SyncConfigSchema.FederatedTenant];
			}
		}

		public bool? DisableWindowsLiveID
		{
			get
			{
				if (!this.FederatedTenant)
				{
					return (bool?)this[SyncConfigSchema.DisableWindowsLiveID];
				}
				return null;
			}
		}

		public string FederatedIdentitySourceADAttribute
		{
			get
			{
				return (string)this[SyncConfigSchema.FederatedIdentitySourceADAttribute];
			}
		}

		public bool WlidUseSMTPPrimary
		{
			get
			{
				return (bool)this[SyncConfigSchema.WlidUseSMTPPrimary];
			}
		}

		public string PasswordFilePath
		{
			get
			{
				return (string)this[SyncConfigSchema.PasswordFilePath];
			}
		}

		public SmtpDomain FederatedNamespace { get; internal set; }

		public bool? ResetPasswordOnNextLogon
		{
			get
			{
				if (!this.FederatedTenant)
				{
					return (bool?)this[SyncConfigSchema.ResetPasswordOnNextLogon];
				}
				return null;
			}
		}

		public SmtpDomainWithSubdomains ProvisioningDomain
		{
			get
			{
				return (SmtpDomainWithSubdomains)this[SyncConfigSchema.ProvisioningDomain];
			}
		}

		public EnterpriseExchangeVersionEnum EnterpriseExchangeVersion
		{
			get
			{
				return (EnterpriseExchangeVersionEnum)this[SyncConfigSchema.EnterpriseExchangeVersion];
			}
		}

		internal const string DefaultFederatedIdentitySourceADAttribute = "objectGuid";

		internal const string DefaultPasswordFile = "password.xml";

		private static SyncConfigSchema schema = ObjectSchema.GetInstance<SyncConfigSchema>();
	}
}
