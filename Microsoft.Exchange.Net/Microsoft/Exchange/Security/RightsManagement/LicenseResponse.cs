using System;

namespace Microsoft.Exchange.Security.RightsManagement
{
	internal sealed class LicenseResponse
	{
		public LicenseResponse(string license, ContentRight? usageRights)
		{
			this.Exception = null;
			this.License = license;
			this.UsageRights = usageRights;
		}

		public LicenseResponse(RightsManagementException exception)
		{
			this.Exception = exception;
		}

		public readonly RightsManagementException Exception;

		public readonly string License;

		public readonly ContentRight? UsageRights;
	}
}
