using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Get", "AuthServer")]
	public sealed class GetAuthServer : GetSystemConfigurationObjectTask<AuthServerIdParameter, AuthServer>
	{
		protected override bool DeepSearch
		{
			get
			{
				return true;
			}
		}

		protected override IConfigDataProvider CreateSession()
		{
			return base.RootOrgGlobalConfigSession;
		}
	}
}
