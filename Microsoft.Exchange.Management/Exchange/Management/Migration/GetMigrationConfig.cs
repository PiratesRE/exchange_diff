using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Migration.Logging;

namespace Microsoft.Exchange.Management.Migration
{
	[Cmdlet("Get", "MigrationConfig")]
	public sealed class GetMigrationConfig : MigrationSessionGetTaskBase<MigrationConfigIdParameter, MigrationConfig>
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

		public override string Action
		{
			get
			{
				return "GetMigrationConfig";
			}
		}

		protected override IConfigDataProvider CreateSession()
		{
			IConfigDataProvider result = base.CreateSession();
			MigrationLogContext.Current.Source = "Get-MigrationConfig";
			return result;
		}

		protected override OrganizationId ResolveCurrentOrganization()
		{
			if (!MapiTaskHelper.IsDatacenter && this.Identity == null)
			{
				this.Identity = new MigrationConfigIdParameter();
				this.Identity.Id = new MigrationConfigId(OrganizationId.ForestWideOrgId);
				return OrganizationId.ForestWideOrgId;
			}
			if (this.Identity == null)
			{
				this.Identity = new MigrationConfigIdParameter();
			}
			if (this.Identity.Id != null)
			{
				return this.Identity.Id.OrganizationId;
			}
			base.Organization = this.Identity.OrganizationIdentifier;
			OrganizationId organizationId = base.ResolveCurrentOrganization();
			this.Identity.Id = new MigrationConfigId(organizationId);
			return organizationId;
		}
	}
}
