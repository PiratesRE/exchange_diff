using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Security.RightsManagement;

namespace Microsoft.Exchange.Data.Storage.OfflineRms
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class ActiveCryptoModeResult
	{
		internal ActiveCryptoModeResult(int cryptoMode, RightsManagementServerException e)
		{
			this.ActiveCryptoMode = cryptoMode;
			this.Error = e;
		}

		public int ActiveCryptoMode { get; private set; }

		public RightsManagementServerException Error { get; private set; }
	}
}
