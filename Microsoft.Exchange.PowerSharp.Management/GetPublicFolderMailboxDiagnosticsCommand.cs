using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class GetPublicFolderMailboxDiagnosticsCommand : SyntheticCommandWithPipelineInput<ADRecipient, ADRecipient>
	{
		private GetPublicFolderMailboxDiagnosticsCommand() : base("Get-PublicFolderMailboxDiagnostics")
		{
		}

		public GetPublicFolderMailboxDiagnosticsCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual GetPublicFolderMailboxDiagnosticsCommand SetParameters(GetPublicFolderMailboxDiagnosticsCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual GetPublicFolderMailboxDiagnosticsCommand SetParameters(GetPublicFolderMailboxDiagnosticsCommand.IdentityParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class DefaultParameters : ParametersBase
		{
			public virtual SwitchParameter IncludeDumpsterInfo
			{
				set
				{
					base.PowerSharpParameters["IncludeDumpsterInfo"] = value;
				}
			}

			public virtual SwitchParameter IncludeHierarchyInfo
			{
				set
				{
					base.PowerSharpParameters["IncludeHierarchyInfo"] = value;
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

		public class IdentityParameters : ParametersBase
		{
			public virtual string Identity
			{
				set
				{
					base.PowerSharpParameters["Identity"] = ((value != null) ? new MailboxIdParameter(value) : null);
				}
			}

			public virtual SwitchParameter IncludeDumpsterInfo
			{
				set
				{
					base.PowerSharpParameters["IncludeDumpsterInfo"] = value;
				}
			}

			public virtual SwitchParameter IncludeHierarchyInfo
			{
				set
				{
					base.PowerSharpParameters["IncludeHierarchyInfo"] = value;
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
