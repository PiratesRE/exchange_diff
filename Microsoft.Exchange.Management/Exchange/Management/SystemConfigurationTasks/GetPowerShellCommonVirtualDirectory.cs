using System;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	public abstract class GetPowerShellCommonVirtualDirectory<T> : GetExchangeServiceVirtualDirectory<T> where T : ADPowerShellCommonVirtualDirectory, new()
	{
		protected abstract PowerShellVirtualDirectoryType AllowedVirtualDirectoryType { get; }

		protected override void ProcessMetabaseProperties(ExchangeVirtualDirectory dataObject)
		{
			TaskLogger.LogEnter();
			base.ProcessMetabaseProperties(dataObject);
			ADPowerShellCommonVirtualDirectory adpowerShellCommonVirtualDirectory = (ADPowerShellCommonVirtualDirectory)dataObject;
			adpowerShellCommonVirtualDirectory.CertificateAuthentication = base.GetCertificateAuthentication(dataObject);
			TaskLogger.LogExit();
		}

		protected override void WriteResult(IConfigurable dataObject)
		{
			ADPowerShellCommonVirtualDirectory adpowerShellCommonVirtualDirectory = dataObject as ADPowerShellCommonVirtualDirectory;
			if (adpowerShellCommonVirtualDirectory != null && adpowerShellCommonVirtualDirectory.VirtualDirectoryType == this.AllowedVirtualDirectoryType)
			{
				base.WriteResult(dataObject);
			}
		}
	}
}
