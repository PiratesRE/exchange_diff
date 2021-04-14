using System;
using System.Management.Automation;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class NewFederationTrustCommand : SyntheticCommandWithPipelineInput<FederationTrust, FederationTrust>
	{
		private NewFederationTrustCommand() : base("New-FederationTrust")
		{
		}

		public NewFederationTrustCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual NewFederationTrustCommand SetParameters(NewFederationTrustCommand.SkipNamespaceProviderProvisioningParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual NewFederationTrustCommand SetParameters(NewFederationTrustCommand.FederationTrustParameterParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual NewFederationTrustCommand SetParameters(NewFederationTrustCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class SkipNamespaceProviderProvisioningParameters : ParametersBase
		{
			public virtual string ApplicationIdentifier
			{
				set
				{
					base.PowerSharpParameters["ApplicationIdentifier"] = value;
				}
			}

			public virtual string AdministratorProvisioningId
			{
				set
				{
					base.PowerSharpParameters["AdministratorProvisioningId"] = value;
				}
			}

			public virtual SwitchParameter SkipNamespaceProviderProvisioning
			{
				set
				{
					base.PowerSharpParameters["SkipNamespaceProviderProvisioning"] = value;
				}
			}

			public virtual string ApplicationUri
			{
				set
				{
					base.PowerSharpParameters["ApplicationUri"] = value;
				}
			}

			public virtual string Thumbprint
			{
				set
				{
					base.PowerSharpParameters["Thumbprint"] = value;
				}
			}

			public virtual Uri MetadataUrl
			{
				set
				{
					base.PowerSharpParameters["MetadataUrl"] = value;
				}
			}

			public virtual string Name
			{
				set
				{
					base.PowerSharpParameters["Name"] = value;
				}
			}

			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
				}
			}

			public virtual SwitchParameter Verbose
			{
				set
				{
					base.PowerSharpParameters["Verbose"] = value;
				}
			}

			public virtual SwitchParameter Debug
			{
				set
				{
					base.PowerSharpParameters["Debug"] = value;
				}
			}

			public virtual ActionPreference ErrorAction
			{
				set
				{
					base.PowerSharpParameters["ErrorAction"] = value;
				}
			}

			public virtual ActionPreference WarningAction
			{
				set
				{
					base.PowerSharpParameters["WarningAction"] = value;
				}
			}

			public virtual SwitchParameter WhatIf
			{
				set
				{
					base.PowerSharpParameters["WhatIf"] = value;
				}
			}
		}

		public class FederationTrustParameterParameters : ParametersBase
		{
			public virtual string Thumbprint
			{
				set
				{
					base.PowerSharpParameters["Thumbprint"] = value;
				}
			}

			public virtual Uri MetadataUrl
			{
				set
				{
					base.PowerSharpParameters["MetadataUrl"] = value;
				}
			}

			public virtual SwitchParameter UseLegacyProvisioningService
			{
				set
				{
					base.PowerSharpParameters["UseLegacyProvisioningService"] = value;
				}
			}

			public virtual string Name
			{
				set
				{
					base.PowerSharpParameters["Name"] = value;
				}
			}

			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
				}
			}

			public virtual SwitchParameter Verbose
			{
				set
				{
					base.PowerSharpParameters["Verbose"] = value;
				}
			}

			public virtual SwitchParameter Debug
			{
				set
				{
					base.PowerSharpParameters["Debug"] = value;
				}
			}

			public virtual ActionPreference ErrorAction
			{
				set
				{
					base.PowerSharpParameters["ErrorAction"] = value;
				}
			}

			public virtual ActionPreference WarningAction
			{
				set
				{
					base.PowerSharpParameters["WarningAction"] = value;
				}
			}

			public virtual SwitchParameter WhatIf
			{
				set
				{
					base.PowerSharpParameters["WhatIf"] = value;
				}
			}
		}

		public class DefaultParameters : ParametersBase
		{
			public virtual string Name
			{
				set
				{
					base.PowerSharpParameters["Name"] = value;
				}
			}

			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
				}
			}

			public virtual SwitchParameter Verbose
			{
				set
				{
					base.PowerSharpParameters["Verbose"] = value;
				}
			}

			public virtual SwitchParameter Debug
			{
				set
				{
					base.PowerSharpParameters["Debug"] = value;
				}
			}

			public virtual ActionPreference ErrorAction
			{
				set
				{
					base.PowerSharpParameters["ErrorAction"] = value;
				}
			}

			public virtual ActionPreference WarningAction
			{
				set
				{
					base.PowerSharpParameters["WarningAction"] = value;
				}
			}

			public virtual SwitchParameter WhatIf
			{
				set
				{
					base.PowerSharpParameters["WhatIf"] = value;
				}
			}
		}
	}
}
