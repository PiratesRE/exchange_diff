using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	public class GetTransportServiceBase : GetSystemConfigurationObjectTask<TransportServerIdParameter, Server>
	{
		protected override QueryFilter InternalFilter
		{
			get
			{
				return new BitMaskOrFilter(ServerSchema.CurrentServerRole, 96UL);
			}
		}

		protected override bool DeepSearch
		{
			get
			{
				return true;
			}
		}

		protected override void InternalValidate()
		{
			Server server;
			try
			{
				server = ((ITopologyConfigurationSession)base.DataSession).ReadLocalServer();
			}
			catch (TransientException exception)
			{
				base.WriteError(exception, ErrorCategory.ResourceUnavailable, null);
				return;
			}
			if (server != null)
			{
				this.isTaskOnEdge = server.IsEdgeServer;
				return;
			}
		}

		protected override void WriteResult(IConfigurable dataObject)
		{
			Server server = (Server)dataObject;
			if (this.isTaskOnEdge && server.IsHubTransportServer)
			{
				return;
			}
			TaskLogger.LogEnter(new object[]
			{
				dataObject.Identity,
				dataObject
			});
			base.WriteResult(new TransportServer(server));
			TaskLogger.LogExit();
		}

		private bool isTaskOnEdge;
	}
}
