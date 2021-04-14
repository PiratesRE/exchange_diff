using System;
using System.Management.Automation;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Get", "OutlookAnywhere", DefaultParameterSetName = "Identity")]
	public sealed class GetRpcHttp : GetExchangeVirtualDirectory<ADRpcHttpVirtualDirectory>
	{
		protected override bool CanIgnoreMissingMetabaseEntry()
		{
			return true;
		}

		protected override LocalizedString GetMissingMetabaseEntryWarning(ExchangeVirtualDirectory vdir)
		{
			return Strings.WarnRpcHttpAdOrphanFound(vdir.Id.Name, ADVirtualDirectory.GetServerNameFromVDirObjectId(vdir.Id));
		}
	}
}
