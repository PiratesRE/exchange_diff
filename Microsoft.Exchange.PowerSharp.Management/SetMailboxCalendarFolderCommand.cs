using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Management;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class SetMailboxCalendarFolderCommand : SyntheticCommandWithPipelineInputNoOutput<MailboxCalendarFolder>
	{
		private SetMailboxCalendarFolderCommand() : base("Set-MailboxCalendarFolder")
		{
		}

		public SetMailboxCalendarFolderCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual SetMailboxCalendarFolderCommand SetParameters(SetMailboxCalendarFolderCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual SetMailboxCalendarFolderCommand SetParameters(SetMailboxCalendarFolderCommand.IdentityParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class DefaultParameters : ParametersBase
		{
			public virtual SwitchParameter ResetUrl
			{
				set
				{
					base.PowerSharpParameters["ResetUrl"] = value;
				}
			}

			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
				}
			}

			public virtual bool PublishEnabled
			{
				set
				{
					base.PowerSharpParameters["PublishEnabled"] = value;
				}
			}

			public virtual DateRangeEnumType PublishDateRangeFrom
			{
				set
				{
					base.PowerSharpParameters["PublishDateRangeFrom"] = value;
				}
			}

			public virtual DateRangeEnumType PublishDateRangeTo
			{
				set
				{
					base.PowerSharpParameters["PublishDateRangeTo"] = value;
				}
			}

			public virtual DetailLevelEnumType DetailLevel
			{
				set
				{
					base.PowerSharpParameters["DetailLevel"] = value;
				}
			}

			public virtual bool SearchableUrlEnabled
			{
				set
				{
					base.PowerSharpParameters["SearchableUrlEnabled"] = value;
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
					base.PowerSharpParameters["Identity"] = ((value != null) ? new MailboxFolderIdParameter(value) : null);
				}
			}

			public virtual SwitchParameter ResetUrl
			{
				set
				{
					base.PowerSharpParameters["ResetUrl"] = value;
				}
			}

			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
				}
			}

			public virtual bool PublishEnabled
			{
				set
				{
					base.PowerSharpParameters["PublishEnabled"] = value;
				}
			}

			public virtual DateRangeEnumType PublishDateRangeFrom
			{
				set
				{
					base.PowerSharpParameters["PublishDateRangeFrom"] = value;
				}
			}

			public virtual DateRangeEnumType PublishDateRangeTo
			{
				set
				{
					base.PowerSharpParameters["PublishDateRangeTo"] = value;
				}
			}

			public virtual DetailLevelEnumType DetailLevel
			{
				set
				{
					base.PowerSharpParameters["DetailLevel"] = value;
				}
			}

			public virtual bool SearchableUrlEnabled
			{
				set
				{
					base.PowerSharpParameters["SearchableUrlEnabled"] = value;
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
