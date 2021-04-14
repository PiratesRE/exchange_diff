using System;
using System.Management.Automation;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Metabase;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Get", "ActiveSyncVirtualDirectory", DefaultParameterSetName = "Identity")]
	public sealed class GetMobileSyncVirtualDirectory : GetExchangeVirtualDirectory<ADMobileVirtualDirectory>
	{
		protected override void ProcessMetabaseProperties(ExchangeVirtualDirectory dataObject)
		{
			ADMobileVirtualDirectory vdirObject = (ADMobileVirtualDirectory)dataObject;
			try
			{
				MobileSyncVirtualDirectoryHelper.ReadFromMetabase(vdirObject, this);
			}
			catch (IISNotInstalledException exception)
			{
				this.WriteError(exception, ErrorCategory.ObjectNotFound, null, false);
			}
			catch (UnauthorizedAccessException exception2)
			{
				this.WriteError(exception2, ErrorCategory.PermissionDenied, null, false);
			}
		}

		protected override bool CanIgnoreMissingMetabaseEntry()
		{
			return true;
		}
	}
}
