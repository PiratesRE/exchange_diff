using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Set", "OrganizationRelationship", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class SetOrganizationRelationship : SetSystemConfigurationObjectTask<OrganizationRelationshipIdParameter, OrganizationRelationship>
	{
		[Parameter(ValueFromPipelineByPropertyName = true)]
		public MultiValuedProperty<SmtpDomain> DomainNames
		{
			get
			{
				return (MultiValuedProperty<SmtpDomain>)base.Fields[OrganizationRelationshipSchema.DomainNames];
			}
			set
			{
				base.Fields[OrganizationRelationshipSchema.DomainNames] = value;
			}
		}

		[Parameter]
		public bool FreeBusyAccessEnabled
		{
			get
			{
				return (bool)base.Fields[OrganizationRelationshipSchema.FreeBusyAccessEnabled];
			}
			set
			{
				base.Fields[OrganizationRelationshipSchema.FreeBusyAccessEnabled] = value;
			}
		}

		[Parameter]
		public FreeBusyAccessLevel FreeBusyAccessLevel
		{
			get
			{
				return (FreeBusyAccessLevel)base.Fields[OrganizationRelationshipSchema.FreeBusyAccessLevel];
			}
			set
			{
				base.Fields[OrganizationRelationshipSchema.FreeBusyAccessLevel] = value;
			}
		}

		[Parameter]
		public GroupIdParameter FreeBusyAccessScope
		{
			get
			{
				return (GroupIdParameter)base.Fields["FreeBusyAccessScope"];
			}
			set
			{
				base.Fields["FreeBusyAccessScope"] = value;
			}
		}

		[Parameter]
		public bool MailboxMoveEnabled
		{
			get
			{
				return (bool)base.Fields[OrganizationRelationshipSchema.MailboxMoveEnabled];
			}
			set
			{
				base.Fields[OrganizationRelationshipSchema.MailboxMoveEnabled] = value;
			}
		}

		[Parameter]
		public bool DeliveryReportEnabled
		{
			get
			{
				return (bool)base.Fields[OrganizationRelationshipSchema.DeliveryReportEnabled];
			}
			set
			{
				base.Fields[OrganizationRelationshipSchema.DeliveryReportEnabled] = value;
			}
		}

		[Parameter]
		public bool MailTipsAccessEnabled
		{
			get
			{
				return (bool)base.Fields[OrganizationRelationshipSchema.MailTipsAccessEnabled];
			}
			set
			{
				base.Fields[OrganizationRelationshipSchema.MailTipsAccessEnabled] = value;
			}
		}

		[Parameter]
		public MailTipsAccessLevel MailTipsAccessLevel
		{
			get
			{
				return (MailTipsAccessLevel)base.Fields[OrganizationRelationshipSchema.MailTipsAccessLevel];
			}
			set
			{
				base.Fields[OrganizationRelationshipSchema.MailTipsAccessLevel] = value;
			}
		}

		[Parameter]
		public GroupIdParameter MailTipsAccessScope
		{
			get
			{
				return (GroupIdParameter)base.Fields["MailTipsAccessScope"];
			}
			set
			{
				base.Fields["MailTipsAccessScope"] = value;
			}
		}

		[Parameter]
		public bool ArchiveAccessEnabled
		{
			get
			{
				return (bool)base.Fields[OrganizationRelationshipSchema.ArchiveAccessEnabled];
			}
			set
			{
				base.Fields[OrganizationRelationshipSchema.ArchiveAccessEnabled] = value;
			}
		}

		[Parameter]
		public bool PhotosEnabled
		{
			get
			{
				return (bool)base.Fields[OrganizationRelationshipSchema.PhotosEnabled];
			}
			set
			{
				base.Fields[OrganizationRelationshipSchema.PhotosEnabled] = value;
			}
		}

		[Parameter(ValueFromPipelineByPropertyName = true)]
		public Uri TargetApplicationUri
		{
			get
			{
				return (Uri)base.Fields[OrganizationRelationshipSchema.TargetApplicationUri];
			}
			set
			{
				base.Fields[OrganizationRelationshipSchema.TargetApplicationUri] = value;
			}
		}

		[Parameter]
		public Uri TargetSharingEpr
		{
			get
			{
				return (Uri)base.Fields[OrganizationRelationshipSchema.TargetSharingEpr];
			}
			set
			{
				base.Fields[OrganizationRelationshipSchema.TargetSharingEpr] = value;
			}
		}

		[Parameter(ValueFromPipelineByPropertyName = true)]
		public Uri TargetAutodiscoverEpr
		{
			get
			{
				return (Uri)base.Fields[OrganizationRelationshipSchema.TargetAutodiscoverEpr];
			}
			set
			{
				base.Fields[OrganizationRelationshipSchema.TargetAutodiscoverEpr] = value;
			}
		}

		[Parameter(ValueFromPipelineByPropertyName = true)]
		public Uri TargetOwaURL
		{
			get
			{
				return (Uri)base.Fields[OrganizationRelationshipSchema.TargetOwaURL];
			}
			set
			{
				base.Fields[OrganizationRelationshipSchema.TargetOwaURL] = value;
			}
		}

		[Parameter]
		public SmtpAddress OrganizationContact
		{
			get
			{
				return (SmtpAddress)base.Fields[OrganizationRelationshipSchema.OrganizationContact];
			}
			set
			{
				base.Fields[OrganizationRelationshipSchema.OrganizationContact] = value;
			}
		}

		[Parameter]
		public bool Enabled
		{
			get
			{
				return (bool)base.Fields[OrganizationRelationshipSchema.Enabled];
			}
			set
			{
				base.Fields[OrganizationRelationshipSchema.Enabled] = value;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageSetOrganizationRelationship(this.Identity.ToString());
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter Force { get; set; }

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			base.InternalValidate();
			foreach (ADPropertyDefinition adpropertyDefinition in SetOrganizationRelationship.setProperties)
			{
				if (base.Fields.IsModified(adpropertyDefinition))
				{
					this.DataObject[adpropertyDefinition] = base.Fields[adpropertyDefinition];
				}
			}
			if (NewOrganizationRelationship.DomainsExist(this.DataObject.DomainNames, this.ConfigurationSession, new Guid?(this.DataObject.Guid)))
			{
				base.WriteError(new DuplicateOrganizationRelationshipDomainException(base.FormatMultiValuedProperty(this.DataObject.DomainNames)), ErrorCategory.InvalidOperation, this.Identity);
			}
			if (base.Fields.IsModified("FreeBusyAccessScope"))
			{
				if (this.FreeBusyAccessScope != null)
				{
					ADGroup adgroup = (ADGroup)base.GetDataObject<ADGroup>(this.FreeBusyAccessScope, base.TenantGlobalCatalogSession, null, new LocalizedString?(Strings.ErrorRecipientNotFound(this.FreeBusyAccessScope.ToString())), new LocalizedString?(Strings.ErrorRecipientNotUnique(this.FreeBusyAccessScope.ToString())));
					this.DataObject.FreeBusyAccessScope = adgroup.Id;
				}
				else
				{
					this.DataObject.FreeBusyAccessScope = null;
				}
			}
			if (base.Fields.IsModified("MailTipsAccessScope"))
			{
				if (this.MailTipsAccessScope != null)
				{
					ADGroup adgroup2 = (ADGroup)base.GetDataObject<ADGroup>(this.MailTipsAccessScope, base.TenantGlobalCatalogSession, null, new LocalizedString?(Strings.ErrorRecipientNotFound(this.MailTipsAccessScope.ToString())), new LocalizedString?(Strings.ErrorRecipientNotUnique(this.MailTipsAccessScope.ToString())));
					this.DataObject.MailTipsAccessScope = adgroup2.Id;
				}
				else
				{
					this.DataObject.MailTipsAccessScope = null;
				}
			}
			TaskLogger.LogExit();
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			if (!this.Force && this.DataObject.IsChanged(OrganizationRelationshipSchema.Enabled) && !base.ShouldContinue(this.DataObject.Enabled ? Strings.ConfirmationMessageEnableOrganizationRelationship(this.Identity.ToString()) : Strings.ConfirmationMessageDisableOrganizationRelationship(this.Identity.ToString())))
			{
				TaskLogger.LogExit();
				return;
			}
			base.InternalProcessRecord();
			TaskLogger.LogExit();
		}

		private static readonly PropertyDefinition[] setProperties = new PropertyDefinition[]
		{
			OrganizationRelationshipSchema.DomainNames,
			OrganizationRelationshipSchema.FreeBusyAccessEnabled,
			OrganizationRelationshipSchema.FreeBusyAccessLevel,
			OrganizationRelationshipSchema.MailboxMoveEnabled,
			OrganizationRelationshipSchema.DeliveryReportEnabled,
			OrganizationRelationshipSchema.MailTipsAccessEnabled,
			OrganizationRelationshipSchema.MailTipsAccessLevel,
			OrganizationRelationshipSchema.TargetApplicationUri,
			OrganizationRelationshipSchema.TargetOwaURL,
			OrganizationRelationshipSchema.TargetSharingEpr,
			OrganizationRelationshipSchema.TargetAutodiscoverEpr,
			OrganizationRelationshipSchema.OrganizationContact,
			OrganizationRelationshipSchema.ArchiveAccessEnabled,
			OrganizationRelationshipSchema.PhotosEnabled,
			OrganizationRelationshipSchema.Enabled
		};
	}
}
