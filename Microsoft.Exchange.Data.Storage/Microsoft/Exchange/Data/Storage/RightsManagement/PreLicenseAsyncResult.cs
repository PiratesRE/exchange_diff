using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Security.RightsManagement;

namespace Microsoft.Exchange.Data.Storage.RightsManagement
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class PreLicenseAsyncResult : RightsManagementAsyncResult
	{
		public LicenseResponse[] Responses { get; set; }

		public PreLicenseAsyncResult(RmsClientManagerContext context, PreLicenseWSManager preLicenseManager, object callerState, AsyncCallback callerCallback) : base(context, callerState, callerCallback)
		{
			this.preLicenseManager = preLicenseManager;
		}

		public PreLicenseWSManager PreLicenseManager
		{
			get
			{
				return this.preLicenseManager;
			}
			set
			{
				this.preLicenseManager = value;
			}
		}

		public void ReleaseManagers()
		{
			if (this.preLicenseManager != null)
			{
				this.preLicenseManager.Dispose();
				this.preLicenseManager = null;
			}
		}

		private PreLicenseWSManager preLicenseManager;
	}
}
