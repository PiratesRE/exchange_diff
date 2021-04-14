using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi.Unmanaged
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal struct SCountRestriction
	{
		internal int ulCount;

		internal unsafe SRestriction* lpRes;
	}
}
