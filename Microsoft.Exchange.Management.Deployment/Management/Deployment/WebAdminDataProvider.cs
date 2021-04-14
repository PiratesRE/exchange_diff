using System;
using Microsoft.Web.Administration;

namespace Microsoft.Exchange.Management.Deployment
{
	internal class WebAdminDataProvider : IWebAdminDataProvider
	{
		public WebAdminDataProvider()
		{
			this.iisSeverManager = new ServerManager();
		}

		public bool Enable32BitAppOnWin64
		{
			get
			{
				return this.iisSeverManager.ApplicationPoolDefaults.Enable32BitAppOnWin64;
			}
		}

		private ServerManager iisSeverManager;
	}
}
