using System;
using System.Management.Automation;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Providers;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class NewMailMessageCommand : SyntheticCommandWithPipelineInput<MailMessage, MailMessage>
	{
		private NewMailMessageCommand() : base("New-MailMessage")
		{
		}

		public NewMailMessageCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual NewMailMessageCommand SetParameters(NewMailMessageCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class DefaultParameters : ParametersBase
		{
			public virtual string Subject
			{
				set
				{
					base.PowerSharpParameters["Subject"] = value;
				}
			}

			public virtual string Body
			{
				set
				{
					base.PowerSharpParameters["Body"] = value;
				}
			}

			public virtual MailBodyFormat BodyFormat
			{
				set
				{
					base.PowerSharpParameters["BodyFormat"] = value;
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
