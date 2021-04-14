using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Set", "MServSyncConfigFlags", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class SetMServSyncConfigFlags : SetSystemConfigurationObjectTask<OrganizationIdParameter, ADOrganizationalUnit>
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

		[Parameter(Mandatory = false)]
		public SwitchParameter SyncMBXAndDLToMserv
		{
			get
			{
				return (SwitchParameter)(base.Fields[ADOrganizationalUnitSchema.SyncMBXAndDLToMserv] ?? false);
			}
			set
			{
				base.Fields[ADOrganizationalUnitSchema.SyncMBXAndDLToMserv] = value;
			}
		}

		protected override IConfigDataProvider CreateSession()
		{
			IConfigurationSession configurationSession = (IConfigurationSession)base.CreateSession();
			configurationSession.UseConfigNC = false;
			configurationSession.UseGlobalCatalog = true;
			configurationSession.EnforceDefaultScope = false;
			return configurationSession;
		}

		protected override IConfigurable PrepareDataObject()
		{
			ADOrganizationalUnit adorganizationalUnit = (ADOrganizationalUnit)base.PrepareDataObject();
			if (base.Fields.IsModified(ADOrganizationalUnitSchema.SMTPAddressCheckWithAcceptedDomain))
			{
				adorganizationalUnit.SMTPAddressCheckWithAcceptedDomain = this.SMTPAddressCheckWithAcceptedDomain;
			}
			if (base.Fields.IsModified(ADOrganizationalUnitSchema.MSOSyncEnabled))
			{
				adorganizationalUnit.MSOSyncEnabled = this.MSOSyncEnabled;
			}
			if (base.Fields.IsModified(ADOrganizationalUnitSchema.SyncMBXAndDLToMserv))
			{
				adorganizationalUnit.SyncMBXAndDLToMServ = this.SyncMBXAndDLToMserv;
			}
			return adorganizationalUnit;
		}
	}
}
