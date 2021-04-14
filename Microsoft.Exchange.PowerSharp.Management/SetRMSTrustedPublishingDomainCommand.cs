using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class SetRMSTrustedPublishingDomainCommand : SyntheticCommandWithPipelineInputNoOutput<RMSTrustedPublishingDomain>
	{
		private SetRMSTrustedPublishingDomainCommand() : base("Set-RMSTrustedPublishingDomain")
		{
		}

		public SetRMSTrustedPublishingDomainCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual SetRMSTrustedPublishingDomainCommand SetParameters(SetRMSTrustedPublishingDomainCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual SetRMSTrustedPublishingDomainCommand SetParameters(SetRMSTrustedPublishingDomainCommand.IdentityParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class DefaultParameters : ParametersBase
		{
			public virtual Uri IntranetLicensingUrl
			{
				set
				{
					base.PowerSharpParameters["IntranetLicensingUrl"] = value;
				}
			}

			public virtual Uri ExtranetLicensingUrl
			{
				set
				{
					base.PowerSharpParameters["ExtranetLicensingUrl"] = value;
				}
			}

			public virtual Uri IntranetCertificationUrl
			{
				set
				{
					base.PowerSharpParameters["IntranetCertificationUrl"] = value;
				}
			}

			public virtual Uri ExtranetCertificationUrl
			{
				set
				{
					base.PowerSharpParameters["ExtranetCertificationUrl"] = value;
				}
			}

			public virtual SwitchParameter Default
			{
				set
				{
					base.PowerSharpParameters["Default"] = value;
				}
			}

			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
				}
			}

			public virtual string Name
			{
				set
				{
					base.PowerSharpParameters["Name"] = value;
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

		public class IdentityParameters : ParametersBase
		{
			public virtual string Identity
			{
				set
				{
					base.PowerSharpParameters["Identity"] = ((value != null) ? new RmsTrustedPublishingDomainIdParameter(value) : null);
				}
			}

			public virtual Uri IntranetLicensingUrl
			{
				set
				{
					base.PowerSharpParameters["IntranetLicensingUrl"] = value;
				}
			}

			public virtual Uri ExtranetLicensingUrl
			{
				set
				{
					base.PowerSharpParameters["ExtranetLicensingUrl"] = value;
				}
			}

			public virtual Uri IntranetCertificationUrl
			{
				set
				{
					base.PowerSharpParameters["IntranetCertificationUrl"] = value;
				}
			}

			public virtual Uri ExtranetCertificationUrl
			{
				set
				{
					base.PowerSharpParameters["ExtranetCertificationUrl"] = value;
				}
			}

			public virtual SwitchParameter Default
			{
				set
				{
					base.PowerSharpParameters["Default"] = value;
				}
			}

			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
				}
			}

			public virtual string Name
			{
				set
				{
					base.PowerSharpParameters["Name"] = value;
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
