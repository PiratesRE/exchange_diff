using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Get", "AuthConfig")]
	public sealed class GetAuthConfig : GetSingletonSystemConfigurationObjectTask<AuthConfig>
	{
	}
}
