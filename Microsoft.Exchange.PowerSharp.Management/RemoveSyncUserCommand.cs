﻿using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class RemoveSyncUserCommand : SyntheticCommandWithPipelineInput<ADUser, ADUser>
	{
		private RemoveSyncUserCommand() : base("Remove-SyncUser")
		{
		}

		public RemoveSyncUserCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual RemoveSyncUserCommand SetParameters(RemoveSyncUserCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual RemoveSyncUserCommand SetParameters(RemoveSyncUserCommand.IdentityParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class DefaultParameters : ParametersBase
		{
			public virtual SwitchParameter ForReconciliation
			{
				set
				{
					base.PowerSharpParameters["ForReconciliation"] = value;
				}
			}

			public virtual bool Permanent
			{
				set
				{
					base.PowerSharpParameters["Permanent"] = value;
				}
			}

			public virtual SwitchParameter KeepWindowsLiveID
			{
				set
				{
					base.PowerSharpParameters["KeepWindowsLiveID"] = value;
				}
			}

			public virtual SwitchParameter IgnoreDefaultScope
			{
				set
				{
					base.PowerSharpParameters["IgnoreDefaultScope"] = value;
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

			public virtual SwitchParameter Confirm
			{
				set
				{
					base.PowerSharpParameters["Confirm"] = value;
				}
			}
		}

		public class IdentityParameters : ParametersBase
		{
			public virtual string Identity
			{
				set
				{
					base.PowerSharpParameters["Identity"] = ((value != null) ? new NonMailEnabledUserIdParameter(value) : null);
				}
			}

			public virtual SwitchParameter ForReconciliation
			{
				set
				{
					base.PowerSharpParameters["ForReconciliation"] = value;
				}
			}

			public virtual bool Permanent
			{
				set
				{
					base.PowerSharpParameters["Permanent"] = value;
				}
			}

			public virtual SwitchParameter KeepWindowsLiveID
			{
				set
				{
					base.PowerSharpParameters["KeepWindowsLiveID"] = value;
				}
			}

			public virtual SwitchParameter IgnoreDefaultScope
			{
				set
				{
					base.PowerSharpParameters["IgnoreDefaultScope"] = value;
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

			public virtual SwitchParameter Confirm
			{
				set
				{
					base.PowerSharpParameters["Confirm"] = value;
				}
			}
		}
	}
}
