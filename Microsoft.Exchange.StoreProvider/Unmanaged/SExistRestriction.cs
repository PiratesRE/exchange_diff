using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi.Unmanaged
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal struct SExistRestriction
	{
		internal int ulReserved1;

		internal int ulPropTag;

		internal int ulReserved2;
	}
}
