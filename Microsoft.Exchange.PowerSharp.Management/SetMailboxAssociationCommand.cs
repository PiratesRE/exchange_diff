using System;
using System.Management.Automation;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class SetMailboxAssociationCommand : SyntheticCommandWithPipelineInputNoOutput<MailboxAssociationPresentationObject>
	{
		private SetMailboxAssociationCommand() : base("Set-MailboxAssociation")
		{
		}

		public SetMailboxAssociationCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual SetMailboxAssociationCommand SetParameters(SetMailboxAssociationCommand.IdentityParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual SetMailboxAssociationCommand SetParameters(SetMailboxAssociationCommand.DefaultParameters parameters)
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
					base.PowerSharpParameters["Identity"] = ((value != null) ? new MailboxAssociationIdParameter(value) : null);
				}
			}

			public virtual SwitchParameter UpdateSlavedData
			{
				set
				{
					base.PowerSharpParameters["UpdateSlavedData"] = value;
				}
			}

			public virtual SwitchParameter ReplicateMasteredData
			{
				set
				{
					base.PowerSharpParameters["ReplicateMasteredData"] = value;
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

			public virtual string ExternalId
			{
				set
				{
					base.PowerSharpParameters["ExternalId"] = value;
				}
			}

			public virtual string LegacyDn
			{
				set
				{
					base.PowerSharpParameters["LegacyDn"] = value;
				}
			}

			public virtual bool IsMember
			{
				set
				{
					base.PowerSharpParameters["IsMember"] = value;
				}
			}

			public virtual string JoinedBy
			{
				set
				{
					base.PowerSharpParameters["JoinedBy"] = value;
				}
			}

			public virtual SmtpAddress GroupSmtpAddress
			{
				set
				{
					base.PowerSharpParameters["GroupSmtpAddress"] = value;
				}
			}

			public virtual SmtpAddress UserSmtpAddress
			{
				set
				{
					base.PowerSharpParameters["UserSmtpAddress"] = value;
				}
			}

			public virtual bool IsPin
			{
				set
				{
					base.PowerSharpParameters["IsPin"] = value;
				}
			}

			public virtual bool ShouldEscalate
			{
				set
				{
					base.PowerSharpParameters["ShouldEscalate"] = value;
				}
			}

			public virtual bool IsAutoSubscribed
			{
				set
				{
					base.PowerSharpParameters["IsAutoSubscribed"] = value;
				}
			}

			public virtual ExDateTime JoinDate
			{
				set
				{
					base.PowerSharpParameters["JoinDate"] = value;
				}
			}

			public virtual ExDateTime LastVisitedDate
			{
				set
				{
					base.PowerSharpParameters["LastVisitedDate"] = value;
				}
			}

			public virtual ExDateTime PinDate
			{
				set
				{
					base.PowerSharpParameters["PinDate"] = value;
				}
			}

			public virtual int CurrentVersion
			{
				set
				{
					base.PowerSharpParameters["CurrentVersion"] = value;
				}
			}

			public virtual int SyncedVersion
			{
				set
				{
					base.PowerSharpParameters["SyncedVersion"] = value;
				}
			}

			public virtual string LastSyncError
			{
				set
				{
					base.PowerSharpParameters["LastSyncError"] = value;
				}
			}

			public virtual int SyncAttempts
			{
				set
				{
					base.PowerSharpParameters["SyncAttempts"] = value;
				}
			}

			public virtual string SyncedSchemaVersion
			{
				set
				{
					base.PowerSharpParameters["SyncedSchemaVersion"] = value;
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
			public virtual SwitchParameter UpdateSlavedData
			{
				set
				{
					base.PowerSharpParameters["UpdateSlavedData"] = value;
				}
			}

			public virtual SwitchParameter ReplicateMasteredData
			{
				set
				{
					base.PowerSharpParameters["ReplicateMasteredData"] = value;
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

			public virtual string ExternalId
			{
				set
				{
					base.PowerSharpParameters["ExternalId"] = value;
				}
			}

			public virtual string LegacyDn
			{
				set
				{
					base.PowerSharpParameters["LegacyDn"] = value;
				}
			}

			public virtual bool IsMember
			{
				set
				{
					base.PowerSharpParameters["IsMember"] = value;
				}
			}

			public virtual string JoinedBy
			{
				set
				{
					base.PowerSharpParameters["JoinedBy"] = value;
				}
			}

			public virtual SmtpAddress GroupSmtpAddress
			{
				set
				{
					base.PowerSharpParameters["GroupSmtpAddress"] = value;
				}
			}

			public virtual SmtpAddress UserSmtpAddress
			{
				set
				{
					base.PowerSharpParameters["UserSmtpAddress"] = value;
				}
			}

			public virtual bool IsPin
			{
				set
				{
					base.PowerSharpParameters["IsPin"] = value;
				}
			}

			public virtual bool ShouldEscalate
			{
				set
				{
					base.PowerSharpParameters["ShouldEscalate"] = value;
				}
			}

			public virtual bool IsAutoSubscribed
			{
				set
				{
					base.PowerSharpParameters["IsAutoSubscribed"] = value;
				}
			}

			public virtual ExDateTime JoinDate
			{
				set
				{
					base.PowerSharpParameters["JoinDate"] = value;
				}
			}

			public virtual ExDateTime LastVisitedDate
			{
				set
				{
					base.PowerSharpParameters["LastVisitedDate"] = value;
				}
			}

			public virtual ExDateTime PinDate
			{
				set
				{
					base.PowerSharpParameters["PinDate"] = value;
				}
			}

			public virtual int CurrentVersion
			{
				set
				{
					base.PowerSharpParameters["CurrentVersion"] = value;
				}
			}

			public virtual int SyncedVersion
			{
				set
				{
					base.PowerSharpParameters["SyncedVersion"] = value;
				}
			}

			public virtual string LastSyncError
			{
				set
				{
					base.PowerSharpParameters["LastSyncError"] = value;
				}
			}

			public virtual int SyncAttempts
			{
				set
				{
					base.PowerSharpParameters["SyncAttempts"] = value;
				}
			}

			public virtual string SyncedSchemaVersion
			{
				set
				{
					base.PowerSharpParameters["SyncedSchemaVersion"] = value;
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
