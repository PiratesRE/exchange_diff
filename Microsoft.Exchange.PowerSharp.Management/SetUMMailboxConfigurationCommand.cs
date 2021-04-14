using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class SetUMMailboxConfigurationCommand : SyntheticCommandWithPipelineInputNoOutput<ADUser>
	{
		private SetUMMailboxConfigurationCommand() : base("Set-UMMailboxConfiguration")
		{
		}

		public SetUMMailboxConfigurationCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual SetUMMailboxConfigurationCommand SetParameters(SetUMMailboxConfigurationCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual SetUMMailboxConfigurationCommand SetParameters(SetUMMailboxConfigurationCommand.IdentityParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class DefaultParameters : ParametersBase
		{
			public virtual MailboxGreetingEnum Greeting
			{
				set
				{
					base.PowerSharpParameters["Greeting"] = value;
				}
			}

			public virtual string FolderToReadEmailsFrom
			{
				set
				{
					base.PowerSharpParameters["FolderToReadEmailsFrom"] = ((value != null) ? new MailboxFolderIdParameter(value) : null);
				}
			}

			public virtual bool ReadOldestUnreadVoiceMessagesFirst
			{
				set
				{
					base.PowerSharpParameters["ReadOldestUnreadVoiceMessagesFirst"] = value;
				}
			}

			public virtual string DefaultPlayOnPhoneNumber
			{
				set
				{
					base.PowerSharpParameters["DefaultPlayOnPhoneNumber"] = value;
				}
			}

			public virtual bool ReceivedVoiceMailPreviewEnabled
			{
				set
				{
					base.PowerSharpParameters["ReceivedVoiceMailPreviewEnabled"] = value;
				}
			}

			public virtual bool SentVoiceMailPreviewEnabled
			{
				set
				{
					base.PowerSharpParameters["SentVoiceMailPreviewEnabled"] = value;
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

			public virtual MailboxGreetingEnum Greeting
			{
				set
				{
					base.PowerSharpParameters["Greeting"] = value;
				}
			}

			public virtual string FolderToReadEmailsFrom
			{
				set
				{
					base.PowerSharpParameters["FolderToReadEmailsFrom"] = ((value != null) ? new MailboxFolderIdParameter(value) : null);
				}
			}

			public virtual bool ReadOldestUnreadVoiceMessagesFirst
			{
				set
				{
					base.PowerSharpParameters["ReadOldestUnreadVoiceMessagesFirst"] = value;
				}
			}

			public virtual string DefaultPlayOnPhoneNumber
			{
				set
				{
					base.PowerSharpParameters["DefaultPlayOnPhoneNumber"] = value;
				}
			}

			public virtual bool ReceivedVoiceMailPreviewEnabled
			{
				set
				{
					base.PowerSharpParameters["ReceivedVoiceMailPreviewEnabled"] = value;
				}
			}

			public virtual bool SentVoiceMailPreviewEnabled
			{
				set
				{
					base.PowerSharpParameters["SentVoiceMailPreviewEnabled"] = value;
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
