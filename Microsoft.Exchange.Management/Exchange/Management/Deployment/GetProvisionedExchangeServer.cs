using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Management.Deployment
{
	[Cmdlet("Get", "ProvisionedExchangeServer")]
	[ClassAccessLevel(AccessLevel.Consumer)]
	public sealed class GetProvisionedExchangeServer : GetSystemConfigurationObjectTask<ServerIdParameter, Server>
	{
		protected override void WriteResult(IConfigurable dataObject)
		{
			Server server = (Server)dataObject;
			if (server.IsProvisionedServer)
			{
				base.WriteResult(dataObject);
			}
		}
	}
}
