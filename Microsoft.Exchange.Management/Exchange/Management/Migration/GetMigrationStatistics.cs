using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Migration.Logging;

namespace Microsoft.Exchange.Management.Migration
{
	[Cmdlet("Get", "MigrationStatistics")]
	public sealed class GetMigrationStatistics : MigrationSessionGetTaskBase<MigrationStatisticsIdParameter, MigrationStatistics>
	{
		public override string Action
		{
			get
			{
				return "GetMigrationStatistics";
			}
		}

		protected override IConfigDataProvider CreateSession()
		{
			IConfigDataProvider result = base.CreateSession();
			MigrationLogContext.Current.Source = "Get-MigrationStatistics";
			return result;
		}

		protected override OrganizationId ResolveCurrentOrganization()
		{
			if (!MapiTaskHelper.IsDatacenter && this.Identity == null)
			{
				this.Identity = new MigrationStatisticsIdParameter();
				this.Identity.Id = new MigrationStatisticsId(OrganizationId.ForestWideOrgId);
				return OrganizationId.ForestWideOrgId;
			}
			if (this.Identity == null)
			{
				this.Identity = new MigrationStatisticsIdParameter();
			}
			if (this.Identity.Id != null)
			{
				return this.Identity.Id.OrganizationId;
			}
			base.Organization = this.Identity.OrganizationIdentifier;
			OrganizationId organizationId = base.ResolveCurrentOrganization();
			this.Identity.Id = new MigrationStatisticsId(organizationId);
			return organizationId;
		}
	}
}
