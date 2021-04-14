using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("New", "HttpContainer")]
	public sealed class NewHttpContainer : NewFixedNameSystemConfigurationObjectTask<HttpContainer>
	{
		public NewHttpContainer()
		{
			this.serverName = Environment.MachineName;
		}

		protected override IConfigurable PrepareDataObject()
		{
			base.PrepareDataObject();
			this.DataObject.Name = NewHttpContainer.httpContainer;
			ADObjectId orgContainerId = ((IConfigurationSession)base.DataSession).GetOrgContainerId();
			ADObjectId childId = orgContainerId.GetChildId(NewHttpContainer.adminGroupContainer).GetChildId(NewHttpContainer.adminGroup).GetChildId(NewHttpContainer.serversContainer).GetChildId(this.serverName).GetChildId(NewHttpContainer.protocolsContainer).GetChildId(NewHttpContainer.httpContainer);
			this.DataObject.SetId(childId);
			return this.DataObject;
		}

		protected override void InternalProcessRecord()
		{
			ADObjectId orgContainerId = ((IConfigurationSession)base.DataSession).GetOrgContainerId();
			ADObjectId childId = orgContainerId.GetChildId(NewHttpContainer.adminGroupContainer).GetChildId(NewHttpContainer.adminGroup).GetChildId(NewHttpContainer.serversContainer).GetChildId(this.serverName);
			ProtocolsContainer protocolsContainer = new ProtocolsContainer();
			ADObjectId childId2 = childId.GetChildId(NewHttpContainer.protocolsContainer);
			protocolsContainer.SetId(childId2);
			try
			{
				base.DataSession.Save(protocolsContainer);
			}
			catch (ADObjectAlreadyExistsException)
			{
			}
			try
			{
				base.InternalProcessRecord();
			}
			catch (ADOperationException)
			{
			}
		}

		private static readonly string adminGroupContainer = "Administrative Groups";

		private static readonly string serversContainer = "Servers";

		private static readonly string protocolsContainer = "Protocols";

		private static readonly string httpContainer = "HTTP";

		private static readonly string adminGroup = AdministrativeGroup.DefaultName;

		private readonly string serverName;
	}
}
