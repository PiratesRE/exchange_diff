using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi.Unmanaged
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal struct SComparePropsRestriction
	{
		internal int relop;

		internal int ulPropTag1;

		internal int ulPropTag2;
	}
}
