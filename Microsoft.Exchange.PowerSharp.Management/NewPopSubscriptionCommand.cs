using System;
using System.Management.Automation;
using System.Security;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Transport.Sync.Common.Subscription.Pop;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class NewPopSubscriptionCommand : SyntheticCommandWithPipelineInput<PopSubscriptionProxy, PopSubscriptionProxy>
	{
		private NewPopSubscriptionCommand() : base("New-PopSubscription")
		{
		}

		public NewPopSubscriptionCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual NewPopSubscriptionCommand SetParameters(NewPopSubscriptionCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class DefaultParameters : ParametersBase
		{
			public virtual Fqdn IncomingServer
			{
				set
				{
					base.PowerSharpParameters["IncomingServer"] = value;
				}
			}

			public virtual int IncomingPort
			{
				set
				{
					base.PowerSharpParameters["IncomingPort"] = value;
				}
			}

			public virtual string IncomingUserName
			{
				set
				{
					base.PowerSharpParameters["IncomingUserName"] = value;
				}
			}

			public virtual SecureString IncomingPassword
			{
				set
				{
					base.PowerSharpParameters["IncomingPassword"] = value;
				}
			}

			public virtual AuthenticationMechanism IncomingAuth
			{
				set
				{
					base.PowerSharpParameters["IncomingAuth"] = value;
				}
			}

			public virtual SecurityMechanism IncomingSecurity
			{
				set
				{
					base.PowerSharpParameters["IncomingSecurity"] = value;
				}
			}

			public virtual bool LeaveOnServer
			{
				set
				{
					base.PowerSharpParameters["LeaveOnServer"] = value;
				}
			}

			public virtual SwitchParameter Force
			{
				set
				{
					base.PowerSharpParameters["Force"] = value;
				}
			}

			public virtual string Name
			{
				set
				{
					base.PowerSharpParameters["Name"] = value;
				}
			}

			public virtual string Mailbox
			{
				set
				{
					base.PowerSharpParameters["Mailbox"] = ((value != null) ? new MailboxIdParameter(value) : null);
				}
			}

			public virtual string DisplayName
			{
				set
				{
					base.PowerSharpParameters["DisplayName"] = value;
				}
			}

			public virtual SmtpAddress EmailAddress
			{
				set
				{
					base.PowerSharpParameters["EmailAddress"] = value;
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
