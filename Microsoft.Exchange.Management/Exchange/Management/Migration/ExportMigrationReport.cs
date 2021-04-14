using System;
using System.IO;
using System.Management.Automation;
using System.Web;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Migration;
using Microsoft.Exchange.Migration.Logging;

namespace Microsoft.Exchange.Management.Migration
{
	[Cmdlet("Export", "MigrationReport", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class ExportMigrationReport : ObjectActionTenantADTask<MigrationReportIdParameter, MigrationReport>
	{
		[Parameter(Mandatory = true, ParameterSetName = "StreamBased")]
		public Stream CsvStream { get; set; }

		[Parameter(Mandatory = true, ParameterSetName = "Paged")]
		public int StartingRowIndex
		{
			get
			{
				return (int)(base.Fields["StartingRowIndex"] ?? 0);
			}
			set
			{
				base.Fields["StartingRowIndex"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "Paged")]
		public int RowCount
		{
			get
			{
				return (int)(base.Fields["RowCount"] ?? 0);
			}
			set
			{
				base.Fields["RowCount"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "StreamBased", ValueFromPipeline = true, ValueFromPipelineByPropertyName = true, Position = 0)]
		[Parameter(Mandatory = true, ParameterSetName = "Paged", ValueFromPipeline = true, ValueFromPipelineByPropertyName = true, Position = 0)]
		public override MigrationReportIdParameter Identity
		{
			get
			{
				return base.Identity;
			}
			set
			{
				base.Identity = value;
			}
		}

		[Parameter(Mandatory = false)]
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

		[Parameter(Mandatory = false)]
		public MailboxIdParameter Partition
		{
			get
			{
				return (MailboxIdParameter)base.Fields["Partition"];
			}
			set
			{
				base.Fields["Partition"] = value;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageExportMigrationReport(this.TenantName);
			}
		}

		private string TenantName
		{
			get
			{
				if (!(base.CurrentOrganizationId != null) || base.CurrentOrganizationId.OrganizationalUnit == null)
				{
					return string.Empty;
				}
				return base.CurrentOrganizationId.OrganizationalUnit.Name;
			}
		}

		protected override IConfigDataProvider CreateSession()
		{
			MigrationLogger.Initialize();
			MigrationLogContext.Current.Source = "Export-MigrationReport";
			MigrationLogContext.Current.Organization = base.CurrentOrganizationId.OrganizationalUnit;
			return MigrationReportDataProvider.CreateDataProvider("ExportMigrationReport", base.TenantGlobalCatalogSession, this.CsvStream, this.StartingRowIndex, this.RowCount, this.partitionMailbox, object.Equals(base.ExecutingUserOrganizationId, base.CurrentOrganizationId));
		}

		protected override void InternalProcessRecord()
		{
			base.WriteObject(this.DataObject);
			base.InternalProcessRecord();
		}

		protected override void InternalBeginProcessing()
		{
			if (this.Organization != null)
			{
				base.CurrentOrganizationId = this.ResolveCurrentOrganization();
			}
			this.partitionMailbox = MigrationObjectTaskBase<MigrationReportIdParameter>.ResolvePartitionMailbox(this.Partition, base.TenantGlobalCatalogSession, base.ServerSettings, new DataAccessHelper.CategorizedGetDataObjectDelegate(base.GetDataObject<ADUser>), new Task.ErrorLoggerDelegate(base.WriteError), base.CurrentOrganizationId == OrganizationId.ForestWideOrgId && MapiTaskHelper.IsDatacenter);
			base.InternalBeginProcessing();
		}

		protected override bool IsKnownException(Exception exception)
		{
			return MigrationBatchDataProvider.IsKnownException(exception) || exception is HttpException || base.IsKnownException(exception);
		}

		protected override void InternalStateReset()
		{
			this.DisposeSession();
			base.InternalStateReset();
		}

		protected override void InternalValidate()
		{
			if ((base.Fields.IsChanged("RowCount") || base.Fields.IsModified("RowCount")) && (this.RowCount < MigrationConstraints.ExportMigrationReportRowCountConstraint.MinimumValue || this.RowCount > MigrationConstraints.ExportMigrationReportRowCountConstraint.MaximumValue))
			{
				this.WriteError(new MigrationPermanentException(Strings.ExportMigrationBatchRowCountOutOfBoundsException(this.RowCount, MigrationConstraints.ExportMigrationReportRowCountConstraint.MinimumValue, MigrationConstraints.ExportMigrationReportRowCountConstraint.MaximumValue)));
			}
			if ((base.Fields.IsChanged("StartingRowIndex") || base.Fields.IsModified("StartingRowIndex")) && this.StartingRowIndex < 0)
			{
				this.WriteError(new MigrationPermanentException(Strings.ExportMigrationBatchStartingRowIndexOutOfBoundException(this.StartingRowIndex)));
			}
			base.InternalValidate();
		}

		protected override void Dispose(bool disposing)
		{
			try
			{
				if (!this.disposed)
				{
					if (disposing)
					{
						this.DisposeSession();
					}
					this.disposed = true;
				}
			}
			finally
			{
				base.Dispose(disposing);
			}
		}

		private void WriteError(LocalizedException exception)
		{
			MigrationLogger.Log(MigrationEventType.Warning, MigrationLogger.GetDiagnosticInfo(exception, null), new object[0]);
			base.WriteError(exception, (ErrorCategory)1000, null);
		}

		private OrganizationId ResolveCurrentOrganization()
		{
			ADObjectId rootOrgContainerIdForLocalForest = ADSystemConfigurationSession.GetRootOrgContainerIdForLocalForest();
			ADSessionSettings sessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopes(rootOrgContainerIdForLocalForest, base.CurrentOrganizationId, base.ExecutingUserOrganizationId, true);
			IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(base.DomainController, true, ConsistencyMode.PartiallyConsistent, null, sessionSettings, 362, "ResolveCurrentOrganization", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\Migration\\ExportMigrationReport.cs");
			tenantOrTopologyConfigurationSession.UseConfigNC = false;
			ADOrganizationalUnit adorganizationalUnit = (ADOrganizationalUnit)base.GetDataObject<ADOrganizationalUnit>(this.Organization, tenantOrTopologyConfigurationSession, null, new LocalizedString?(Strings.ErrorOrganizationNotFound(this.Organization.ToString())), new LocalizedString?(Strings.ErrorOrganizationNotUnique(this.Organization.ToString())));
			return adorganizationalUnit.OrganizationId;
		}

		private void DisposeSession()
		{
			if (base.DataSession is IDisposable)
			{
				MigrationLogger.Close();
				((IDisposable)base.DataSession).Dispose();
			}
		}

		private const string ParameterSetStream = "StreamBased";

		private const string ParameterSetPaged = "Paged";

		private const string ParameterStartingRowIndex = "StartingRowIndex";

		private const string ParameterRowCount = "RowCount";

		private const string Action = "ExportMigrationReport";

		private bool disposed;

		private ADUser partitionMailbox;
	}
}
