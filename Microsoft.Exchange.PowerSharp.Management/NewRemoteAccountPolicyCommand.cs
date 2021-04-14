using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class NewRemoteAccountPolicyCommand : SyntheticCommandWithPipelineInput<RemoteAccountPolicy, RemoteAccountPolicy>
	{
		private NewRemoteAccountPolicyCommand() : base("New-RemoteAccountPolicy")
		{
		}

		public NewRemoteAccountPolicyCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual NewRemoteAccountPolicyCommand SetParameters(NewRemoteAccountPolicyCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class DefaultParameters : ParametersBase
		{
			public virtual EnhancedTimeSpan PollingInterval
			{
				set
				{
					base.PowerSharpParameters["PollingInterval"] = value;
				}
			}

			public virtual EnhancedTimeSpan TimeBeforeInactive
			{
				set
				{
					base.PowerSharpParameters["TimeBeforeInactive"] = value;
				}
			}

			public virtual EnhancedTimeSpan TimeBeforeDormant
			{
				set
				{
					base.PowerSharpParameters["TimeBeforeDormant"] = value;
				}
			}

			public virtual int MaxSyncAccounts
			{
				set
				{
					base.PowerSharpParameters["MaxSyncAccounts"] = value;
				}
			}

			public virtual bool SyncNowAllowed
			{
				set
				{
					base.PowerSharpParameters["SyncNowAllowed"] = value;
				}
			}

			public virtual string Organization
			{
				set
				{
					base.PowerSharpParameters["Organization"] = ((value != null) ? new OrganizationIdParameter(value) : null);
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
	}
}
