using System;
using System.Management.Automation;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Install", "RecipientFilterConfig")]
	public sealed class InstallRecipientFilterConfig : InstallAntispamConfig<RecipientFilterConfig>
	{
		protected override string CanonicalName
		{
			get
			{
				return "RecipientFilterConfig";
			}
		}
	}
}
