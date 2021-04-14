using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.Tasks
{
	[Cmdlet("Install", "AutoDiscoverContainer")]
	public sealed class InstallAutoDiscoverContainer : NewFixedNameSystemConfigurationObjectTask<ADContainer>
	{
		protected override IConfigDataProvider CreateSession()
		{
			return DirectorySessionFactory.Default.CreateTopologyConfigurationSession(base.DomainController, false, ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 28, "CreateSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\DirectorySetup\\InstallAutoDiscoverContainer.cs");
		}

		protected override IConfigurable PrepareDataObject()
		{
			ADContainer adcontainer = (ADContainer)base.PrepareDataObject();
			ITopologyConfigurationSession topologyConfigurationSession = (ITopologyConfigurationSession)base.DataSession;
			ADObjectId autoDiscoverGlobalContainerId = topologyConfigurationSession.GetAutoDiscoverGlobalContainerId();
			adcontainer.SetId(autoDiscoverGlobalContainerId);
			return adcontainer;
		}

		protected override void InternalProcessRecord()
		{
			try
			{
				base.InternalProcessRecord();
			}
			catch (ADObjectAlreadyExistsException)
			{
			}
		}
	}
}
