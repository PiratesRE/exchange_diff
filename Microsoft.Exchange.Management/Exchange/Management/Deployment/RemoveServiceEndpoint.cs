using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.Deployment
{
	[Cmdlet("Remove", "ServiceEndpoint")]
	public sealed class RemoveServiceEndpoint : RemoveSystemConfigurationObjectTask<ADServiceConnectionPointIdParameter, ADServiceConnectionPoint>
	{
	}
}
