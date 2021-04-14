using System;
using System.Management.Automation;
using System.Net;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class SetIPAllowListProviderCommand : SyntheticCommandWithPipelineInputNoOutput<IPAllowListProvider>
	{
		private SetIPAllowListProviderCommand() : base("Set-IPAllowListProvider")
		{
		}

		public SetIPAllowListProviderCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual SetIPAllowListProviderCommand SetParameters(SetIPAllowListProviderCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual SetIPAllowListProviderCommand SetParameters(SetIPAllowListProviderCommand.IdentityParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class DefaultParameters : ParametersBase
		{
			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
				}
			}

			public virtual SmtpDomain LookupDomain
			{
				set
				{
					base.PowerSharpParameters["LookupDomain"] = value;
				}
			}

			public virtual bool Enabled
			{
				set
				{
					base.PowerSharpParameters["Enabled"] = value;
				}
			}

			public virtual bool AnyMatch
			{
				set
				{
					base.PowerSharpParameters["AnyMatch"] = value;
				}
			}

			public virtual IPAddress BitmaskMatch
			{
				set
				{
					base.PowerSharpParameters["BitmaskMatch"] = value;
				}
			}

			public virtual MultiValuedProperty<IPAddress> IPAddressesMatch
			{
				set
				{
					base.PowerSharpParameters["IPAddressesMatch"] = value;
				}
			}

			public virtual int Priority
			{
				set
				{
					base.PowerSharpParameters["Priority"] = value;
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
					base.PowerSharpParameters["Identity"] = ((value != null) ? new IPAllowListProviderIdParameter(value) : null);
				}
			}

			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
				}
			}

			public virtual SmtpDomain LookupDomain
			{
				set
				{
					base.PowerSharpParameters["LookupDomain"] = value;
				}
			}

			public virtual bool Enabled
			{
				set
				{
					base.PowerSharpParameters["Enabled"] = value;
				}
			}

			public virtual bool AnyMatch
			{
				set
				{
					base.PowerSharpParameters["AnyMatch"] = value;
				}
			}

			public virtual IPAddress BitmaskMatch
			{
				set
				{
					base.PowerSharpParameters["BitmaskMatch"] = value;
				}
			}

			public virtual MultiValuedProperty<IPAddress> IPAddressesMatch
			{
				set
				{
					base.PowerSharpParameters["IPAddressesMatch"] = value;
				}
			}

			public virtual int Priority
			{
				set
				{
					base.PowerSharpParameters["Priority"] = value;
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
