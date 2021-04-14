using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Security.RightsManagement;

namespace Microsoft.Exchange.Data.Storage.OfflineRms
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class UseLicenseResult
	{
		internal UseLicenseResult(string endUseLicense, RightsManagementServerException e)
		{
			this.EndUseLicense = endUseLicense;
			this.Error = e;
		}

		public string EndUseLicense { get; private set; }

		public RightsManagementServerException Error { get; private set; }
	}
}
