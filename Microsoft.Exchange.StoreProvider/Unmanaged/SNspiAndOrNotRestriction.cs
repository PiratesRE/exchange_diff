using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi.Unmanaged
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal struct SNspiAndOrNotRestriction
	{
		internal int cRes;

		internal unsafe SNspiRestriction* lpRes;
	}
}
