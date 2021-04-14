using System;
using System.Management.Automation;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Migration;
using Microsoft.Exchange.Migration.Logging;

namespace Microsoft.Exchange.Management.Migration
{
	[Cmdlet("Get", "MigrationUser", DefaultParameterSetName = "Identity")]
	public sealed class GetMigrationUser : MigrationGetTaskBase<MigrationUserIdParameter, MigrationUser>
	{
		private new SwitchParameter Diagnostic
		{
			get
			{
				return base.Diagnostic;
			}
			set
			{
				base.Diagnostic = value;
			}
		}

		private new string DiagnosticArgument
		{
			get
			{
				return base.DiagnosticArgument;
			}
			set
			{
				base.DiagnosticArgument = value;
			}
		}

		public GetMigrationUser()
		{
			base.InternalResultSize = new Unlimited<uint>(1000U);
		}

		public override string Action
		{
			get
			{
				return "GetMigrationUser";
			}
		}

		[Parameter]
		public Unlimited<uint> ResultSize
		{
			get
			{
				return base.InternalResultSize;
			}
			set
			{
				base.InternalResultSize = value;
			}
		}

		[ValidateNotNull]
		[Parameter(Mandatory = true, ParameterSetName = "MailboxGuid")]
		public Guid? MailboxGuid
		{
			get
			{
				return (Guid?)base.Fields["MailboxGuid"];
			}
			set
			{
				base.Fields["MailboxGuid"] = value;
			}
		}

		[Parameter(ParameterSetName = "StatusAndBatchId")]
		[ValidateNotNullOrEmpty]
		public MigrationBatchIdParameter BatchId
		{
			get
			{
				return (MigrationBatchIdParameter)base.Fields["BatchId"];
			}
			set
			{
				base.Fields["BatchId"] = value;
			}
		}

		[Parameter(ParameterSetName = "StatusAndBatchId")]
		[ValidateNotNull]
		public MigrationUserStatus? Status
		{
			get
			{
				return (MigrationUserStatus?)base.Fields["Status"];
			}
			set
			{
				base.Fields["Status"] = value;
			}
		}

		[ValidateNotNull]
		[Parameter(ParameterSetName = "StatusAndBatchId")]
		public MigrationUserStatusSummary? StatusSummary
		{
			get
			{
				return (MigrationUserStatusSummary?)base.Fields["StatusSummary"];
			}
			set
			{
				if (!MigrationUser.MapFromSummaryToStatus.ContainsKey(value.Value))
				{
					throw new ArgumentOutOfRangeException("StatusSummary", (int)value.Value, Strings.UnknownMigrationUserStatusSummaryValue);
				}
				base.Fields["StatusSummary"] = value;
			}
		}

		protected override IConfigDataProvider CreateSession()
		{
			MigrationLogger.Initialize();
			MigrationLogContext.Current.Source = "Get-MigrationUser";
			MigrationLogContext.Current.Organization = base.CurrentOrganizationId.OrganizationalUnit;
			return MigrationUserDataProvider.CreateDataProvider(this.Action, base.TenantGlobalCatalogSession, this.partitionMailbox, null);
		}

		protected override QueryFilter InternalFilter
		{
			get
			{
				if (base.ParameterSetName.Equals("MailboxGuid"))
				{
					if (this.MailboxGuid == null || this.MailboxGuid.Value == Guid.Empty)
					{
						return null;
					}
					return new ComparisonFilter(ComparisonOperator.Equal, MigrationUserSchema.MailboxGuid, this.MailboxGuid.Value);
				}
				else
				{
					if (base.ParameterSetName.Equals("StatusAndBatchId"))
					{
						QueryFilter queryFilter = null;
						if (this.Status != null)
						{
							queryFilter = new ComparisonFilter(ComparisonOperator.Equal, MigrationUserSchema.Status, this.Status.Value);
						}
						else if (this.StatusSummary != null)
						{
							queryFilter = new ComparisonFilter(ComparisonOperator.Equal, MigrationUserSchema.StatusSummary, this.StatusSummary.Value);
						}
						if (this.BatchId != null)
						{
							queryFilter = QueryFilter.AndTogether(new QueryFilter[]
							{
								queryFilter,
								new ComparisonFilter(ComparisonOperator.Equal, MigrationUserSchema.BatchId, this.BatchId.MigrationBatchId)
							});
						}
						return queryFilter;
					}
					return null;
				}
			}
		}

		private const string ParameterMailboxGuid = "MailboxGuid";

		private const string ParameterBatchId = "BatchId";

		private const string ParameterStatus = "Status";

		private const string ParameterStatusSummary = "StatusSummary";

		private const string ParameterSetStatusAndBatchId = "StatusAndBatchId";
	}
}
