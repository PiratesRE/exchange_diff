using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class SetAdSiteCommand : SyntheticCommandWithPipelineInputNoOutput<ADSite>
	{
		private SetAdSiteCommand() : base("Set-AdSite")
		{
		}

		public SetAdSiteCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual SetAdSiteCommand SetParameters(SetAdSiteCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual SetAdSiteCommand SetParameters(SetAdSiteCommand.IdentityParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class DefaultParameters : ParametersBase
		{
			public virtual MultiValuedProperty<AdSiteIdParameter> ResponsibleForSites
			{
				set
				{
					base.PowerSharpParameters["ResponsibleForSites"] = value;
				}
			}

			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
				}
			}

			public virtual bool HubSiteEnabled
			{
				set
				{
					base.PowerSharpParameters["HubSiteEnabled"] = value;
				}
			}

			public virtual bool InboundMailEnabled
			{
				set
				{
					base.PowerSharpParameters["InboundMailEnabled"] = value;
				}
			}

			public virtual int PartnerId
			{
				set
				{
					base.PowerSharpParameters["PartnerId"] = value;
				}
			}

			public virtual int MinorPartnerId
			{
				set
				{
					base.PowerSharpParameters["MinorPartnerId"] = value;
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
					base.PowerSharpParameters["Identity"] = ((value != null) ? new AdSiteIdParameter(value) : null);
				}
			}

			public virtual MultiValuedProperty<AdSiteIdParameter> ResponsibleForSites
			{
				set
				{
					base.PowerSharpParameters["ResponsibleForSites"] = value;
				}
			}

			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
				}
			}

			public virtual bool HubSiteEnabled
			{
				set
				{
					base.PowerSharpParameters["HubSiteEnabled"] = value;
				}
			}

			public virtual bool InboundMailEnabled
			{
				set
				{
					base.PowerSharpParameters["InboundMailEnabled"] = value;
				}
			}

			public virtual int PartnerId
			{
				set
				{
					base.PowerSharpParameters["PartnerId"] = value;
				}
			}

			public virtual int MinorPartnerId
			{
				set
				{
					base.PowerSharpParameters["MinorPartnerId"] = value;
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
