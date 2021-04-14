using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.WebServices.Data;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("New", "MailboxAuditLogSearch", DefaultParameterSetName = "Identity", SupportsShouldProcess = true)]
	public sealed class NewMailboxAuditLogSearch : NewAuditLogSearchBase<MailboxAuditLogSearch>
	{
		[Parameter(Mandatory = false)]
		public MultiValuedProperty<MailboxIdParameter> Mailboxes
		{
			get
			{
				return (MultiValuedProperty<MailboxIdParameter>)base.Fields["Mailboxes"];
			}
			set
			{
				base.Fields["Mailboxes"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<AuditScopes> LogonTypes
		{
			get
			{
				return (MultiValuedProperty<AuditScopes>)base.Fields["LogonTypes"];
			}
			set
			{
				base.Fields["LogonTypes"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<MailboxAuditOperations> Operations
		{
			get
			{
				return (MultiValuedProperty<MailboxAuditOperations>)base.Fields["Operations"];
			}
			set
			{
				base.Fields["Operations"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter ShowDetails
		{
			get
			{
				return (SwitchParameter)(base.Fields["ShowDetails"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["ShowDetails"] = value;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageNewMailboxAuditLogSearch(base.CurrentOrgContainerId.ToString());
			}
		}

		protected override void InternalValidate()
		{
			if (NewMailboxAuditLogSearch.MailboxAuditLogSearchRequestThreshold == null)
			{
				int? num = NewAuditLogSearchBase<MailboxAuditLogSearch>.ReadIntegerAppSetting("AsyncMailboxAuditLogSearchRequestThreshold");
				if (num == null || num < 1)
				{
					NewMailboxAuditLogSearch.MailboxAuditLogSearchRequestThreshold = new int?(50);
				}
				else
				{
					NewMailboxAuditLogSearch.MailboxAuditLogSearchRequestThreshold = num;
				}
			}
			NewMailboxAuditLogSearch.SearchDataProvider searchDataProvider = (NewMailboxAuditLogSearch.SearchDataProvider)base.DataSession;
			if (searchDataProvider.GetAuditLogSearchCount() >= NewMailboxAuditLogSearch.MailboxAuditLogSearchRequestThreshold.Value)
			{
				base.WriteError(new InvalidOperationException(Strings.CannotCreateAuditLogSearchDueToSearchQuota), ErrorCategory.QuotaExceeded, null);
			}
			base.InternalValidate();
		}

		internal override IConfigDataProvider InternalCreateSearchDataProvider(ExchangePrincipal principal, OrganizationId organizationId)
		{
			return new NewMailboxAuditLogSearch.SearchDataProvider(principal, organizationId);
		}

		protected override IConfigurable PrepareDataObject()
		{
			base.PrepareDataObject();
			this.DataObject.Mailboxes = MailboxAuditLogSearch.ConvertTo(this.recipientSession, this.Mailboxes, new DataAccessHelper.GetDataObjectDelegate(base.GetDataObject<ADRecipient>), new Task.TaskErrorLoggingDelegate(base.WriteError));
			if (this.LogonTypes != null)
			{
				this.DataObject.LogonTypesUserInput = this.LogonTypes;
			}
			if (this.Operations != null)
			{
				this.DataObject.OperationsUserInput = this.Operations;
			}
			this.DataObject.ShowDetails = this.ShowDetails;
			this.DataObject.Validate(new Task.TaskErrorLoggingDelegate(base.WriteError));
			return this.DataObject;
		}

		private const string MailboxAuditLogSearchRequestThresholdKey = "AsyncMailboxAuditLogSearchRequestThreshold";

		private const int DefaultMailboxAuditLogSearchRequestThreshold = 50;

		private static int? MailboxAuditLogSearchRequestThreshold = null;

		private class SearchDataProvider : AuditLogSearchEwsDataProvider
		{
			public SearchDataProvider(ExchangePrincipal primaryMailbox, OrganizationId organizationId) : base(primaryMailbox)
			{
				this.organizationId = organizationId;
			}

			public override void Save(IConfigurable instance)
			{
				MailboxAuditLogSearch mailboxAuditLogSearch = (MailboxAuditLogSearch)instance;
				MailboxAuditLogSearch mailboxAuditLogSearch2 = new MailboxAuditLogSearch();
				mailboxAuditLogSearch2.Identity = (AuditLogSearchId)mailboxAuditLogSearch.Identity;
				mailboxAuditLogSearch2.Name = mailboxAuditLogSearch.Name;
				mailboxAuditLogSearch2.StartDateUtc = new DateTime?(mailboxAuditLogSearch.StartDateUtc.Value);
				mailboxAuditLogSearch2.EndDateUtc = new DateTime?(mailboxAuditLogSearch.EndDateUtc.Value);
				mailboxAuditLogSearch2.StatusMailRecipients = NewAuditLogSearchBase<MailboxAuditLogSearch>.GetMultiValuedSmptAddressAsStrings(mailboxAuditLogSearch.StatusMailRecipients);
				mailboxAuditLogSearch2.CreatedBy = mailboxAuditLogSearch.CreatedBy;
				mailboxAuditLogSearch2.CreatedByEx = mailboxAuditLogSearch.CreatedByEx;
				mailboxAuditLogSearch2.Mailboxes = mailboxAuditLogSearch.Mailboxes;
				mailboxAuditLogSearch2.LogonTypes = mailboxAuditLogSearch.LogonTypes;
				mailboxAuditLogSearch2.Operations = mailboxAuditLogSearch.Operations;
				mailboxAuditLogSearch2.ShowDetails = new bool?(mailboxAuditLogSearch.ShowDetails);
				if (mailboxAuditLogSearch.ExternalAccess != null)
				{
					mailboxAuditLogSearch2.ExternalAccess = (mailboxAuditLogSearch.ExternalAccess.Value ? bool.TrueString : bool.FalseString);
				}
				base.Save(mailboxAuditLogSearch2);
				AuditQueuesOpticsLogData auditQueuesOpticsLogData = new AuditQueuesOpticsLogData
				{
					QueueType = AuditQueueType.AsyncMailboxSearch,
					EventType = QueueEventType.Queue,
					CorrelationId = mailboxAuditLogSearch2.Identity.Guid.ToString(),
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
					using (IEnumerator<MailboxAuditLogSearch> enumerator = this.FindInFolder<MailboxAuditLogSearch>(filter, this.GetDefaultFolder()).GetEnumerator())
					{
						if (enumerator.MoveNext())
						{
							MailboxAuditLogSearch mailboxAuditLogSearch = enumerator.Current;
							MailboxAuditLogSearch mailboxAuditLogSearch2 = new MailboxAuditLogSearch();
							mailboxAuditLogSearch2.SetId(mailboxAuditLogSearch.Identity);
							mailboxAuditLogSearch2.Name = mailboxAuditLogSearch.Name;
							mailboxAuditLogSearch2.StartDateUtc = new DateTime?(mailboxAuditLogSearch.StartDateUtc.Value);
							mailboxAuditLogSearch2.EndDateUtc = new DateTime?(mailboxAuditLogSearch.EndDateUtc.Value);
							mailboxAuditLogSearch2.StatusMailRecipients = NewAuditLogSearchBase<MailboxAuditLogSearch>.GetMultiValuedStringsAsSmptAddresses(mailboxAuditLogSearch.StatusMailRecipients);
							mailboxAuditLogSearch2.CreatedBy = mailboxAuditLogSearch.CreatedBy;
							mailboxAuditLogSearch2.CreatedByEx = mailboxAuditLogSearch.CreatedByEx;
							mailboxAuditLogSearch2.Mailboxes = mailboxAuditLogSearch.Mailboxes;
							mailboxAuditLogSearch2.LogonTypes = mailboxAuditLogSearch.LogonTypes;
							mailboxAuditLogSearch2.Operations = mailboxAuditLogSearch.Operations;
							mailboxAuditLogSearch2.ShowDetails = (mailboxAuditLogSearch.ShowDetails != null && mailboxAuditLogSearch.ShowDetails.Value);
							bool value;
							if (!string.IsNullOrEmpty(mailboxAuditLogSearch.ExternalAccess) && bool.TryParse(mailboxAuditLogSearch.ExternalAccess, out value))
							{
								mailboxAuditLogSearch2.ExternalAccess = new bool?(value);
							}
							return mailboxAuditLogSearch2;
						}
					}
				}
				return null;
			}

			protected override FolderId GetDefaultFolder()
			{
				if (this.defaultFolder == null)
				{
					this.defaultFolder = base.GetOrCreateFolder("MailboxAuditLogSearch");
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
