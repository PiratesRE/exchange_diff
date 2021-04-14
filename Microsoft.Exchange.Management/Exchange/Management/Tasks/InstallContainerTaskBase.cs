using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.Tasks
{
	public class InstallContainerTaskBase<TDataObject> : NewMultitenancyFixedNameSystemConfigurationObjectTask<TDataObject> where TDataObject : ADConfigurationObject, new()
	{
		[Parameter(Mandatory = true, Position = 0)]
		public string[] Name
		{
			get
			{
				return (string[])base.Fields["Name"];
			}
			set
			{
				base.Fields["Name"] = value;
			}
		}

		protected virtual ADObjectId GetBaseContainer()
		{
			return base.CurrentOrgContainerId;
		}

		protected override IConfigDataProvider CreateSession()
		{
			base.CreateSession();
			if (base.Organization != null)
			{
				return DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(base.DomainController, false, ConsistencyMode.PartiallyConsistent, null, base.SessionSettings, 64, "CreateSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\DirectorySetup\\InstallContainerTaskBase.cs");
			}
			return DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(base.DomainController, false, ConsistencyMode.PartiallyConsistent, null, base.SessionSettings, ConfigScopes.TenantSubTree, 74, "CreateSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\DirectorySetup\\InstallContainerTaskBase.cs");
		}

		protected override IConfigurable PrepareDataObject()
		{
			TDataObject tdataObject = (TDataObject)((object)base.PrepareDataObject());
			ADObjectId adobjectId = this.GetBaseContainer();
			foreach (string unescapedCommonName in this.Name)
			{
				adobjectId = adobjectId.GetChildId(unescapedCommonName);
			}
			tdataObject.SetId(adobjectId);
			return tdataObject;
		}

		protected override void InternalProcessRecord()
		{
			IConfigDataProvider dataSession = base.DataSession;
			TDataObject dataObject = this.DataObject;
			if (dataSession.Read<TDataObject>(dataObject.Id) == null)
			{
				base.InternalProcessRecord();
			}
		}
	}
}
