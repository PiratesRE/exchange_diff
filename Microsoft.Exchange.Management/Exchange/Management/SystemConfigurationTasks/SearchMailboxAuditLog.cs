using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.ApplicationLogic;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Management.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Search", "MailboxAuditLog", DefaultParameterSetName = "Identity")]
	public sealed class SearchMailboxAuditLog : GetTenantADObjectWithIdentityTaskBase<MailboxIdParameter, ADUser>
	{
		protected override bool DeepSearch
		{
			get
			{
				return true;
			}
		}

		protected override QueryFilter InternalFilter
		{
			get
			{
				QueryFilter internalFilter = base.InternalFilter;
				QueryFilter queryFilter = this.searchCriteria.GetRecipientFilter();
				if (internalFilter != null)
				{
					queryFilter = new AndFilter(new QueryFilter[]
					{
						internalFilter,
						queryFilter
					});
				}
				return queryFilter;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "MultipleMailboxesSearch")]
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

		[Parameter(Mandatory = false, ParameterSetName = "Identity")]
		public OrganizationIdParameter Organization
		{
			get
			{
				return (OrganizationIdParameter)base.Fields["Organization"];
			}
			set
			{
				base.Fields["Organization"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Identity")]
		[Parameter(Mandatory = false, ParameterSetName = "MultipleMailboxesSearch")]
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

		[Parameter(Mandatory = false, ParameterSetName = "Identity")]
		[Parameter(Mandatory = false, ParameterSetName = "MultipleMailboxesSearch")]
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

		[Parameter(Mandatory = false, ParameterSetName = "MultipleMailboxesSearch")]
		[Parameter(Mandatory = false, ParameterSetName = "Identity")]
		public bool? ExternalAccess
		{
			get
			{
				return (bool?)base.Fields["ExternalAccess"];
			}
			set
			{
				base.Fields["ExternalAccess"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Identity")]
		[ValidateRange(1, 250000)]
		[Parameter(Mandatory = false, ParameterSetName = "MultipleMailboxesSearch")]
		public int ResultSize
		{
			get
			{
				return (int)base.Fields["ResultSize"];
			}
			set
			{
				base.Fields["ResultSize"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Identity")]
		[Parameter(Mandatory = false, ParameterSetName = "MultipleMailboxesSearch")]
		public ExDateTime? StartDate
		{
			get
			{
				return (ExDateTime?)base.Fields["StartDate"];
			}
			set
			{
				base.Fields["StartDate"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Identity")]
		[Parameter(Mandatory = false, ParameterSetName = "MultipleMailboxesSearch")]
		public ExDateTime? EndDate
		{
			get
			{
				return (ExDateTime?)base.Fields["EndDate"];
			}
			set
			{
				base.Fields["EndDate"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Identity")]
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
				return Strings.ConfirmSearchMailboxAuditLogTask(base.CurrentOrgContainerId.ToString());
			}
		}

		protected override void InternalValidate()
		{
			base.InternalValidate();
			this.resultCount = 0;
			if (this.StartDate == null && this.EndDate == null)
			{
				this.EndDate = new ExDateTime?(ExDateTime.Now);
				this.StartDate = new ExDateTime?(this.EndDate.Value.AddDays(-15.0));
			}
			if (this.StartDate != null && this.EndDate == null)
			{
				this.EndDate = new ExDateTime?(this.StartDate.Value.AddDays(15.0));
			}
			if (this.StartDate == null && this.EndDate != null)
			{
				this.StartDate = new ExDateTime?(this.EndDate.Value.AddDays(-15.0));
			}
			if (this.StartDate.Value > this.EndDate.Value)
			{
				base.WriteError(new ArgumentException(Strings.ErrorMailboxAuditLogSearchStartDateIsLaterThanEndDate(this.StartDate.Value.ToString(), this.EndDate.Value.ToString())), ErrorCategory.InvalidArgument, null);
			}
			if (base.Fields["ResultSize"] == null)
			{
				this.ResultSize = 10000;
			}
			this.searchCriteria = new MailboxAuditLogSearch
			{
				OrganizationId = base.CurrentOrganizationId,
				LogonTypesUserInput = this.LogonTypes,
				OperationsUserInput = this.Operations,
				ShowDetails = this.ShowDetails.ToBool(),
				ExternalAccess = this.ExternalAccess
			};
			if (!this.StartDate.Value.HasTimeZone)
			{
				ExDateTime exDateTime = ExDateTime.Create(ExTimeZone.CurrentTimeZone, this.StartDate.Value.UniversalTime)[0];
				this.searchCriteria.StartDateUtc = new DateTime?(exDateTime.UniversalTime);
			}
			else
			{
				this.searchCriteria.StartDateUtc = new DateTime?(this.StartDate.Value.UniversalTime);
			}
			if (!this.EndDate.Value.HasTimeZone)
			{
				ExDateTime exDateTime2 = ExDateTime.Create(ExTimeZone.CurrentTimeZone, this.EndDate.Value.UniversalTime)[0];
				this.searchCriteria.EndDateUtc = new DateTime?(exDateTime2.UniversalTime);
			}
			else
			{
				this.searchCriteria.EndDateUtc = new DateTime?(this.EndDate.Value.UniversalTime);
			}
			this.searchCriteria.Mailboxes = MailboxAuditLogSearch.ConvertTo((IRecipientSession)base.DataSession, this.Mailboxes, new DataAccessHelper.GetDataObjectDelegate(base.GetDataObject<ADRecipient>), new Task.TaskErrorLoggingDelegate(base.WriteError));
			this.searchCriteria.Validate(new Task.TaskErrorLoggingDelegate(base.WriteError));
			this.searchStatistics = new AuditLogOpticsLogData();
			this.searchStatistics.OrganizationId = this.searchCriteria.OrganizationId;
			this.searchStatistics.SearchType = "Mailbox";
			this.searchStatistics.CallResult = false;
			this.searchStatistics.QueryComplexity = this.searchCriteria.QueryComplexity;
			this.searchStatistics.IsAsynchronous = false;
			this.searchStatistics.ShowDetails = this.searchCriteria.ShowDetails;
			this.searchStatistics.SearchStartDateTime = this.searchCriteria.StartDateUtc;
			this.searchStatistics.SearchEndDateTime = this.searchCriteria.EndDateUtc;
			this.worker = new MailboxAuditLogSearchWorker(600, this.searchCriteria, this.ResultSize, this.searchStatistics);
		}

		protected override OrganizationId ResolveCurrentOrganization()
		{
			if (this.Organization != null)
			{
				ADSessionSettings sessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopes(base.RootOrgContainerId, base.CurrentOrganizationId, base.ExecutingUserOrganizationId, true);
				IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(base.DomainController, true, ConsistencyMode.PartiallyConsistent, base.NetCredential, sessionSettings, ConfigScopes.TenantSubTree, 344, "ResolveCurrentOrganization", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\MailboxAuditLog\\SearchMailboxAuditLog.cs");
				tenantOrTopologyConfigurationSession.UseConfigNC = false;
				ADOrganizationalUnit adorganizationalUnit = (ADOrganizationalUnit)base.GetDataObject<ADOrganizationalUnit>(this.Organization, tenantOrTopologyConfigurationSession, null, new LocalizedString?(Strings.ErrorOrganizationNotFound(this.Organization.ToString())), new LocalizedString?(Strings.ErrorOrganizationNotUnique(this.Organization.ToString())));
				return adorganizationalUnit.OrganizationId;
			}
			return base.ResolveCurrentOrganization();
		}

		protected override void InternalProcessRecord()
		{
			DiagnosticContext.Reset();
			if (base.ParameterSetName == "MultipleMailboxesSearch")
			{
				if (this.Mailboxes != null && this.Mailboxes.Count == 0)
				{
					this.ProcessRecordInternal();
					return;
				}
				using (MultiValuedProperty<MailboxIdParameter>.Enumerator enumerator = this.Mailboxes.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						MailboxIdParameter identity = enumerator.Current;
						this.Identity = identity;
						this.ProcessRecordInternal();
					}
					return;
				}
			}
			this.ProcessRecordInternal();
		}

		private void ProcessRecordInternal()
		{
			TaskLogger.LogEnter(new object[]
			{
				this.Identity
			});
			if (this.Identity != null)
			{
				LocalizedString? localizedString;
				IEnumerable<ADUser> dataObjects = base.GetDataObjects<ADUser>(this.Identity, base.DataSession, this.RootId, base.OptionalIdentityData, out localizedString);
				this.WriteResult<ADUser>(dataObjects);
				if (!base.HasErrors && base.WriteObjectCount == 0U && !this.isUserObjValid)
				{
					base.WriteError(new ManagementObjectNotFoundException(localizedString ?? base.GetErrorMessageObjectNotFound(this.Identity.ToString(), typeof(ADUser).ToString(), (base.DataSession != null) ? base.DataSession.Source : null)), (ErrorCategory)1003, null);
				}
			}
			else
			{
				base.InternalProcessRecord();
			}
			TaskLogger.LogExit();
		}

		protected override void WriteResult(IConfigurable dataObject)
		{
			ADUser mailbox = (ADUser)dataObject;
			this.isUserObjValid = true;
			if (this.resultCount <= this.ResultSize)
			{
				DiagnosticContext.TraceLocation((LID)47804U);
				Exception ex = null;
				try
				{
					IEnumerable<MailboxAuditLogRecord> enumerable = this.worker.SearchMailboxAudits(mailbox);
					foreach (MailboxAuditLogRecord dataObject2 in enumerable)
					{
						this.resultCount++;
						if (this.resultCount > this.ResultSize)
						{
							DiagnosticContext.TraceLocation((LID)64188U);
							break;
						}
						base.WriteResult(dataObject2);
					}
				}
				catch (StorageTransientException ex2)
				{
					DiagnosticContext.TraceLocation((LID)39612U);
					TaskLogger.Trace("Search mailbox audit log failed with transient storage exception. {0}", new object[]
					{
						ex2
					});
					ex = ex2;
					base.WriteError(ex2, ErrorCategory.ReadError, null);
				}
				catch (StoragePermanentException ex3)
				{
					DiagnosticContext.TraceLocation((LID)55996U);
					TaskLogger.Trace("Search mailbox audit log failed with permanent storage exception. {0}", new object[]
					{
						ex3
					});
					ex = ex3;
					base.WriteError(ex3, ErrorCategory.ReadError, null);
				}
				catch (MailboxAuditLogSearchException ex4)
				{
					DiagnosticContext.TraceLocation((LID)43708U);
					ex = ex4;
					base.WriteError(ex4, ErrorCategory.NotSpecified, null);
				}
				catch (AuditLogException ex5)
				{
					DiagnosticContext.TraceLocation((LID)60092U);
					ex = ex5;
					base.WriteError(ex5, ErrorCategory.NotSpecified, null);
				}
				finally
				{
					if (this.searchStatistics != null)
					{
						this.searchStatistics.MailboxCount++;
						if (ex != null)
						{
							this.searchStatistics.ErrorType = ex;
							this.searchStatistics.ErrorCount++;
						}
					}
					TaskLogger.LogExit();
				}
			}
		}

		protected override IConfigDataProvider CreateSession()
		{
			TaskLogger.LogEnter();
			IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(base.DomainController, true, ConsistencyMode.PartiallyConsistent, base.SessionSettings, 531, "CreateSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\MailboxAuditLog\\SearchMailboxAuditLog.cs");
			TaskLogger.LogExit();
			return tenantOrRootOrgRecipientSession;
		}

		protected override void InternalEndProcessing()
		{
			TaskLogger.LogEnter();
			try
			{
				if (this.searchStatistics != null)
				{
					this.searchStatistics.CallResult = (this.searchStatistics.ErrorCount == 0);
					this.searchStatistics.Dispose();
					this.searchStatistics = null;
				}
			}
			finally
			{
				base.InternalEndProcessing();
			}
			TaskLogger.LogExit();
		}

		protected override void InternalStopProcessing()
		{
			TaskLogger.LogEnter();
			try
			{
				if (this.searchStatistics != null)
				{
					this.searchStatistics.Dispose();
					this.searchStatistics = null;
				}
			}
			finally
			{
				base.InternalStopProcessing();
			}
			TaskLogger.LogExit();
		}

		private const string ParameterSetMailboxes = "MultipleMailboxesSearch";

		private const int DefaultSearchTimePeriodInDays = 15;

		internal const int DefaultSearchTimeoutSeconds = 600;

		private const int DefaultSearchResultSize = 10000;

		private bool isUserObjValid;

		private int resultCount;

		private AuditLogOpticsLogData searchStatistics;

		private MailboxAuditLogSearchWorker worker;

		private MailboxAuditLogSearch searchCriteria;
	}
}
