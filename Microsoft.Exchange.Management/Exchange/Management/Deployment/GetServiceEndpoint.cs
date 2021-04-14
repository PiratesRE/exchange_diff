using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.Deployment
{
	[Cmdlet("Get", "ServiceEndpoint", DefaultParameterSetName = "Identity")]
	public sealed class GetServiceEndpoint : GetSystemConfigurationObjectTask<ADServiceConnectionPointIdParameter, ADServiceConnectionPoint>
	{
		protected override bool DeepSearch
		{
			get
			{
				return true;
			}
		}

		protected override ObjectId RootId
		{
			get
			{
				IConfigurationSession configurationSession = (IConfigurationSession)base.DataSession;
				return configurationSession.GetOrgContainerId().GetChildId(ServiceEndpointContainer.DefaultName);
			}
		}
	}
}
