using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Auditing;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.WebServices.Data;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("New", "AdminAuditLogSearch", DefaultParameterSetName = "Identity", SupportsShouldProcess = true)]
	public sealed class NewAdminAuditLogSearch : NewAuditLogSearchBase<AdminAuditLogSearch>
	{
		[Parameter(Mandatory = false)]
		public MultiValuedProperty<string> Cmdlets
		{
			get
			{
				return (MultiValuedProperty<string>)base.Fields["Cmdlets"];
			}
			set
			{
				base.Fields["Cmdlets"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<string> Parameters
		{
			get
			{
				return (MultiValuedProperty<string>)base.Fields["Parameters"];
			}
			set
			{
				base.Fields["Parameters"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<string> ObjectIds
		{
			get
			{
				return (MultiValuedProperty<string>)base.Fields["ObjectIds"];
			}
			set
			{
				base.Fields["ObjectIds"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<SecurityPrincipalIdParameter> UserIds
		{
			get
			{
				return (MultiValuedProperty<SecurityPrincipalIdParameter>)base.Fields["UserIds"];
			}
			set
			{
				base.Fields["UserIds"] = value;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageNewAdminAuditLogSearch(base.CurrentOrgContainerId.ToString());
			}
		}

		protected override void InternalValidate()
		{
			if (NewAdminAuditLogSearch.AdminAuditLogSearchRequestThreshold == null)
			{
				int? num = NewAuditLogSearchBase<AdminAuditLogSearch>.ReadIntegerAppSetting("AsyncAdminAuditLogSearchRequestThreshold");
				if (num == null || num < 1)
				{
					NewAdminAuditLogSearch.AdminAuditLogSearchRequestThreshold = new int?(50);
				}
				else
				{
					NewAdminAuditLogSearch.AdminAuditLogSearchRequestThreshold = num;
				}
			}
			NewAdminAuditLogSearch.SearchDataProvider searchDataProvider = (NewAdminAuditLogSearch.SearchDataProvider)base.DataSession;
			if (searchDataProvider.GetAuditLogSearchCount() >= NewAdminAuditLogSearch.AdminAuditLogSearchRequestThreshold.Value)
			{
				base.WriteError(new InvalidOperationException(Strings.CannotCreateAuditLogSearchDueToSearchQuota), ErrorCategory.QuotaExceeded, null);
			}
			base.InternalValidate();
		}

		internal override IConfigDataProvider InternalCreateSearchDataProvider(ExchangePrincipal principal, OrganizationId organizationId)
		{
			return new NewAdminAuditLogSearch.SearchDataProvider(principal, organizationId);
		}

		protected override IConfigurable PrepareDataObject()
		{
			base.PrepareDataObject();
			if (this.Cmdlets != null)
			{
				this.DataObject.Cmdlets = this.Cmdlets;
			}
			if (this.Parameters != null)
			{
				this.DataObject.Parameters = this.Parameters;
			}
			if (this.ObjectIds != null)
			{
				this.DataObject.ObjectIds = this.ObjectIds;
			}
			if (this.UserIds != null)
			{
				this.DataObject.UserIdsUserInput = this.UserIds;
			}
			this.DataObject.Succeeded = null;
			this.DataObject.StartIndex = 0;
			this.DataObject.ResultSize = 50000;
			this.DataObject.RedactDatacenterAdmins = !AdminAuditExternalAccessDeterminer.IsExternalAccess(base.SessionSettings.ExecutingUserIdentityName, base.SessionSettings.ExecutingUserOrganizationId, base.SessionSettings.CurrentOrganizationId);
			AdminAuditLogHelper.SetResolveUsers(this.DataObject, new DataAccessHelper.GetDataObjectDelegate(base.GetDataObject<ADRecipient>), new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), new Task.TaskWarningLoggingDelegate(this.WriteWarning));
			this.DataObject.Validate(new Task.TaskErrorLoggingDelegate(base.WriteError));
			return this.DataObject;
		}

		protected override void InternalProcessRecord()
		{
			if (AdminAuditLogHelper.ShouldIssueWarning(base.CurrentOrganizationId))
			{
				this.WriteWarning(Strings.WarningNewAdminAuditLogSearchOnPreE15(base.CurrentOrganizationId.ToString()));
				return;
			}
			base.InternalProcessRecord();
		}

		private const string AdminAuditLogSearchRequestThresholdKey = "AsyncAdminAuditLogSearchRequestThreshold";

		private const int DefaultAdminAuditLogSearchRequestThreshold = 50;

		private static int? AdminAuditLogSearchRequestThreshold = null;

		private class SearchDataProvider : AuditLogSearchEwsDataProvider
		{
			public SearchDataProvider(ExchangePrincipal primaryMailbox, OrganizationId organizationId) : base(primaryMailbox)
			{
				this.organizationId = organizationId;
			}

			public override void Save(IConfigurable instance)
			{
				AdminAuditLogSearch adminAuditLogSearch = (AdminAuditLogSearch)instance;
				AdminAuditLogSearch adminAuditLogSearch2 = new AdminAuditLogSearch();
				adminAuditLogSearch2.Identity = (AuditLogSearchId)adminAuditLogSearch.Identity;
				adminAuditLogSearch2.Name = adminAuditLogSearch.Name;
				adminAuditLogSearch2.StartDateUtc = new DateTime?(adminAuditLogSearch.StartDateUtc.Value);
				adminAuditLogSearch2.EndDateUtc = new DateTime?(adminAuditLogSearch.EndDateUtc.Value);
				adminAuditLogSearch2.StatusMailRecipients = NewAuditLogSearchBase<AdminAuditLogSearch>.GetMultiValuedSmptAddressAsStrings(adminAuditLogSearch.StatusMailRecipients);
				adminAuditLogSearch2.CreatedBy = adminAuditLogSearch.CreatedBy;
				adminAuditLogSearch2.CreatedByEx = adminAuditLogSearch.CreatedByEx;
				adminAuditLogSearch2.Cmdlets = adminAuditLogSearch.Cmdlets;
				adminAuditLogSearch2.Parameters = adminAuditLogSearch.Parameters;
				adminAuditLogSearch2.ObjectIds = adminAuditLogSearch.ObjectIds;
				if (adminAuditLogSearch.ExternalAccess != null)
				{
					adminAuditLogSearch2.ExternalAccess = (adminAuditLogSearch.ExternalAccess.Value ? bool.TrueString : bool.FalseString);
				}
				adminAuditLogSearch2.UserIds = adminAuditLogSearch.UserIds;
				adminAuditLogSearch2.ResolvedUsers = adminAuditLogSearch.ResolvedUsers;
				adminAuditLogSearch2.RedactDatacenterAdmins = adminAuditLogSearch.RedactDatacenterAdmins;
				base.Save(adminAuditLogSearch2);
				AuditQueuesOpticsLogData auditQueuesOpticsLogData = new AuditQueuesOpticsLogData
				{
					QueueType = AuditQueueType.AsyncAdminSearch,
					EventType = QueueEventType.Queue,
					CorrelationId = adminAuditLogSearch2.Identity.Guid.ToString(),
					OrganizationId = this.organizationId,
					QueueLength = ((this.defaultFolder != null) ? (this.defaultFolder.TotalCount + 1) : 1)
				};
				auditQueuesOpticsLogData.Log();
				instance.ResetChangeTracking();
			}

			public override IConfigurable Read<T>(ObjectId identity)
			{
				AuditLogSearchId auditLogSearchId = identity as AuditLogSearchId;
				if (auditLogSearchId != null)
				{
					SearchFilter filter = new SearchFilter.IsEqualTo(AuditLogSearchBaseEwsSchema.Identity.StorePropertyDefinition, auditLogSearchId.Guid.ToString());
					using (IEnumerator<AdminAuditLogSearch> enumerator = this.FindInFolder<AdminAuditLogSearch>(filter, this.GetDefaultFolder()).GetEnumerator())
					{
						if (enumerator.MoveNext())
						{
							AdminAuditLogSearch adminAuditLogSearch = enumerator.Current;
							AdminAuditLogSearch adminAuditLogSearch2 = new AdminAuditLogSearch();
							adminAuditLogSearch2.SetId(adminAuditLogSearch.Identity);
							adminAuditLogSearch2.Name = adminAuditLogSearch.Name;
							adminAuditLogSearch2.StartDateUtc = new DateTime?(adminAuditLogSearch.StartDateUtc.Value);
							adminAuditLogSearch2.EndDateUtc = new DateTime?(adminAuditLogSearch.EndDateUtc.Value);
							adminAuditLogSearch2.StatusMailRecipients = NewAuditLogSearchBase<AdminAuditLogSearch>.GetMultiValuedStringsAsSmptAddresses(adminAuditLogSearch.StatusMailRecipients);
							adminAuditLogSearch2.CreatedBy = adminAuditLogSearch.CreatedBy;
							adminAuditLogSearch2.CreatedByEx = adminAuditLogSearch.CreatedByEx;
							adminAuditLogSearch2.Cmdlets = adminAuditLogSearch.Cmdlets;
							adminAuditLogSearch2.Parameters = adminAuditLogSearch.Parameters;
							adminAuditLogSearch2.ObjectIds = adminAuditLogSearch.ObjectIds;
							bool value;
							if (!string.IsNullOrEmpty(adminAuditLogSearch.ExternalAccess) && bool.TryParse(adminAuditLogSearch.ExternalAccess, out value))
							{
								adminAuditLogSearch2.ExternalAccess = new bool?(value);
							}
							adminAuditLogSearch2.UserIds = adminAuditLogSearch.UserIds;
							adminAuditLogSearch2.ResolvedUsers = adminAuditLogSearch.ResolvedUsers;
							return adminAuditLogSearch2;
						}
					}
				}
				return null;
			}

			protected override FolderId GetDefaultFolder()
			{
				if (this.defaultFolder == null)
				{
					this.defaultFolder = base.GetOrCreateFolder("AdminAuditLogSearch");
				}
				return this.defaultFolder.Id;
			}

			public int GetAuditLogSearchCount()
			{
				int result;
				try
				{
					this.GetDefaultFolder();
					result = this.defaultFolder.TotalCount;
				}
				catch (LocalizedException)
				{
					result = 0;
				}
				return result;
			}

			private OrganizationId organizationId;

			private Folder defaultFolder;
		}
	}
}
