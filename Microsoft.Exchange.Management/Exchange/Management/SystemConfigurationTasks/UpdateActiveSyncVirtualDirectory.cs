using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Update", "ActiveSyncVirtualDirectory")]
	public sealed class UpdateActiveSyncVirtualDirectory : SetMobileSyncVirtualDirectoryBase
	{
		protected override bool IsInSetup
		{
			get
			{
				return true;
			}
		}

		protected override IConfigurable PrepareDataObject()
		{
			ADMobileVirtualDirectory admobileVirtualDirectory = (ADMobileVirtualDirectory)base.PrepareDataObject();
			if (base.HasErrors)
			{
				return null;
			}
			if (admobileVirtualDirectory.ExchangeVersion.IsOlderThan(admobileVirtualDirectory.MaximumSupportedExchangeObjectVersion))
			{
				admobileVirtualDirectory.SetExchangeVersion(admobileVirtualDirectory.MaximumSupportedExchangeObjectVersion);
			}
			return admobileVirtualDirectory;
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			base.InternalProcessRecord();
			try
			{
				MobileSyncVirtualDirectoryHelper.InstallProxySubDirectory(this.DataObject, this);
			}
			catch (Exception ex)
			{
				TaskLogger.Trace("Exception occurred in InstallProxySubDirectory(): {0}", new object[]
				{
					ex.Message
				});
				this.WriteWarning(Strings.ActiveSyncMetabaseProxyInstallFailure);
				throw;
			}
			TaskLogger.LogExit();
		}
	}
}
