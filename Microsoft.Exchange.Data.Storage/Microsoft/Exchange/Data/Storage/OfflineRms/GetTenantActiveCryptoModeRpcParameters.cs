using System;
using Microsoft.Exchange.Data.Storage.RightsManagement;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.OfflineRms
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class GetTenantActiveCryptoModeRpcParameters : LicensingRpcParameters
	{
		public GetTenantActiveCryptoModeRpcParameters(byte[] data) : base(data)
		{
		}

		public GetTenantActiveCryptoModeRpcParameters(RmsClientManagerContext rmsClientManagerContext) : base(rmsClientManagerContext)
		{
			if (rmsClientManagerContext == null)
			{
				throw new ArgumentNullException("rmsClientManagerContext");
			}
		}
	}
}
