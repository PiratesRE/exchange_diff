using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.PowerSharp.Management
{
	public class SetOrganizationRelationshipCommand : SyntheticCommandWithPipelineInputNoOutput<OrganizationRelationship>
	{
		private SetOrganizationRelationshipCommand() : base("Set-OrganizationRelationship")
		{
		}

		public SetOrganizationRelationshipCommand(ExchangeManagementSession session) : this()
		{
			base.Session = session;
		}

		public virtual SetOrganizationRelationshipCommand SetParameters(SetOrganizationRelationshipCommand.DefaultParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public virtual SetOrganizationRelationshipCommand SetParameters(SetOrganizationRelationshipCommand.IdentityParameters parameters)
		{
			base.SetParameters(parameters);
			return this;
		}

		public class DefaultParameters : ParametersBase
		{
			public virtual MultiValuedProperty<SmtpDomain> DomainNames
			{
				set
				{
					base.PowerSharpParameters["DomainNames"] = value;
				}
			}

			public virtual bool FreeBusyAccessEnabled
			{
				set
				{
					base.PowerSharpParameters["FreeBusyAccessEnabled"] = value;
				}
			}

			public virtual FreeBusyAccessLevel FreeBusyAccessLevel
			{
				set
				{
					base.PowerSharpParameters["FreeBusyAccessLevel"] = value;
				}
			}

			public virtual string FreeBusyAccessScope
			{
				set
				{
					base.PowerSharpParameters["FreeBusyAccessScope"] = ((value != null) ? new GroupIdParameter(value) : null);
				}
			}

			public virtual bool MailboxMoveEnabled
			{
				set
				{
					base.PowerSharpParameters["MailboxMoveEnabled"] = value;
				}
			}

			public virtual bool DeliveryReportEnabled
			{
				set
				{
					base.PowerSharpParameters["DeliveryReportEnabled"] = value;
				}
			}

			public virtual bool MailTipsAccessEnabled
			{
				set
				{
					base.PowerSharpParameters["MailTipsAccessEnabled"] = value;
				}
			}

			public virtual MailTipsAccessLevel MailTipsAccessLevel
			{
				set
				{
					base.PowerSharpParameters["MailTipsAccessLevel"] = value;
				}
			}

			public virtual string MailTipsAccessScope
			{
				set
				{
					base.PowerSharpParameters["MailTipsAccessScope"] = ((value != null) ? new GroupIdParameter(value) : null);
				}
			}

			public virtual bool ArchiveAccessEnabled
			{
				set
				{
					base.PowerSharpParameters["ArchiveAccessEnabled"] = value;
				}
			}

			public virtual bool PhotosEnabled
			{
				set
				{
					base.PowerSharpParameters["PhotosEnabled"] = value;
				}
			}

			public virtual Uri TargetApplicationUri
			{
				set
				{
					base.PowerSharpParameters["TargetApplicationUri"] = value;
				}
			}

			public virtual Uri TargetSharingEpr
			{
				set
				{
					base.PowerSharpParameters["TargetSharingEpr"] = value;
				}
			}

			public virtual Uri TargetAutodiscoverEpr
			{
				set
				{
					base.PowerSharpParameters["TargetAutodiscoverEpr"] = value;
				}
			}

			public virtual Uri TargetOwaURL
			{
				set
				{
					base.PowerSharpParameters["TargetOwaURL"] = value;
				}
			}

			public virtual SmtpAddress OrganizationContact
			{
				set
				{
					base.PowerSharpParameters["OrganizationContact"] = value;
				}
			}

			public virtual bool Enabled
			{
				set
				{
					base.PowerSharpParameters["Enabled"] = value;
				}
			}

			public virtual SwitchParameter Force
			{
				set
				{
					base.PowerSharpParameters["Force"] = value;
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
			public virtual OrganizationRelationshipIdParameter Identity
			{
				set
				{
					base.PowerSharpParameters["Identity"] = value;
				}
			}

			public virtual MultiValuedProperty<SmtpDomain> DomainNames
			{
				set
				{
					base.PowerSharpParameters["DomainNames"] = value;
				}
			}

			public virtual bool FreeBusyAccessEnabled
			{
				set
				{
					base.PowerSharpParameters["FreeBusyAccessEnabled"] = value;
				}
			}

			public virtual FreeBusyAccessLevel FreeBusyAccessLevel
			{
				set
				{
					base.PowerSharpParameters["FreeBusyAccessLevel"] = value;
				}
			}

			public virtual string FreeBusyAccessScope
			{
				set
				{
					base.PowerSharpParameters["FreeBusyAccessScope"] = ((value != null) ? new GroupIdParameter(value) : null);
				}
			}

			public virtual bool MailboxMoveEnabled
			{
				set
				{
					base.PowerSharpParameters["MailboxMoveEnabled"] = value;
				}
			}

			public virtual bool DeliveryReportEnabled
			{
				set
				{
					base.PowerSharpParameters["DeliveryReportEnabled"] = value;
				}
			}

			public virtual bool MailTipsAccessEnabled
			{
				set
				{
					base.PowerSharpParameters["MailTipsAccessEnabled"] = value;
				}
			}

			public virtual MailTipsAccessLevel MailTipsAccessLevel
			{
				set
				{
					base.PowerSharpParameters["MailTipsAccessLevel"] = value;
				}
			}

			public virtual string MailTipsAccessScope
			{
				set
				{
					base.PowerSharpParameters["MailTipsAccessScope"] = ((value != null) ? new GroupIdParameter(value) : null);
				}
			}

			public virtual bool ArchiveAccessEnabled
			{
				set
				{
					base.PowerSharpParameters["ArchiveAccessEnabled"] = value;
				}
			}

			public virtual bool PhotosEnabled
			{
				set
				{
					base.PowerSharpParameters["PhotosEnabled"] = value;
				}
			}

			public virtual Uri TargetApplicationUri
			{
				set
				{
					base.PowerSharpParameters["TargetApplicationUri"] = value;
				}
			}

			public virtual Uri TargetSharingEpr
			{
				set
				{
					base.PowerSharpParameters["TargetSharingEpr"] = value;
				}
			}

			public virtual Uri TargetAutodiscoverEpr
			{
				set
				{
					base.PowerSharpParameters["TargetAutodiscoverEpr"] = value;
				}
			}

			public virtual Uri TargetOwaURL
			{
				set
				{
					base.PowerSharpParameters["TargetOwaURL"] = value;
				}
			}

			public virtual SmtpAddress OrganizationContact
			{
				set
				{
					base.PowerSharpParameters["OrganizationContact"] = value;
				}
			}

			public virtual bool Enabled
			{
				set
				{
					base.PowerSharpParameters["Enabled"] = value;
				}
			}

			public virtual SwitchParameter Force
			{
				set
				{
					base.PowerSharpParameters["Force"] = value;
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
