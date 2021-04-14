using System;
using System.Globalization;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class SetPublicFolderCommand : SyntheticCommandWithPipelineInputNoOutput<PublicFolder>
	{
		private SetPublicFolderCommand() : base("Set-PublicFolder")
		{
		}

		public SetPublicFolderCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual SetPublicFolderCommand SetParameters(SetPublicFolderCommand.IdentityParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual SetPublicFolderCommand SetParameters(SetPublicFolderCommand.DefaultParameters parameters)
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
					base.PowerSharpParameters["Identity"] = ((value != null) ? new PublicFolderIdParameter(value) : null);
				}
			}

			public virtual string Path
			{
				set
				{
					base.PowerSharpParameters["Path"] = ((value != null) ? new PublicFolderIdParameter(value) : null);
				}
			}

			public virtual string OverrideContentMailbox
			{
				set
				{
					base.PowerSharpParameters["OverrideContentMailbox"] = ((value != null) ? new MailboxIdParameter(value) : null);
				}
			}

			public virtual SwitchParameter Force
			{
				set
				{
					base.PowerSharpParameters["Force"] = value;
				}
			}

			public virtual bool? MailEnabled
			{
				set
				{
					base.PowerSharpParameters["MailEnabled"] = value;
				}
			}

			public virtual Guid? MailRecipientGuid
			{
				set
				{
					base.PowerSharpParameters["MailRecipientGuid"] = value;
				}
			}

			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
				}
			}

			public virtual string Name
			{
				set
				{
					base.PowerSharpParameters["Name"] = value;
				}
			}

			public virtual CultureInfo EformsLocaleId
			{
				set
				{
					base.PowerSharpParameters["EformsLocaleId"] = value;
				}
			}

			public virtual bool PerUserReadStateEnabled
			{
				set
				{
					base.PowerSharpParameters["PerUserReadStateEnabled"] = value;
				}
			}

			public virtual EnhancedTimeSpan? AgeLimit
			{
				set
				{
					base.PowerSharpParameters["AgeLimit"] = value;
				}
			}

			public virtual EnhancedTimeSpan? RetainDeletedItemsFor
			{
				set
				{
					base.PowerSharpParameters["RetainDeletedItemsFor"] = value;
				}
			}

			public virtual Unlimited<ByteQuantifiedSize>? ProhibitPostQuota
			{
				set
				{
					base.PowerSharpParameters["ProhibitPostQuota"] = value;
				}
			}

			public virtual Unlimited<ByteQuantifiedSize>? IssueWarningQuota
			{
				set
				{
					base.PowerSharpParameters["IssueWarningQuota"] = value;
				}
			}

			public virtual Unlimited<ByteQuantifiedSize>? MaxItemSize
			{
				set
				{
					base.PowerSharpParameters["MaxItemSize"] = value;
				}
			}

			public virtual ExDateTime? LastMovedTime
			{
				set
				{
					base.PowerSharpParameters["LastMovedTime"] = value;
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
			public virtual string Path
			{
				set
				{
					base.PowerSharpParameters["Path"] = ((value != null) ? new PublicFolderIdParameter(value) : null);
				}
			}

			public virtual string OverrideContentMailbox
			{
				set
				{
					base.PowerSharpParameters["OverrideContentMailbox"] = ((value != null) ? new MailboxIdParameter(value) : null);
				}
			}

			public virtual SwitchParameter Force
			{
				set
				{
					base.PowerSharpParameters["Force"] = value;
				}
			}

			public virtual bool? MailEnabled
			{
				set
				{
					base.PowerSharpParameters["MailEnabled"] = value;
				}
			}

			public virtual Guid? MailRecipientGuid
			{
				set
				{
					base.PowerSharpParameters["MailRecipientGuid"] = value;
				}
			}

			public virtual Fqdn DomainController
			{
				set
				{
					base.PowerSharpParameters["DomainController"] = value;
				}
			}

			public virtual string Name
			{
				set
				{
					base.PowerSharpParameters["Name"] = value;
				}
			}

			public virtual CultureInfo EformsLocaleId
			{
				set
				{
					base.PowerSharpParameters["EformsLocaleId"] = value;
				}
			}

			public virtual bool PerUserReadStateEnabled
			{
				set
				{
					base.PowerSharpParameters["PerUserReadStateEnabled"] = value;
				}
			}

			public virtual EnhancedTimeSpan? AgeLimit
			{
				set
				{
					base.PowerSharpParameters["AgeLimit"] = value;
				}
			}

			public virtual EnhancedTimeSpan? RetainDeletedItemsFor
			{
				set
				{
					base.PowerSharpParameters["RetainDeletedItemsFor"] = value;
				}
			}

			public virtual Unlimited<ByteQuantifiedSize>? ProhibitPostQuota
			{
				set
				{
					base.PowerSharpParameters["ProhibitPostQuota"] = value;
				}
			}

			public virtual Unlimited<ByteQuantifiedSize>? IssueWarningQuota
			{
				set
				{
					base.PowerSharpParameters["IssueWarningQuota"] = value;
				}
			}

			public virtual Unlimited<ByteQuantifiedSize>? MaxItemSize
			{
				set
				{
					base.PowerSharpParameters["MaxItemSize"] = value;
				}
			}

			public virtual ExDateTime? LastMovedTime
			{
				set
				{
					base.PowerSharpParameters["LastMovedTime"] = value;
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
