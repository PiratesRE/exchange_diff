using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory.Sync;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class SetFailedMSOSyncObjectCommand : SyntheticCommandWithPipelineInputNoOutput<FailedMSOSyncObjectPresentationObject>
	{
		private SetFailedMSOSyncObjectCommand() : base("Set-FailedMSOSyncObject")
		{
		}

		public SetFailedMSOSyncObjectCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual SetFailedMSOSyncObjectCommand SetParameters(SetFailedMSOSyncObjectCommand.IdentityParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual SetFailedMSOSyncObjectCommand SetParameters(SetFailedMSOSyncObjectCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class IdentityParameters : ParametersBase
		{
			public virtual string Identity
			{
				set
				{
					base.PowerSharpParameters["Identity"] = ((value != null) ? new FailedMSOSyncObjectIdParameter(value) : null);
				}
			}

			public virtual bool IsIgnoredInHaltCondition
			{
				set
				{
					base.PowerSharpParameters["IsIgnoredInHaltCondition"] = value;
				}
			}

			public virtual bool IsRetriable
			{
				set
				{
					base.PowerSharpParameters["IsRetriable"] = value;
				}
			}

			public virtual string Comment
			{
				set
				{
					base.PowerSharpParameters["Comment"] = value;
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
			public virtual bool IsIgnoredInHaltCondition
			{
				set
				{
					base.PowerSharpParameters["IsIgnoredInHaltCondition"] = value;
				}
			}

			public virtual bool IsRetriable
			{
				set
				{
					base.PowerSharpParameters["IsRetriable"] = value;
				}
			}

			public virtual string Comment
			{
				set
				{
					base.PowerSharpParameters["Comment"] = value;
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
