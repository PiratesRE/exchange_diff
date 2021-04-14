using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi.Unmanaged
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal struct SNearRestriction
	{
		internal int ulDistance;

		internal int ulOrdered;

		internal unsafe SRestriction* lpRes;
	}
}
