using System;
using Microsoft.Exchange.Data.Storage.ActiveManager;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IDatabaseLocationProvider
	{
		DatabaseLocationInfo GetLocationInfo(Guid mdbGuid, bool bypassCache, bool ignoreSiteBoundary);
	}
}
