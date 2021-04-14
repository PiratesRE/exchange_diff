using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Get", "FederationTrust", DefaultParameterSetName = "Identity")]
	public sealed class GetFederationTrust : GetSystemConfigurationObjectTask<FederationTrustIdParameter, FederationTrust>
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
			return base.ReadWriteRootOrgGlobalConfigSession;
		}
	}
}
