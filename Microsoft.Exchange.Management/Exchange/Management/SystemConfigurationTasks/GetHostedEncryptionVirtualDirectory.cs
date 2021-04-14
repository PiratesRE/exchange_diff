using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Get", "HostedEncryptionVirtualDirectory", DefaultParameterSetName = "Identity")]
	public sealed class GetHostedEncryptionVirtualDirectory : GetExchangeVirtualDirectory<ADE4eVirtualDirectory>
	{
		protected override void ProcessMetabaseProperties(ExchangeVirtualDirectory dataObject)
		{
			TaskLogger.LogEnter();
			base.ProcessMetabaseProperties(dataObject);
			ADE4eVirtualDirectory ade4eVirtualDirectory = (ADE4eVirtualDirectory)dataObject;
			ade4eVirtualDirectory.LoadSettings();
			WebAppVirtualDirectoryHelper.UpdateFromMetabase(ade4eVirtualDirectory);
			TaskLogger.LogExit();
		}

		protected override bool CanIgnoreMissingMetabaseEntry()
		{
			return true;
		}
	}
}
