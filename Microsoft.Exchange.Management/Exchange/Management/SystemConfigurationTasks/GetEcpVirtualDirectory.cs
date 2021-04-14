using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Get", "EcpVirtualDirectory", DefaultParameterSetName = "Identity")]
	public sealed class GetEcpVirtualDirectory : GetExchangeVirtualDirectory<ADEcpVirtualDirectory>
	{
		protected override void ProcessMetabaseProperties(ExchangeVirtualDirectory dataObject)
		{
			TaskLogger.LogEnter();
			base.ProcessMetabaseProperties(dataObject);
			ADEcpVirtualDirectory webAppVirtualDirectory = (ADEcpVirtualDirectory)dataObject;
			WebAppVirtualDirectoryHelper.UpdateFromMetabase(webAppVirtualDirectory);
			TaskLogger.LogExit();
		}

		protected override bool CanIgnoreMissingMetabaseEntry()
		{
			return true;
		}

		protected override LocalizedString GetMissingMetabaseEntryWarning(ExchangeVirtualDirectory vdir)
		{
			return Strings.EcpAdOrphanFound(vdir.Id.Name);
		}
	}
}
