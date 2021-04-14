using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi.Unmanaged
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal struct SAndOrNotRestriction
	{
		internal int cRes;

		internal unsafe SRestriction* lpRes;
	}
}
