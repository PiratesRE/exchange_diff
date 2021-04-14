using System;
using System.Management.Automation;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Install", "SenderIdConfig")]
	public sealed class InstallSenderIdConfig : InstallAntispamConfig<SenderIdConfig>
	{
		protected override string CanonicalName
		{
			get
			{
				return "SenderIdConfig";
			}
		}
	}
}
