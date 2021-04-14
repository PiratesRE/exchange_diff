using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Configuration.Tasks
{
	public abstract class NewFixedNameSystemConfigurationObjectTask<TDataObject> : NewADTaskBase<TDataObject> where TDataObject : ADObject, new()
	{
		protected override IConfigDataProvider CreateSession()
		{
			return DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(base.DomainController, false, ConsistencyMode.PartiallyConsistent, base.SessionSettings, 955, "CreateSession", "f:\\15.00.1497\\sources\\dev\\Configuration\\src\\ObjectModel\\BaseTasks\\NewAdObjectTask.cs");
		}

		protected void CreateParentContainerIfNeeded(ADObject dataObject)
		{
			if (dataObject == null)
			{
				throw new ArgumentNullException("dataObject");
			}
			ADObjectId adobjectId = dataObject.Identity as ADObjectId;
			if (adobjectId == null)
			{
				throw new ArgumentNullException("adObjectId");
			}
			ADObjectId parent = adobjectId.Parent;
			if (base.DataSession.Read<Container>(parent) == null)
			{
				Container container = new Container();
				container.SetId(parent);
				if (dataObject.OrganizationId != null)
				{
					container.OrganizationId = dataObject.OrganizationId;
				}
				IConfigurationSession configurationSession = (IConfigurationSession)base.DataSession;
				configurationSession.Save(container);
			}
		}
	}
}
