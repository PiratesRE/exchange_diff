using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage.Auditing;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Management.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Search", "AdminAuditLog", DefaultParameterSetName = "Identity")]
	public sealed class SearchAdminAuditLog : GetMultitenancySingletonSystemConfigurationObjectTask<AdminAuditLogConfig>
	{
		[Parameter(Mandatory = false, ParameterSetName = "Identity", ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
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

		[Parameter(Mandatory = false, ParameterSetName = "Identity")]
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

		[Parameter(Mandatory = false, ParameterSetName = "Identity")]
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

		[Parameter(Mandatory = false, ParameterSetName = "Identity")]
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

		[Parameter(Mandatory = false, ParameterSetName = "Identity")]
		public bool? IsSuccess
		{
			get
			{
				return (bool?)base.Fields["IsSuccess"];
			}
			set
			{
				base.Fields["IsSuccess"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Identity")]
		public int StartIndex
		{
			get
			{
				return (int)base.Fields["StartIndex"];
			}
			set
			{
				base.Fields["StartIndex"] = value;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmSearchAdminAuditLogConfigTask(base.CurrentOrgContainerId.ToString());
			}
		}

		protected override ObjectId RootId
		{
			get
			{
				ADObjectId orgContainerId = base.CurrentOrgContainerId;
				if (base.SharedConfiguration != null)
				{
					orgContainerId = base.SharedConfiguration.SharedConfigurationCU.Id;
				}
				return AdminAuditLogConfig.GetWellKnownParentLocation(orgContainerId);
			}
		}

		protected override SharedTenantConfigurationMode SharedTenantConfigurationMode
		{
			get
			{
				return SharedTenantConfigurationMode.Static;
			}
		}

		protected override void InternalValidate()
		{
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
				base.WriteError(new ArgumentException(Strings.AdminAuditLogSearchStartDateIsLaterThanEndDate(this.StartDate.Value.ToString(), this.EndDate.Value.ToString())), ErrorCategory.InvalidArgument, null);
			}
			if (base.Fields["ResultSize"] == null)
			{
				this.ResultSize = 1000;
			}
			int num = 250000;
			if (base.Fields["StartIndex"] == null)
			{
				this.StartIndex = 0;
			}
			if (this.StartIndex < 0 || this.StartIndex > num)
			{
				base.WriteError(new ArgumentOutOfRangeException("StartIndex", this.StartIndex, Strings.AdminAuditLogSearchOutOfRangeStartIndex(num)), ErrorCategory.InvalidArgument, null);
			}
			this.searchObject = new AdminAuditLogSearch
			{
				OrganizationId = base.CurrentOrganizationId,
				Cmdlets = this.Cmdlets,
				Parameters = this.Parameters,
				ObjectIds = this.ObjectIds,
				UserIdsUserInput = this.UserIds,
				Succeeded = this.IsSuccess,
				StartIndex = this.StartIndex,
				ExternalAccess = this.ExternalAccess,
				ResultSize = this.ResultSize,
				RedactDatacenterAdmins = !AdminAuditExternalAccessDeterminer.IsExternalAccess(base.SessionSettings.ExecutingUserIdentityName, base.SessionSettings.ExecutingUserOrganizationId, base.SessionSettings.CurrentOrganizationId)
			};
			if (!this.StartDate.Value.HasTimeZone)
			{
				ExDateTime exDateTime = ExDateTime.Create(ExTimeZone.CurrentTimeZone, this.StartDate.Value.UniversalTime)[0];
				this.searchObject.StartDateUtc = new DateTime?(exDateTime.UniversalTime);
			}
			else
			{
				this.searchObject.StartDateUtc = new DateTime?(this.StartDate.Value.UniversalTime);
			}
			if (!this.EndDate.Value.HasTimeZone)
			{
				ExDateTime exDateTime2 = ExDateTime.Create(ExTimeZone.CurrentTimeZone, this.EndDate.Value.UniversalTime)[0];
				this.searchObject.EndDateUtc = new DateTime?(exDateTime2.UniversalTime);
			}
			else
			{
				this.searchObject.EndDateUtc = new DateTime?(this.EndDate.Value.UniversalTime);
			}
			AdminAuditLogHelper.SetResolveUsers(this.searchObject, new DataAccessHelper.GetDataObjectDelegate(base.GetDataObject<ADRecipient>), new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), new Task.TaskWarningLoggingDelegate(this.WriteWarning));
			this.searchObject.Validate(new Task.TaskErrorLoggingDelegate(base.WriteError));
			base.InternalValidate();
		}

		protected override void WriteResult<T>(IEnumerable<T> dataObjects)
		{
			try
			{
				TaskLogger.LogEnter(new object[]
				{
					dataObjects
				});
				DiagnosticContext.Reset();
				using (AuditLogOpticsLogData auditLogOpticsLogData = new AuditLogOpticsLogData())
				{
					auditLogOpticsLogData.IsAsynchronous = false;
					auditLogOpticsLogData.CallResult = false;
					auditLogOpticsLogData.SearchStartDateTime = this.searchObject.StartDateUtc;
					auditLogOpticsLogData.SearchEndDateTime = this.searchObject.EndDateUtc;
					try
					{
						if (AdminAuditLogHelper.ShouldIssueWarning(base.CurrentOrganizationId))
						{
							DiagnosticContext.TraceLocation((LID)42684U);
							this.WriteWarning(Strings.WarningSearchAdminAuditLogOnPreE15(base.CurrentOrganizationId.ToString()));
						}
						else
						{
							if (dataObjects != null)
							{
								using (IEnumerator<AdminAuditLogConfig> enumerator = (IEnumerator<AdminAuditLogConfig>)dataObjects.GetEnumerator())
								{
									this.GetAuditConfigObject(enumerator);
									auditLogOpticsLogData.SearchType = "Admin";
									auditLogOpticsLogData.OrganizationId = this.searchObject.OrganizationId;
									auditLogOpticsLogData.ShowDetails = true;
									auditLogOpticsLogData.MailboxCount = 1;
									AdminAuditLogSearchWorker adminAuditLogSearchWorker = new AdminAuditLogSearchWorker(600, this.searchObject, auditLogOpticsLogData);
									base.WriteVerbose(Strings.VerboseStartAuditLogSearch);
									AdminAuditLogEvent[] array = adminAuditLogSearchWorker.Search();
									base.WriteVerbose(Strings.VerboseSearchCompleted((array != null) ? array.Length : 0));
									foreach (AdminAuditLogEvent dataObject in array)
									{
										this.WriteResult(dataObject);
									}
									auditLogOpticsLogData.CallResult = true;
									goto IL_181;
								}
							}
							DiagnosticContext.TraceLocation((LID)59068U);
							Exception ex = new AdminAuditLogSearchException(Strings.ErrorAdminAuditLogConfig(base.CurrentOrganizationId.ToString()));
							auditLogOpticsLogData.ErrorType = ex;
							auditLogOpticsLogData.ErrorCount++;
							base.WriteError(ex, ErrorCategory.ObjectNotFound, null);
							IL_181:;
						}
					}
					catch (ArgumentException ex2)
					{
						DiagnosticContext.TraceLocation((LID)34492U);
						auditLogOpticsLogData.ErrorType = ex2;
						auditLogOpticsLogData.ErrorCount++;
						base.WriteError(ex2, ErrorCategory.InvalidArgument, null);
					}
					catch (AdminAuditLogSearchException ex3)
					{
						DiagnosticContext.TraceLocation((LID)50876U);
						auditLogOpticsLogData.ErrorType = ex3;
						auditLogOpticsLogData.ErrorCount++;
						base.WriteError(ex3, ErrorCategory.NotSpecified, null);
					}
				}
			}
			finally
			{
				TaskLogger.LogExit();
			}
		}

		private AdminAuditLogConfig GetAuditConfigObject(IEnumerator<AdminAuditLogConfig> dataObjects)
		{
			TaskLogger.LogEnter(new object[]
			{
				dataObjects
			});
			base.WriteDebug(Strings.DebugStartRetrievingAuditConfig);
			dataObjects.MoveNext();
			AdminAuditLogConfig adminAuditLogConfig = dataObjects.Current;
			if (adminAuditLogConfig == null || adminAuditLogConfig.Identity == null)
			{
				base.WriteError(new AdminAuditLogSearchException(Strings.ErrorAdminAuditLogConfig(base.CurrentOrganizationId.ToString())), ErrorCategory.ObjectNotFound, null);
			}
			TaskLogger.LogExit();
			return adminAuditLogConfig;
		}

		internal const int defaultSearchTimeoutSeconds = 600;

		private const int DefaultSearchResultSize = 1000;

		private const int DefaultSearchTimePeriodInDays = 15;

		private AdminAuditLogSearch searchObject;
	}
}
