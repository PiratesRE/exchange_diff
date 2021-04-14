using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Get", "EdgeSubscription", DefaultParameterSetName = "Identity")]
	public sealed class GetEdgeSubscription : GetSystemConfigurationObjectTask<TransportServerIdParameter, Server>
	{
		protected override QueryFilter InternalFilter
		{
			get
			{
				return new BitMaskOrFilter(ServerSchema.CurrentServerRole, 64UL);
			}
		}

		protected override bool DeepSearch
		{
			get
			{
				return true;
			}
		}

		protected override void WriteResult(IConfigurable dataObject)
		{
			TaskLogger.LogEnter(new object[]
			{
				dataObject.Identity,
				dataObject
			});
			Server server = (Server)dataObject;
			if (server.GatewayEdgeSyncSubscribed)
			{
				base.WriteResult(new EdgeSubscription(server));
			}
			else
			{
				base.WriteVerbose(Strings.EdgeServerNotSubscribed);
			}
			TaskLogger.LogExit();
		}
	}
}
