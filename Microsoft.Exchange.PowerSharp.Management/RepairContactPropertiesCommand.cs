using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class RepairContactPropertiesCommand : SyntheticCommandWithPipelineInputNoOutput<SwitchParameter>
	{
		private RepairContactPropertiesCommand() : base("Repair-ContactProperties")
		{
		}

		public RepairContactPropertiesCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual RepairContactPropertiesCommand SetParameters(RepairContactPropertiesCommand.DisplayNameParameterSetParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual RepairContactPropertiesCommand SetParameters(RepairContactPropertiesCommand.ConversationIndexParameterSetParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual RepairContactPropertiesCommand SetParameters(RepairContactPropertiesCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class DisplayNameParameterSetParameters : ParametersBase
		{
			public virtual SwitchParameter FixDisplayName
			{
				set
				{
					base.PowerSharpParameters["FixDisplayName"] = value;
				}
			}

			public virtual SwitchParameter FixConversationIndexTracking
			{
				set
				{
					base.PowerSharpParameters["FixConversationIndexTracking"] = value;
				}
			}

			public virtual string Identity
			{
				set
				{
					base.PowerSharpParameters["Identity"] = ((value != null) ? new MailboxIdParameter(value) : null);
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
		}

		public class ConversationIndexParameterSetParameters : ParametersBase
		{
			public virtual SwitchParameter FixConversationIndexTracking
			{
				set
				{
					base.PowerSharpParameters["FixConversationIndexTracking"] = value;
				}
			}

			public virtual string Identity
			{
				set
				{
					base.PowerSharpParameters["Identity"] = ((value != null) ? new MailboxIdParameter(value) : null);
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
		}

		public class DefaultParameters : ParametersBase
		{
			public virtual string Identity
			{
				set
				{
					base.PowerSharpParameters["Identity"] = ((value != null) ? new MailboxIdParameter(value) : null);
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
		}
	}
}
