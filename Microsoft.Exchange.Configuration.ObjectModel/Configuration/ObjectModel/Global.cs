using System;
using System.Web;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.ProvisioningCache;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Configuration.ObjectModel
{
	public class Global : HttpApplication
	{
		private void Application_Start(object sender, EventArgs e)
		{
			this.InitializePerformanceCounter();
			this.InitDirectoryTopologyMode();
			ProvisioningCache.InitializeAppRegistrySettings(((IAppSettings)PswsAppSettings.Instance).ProvisioningCacheIdentification);
		}

		private void InitializePerformanceCounter()
		{
			Globals.InitializeMultiPerfCounterInstance("Psws");
		}

		private void InitDirectoryTopologyMode()
		{
			ADSession.DisableAdminTopologyMode();
		}
	}
}
