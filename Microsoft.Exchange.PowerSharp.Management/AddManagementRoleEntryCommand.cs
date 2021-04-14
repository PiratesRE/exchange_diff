using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Management;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class AddManagementRoleEntryCommand : SyntheticCommandWithPipelineInputNoOutput<RoleEntryIdParameter>
	{
		private AddManagementRoleEntryCommand() : base("Add-ManagementRoleEntry")
		{
		}

		public AddManagementRoleEntryCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual AddManagementRoleEntryCommand SetParameters(AddManagementRoleEntryCommand.IdentityParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual AddManagementRoleEntryCommand SetParameters(AddManagementRoleEntryCommand.ParentRoleEntryParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual AddManagementRoleEntryCommand SetParameters(AddManagementRoleEntryCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class IdentityParameters : ParametersBase
		{
			public virtual SwitchParameter UnScopedTopLevel
			{
				set
				{
					base.PowerSharpParameters["UnScopedTopLevel"] = value;
				}
			}

			public virtual SwitchParameter SkipScriptExistenceCheck
			{
				set
				{
					base.PowerSharpParameters["SkipScriptExistenceCheck"] = value;
				}
			}

			public virtual string Parameters
			{
				set
				{
					base.PowerSharpParameters["Parameters"] = value;
				}
			}

			public virtual string PSSnapinName
			{
				set
				{
					base.PowerSharpParameters["PSSnapinName"] = value;
				}
			}

			public virtual ManagementRoleEntryType Type
			{
				set
				{
					base.PowerSharpParameters["Type"] = value;
				}
			}

			public virtual string Identity
			{
				set
				{
					base.PowerSharpParameters["Identity"] = ((value != null) ? new RoleEntryIdParameter(value) : null);
				}
			}

			public virtual SwitchParameter Overwrite
			{
				set
				{
					base.PowerSharpParameters["Overwrite"] = value;
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

		public class ParentRoleEntryParameters : ParametersBase
		{
			public virtual string ParentRoleEntry
			{
				set
				{
					base.PowerSharpParameters["ParentRoleEntry"] = ((value != null) ? new RoleEntryIdParameter(value) : null);
				}
			}

			public virtual string Role
			{
				set
				{
					base.PowerSharpParameters["Role"] = ((value != null) ? new RoleIdParameter(value) : null);
				}
			}

			public virtual SwitchParameter Overwrite
			{
				set
				{
					base.PowerSharpParameters["Overwrite"] = value;
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

		public class DefaultParameters : ParametersBase
		{
			public virtual SwitchParameter Overwrite
			{
				set
				{
					base.PowerSharpParameters["Overwrite"] = value;
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
