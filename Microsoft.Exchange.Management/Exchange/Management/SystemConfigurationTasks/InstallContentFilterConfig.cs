using System;
using System.Management.Automation;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Install", "ContentFilterConfig")]
	public sealed class InstallContentFilterConfig : InstallAntispamConfig<ContentFilterConfig>
	{
		protected override string CanonicalName
		{
			get
			{
				return "ContentFilterConfig";
			}
		}

		protected override IConfigurable PrepareDataObject()
		{
			ContentFilterConfig contentFilterConfig = (ContentFilterConfig)base.PrepareDataObject();
			contentFilterConfig.SCLRejectEnabled = true;
			contentFilterConfig.OutlookEmailPostmarkValidationEnabled = true;
			return contentFilterConfig;
		}
	}
}
