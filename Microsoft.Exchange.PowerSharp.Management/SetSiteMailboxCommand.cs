using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Management;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class SetSiteMailboxCommand : SyntheticCommandWithPipelineInputNoOutput<TeamMailbox>
	{
		private SetSiteMailboxCommand() : base("Set-SiteMailbox")
		{
		}

		public SetSiteMailboxCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual SetSiteMailboxCommand SetParameters(SetSiteMailboxCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual SetSiteMailboxCommand SetParameters(SetSiteMailboxCommand.TeamMailboxIWParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class DefaultParameters : ParametersBase
		{
			public virtual string Identity
			{
				set
				{
					base.PowerSharpParameters["Identity"] = ((value != null) ? new RecipientIdParameter(value) : null);
				}
			}

			public virtual string DisplayName
			{
				set
				{
					base.PowerSharpParameters["DisplayName"] = value;
				}
			}

			public virtual bool Active
			{
				set
				{
					base.PowerSharpParameters["Active"] = value;
				}
			}

			public virtual bool RemoveDuplicateMessages
			{
				set
				{
					base.PowerSharpParameters["RemoveDuplicateMessages"] = value;
				}
			}

			public virtual RecipientIdParameter Members
			{
				set
				{
					base.PowerSharpParameters["Members"] = value;
				}
			}

			public virtual RecipientIdParameter Owners
			{
				set
				{
					base.PowerSharpParameters["Owners"] = value;
				}
			}

			public virtual Uri SharePointUrl
			{
				set
				{
					base.PowerSharpParameters["SharePointUrl"] = value;
				}
			}

			public virtual SwitchParameter Force
			{
				set
				{
					base.PowerSharpParameters["Force"] = value;
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

		public class TeamMailboxIWParameters : ParametersBase
		{
			public virtual bool ShowInMyClient
			{
				set
				{
					base.PowerSharpParameters["ShowInMyClient"] = value;
				}
			}

			public virtual string Identity
			{
				set
				{
					base.PowerSharpParameters["Identity"] = ((value != null) ? new RecipientIdParameter(value) : null);
				}
			}

			public virtual string DisplayName
			{
				set
				{
					base.PowerSharpParameters["DisplayName"] = value;
				}
			}

			public virtual bool Active
			{
				set
				{
					base.PowerSharpParameters["Active"] = value;
				}
			}

			public virtual bool RemoveDuplicateMessages
			{
				set
				{
					base.PowerSharpParameters["RemoveDuplicateMessages"] = value;
				}
			}

			public virtual RecipientIdParameter Members
			{
				set
				{
					base.PowerSharpParameters["Members"] = value;
				}
			}

			public virtual RecipientIdParameter Owners
			{
				set
				{
					base.PowerSharpParameters["Owners"] = value;
				}
			}

			public virtual Uri SharePointUrl
			{
				set
				{
					base.PowerSharpParameters["SharePointUrl"] = value;
				}
			}

			public virtual SwitchParameter Force
			{
				set
				{
					base.PowerSharpParameters["Force"] = value;
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
