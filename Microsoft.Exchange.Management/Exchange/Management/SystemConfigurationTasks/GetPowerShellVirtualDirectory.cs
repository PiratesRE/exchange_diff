using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Get", "PowerShellVirtualDirectory", DefaultParameterSetName = "Identity")]
	public sealed class GetPowerShellVirtualDirectory : GetPowerShellCommonVirtualDirectory<ADPowerShellVirtualDirectory>
	{
		protected override PowerShellVirtualDirectoryType AllowedVirtualDirectoryType
		{
			get
			{
				return PowerShellVirtualDirectoryType.PowerShell;
			}
		}

		protected override void ProcessMetabaseProperties(ExchangeVirtualDirectory dataObject)
		{
			TaskLogger.LogEnter();
			base.ProcessMetabaseProperties(dataObject);
			ADPowerShellVirtualDirectory adpowerShellVirtualDirectory = (ADPowerShellVirtualDirectory)dataObject;
			adpowerShellVirtualDirectory.RequireSSL = ExchangeServiceVDirHelper.IsSSLRequired(adpowerShellVirtualDirectory, new Task.TaskErrorLoggingDelegate(base.WriteError));
			TaskLogger.LogExit();
		}
	}
}
