using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.InfoWorker.Common.OOF;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class SetMailboxAutoReplyConfigurationCommand : SyntheticCommandWithPipelineInputNoOutput<MailboxAutoReplyConfiguration>
	{
		private SetMailboxAutoReplyConfigurationCommand() : base("Set-MailboxAutoReplyConfiguration")
		{
		}

		public SetMailboxAutoReplyConfigurationCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual SetMailboxAutoReplyConfigurationCommand SetParameters(SetMailboxAutoReplyConfigurationCommand.IdentityParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual SetMailboxAutoReplyConfigurationCommand SetParameters(SetMailboxAutoReplyConfigurationCommand.DefaultParameters parameters)
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
					base.PowerSharpParameters["Identity"] = ((value != null) ? new MailboxIdParameter(value) : null);
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

			public virtual OofState AutoReplyState
			{
				set
				{
					base.PowerSharpParameters["AutoReplyState"] = value;
				}
			}

			public virtual DateTime EndTime
			{
				set
				{
					base.PowerSharpParameters["EndTime"] = value;
				}
			}

			public virtual ExternalAudience ExternalAudience
			{
				set
				{
					base.PowerSharpParameters["ExternalAudience"] = value;
				}
			}

			public virtual string ExternalMessage
			{
				set
				{
					base.PowerSharpParameters["ExternalMessage"] = value;
				}
			}

			public virtual string InternalMessage
			{
				set
				{
					base.PowerSharpParameters["InternalMessage"] = value;
				}
			}

			public virtual DateTime StartTime
			{
				set
				{
					base.PowerSharpParameters["StartTime"] = value;
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

			public virtual OofState AutoReplyState
			{
				set
				{
					base.PowerSharpParameters["AutoReplyState"] = value;
				}
			}

			public virtual DateTime EndTime
			{
				set
				{
					base.PowerSharpParameters["EndTime"] = value;
				}
			}

			public virtual ExternalAudience ExternalAudience
			{
				set
				{
					base.PowerSharpParameters["ExternalAudience"] = value;
				}
			}

			public virtual string ExternalMessage
			{
				set
				{
					base.PowerSharpParameters["ExternalMessage"] = value;
				}
			}

			public virtual string InternalMessage
			{
				set
				{
					base.PowerSharpParameters["InternalMessage"] = value;
				}
			}

			public virtual DateTime StartTime
			{
				set
				{
					base.PowerSharpParameters["StartTime"] = value;
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
