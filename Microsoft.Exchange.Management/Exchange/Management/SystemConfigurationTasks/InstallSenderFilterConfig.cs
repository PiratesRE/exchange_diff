using System;
using System.Management.Automation;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Install", "SenderFilterConfig")]
	public sealed class InstallSenderFilterConfig : InstallAntispamConfig<SenderFilterConfig>
	{
		protected override string CanonicalName
		{
			get
			{
				return "SenderFilterConfig";
			}
		}
	}
}
