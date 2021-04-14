using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.Tasks
{
	[Cmdlet("install", "OrganizationalUnit")]
	public sealed class InstallOrganizationalUnitTask : NewSystemConfigurationObjectTask<ADOrganizationalUnit>
	{
		[Parameter(Mandatory = false)]
		public SwitchParameter MSOSyncEnabled
		{
			get
			{
				return (SwitchParameter)(base.Fields[ADOrganizationalUnitSchema.MSOSyncEnabled] ?? false);
			}
			set
			{
				base.Fields[ADOrganizationalUnitSchema.MSOSyncEnabled] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter SMTPAddressCheckWithAcceptedDomain
		{
			get
			{
				return (SwitchParameter)(base.Fields[ADOrganizationalUnitSchema.SMTPAddressCheckWithAcceptedDomain] ?? true);
			}
			set
			{
				base.Fields[ADOrganizationalUnitSchema.SMTPAddressCheckWithAcceptedDomain] = value;
			}
		}

		[Parameter(Mandatory = true)]
		public Guid AccountPartition
		{
			get
			{
				return (Guid)(base.Fields["Partition"] ?? Guid.Empty);
			}
			set
			{
				base.Fields["Partition"] = value;
			}
		}

		protected override IConfigDataProvider CreateSession()
		{
			ITenantConfigurationSession tenantConfigurationSession = DirectorySessionFactory.Default.CreateTenantConfigurationSession(base.DomainController, false, ConsistencyMode.PartiallyConsistent, null, ADSessionSettings.FromAllTenantsPartitionId(new PartitionId(this.AccountPartition)), 67, "CreateSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\DirectorySetup\\InstallOrganizationalUnitTask.cs");
			tenantConfigurationSession.UseConfigNC = false;
			return tenantConfigurationSession;
		}

		protected override IConfigurable PrepareDataObject()
		{
			ADOrganizationalUnit adorganizationalUnit = (ADOrganizationalUnit)base.PrepareDataObject();
			ADObjectId adobjectId = ((ITenantConfigurationSession)base.DataSession).GetHostedOrganizationsRoot();
			adobjectId = adobjectId.GetChildId("OU", base.Name);
			adorganizationalUnit.SetId(adobjectId);
			adorganizationalUnit.ConfigurationUnit = null;
			adorganizationalUnit.MSOSyncEnabled = this.MSOSyncEnabled;
			adorganizationalUnit.SMTPAddressCheckWithAcceptedDomain = this.SMTPAddressCheckWithAcceptedDomain;
			return adorganizationalUnit;
		}

		protected override void InternalProcessRecord()
		{
			base.InternalProcessRecord();
			if (!base.HasErrors)
			{
				ADObjectId childId = this.DataObject.Id.GetChildId("OU", "Soft Deleted Objects");
				ADOrganizationalUnit adorganizationalUnit = new ADOrganizationalUnit();
				adorganizationalUnit.SetId(childId);
				base.DataSession.Save(adorganizationalUnit);
			}
		}
	}
}
