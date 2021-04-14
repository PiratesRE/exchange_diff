using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class NewActiveSyncDeviceAccessRuleCommand : SyntheticCommandWithPipelineInput<ActiveSyncDeviceAccessRule, ActiveSyncDeviceAccessRule>
	{
		private NewActiveSyncDeviceAccessRuleCommand() : base("New-ActiveSyncDeviceAccessRule")
		{
		}

		public NewActiveSyncDeviceAccessRuleCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual NewActiveSyncDeviceAccessRuleCommand SetParameters(NewActiveSyncDeviceAccessRuleCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class DefaultParameters : ParametersBase
		{
			public virtual DeviceAccessLevel AccessLevel
			{
				set
				{
					base.PowerSharpParameters["AccessLevel"] = value;
				}
			}

			public virtual DeviceAccessCharacteristic Characteristic
			{
				set
				{
					base.PowerSharpParameters["Characteristic"] = value;
				}
			}

			public virtual string QueryString
			{
				set
				{
					base.PowerSharpParameters["QueryString"] = value;
				}
			}

			public virtual string Organization
			{
				set
				{
					base.PowerSharpParameters["Organization"] = ((value != null) ? new OrganizationIdParameter(value) : null);
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
