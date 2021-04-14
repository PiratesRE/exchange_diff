using System;

namespace Microsoft.Exchange.Data.Storage.ActiveManager
{
	public enum DatabaseLocationInfoResult
	{
		Success,
		Unknown,
		InTransitSameSite,
		InTransitCrossSite,
		SiteViolation
	}
}
