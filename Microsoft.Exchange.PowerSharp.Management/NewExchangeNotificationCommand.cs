using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Management.TenantMonitoring;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class NewExchangeNotificationCommand : SyntheticCommandWithPipelineInput<Notification, Notification>
	{
		private NewExchangeNotificationCommand() : base("New-ExchangeNotification")
		{
		}

		public NewExchangeNotificationCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual NewExchangeNotificationCommand SetParameters(NewExchangeNotificationCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class DefaultParameters : ParametersBase
		{
			public virtual string Organization
			{
				set
				{
					base.PowerSharpParameters["Organization"] = ((value != null) ? new OrganizationIdParameter(value) : null);
				}
			}

			public virtual uint EventInstanceId
			{
				set
				{
					base.PowerSharpParameters["EventInstanceId"] = value;
				}
			}

			public virtual string EventSource
			{
				set
				{
					base.PowerSharpParameters["EventSource"] = value;
				}
			}

			public virtual int EventCategoryId
			{
				set
				{
					base.PowerSharpParameters["EventCategoryId"] = value;
				}
			}

			public virtual ExDateTime EventTime
			{
				set
				{
					base.PowerSharpParameters["EventTime"] = value;
				}
			}

			public virtual string InsertionStrings
			{
				set
				{
					base.PowerSharpParameters["InsertionStrings"] = value;
				}
			}

			public virtual RecipientIdParameter NotificationRecipients
			{
				set
				{
					base.PowerSharpParameters["NotificationRecipients"] = value;
				}
			}

			public virtual ExDateTime CreationTime
			{
				set
				{
					base.PowerSharpParameters["CreationTime"] = value;
				}
			}

			public virtual string PeriodicKey
			{
				set
				{
					base.PowerSharpParameters["PeriodicKey"] = value;
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
