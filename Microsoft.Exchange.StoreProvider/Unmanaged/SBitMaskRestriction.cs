using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi.Unmanaged
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal struct SBitMaskRestriction
	{
		internal int relBMR;

		internal int ulPropTag;

		internal int ulMask;
	}
}
