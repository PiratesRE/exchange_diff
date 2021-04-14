using System;
using System.Collections.Generic;
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
	[Cmdlet("New", "OrganizationRelationship", SupportsShouldProcess = true)]
	public sealed class NewOrganizationRelationship : NewMultitenancySystemConfigurationObjectTask<OrganizationRelationship>
	{
		[Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true)]
		public MultiValuedProperty<SmtpDomain> DomainNames
		{
			get
			{
				return this.DataObject.DomainNames;
			}
			set
			{
				this.DataObject.DomainNames = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool FreeBusyAccessEnabled
		{
			get
			{
				return this.DataObject.FreeBusyAccessEnabled;
			}
			set
			{
				this.DataObject.FreeBusyAccessEnabled = value;
			}
		}

		[Parameter(Mandatory = false)]
		public FreeBusyAccessLevel FreeBusyAccessLevel
		{
			get
			{
				return this.DataObject.FreeBusyAccessLevel;
			}
			set
			{
				this.DataObject.FreeBusyAccessLevel = value;
			}
		}

		[Parameter(Mandatory = false)]
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

		[Parameter(Mandatory = false)]
		public bool MailboxMoveEnabled
		{
			get
			{
				return this.DataObject.MailboxMoveEnabled;
			}
			set
			{
				this.DataObject.MailboxMoveEnabled = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool DeliveryReportEnabled
		{
			get
			{
				return this.DataObject.DeliveryReportEnabled;
			}
			set
			{
				this.DataObject.DeliveryReportEnabled = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool MailTipsAccessEnabled
		{
			get
			{
				return this.DataObject.MailTipsAccessEnabled;
			}
			set
			{
				this.DataObject.MailTipsAccessEnabled = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MailTipsAccessLevel MailTipsAccessLevel
		{
			get
			{
				return this.DataObject.MailTipsAccessLevel;
			}
			set
			{
				this.DataObject.MailTipsAccessLevel = value;
			}
		}

		[Parameter(Mandatory = false)]
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

		[Parameter(Mandatory = false)]
		public bool ArchiveAccessEnabled
		{
			get
			{
				return this.DataObject.ArchiveAccessEnabled;
			}
			set
			{
				this.DataObject.ArchiveAccessEnabled = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool PhotosEnabled
		{
			get
			{
				return this.DataObject.PhotosEnabled;
			}
			set
			{
				this.DataObject.PhotosEnabled = value;
			}
		}

		[Parameter(Mandatory = false, ValueFromPipelineByPropertyName = true)]
		public Uri TargetApplicationUri
		{
			get
			{
				return this.DataObject.TargetApplicationUri;
			}
			set
			{
				this.DataObject.TargetApplicationUri = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Uri TargetSharingEpr
		{
			get
			{
				return this.DataObject.TargetSharingEpr;
			}
			set
			{
				this.DataObject.TargetSharingEpr = value;
			}
		}

		[Parameter(Mandatory = false, ValueFromPipelineByPropertyName = true)]
		public Uri TargetAutodiscoverEpr
		{
			get
			{
				return this.DataObject.TargetAutodiscoverEpr;
			}
			set
			{
				this.DataObject.TargetAutodiscoverEpr = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SmtpAddress OrganizationContact
		{
			get
			{
				return this.DataObject.OrganizationContact;
			}
			set
			{
				this.DataObject.OrganizationContact = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool Enabled
		{
			get
			{
				return this.DataObject.Enabled;
			}
			set
			{
				this.DataObject.Enabled = value;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageNewOrganizationRelationship(base.Name, base.FormatMultiValuedProperty(this.DomainNames));
			}
		}

		[Parameter(Mandatory = false, ValueFromPipelineByPropertyName = true)]
		public Uri TargetOwaURL
		{
			get
			{
				return this.DataObject.TargetOwaURL;
			}
			set
			{
				this.DataObject.TargetOwaURL = value;
			}
		}

		internal static bool DomainsExist(MultiValuedProperty<SmtpDomain> domains, IConfigurationSession configurationSession)
		{
			return NewOrganizationRelationship.DomainsExist(domains, configurationSession, null);
		}

		internal static bool DomainsExist(MultiValuedProperty<SmtpDomain> domains, IConfigurationSession configurationSession, Guid? objectToExclude)
		{
			List<ComparisonFilter> list = new List<ComparisonFilter>(domains.Count);
			foreach (SmtpDomain smtpDomain in domains)
			{
				list.Add(new ComparisonFilter(ComparisonOperator.Equal, OrganizationRelationshipSchema.DomainNames, smtpDomain.Domain));
			}
			QueryFilter queryFilter;
			if (list.Count == 1)
			{
				queryFilter = list[0];
			}
			else
			{
				queryFilter = new OrFilter(list.ToArray());
			}
			if (objectToExclude != null)
			{
				queryFilter = new AndFilter(new QueryFilter[]
				{
					new ComparisonFilter(ComparisonOperator.NotEqual, ADObjectSchema.Guid, objectToExclude.Value),
					queryFilter
				});
			}
			OrganizationRelationship[] array = configurationSession.Find<OrganizationRelationship>(configurationSession.GetOrgContainerId(), QueryScope.SubTree, queryFilter, null, 1);
			return array.Length > 0;
		}

		protected override IConfigurable PrepareDataObject()
		{
			OrganizationRelationship organizationRelationship = (OrganizationRelationship)base.PrepareDataObject();
			organizationRelationship.SetId((IConfigurationSession)base.DataSession, base.Name);
			return organizationRelationship;
		}

		protected override void InternalBeginProcessing()
		{
			TaskLogger.LogEnter();
			base.InternalBeginProcessing();
			TaskLogger.LogExit();
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			SharedConfigurationTaskHelper.VerifyIsNotTinyTenant(base.CurrentOrgState, new Task.ErrorLoggerDelegate(base.WriteError));
			base.InternalValidate();
			if (NewOrganizationRelationship.DomainsExist(this.DataObject.DomainNames, this.ConfigurationSession))
			{
				base.WriteError(new DuplicateOrganizationRelationshipDomainException(base.FormatMultiValuedProperty(this.DataObject.DomainNames)), ErrorCategory.InvalidOperation, base.Name);
			}
			if (this.FreeBusyAccessScopeADGroup != null)
			{
				this.DataObject.FreeBusyAccessScope = this.FreeBusyAccessScopeADGroup.Id;
			}
			if (this.MailTipsAccessScopeADGroup != null)
			{
				this.DataObject.MailTipsAccessScope = this.MailTipsAccessScopeADGroup.Id;
			}
			TaskLogger.LogExit();
		}

		protected override void WriteResult(IConfigurable dataObject)
		{
			TaskLogger.LogEnter();
			if (this.FreeBusyAccessScopeADGroup != null || this.MailTipsAccessScopeADGroup != null)
			{
				OrganizationRelationship organizationRelationship = (OrganizationRelationship)dataObject;
				if (this.FreeBusyAccessScopeADGroup != null)
				{
					organizationRelationship[OrganizationRelationshipNonAdProperties.FreeBusyAccessScopeCache] = this.FreeBusyAccessScopeADGroup.Id;
				}
				if (this.MailTipsAccessScopeADGroup != null)
				{
					organizationRelationship[OrganizationRelationshipNonAdProperties.MailTipsAccessScopeScopeCache] = this.MailTipsAccessScopeADGroup.Id;
				}
			}
			base.WriteResult(dataObject);
			TaskLogger.LogExit();
		}

		private ADGroup FreeBusyAccessScopeADGroup
		{
			get
			{
				if (this.freeBusyAccessScopeAdGroup == null && this.FreeBusyAccessScope != null)
				{
					this.freeBusyAccessScopeAdGroup = (ADGroup)base.GetDataObject<ADGroup>(this.FreeBusyAccessScope, base.TenantGlobalCatalogSession, null, new LocalizedString?(Strings.ErrorRecipientNotFound(this.FreeBusyAccessScope.ToString())), new LocalizedString?(Strings.ErrorRecipientNotUnique(this.FreeBusyAccessScope.ToString())));
				}
				return this.freeBusyAccessScopeAdGroup;
			}
		}

		private ADGroup MailTipsAccessScopeADGroup
		{
			get
			{
				if (this.mailTipsAccessScopeAdGroup == null && this.MailTipsAccessScope != null)
				{
					this.mailTipsAccessScopeAdGroup = (ADGroup)base.GetDataObject<ADGroup>(this.MailTipsAccessScope, base.TenantGlobalCatalogSession, null, new LocalizedString?(Strings.ErrorRecipientNotFound(this.MailTipsAccessScope.ToString())), new LocalizedString?(Strings.ErrorRecipientNotUnique(this.MailTipsAccessScope.ToString())));
				}
				return this.mailTipsAccessScopeAdGroup;
			}
		}

		internal const string MailTipsAccessScopeFieldName = "MailTipsAccessScope";

		private ADGroup freeBusyAccessScopeAdGroup;

		private ADGroup mailTipsAccessScopeAdGroup;
	}
}
