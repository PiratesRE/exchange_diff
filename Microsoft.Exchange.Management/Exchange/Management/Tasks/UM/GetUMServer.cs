using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.Tasks.UM
{
	[Cmdlet("Get", "UMService", DefaultParameterSetName = "Identity")]
	public sealed class GetUMServer : GetSystemConfigurationObjectTask<UMServerIdParameter, Server>
	{
		protected override QueryFilter InternalFilter
		{
			get
			{
				return new BitMaskAndFilter(ServerSchema.CurrentServerRole, 16UL);
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
			base.WriteResult(new UMServer((Server)dataObject));
			TaskLogger.LogExit();
		}
	}
}
