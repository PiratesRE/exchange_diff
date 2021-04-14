using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class UpdateMovedMailboxCommand : SyntheticCommandWithPipelineInput<ADUser, ADUser>
	{
		private UpdateMovedMailboxCommand() : base("Update-MovedMailbox")
		{
		}

		public UpdateMovedMailboxCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual UpdateMovedMailboxCommand SetParameters(UpdateMovedMailboxCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual UpdateMovedMailboxCommand SetParameters(UpdateMovedMailboxCommand.UpdateMailboxParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual UpdateMovedMailboxCommand SetParameters(UpdateMovedMailboxCommand.MorphToMailboxParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual UpdateMovedMailboxCommand SetParameters(UpdateMovedMailboxCommand.MorphToMailUserParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual UpdateMovedMailboxCommand SetParameters(UpdateMovedMailboxCommand.UpdateArchiveParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class DefaultParameters : ParametersBase
		{
			public virtual string Identity
			{
				set
				{
					base.PowerSharpParameters["Identity"] = ((value != null) ? new MailboxOrMailUserIdParameter(value) : null);
				}
			}

			public virtual byte PartitionHint
			{
				set
				{
					base.PowerSharpParameters["PartitionHint"] = value;
				}
			}

			public virtual Guid NewArchiveMDB
			{
				set
				{
					base.PowerSharpParameters["NewArchiveMDB"] = value;
				}
			}

			public virtual string ArchiveDomain
			{
				set
				{
					base.PowerSharpParameters["ArchiveDomain"] = value;
				}
			}

			public virtual ArchiveStatusFlags ArchiveStatus
			{
				set
				{
					base.PowerSharpParameters["ArchiveStatus"] = value;
				}
			}

			public virtual Fqdn ConfigDomainController
			{
				set
				{
					base.PowerSharpParameters["ConfigDomainController"] = value;
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

		public class UpdateMailboxParameters : ParametersBase
		{
			public virtual Guid NewHomeMDB
			{
				set
				{
					base.PowerSharpParameters["NewHomeMDB"] = value;
				}
			}

			public virtual Guid? NewContainerGuid
			{
				set
				{
					base.PowerSharpParameters["NewContainerGuid"] = value;
				}
			}

			public virtual CrossTenantObjectId NewUnifiedMailboxId
			{
				set
				{
					base.PowerSharpParameters["NewUnifiedMailboxId"] = value;
				}
			}

			public virtual SwitchParameter UpdateMailbox
			{
				set
				{
					base.PowerSharpParameters["UpdateMailbox"] = value;
				}
			}

			public virtual SwitchParameter SkipMailboxReleaseCheck
			{
				set
				{
					base.PowerSharpParameters["SkipMailboxReleaseCheck"] = value;
				}
			}

			public virtual SwitchParameter SkipProvisioningCheck
			{
				set
				{
					base.PowerSharpParameters["SkipProvisioningCheck"] = value;
				}
			}

			public virtual string Identity
			{
				set
				{
					base.PowerSharpParameters["Identity"] = ((value != null) ? new MailboxOrMailUserIdParameter(value) : null);
				}
			}

			public virtual byte PartitionHint
			{
				set
				{
					base.PowerSharpParameters["PartitionHint"] = value;
				}
			}

			public virtual Guid NewArchiveMDB
			{
				set
				{
					base.PowerSharpParameters["NewArchiveMDB"] = value;
				}
			}

			public virtual string ArchiveDomain
			{
				set
				{
					base.PowerSharpParameters["ArchiveDomain"] = value;
				}
			}

			public virtual ArchiveStatusFlags ArchiveStatus
			{
				set
				{
					base.PowerSharpParameters["ArchiveStatus"] = value;
				}
			}

			public virtual Fqdn ConfigDomainController
			{
				set
				{
					base.PowerSharpParameters["ConfigDomainController"] = value;
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

		public class MorphToMailboxParameters : ParametersBase
		{
			public virtual Guid NewHomeMDB
			{
				set
				{
					base.PowerSharpParameters["NewHomeMDB"] = value;
				}
			}

			public virtual SwitchParameter MorphToMailbox
			{
				set
				{
					base.PowerSharpParameters["MorphToMailbox"] = value;
				}
			}

			public virtual ADUser RemoteRecipientData
			{
				set
				{
					base.PowerSharpParameters["RemoteRecipientData"] = value;
				}
			}

			public virtual PSCredential Credential
			{
				set
				{
					base.PowerSharpParameters["Credential"] = value;
				}
			}

			public virtual SwitchParameter SkipProvisioningCheck
			{
				set
				{
					base.PowerSharpParameters["SkipProvisioningCheck"] = value;
				}
			}

			public virtual string Identity
			{
				set
				{
					base.PowerSharpParameters["Identity"] = ((value != null) ? new MailboxOrMailUserIdParameter(value) : null);
				}
			}

			public virtual byte PartitionHint
			{
				set
				{
					base.PowerSharpParameters["PartitionHint"] = value;
				}
			}

			public virtual Guid NewArchiveMDB
			{
				set
				{
					base.PowerSharpParameters["NewArchiveMDB"] = value;
				}
			}

			public virtual string ArchiveDomain
			{
				set
				{
					base.PowerSharpParameters["ArchiveDomain"] = value;
				}
			}

			public virtual ArchiveStatusFlags ArchiveStatus
			{
				set
				{
					base.PowerSharpParameters["ArchiveStatus"] = value;
				}
			}

			public virtual Fqdn ConfigDomainController
			{
				set
				{
					base.PowerSharpParameters["ConfigDomainController"] = value;
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

		public class MorphToMailUserParameters : ParametersBase
		{
			public virtual SwitchParameter MorphToMailUser
			{
				set
				{
					base.PowerSharpParameters["MorphToMailUser"] = value;
				}
			}

			public virtual ADUser RemoteRecipientData
			{
				set
				{
					base.PowerSharpParameters["RemoteRecipientData"] = value;
				}
			}

			public virtual PSCredential Credential
			{
				set
				{
					base.PowerSharpParameters["Credential"] = value;
				}
			}

			public virtual string Identity
			{
				set
				{
					base.PowerSharpParameters["Identity"] = ((value != null) ? new MailboxOrMailUserIdParameter(value) : null);
				}
			}

			public virtual byte PartitionHint
			{
				set
				{
					base.PowerSharpParameters["PartitionHint"] = value;
				}
			}

			public virtual Guid NewArchiveMDB
			{
				set
				{
					base.PowerSharpParameters["NewArchiveMDB"] = value;
				}
			}

			public virtual string ArchiveDomain
			{
				set
				{
					base.PowerSharpParameters["ArchiveDomain"] = value;
				}
			}

			public virtual ArchiveStatusFlags ArchiveStatus
			{
				set
				{
					base.PowerSharpParameters["ArchiveStatus"] = value;
				}
			}

			public virtual Fqdn ConfigDomainController
			{
				set
				{
					base.PowerSharpParameters["ConfigDomainController"] = value;
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

		public class UpdateArchiveParameters : ParametersBase
		{
			public virtual SwitchParameter UpdateArchiveOnly
			{
				set
				{
					base.PowerSharpParameters["UpdateArchiveOnly"] = value;
				}
			}

			public virtual ADUser RemoteRecipientData
			{
				set
				{
					base.PowerSharpParameters["RemoteRecipientData"] = value;
				}
			}

			public virtual SwitchParameter SkipMailboxReleaseCheck
			{
				set
				{
					base.PowerSharpParameters["SkipMailboxReleaseCheck"] = value;
				}
			}

			public virtual string Identity
			{
				set
				{
					base.PowerSharpParameters["Identity"] = ((value != null) ? new MailboxOrMailUserIdParameter(value) : null);
				}
			}

			public virtual byte PartitionHint
			{
				set
				{
					base.PowerSharpParameters["PartitionHint"] = value;
				}
			}

			public virtual Guid NewArchiveMDB
			{
				set
				{
					base.PowerSharpParameters["NewArchiveMDB"] = value;
				}
			}

			public virtual string ArchiveDomain
			{
				set
				{
					base.PowerSharpParameters["ArchiveDomain"] = value;
				}
			}

			public virtual ArchiveStatusFlags ArchiveStatus
			{
				set
				{
					base.PowerSharpParameters["ArchiveStatus"] = value;
				}
			}

			public virtual Fqdn ConfigDomainController
			{
				set
				{
					base.PowerSharpParameters["ConfigDomainController"] = value;
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
